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
    public sealed class Camera
    {
        public bool IsDeleting { get; }
        public string? Mac { get; }
        public string? Host { get; }
        public string? ConnectionHost { get; }
        public string? Type { get; }
        public string? Name { get; }
        public DateTimeOffset UpSince { get; }
        public DateTimeOffset LastSeen { get; }
        public DateTimeOffset ConnectedSince { get; }
        public string? State { get; }
        public string? HardwareRevision { get; }
        public string? FirmwareVersion { get; }
        public string? FirmwareBuild { get; }
        public bool IsUpdating { get; }
        public bool IsAdopting { get; }
        public bool IsManaged { get; }
        public bool IsProvisioned { get; }
        public bool IsRebooting { get; }
        public bool IsSshEnabled { get; }
        public bool CanManage { get; }
        public bool IsHidden { get; }
        public DateTimeOffset LastMotion { get; }
        public double? MicVolume { get; }
        public bool IsMicEnabled { get; }
        public bool IsRecording { get; }
        public bool IsMotionDetected { get; }
        public bool IsAttemptingToConnect { get; }
        public double? PhyRate { get; }
        public bool HdrMode { get; }
        public object? IsProbingForWifi { get; }
        public object? ApMac { get; }
        public object? ApRssi { get; }
        public object? ElementInfo { get; }
        public double? ChimeDuration { get; }
        public bool IsDark { get; }
        public int MotionStartCalls { get; }
        public object? LastRing { get; }
        public ConnectionState? WiredConnectionState { get; }
        public IReadOnlyList<Channel> Channels { get; }
        public IspSettings? IspSettings { get; }
        public TalkbackSettings? talkbackSettings { get; }
        public OsdSettings? OsdSettings { get; }
        public LedSettings? LedSettings { get; }
        public SpeakerSettings? SpeakerSettings { get; }
        public RecordingSettings? RecordingSettings { get; }
        public object? RecordingSchedule { get; }
        public IReadOnlyList<MotionZone> MotionZones { get; }
        public IReadOnlyList<object> PrivacyZones { get; }
        public CameraStats? Stats { get; }
        public CameraFeatures? FeatureFlags { get; }
        public PirSettings? PirSettings { get; }
        public WifiStats? WifiConnectionState { get; }
        public string? Id { get; }
        public bool IsConnected { get; }
        public string? Platform { get; }
        public bool HasSpeaker { get; }
        public bool HasWifi { get; }
        public int AudioBitrate { get; }
        public string? ModelKey { get; }

        [JsonConstructor]
        public Camera(
            bool isDeleting,
            string? mac,
            string? host,
            string? connectionHost,
            string? type,
            string? name,
            DateTimeOffset upSince,
            DateTimeOffset lastSeen,
            DateTimeOffset connectedSince,
            string? state,
            string? hardwareRevision,
            string? firmwareVersion,
            string? firmwareBuild,
            bool isUpdating,
            bool isAdopting,
            bool isManaged,
            bool isProvisioned,
            bool isRebooting,
            bool isSshEnabled,
            bool canManage,
            bool isHidden,
            DateTimeOffset lastMotion,
            double micVolume,
            bool isMicEnabled,
            bool isRecording,
            bool isMotionDetected,
            bool isAttemptingToConnect,
            double phyRate,
            bool hdrMode,
            object? isProbingForWifi,
            object? apMac,
            object? apRssi,
            object? elementInfo,
            double chimeDuration,
            bool isDark,
            int motionStartCalls,
            object? lastRing,
            ConnectionState? wiredConnectionState,
            IReadOnlyList<Channel>? channels,
            IspSettings? ispSettings,
            TalkbackSettings? talkbackSettings,
            OsdSettings? osdSettings,
            LedSettings? ledSettings,
            SpeakerSettings? speakerSettings,
            RecordingSettings? recordingSettings,
            object? recordingSchedule,
            IReadOnlyList<MotionZone>? motionZones,
            IReadOnlyList<object>? privacyZones,
            CameraStats? stats,
            CameraFeatures? featureFlags,
            PirSettings? pirSettings,
            WifiStats? wifiConnectionState,
            string? id,
            bool isConnected,
            string? platform,
            bool hasSpeaker,
            bool hasWifi,
            int audioBitrate,
            string? modelKey)
        {
            IsDeleting = isDeleting;
            Mac = mac;
            Host = host;
            ConnectionHost = connectionHost;
            Type = type;
            Name = name;
            UpSince = upSince;
            LastSeen = lastSeen;
            ConnectedSince = connectedSince;
            State = state;
            HardwareRevision = hardwareRevision;
            FirmwareVersion = firmwareVersion;
            FirmwareBuild = firmwareBuild;
            IsUpdating = isUpdating;
            IsAdopting = isAdopting;
            IsManaged = isManaged;
            IsProvisioned = isProvisioned;
            IsRebooting = isRebooting;
            IsSshEnabled = isSshEnabled;
            CanManage = canManage;
            IsHidden = isHidden;
            LastMotion = lastMotion;
            MicVolume = micVolume;
            IsMicEnabled = isMicEnabled;
            IsRecording = isRecording;
            IsMotionDetected = isMotionDetected;
            IsAttemptingToConnect = isAttemptingToConnect;
            PhyRate = phyRate;
            HdrMode = hdrMode;
            IsProbingForWifi = isProbingForWifi;
            ApMac = apMac;
            ApRssi = apRssi;
            ElementInfo = elementInfo;
            ChimeDuration = chimeDuration;
            IsDark = isDark;
            MotionStartCalls = motionStartCalls;
            LastRing = lastRing;
            WiredConnectionState = wiredConnectionState;
            Channels = channels ?? Array.Empty<Channel>();
            IspSettings = ispSettings;
            this.talkbackSettings = talkbackSettings;
            OsdSettings = osdSettings;
            LedSettings = ledSettings;
            SpeakerSettings = speakerSettings;
            RecordingSettings = recordingSettings;
            RecordingSchedule = recordingSchedule;
            MotionZones = motionZones ?? Array.Empty<MotionZone>();
            PrivacyZones = privacyZones ?? Array.Empty<object>();
            Stats = stats;
            FeatureFlags = featureFlags;
            PirSettings = pirSettings;
            WifiConnectionState = wifiConnectionState;
            Id = id;
            IsConnected = isConnected;
            Platform = platform;
            HasSpeaker = hasSpeaker;
            HasWifi = hasWifi;
            AudioBitrate = audioBitrate;
            ModelKey = modelKey;
        }
    }
}
