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
    public sealed class Nvr
    {
        public string? Mac { get; }
        public string? Host { get; }
        public string? Name { get; }
        public bool CanAutoUpdate { get; }
        public bool IsStatsGatheringEnabled { get; }
        public string? Timezone { get; }
        public string? Version { get; }
        public string? FirmwareVersion { get; }
        public object? UiVersion { get; }
        public string? HardwarePlatform { get; }
        public IReadOnlyDictionary<string, long> Ports { get; }
        public object? SetupCode { get; }
        public long Uptime { get; }
        public long LastSeen { get; }
        public bool IsUpdating { get; }
        public long LastUpdateAt { get; }
        public bool IsConnectedToCloud { get; }
        public object? CloudConnectionError { get; }
        public bool IsStation { get; }
        public bool EnableAutomaticBackups { get; }
        public bool EnableStatsReporting { get; }
        public bool IsSshEnabled { get; }
        public object? ErrorCode { get; }
        public string? ReleaseChannel { get; }
        public string? AvailableUpdate { get; }
        public IReadOnlyList<string> Hosts { get; }
        public Guid HardwareId { get; }
        public string? HardwareRevision { get; }
        public long HostType { get; }
        public bool IsHardware { get; }
        public string? TimeFormat { get; }
        public object? RecordingRetentionDurationMs { get; }
        public bool EnableCrashReporting { get; }
        public bool DisableAudio { get; }
        public WifiSettings? WifiSettings { get; }
        public LocationSettings? LocationSettings { get; }
        public NvrFeatures? FeatureFlags { get; }
        public StorageInfo? StorageInfo { get; }
        public string? Id { get; }
        public bool IsAdopted { get; }
        public bool IsAway { get; }
        public bool IsSetup { get; }
        public string? Network { get; }
        public string? Type { get; }
        public long UpSince { get; }
        public string? ModelKey { get; }

        [JsonConstructor]
        public Nvr(
            string? mac,
            string? host,
            string? name,
            bool canAutoUpdate,
            bool isStatsGatheringEnabled,
            string? timezone,
            string? version,
            string? firmwareVersion,
            object? uiVersion,
            string? hardwarePlatform,
            IReadOnlyDictionary<string, long>? ports,
            object? setupCode,
            long uptime,
            long lastSeen,
            bool isUpdating,
            long lastUpdateAt,
            bool isConnectedToCloud,
            object? cloudConnectionError,
            bool isStation,
            bool enableAutomaticBackups,
            bool enableStatsReporting,
            bool isSshEnabled,
            object? errorCode,
            string? releaseChannel,
            string? availableUpdate,
            IReadOnlyList<string>? hosts,
            Guid hardwareId,
            string? hardwareRevision,
            long hostType,
            bool isHardware,
            string? timeFormat,
            object? recordingRetentionDurationMs,
            bool enableCrashReporting,
            bool disableAudio,
            WifiSettings? wifiSettings,
            LocationSettings? locationSettings,
            NvrFeatures? featureFlags,
            StorageInfo? storageInfo,
            string? id,
            bool isAdopted,
            bool isAway,
            bool isSetup,
            string? network,
            string? type,
            long upSince,
            string? modelKey)
        {
            Mac = mac;
            Host = host;
            Name = name;
            CanAutoUpdate = canAutoUpdate;
            IsStatsGatheringEnabled = isStatsGatheringEnabled;
            Timezone = timezone;
            Version = version;
            FirmwareVersion = firmwareVersion;
            UiVersion = uiVersion;
            HardwarePlatform = hardwarePlatform;
            Ports = ports ?? new Dictionary<string, long>(0);
            SetupCode = setupCode;
            Uptime = uptime;
            LastSeen = lastSeen;
            IsUpdating = isUpdating;
            LastUpdateAt = lastUpdateAt;
            IsConnectedToCloud = isConnectedToCloud;
            CloudConnectionError = cloudConnectionError;
            IsStation = isStation;
            EnableAutomaticBackups = enableAutomaticBackups;
            EnableStatsReporting = enableStatsReporting;
            IsSshEnabled = isSshEnabled;
            ErrorCode = errorCode;
            ReleaseChannel = releaseChannel;
            AvailableUpdate = availableUpdate;
            Hosts = hosts ?? Array.Empty<string>();
            HardwareId = hardwareId;
            HardwareRevision = hardwareRevision;
            HostType = hostType;
            IsHardware = isHardware;
            TimeFormat = timeFormat;
            RecordingRetentionDurationMs = recordingRetentionDurationMs;
            EnableCrashReporting = enableCrashReporting;
            DisableAudio = disableAudio;
            WifiSettings = wifiSettings;
            LocationSettings = locationSettings;
            FeatureFlags = featureFlags;
            StorageInfo = storageInfo;
            Id = id;
            IsAdopted = isAdopted;
            IsAway = isAway;
            IsSetup = isSetup;
            Network = network;
            Type = type;
            UpSince = upSince;
            ModelKey = modelKey;
        }
    }
}
