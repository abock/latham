//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using System.IO;

using static Xamarin.PathHelpers;

using Latham.Project.Model;

namespace Latham.Project
{
    public static class Ingestion
    {
        public static IEnumerable<IngestionItem> EnumerateIngestionFiles(
            ProjectInfo projectInfo,
            IEnumerable<string>? ingestionCandidatePaths = null)
            => EnumerateIngestionFiles(
                projectInfo,
                parseMetadataFromFile: true,
                ingestionCandidatePaths);

        public static IEnumerable<IngestionItem> EnumerateIngestionFiles(
            ProjectInfo projectInfo,
            bool parseMetadataFromFile,
            IEnumerable<string>? ingestionCandidatePaths = null)
            => projectInfo
                .Ingestions
                .SelectMany(ingestion => EnumerateIngestionFiles(
                    ingestion,
                    parseMetadataFromFile,
                    ingestionCandidatePaths));

        public static IEnumerable<IngestionItem> EnumerateIngestionFiles(
            IngestionInfo ingestionInfo,
            bool parseMetadataFromFile,
            IEnumerable<string>? ingestionCandidatePaths = null,
            string? basePath = null)
        {
            if (ingestionInfo.BasePath is string)
                basePath = basePath is string
                    ? Path.Combine(basePath, ingestionInfo.BasePath)
                    : ingestionInfo.BasePath;

            basePath = ResolveFullPath(basePath ?? ".");

            if (ingestionCandidatePaths is null)
                ingestionCandidatePaths = Glob.Expand(
                    basePath: basePath,
                    pattern: ingestionInfo.PathGlob);

            return ingestionCandidatePaths
                .Select(path => (
                    FullPath: path,
                    Match: ingestionInfo.PathFilter?.Match(path)))
                .Where(item => item.Match is null || item.Match.Success)
                .Select(item => IngestionItem.FromFileSystem(
                    basePath,
                    item.FullPath,
                    item.Match,
                    parseDuration: parseMetadataFromFile));
        }
    }
}
