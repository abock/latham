//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Latham.UniFi.Models
{
    public sealed class Protect
    {
        public string? AuthUserId { get; }
        public string? AccessKey { get; }
        public IReadOnlyList<Camera> Cameras { get; }
        public IReadOnlyList<User> Users { get; }
        public IReadOnlyList<Group> Groups { get; }
        public IReadOnlyList<LiveView> LiveViews { get; }
        public Nvr? Nvr { get; }
        public IReadOnlyList<JToken> Viewers { get; }
        public IReadOnlyList<JToken> Lights { get; }
        public IReadOnlyList<JToken> Beacons { get; }
        public IReadOnlyList<JToken> Sensors { get; }
        public Guid LastUpdateId { get; }
        public Uri? CloudPortalUrl { get; }

        [JsonConstructor]
        public Protect(
            string? authUserId,
            string? accessKey,
            IReadOnlyList<Camera>? cameras,
            IReadOnlyList<User>? users,
            IReadOnlyList<Group>? groups,
            IReadOnlyList<LiveView>? liveviews,
            Nvr? nvr,
            IReadOnlyList<JToken>? viewers,
            IReadOnlyList<JToken>? lights,
            IReadOnlyList<JToken>? beacons,
            IReadOnlyList<JToken>? sensors,
            Guid lastUpdateId,
            Uri? cloudPortalUrl)
        {
            AuthUserId = authUserId;
            AccessKey = accessKey;
            Cameras = cameras ?? Array.Empty<Camera>();
            Users = users ?? Array.Empty<User>();
            Groups = groups ?? Array.Empty<Group>();
            LiveViews = liveviews ?? Array.Empty<LiveView>();
            Nvr = nvr;
            Viewers = viewers ?? Array.Empty<JToken>();
            Lights = lights ?? Array.Empty<JToken>();
            Beacons = beacons ?? Array.Empty<JToken>();
            Sensors = sensors ?? Array.Empty<JToken>();
            LastUpdateId = lastUpdateId;
            CloudPortalUrl = cloudPortalUrl;
        }
    }
}
