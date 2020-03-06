//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System;

using Newtonsoft.Json;

namespace Latham.UniFi.Models
{
    public sealed class CloudAccount
    {
        public string? Name { get; }
        public string? Email { get; }
        public Guid CloudId { get; }
        public object? ProfileImg { get; }
        public object? InviteExpireTime { get; }
        public Location? Location { get; }

        [JsonConstructor]
        public CloudAccount(
            string? name,
            string? email,
            Guid cloudId,
            object? profileImg,
            object? inviteExpireTime,
            Location? location)
        {
            Name = name;
            Email = email;
            CloudId = cloudId;
            ProfileImg = profileImg;
            InviteExpireTime = inviteExpireTime;
            Location = location;
        }
    }
}
