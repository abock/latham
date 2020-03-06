//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace Latham.UniFi.Models
{
    public sealed class CameraFeatures
    {
        public bool CanAdjustIrLedLevel { get; }
        public bool CanMagicZoom { get; }
        public bool CanOpticalZoom { get; }
        public bool CanTouchFocus { get; }
        public bool HasAccelerometer { get; }
        public bool HasAec { get; }
        public bool HasBattery { get; }
        public bool HasBluetooth { get; }
        public bool HasChime { get; }
        public bool HasExternalIr { get; }
        public bool HasIcrSensitivity { get; }
        public bool HasLdc { get; }
        public bool HasLedIr { get; }
        public bool HasLedStatus { get; }
        public bool HasLineIn { get; }
        public bool HasMic { get; }
        public bool HasPrivacyMask { get; }
        public bool HasRtc { get; }
        public bool HasSdCard { get; }
        public bool HasSpeaker { get; }
        public bool HasWifi { get; }
        public bool HasHdr { get; }
        public bool HasAutoICROnly { get; }
        public bool HasMotionZones { get; }

        [JsonConstructor]
        public CameraFeatures(
            bool canAdjustIrLedLevel,
            bool canMagicZoom,
            bool canOpticalZoom,
            bool canTouchFocus,
            bool hasAccelerometer,
            bool hasAec,
            bool hasBattery,
            bool hasBluetooth,
            bool hasChime,
            bool hasExternalIr,
            bool hasIcrSensitivity,
            bool hasLdc,
            bool hasLedIr,
            bool hasLedStatus,
            bool hasLineIn,
            bool hasMic,
            bool hasPrivacyMask,
            bool hasRtc,
            bool hasSdCard,
            bool hasSpeaker,
            bool hasWifi,
            bool hasHdr,
            bool hasAutoICROnly,
            bool hasMotionZones)
        {
            CanAdjustIrLedLevel = canAdjustIrLedLevel;
            CanMagicZoom = canMagicZoom;
            CanOpticalZoom = canOpticalZoom;
            CanTouchFocus = canTouchFocus;
            HasAccelerometer = hasAccelerometer;
            HasAec = hasAec;
            HasBattery = hasBattery;
            HasBluetooth = hasBluetooth;
            HasChime = hasChime;
            HasExternalIr = hasExternalIr;
            HasIcrSensitivity = hasIcrSensitivity;
            HasLdc = hasLdc;
            HasLedIr = hasLedIr;
            HasLedStatus = hasLedStatus;
            HasLineIn = hasLineIn;
            HasMic = hasMic;
            HasPrivacyMask = hasPrivacyMask;
            HasRtc = hasRtc;
            HasSdCard = hasSdCard;
            HasSpeaker = hasSpeaker;
            HasWifi = hasWifi;
            HasHdr = hasHdr;
            HasAutoICROnly = hasAutoICROnly;
            HasMotionZones = hasMotionZones;
        }
    }
}
