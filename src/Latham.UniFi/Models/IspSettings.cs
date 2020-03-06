//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

namespace Latham.UniFi.Models
{
    public sealed class IspSettings
    {
        public string? AeMode { get; }
        public string? IrLedMode { get; }
        public double IrLedLevel { get; }
        public double Wdr { get; }
        public double IcrSensitivity { get; }
        public double Brightness { get; }
        public double Contrast { get; }
        public double Hue { get; }
        public double Saturation { get; }
        public double Sharpness { get; }
        public double Denoise { get; }
        public bool IsFlippedVertical { get; }
        public bool IsFlippedHorizontal { get; }
        public bool IsAutoRotateEnabled { get; }
        public bool IsLdcEnabled { get; }
        public bool Is3dnrEnabled { get; }
        public bool IsExternalIrEnabled { get; }
        public bool IsAggressiveAntiFlickerEnabled { get; }
        public bool IsPauseMotionEnabled { get; }
        public double DZoomCenterX { get; }
        public double DZoomCenterY { get; }
        public double DZoomScale { get; }
        public int DZoomStreamId { get; }
        public string? FocusMode { get; }
        public double FocusPosition { get; }
        public double TouchFocusX { get; }
        public double TouchFocusY { get; }
        public double ZoomPosition { get; }

        public IspSettings(
            string? aeMode,
            string? irLedMode,
            double irLedLevel,
            double wdr,
            double icrSensitivity,
            double brightness,
            double contrast,
            double hue,
            double saturation,
            double sharpness,
            double denoise,
            bool isFlippedVertical,
            bool isFlippedHorizontal,
            bool isAutoRotateEnabled,
            bool isLdcEnabled,
            bool is3dnrEnabled,
            bool isExternalIrEnabled,
            bool isAggressiveAntiFlickerEnabled,
            bool isPauseMotionEnabled,
            double dZoomCenterX,
            double dZoomCenterY,
            double dZoomScale,
            int dZoomStreamId,
            string? focusMode,
            double focusPosition,
            double touchFocusX,
            double touchFocusY,
            double zoomPosition)
        {
            AeMode = aeMode;
            IrLedMode = irLedMode;
            IrLedLevel = irLedLevel;
            Wdr = wdr;
            IcrSensitivity = icrSensitivity;
            Brightness = brightness;
            Contrast = contrast;
            Hue = hue;
            Saturation = saturation;
            Sharpness = sharpness;
            Denoise = denoise;
            IsFlippedVertical = isFlippedVertical;
            IsFlippedHorizontal = isFlippedHorizontal;
            IsAutoRotateEnabled = isAutoRotateEnabled;
            IsLdcEnabled = isLdcEnabled;
            Is3dnrEnabled = is3dnrEnabled;
            IsExternalIrEnabled = isExternalIrEnabled;
            IsAggressiveAntiFlickerEnabled = isAggressiveAntiFlickerEnabled;
            IsPauseMotionEnabled = isPauseMotionEnabled;
            DZoomCenterX = dZoomCenterX;
            DZoomCenterY = dZoomCenterY;
            DZoomScale = dZoomScale;
            DZoomStreamId = dZoomStreamId;
            FocusMode = focusMode;
            FocusPosition = focusPosition;
            TouchFocusX = touchFocusX;
            TouchFocusY = touchFocusY;
            ZoomPosition = zoomPosition;
        }
    }
}
