using System;
using Logitech_LCD;
using WebDav.AudioPlayer.UI;

namespace WebDav.AudioPlayer.Util;

internal static class LcdUtils
{
    private static readonly bool _isLcdMonoInit = LogitechLcd.Instance.Init(MainForm.AppName, LcdType.Mono);

    internal static void MonoSetDefault()
    {
        MonoSetText($"    {MainForm.AppName}", "    by", "    Stef Heyenrath");
    }

    internal static void MonoSetText(params string[] lines)
    {
        if (_isLcdMonoInit)
        {
            LogitechLcd.Instance.MonoSetText(0, lines.Length > 0 ? lines[0] : string.Empty);
            LogitechLcd.Instance.MonoSetText(1, lines.Length > 1 ? lines[1] : string.Empty);
            LogitechLcd.Instance.MonoSetText(2, lines.Length > 2 ? lines[2] : string.Empty);
            LogitechLcd.Instance.MonoSetText(3, lines.Length > 3 ? lines[3] : string.Empty);
            LogitechLcd.Instance.Update();
        }
    }

    internal static void MonoSetTimeAndBitrate(string pre, int bitrateKbps, string mode, TimeSpan? currentTime = null, TimeSpan? totalTime = null)
    {
        if (_isLcdMonoInit)
        {
            var bitrate = $"{bitrateKbps} kbps {mode}";

            if (currentTime == null || totalTime == null)
            {
                LogitechLcd.Instance.MonoSetText(3, string.Empty);
            }
            else if (totalTime.HasValue && totalTime.Value.TotalMinutes >= 60)
            {
                LogitechLcd.Instance.MonoSetText(3, $"{pre} {currentTime:hh\\:mm\\:ss} / {totalTime:hh\\:mm\\:ss} ({bitrate})");
            }
            else if (totalTime.HasValue && totalTime.Value.TotalMinutes < 10)
            {
                LogitechLcd.Instance.MonoSetText(3, $"{pre} {currentTime:m\\:ss} / {totalTime:m\\:ss}       ({bitrate})");
            }
            else
            {
                LogitechLcd.Instance.MonoSetText(3, $"{pre} {currentTime:mm\\:ss} / {totalTime:mm\\:ss}      ({bitrate})");
            }

            LogitechLcd.Instance.Update();
        }
    }
}