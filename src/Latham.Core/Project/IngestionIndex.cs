//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Latham.Project.Model;

using static SQLitePCL.raw;

namespace Latham.Project
{
    public class IngestionIndex : IDisposable
    {
        static IngestionIndex()
            => SQLitePCL.Batteries_V2.Init();

        SQLitePCL.sqlite3? dbHandle;

        public IngestionIndex(ProjectInfo projectInfo)
        {
            if (projectInfo.FilePath is null)
                throw new ArgumentException(
                    $"must be loaded from disk using {nameof(ProjectInfo)}.{nameof(ProjectInfo.FromFile)}",
                    nameof(projectInfo));

            if (!File.Exists(projectInfo.FilePath))
                throw new FileNotFoundException("Project does not exist", projectInfo.FilePath);

            var dbPath = projectInfo.IngestionIndexFilePath is null
                ? Path.ChangeExtension(projectInfo.FilePath, "index")
                : Path.Combine(
                    Path.GetDirectoryName(projectInfo.FilePath) ?? ".",
                    projectInfo.IngestionIndexFilePath);

            Open(dbPath);
        }

        public IngestionIndex(string indexPath)
            => Open(indexPath);

        void Open(string indexPath)
        {
            SqliteAssert(sqlite3_open_v2(
                indexPath,
                out dbHandle,
                flags: SQLITE_OPEN_READWRITE | SQLITE_OPEN_CREATE,
                vfs: null),
                $"Unable to open or create index database at '{indexPath}'");

            CreateTables();
        }

        void SqliteAssert(int rc, string? message = null, int expect = SQLITE_OK)
        {
            if (rc == expect)
                return;

            var sqliteMessage = $"{sqlite3_errstr(rc).utf8_to_string()} ({rc})";

            throw new Exception(message is null
                ? sqliteMessage
                : $"{message}: {sqliteMessage}");
        }

        ~IngestionIndex()
        {
            Dispose(false);
            GC.SuppressFinalize(this);
        }

        public void Dispose()
            => Dispose(true);

        void Dispose(bool disposing)
        {
            if (dbHandle is null)
                return;

            var rc = sqlite3_close_v2(dbHandle);
            dbHandle = null;

            if (disposing)
                SqliteAssert(rc);
        }

        public void Reset()
        {
            SqliteAssert(sqlite3_exec(dbHandle, "DELETE FROM Ingestions"));
            SqliteAssert(sqlite3_exec(dbHandle, "DROP TABLE Ingestions"));
            CreateTables();
        }

        void CreateTables()
            => SqliteAssert(sqlite3_exec(dbHandle, @"
                CREATE TABLE IF NOT EXISTS Ingestions (
                    FilePath TEXT PRIMARY KEY,
                    FileSize INTEGER,
                    Tag TEXT,
                    Timestamp INTEGER,
                    TimestampOffset INTEGER,
                    Duration INTEGER
                );
            "));

        DateTimeOffset ColumnsDateTimeOffset(
            SQLitePCL.sqlite3_stmt stmt,
            int timestampColumnIndex,
            int timestampOffsetColumnIndex)
            => new DateTimeOffset(
                sqlite3_column_int64(stmt, timestampColumnIndex),
                TimeSpan.FromTicks(sqlite3_column_int64(stmt, timestampOffsetColumnIndex)));

        IEnumerable<T> Select<T>(string query, Func<SQLitePCL.sqlite3_stmt, T> rowProducer)
        {
            SqliteAssert(sqlite3_prepare_v2(dbHandle, query, out var stmt));

            try
            {
                while (true)
                {
                    var rc = sqlite3_step(stmt);
                    if (rc == SQLITE_DONE)
                        break;

                    if (rc != SQLITE_ROW)
                        SqliteAssert(rc);

                    yield return rowProducer(stmt);
                }
            }
            finally
            {
                SqliteAssert(sqlite3_finalize(stmt));
            }
        }

        public IEnumerable<IngestionItem> SelectAll()
        {
            SqliteAssert(sqlite3_prepare_v2(
                dbHandle,
                @"SELECT
                    FilePath,
                    FileSize,
                    Tag,
                    Timestamp,
                    TimestampOffset,
                    Duration
                FROM Ingestions
                ORDER BY
                    Tag,
                    Timestamp - TimestampOffset",
                out var stmt));

            while (true)
            {
                var rc = sqlite3_step(stmt);
                if (rc == SQLITE_DONE)
                    break;

                if (rc != SQLITE_ROW)
                    SqliteAssert(rc);

                yield return new IngestionItem(
                    sqlite3_column_text(stmt, 0).utf8_to_string(),
                    sqlite3_column_int64(stmt, 1),
                    sqlite3_column_text(stmt, 2).utf8_to_string(),
                    ColumnsDateTimeOffset(stmt, 3, 4),
                    TimeSpan.FromTicks(sqlite3_column_int64(stmt, 5)));
            }

            SqliteAssert(sqlite3_finalize(stmt));
        }

        public DateTimeOffset SelectOldestTimestamp()
            => Select(@"
                SELECT
                    MIN(Timestamp - TimestampOffset),
                    Timestamp,
                    TimestampOffset
                FROM Ingestions",
                stmt => ColumnsDateTimeOffset(stmt, 1, 2)).Single();

        public void Insert(IngestionItem? item)
        {
            if (item is null)
                return;

            Insert(new [] { item });
        }

        public void Insert(IEnumerable<IngestionItem>? items)
        {
            if (items is null)
                return;

            SqliteAssert(sqlite3_exec(dbHandle, "BEGIN TRANSACTION"));

            try
            {
                SqliteAssert(sqlite3_prepare_v2(dbHandle, @"
                    INSERT INTO Ingestions (
                        FilePath,
                        FileSize,
                        Tag,
                        Timestamp,
                        TimestampOffset,
                        Duration
                    ) VALUES (
                        ?1,
                        ?2,
                        ?3,
                        ?4,
                        ?5,
                        ?6
                    ) ON CONFLICT (FilePath) DO NOTHING
                ", out var stmt));

                foreach (var item in items)
                {
                    SqliteAssert(sqlite3_bind_text(stmt, 1, item.FilePath));

                    SqliteAssert(item.FileSize.HasValue
                        ? sqlite3_bind_int64(stmt, 2, item.FileSize.Value)
                        : sqlite3_bind_null(stmt, 2));

                    SqliteAssert(item.Tag is string
                        ? sqlite3_bind_text(stmt, 3, item.Tag)
                        : sqlite3_bind_null(stmt, 3));

                    if (item.Timestamp.HasValue)
                    {
                        var time = item.Timestamp.Value;
                        SqliteAssert(sqlite3_bind_int64(stmt, 4, time.Ticks));
                        SqliteAssert(sqlite3_bind_int64(stmt, 5, time.Offset.Ticks));
                    }
                    else
                    {
                        SqliteAssert(sqlite3_bind_null(stmt, 4));
                        SqliteAssert(sqlite3_bind_null(stmt, 5));
                    }

                    SqliteAssert(item.Duration.HasValue
                        ? sqlite3_bind_int64(stmt, 6, item.Duration.Value.Ticks)
                        : sqlite3_bind_null(stmt, 6));

                    SqliteAssert(sqlite3_step(stmt), expect: SQLITE_DONE);

                    SqliteAssert(sqlite3_reset(stmt));
                }
            }
            finally
            {
                SqliteAssert(sqlite3_exec(dbHandle, "END TRANSACTION"));
            }
        }
    }
}
