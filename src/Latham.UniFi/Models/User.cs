//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Latham.UniFi.Models
{
    public sealed class User
    {
        public IReadOnlyList<string> Permissions { get; }
        public string? LastLoginIp { get; }
        public long LastLoginTime { get; }
        public bool IsOwner { get; }
        public string? LocalUsername { get; }
        public bool EnableNotifications { get; }
        public bool SyncSso { get; }
        public object? Settings { get; }
        public IReadOnlyList<string> Groups { get; }
        public CloudAccount? CloudAccount { get; }
        public IReadOnlyList<object> AlertRules { get; }
        public string? Id { get; }
        public bool HasAcceptedInvite { get; }
        public string? Role { get; }
        public IReadOnlyList<string> AllPermissions { get; }
        public string? ModelKey { get; }

        [JsonConstructor]
        public User(
            IReadOnlyList<string>? permissions,
            string? lastLoginIp,
            long lastLoginTime,
            bool isOwner,
            string? localUsername,
            bool enableNotifications,
            bool syncSso,
            object? settings,
            IReadOnlyList<string>? groups,
            CloudAccount? cloudAccount,
            IReadOnlyList<object>? alertRules,
            string? id,
            bool hasAcceptedInvite,
            string? role,
            IReadOnlyList<string>? allPermissions,
            string? modelKey)
        {
            Permissions = permissions ?? Array.Empty<string>();
            LastLoginIp = lastLoginIp;
            LastLoginTime = lastLoginTime;
            IsOwner = isOwner;
            LocalUsername = localUsername;
            EnableNotifications = enableNotifications;
            SyncSso = syncSso;
            Settings = settings;
            Groups = groups ?? Array.Empty<string>();
            CloudAccount = cloudAccount;
            AlertRules = alertRules ?? Array.Empty<object>();
            Id = id;
            HasAcceptedInvite = hasAcceptedInvite;
            Role = role;
            AllPermissions = allPermissions ?? Array.Empty<string>();
            ModelKey = modelKey;
        }
    }
}
