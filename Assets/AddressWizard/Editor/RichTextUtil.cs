using System;


namespace AddressWizard.Editor
{
    public static class RichTextUtil
    {
        public static string AddCurrentTime(this string text) =>
            $"<color=white>[{DateTime.Now:HH:mm:ss}]</color> {text}";
        
        public static string ToRed(this string text) => $"<color=#ff0000>{text}</color>";
        public static string ToGreen(this string text) => $"<color=#00ff00>{text}</color>";
        public static string ToBlue(this string text) => $"<color=#0000ff>{text}</color>";
        public static string ToYellow(this string text) => $"<color=#ffff00>{text}</color>";
        public static string ToWhite(this string text) => $"<color=#ffffff>{text}</color>";
    }
}
