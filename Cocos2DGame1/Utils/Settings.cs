using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VenLight.Settings
{
    static class Settings
    {

        public static string DIRTheme = AppDomain.CurrentDomain.BaseDirectory + "\\Theme\\";

        static bool isInitied = false;
        static bool useWindowsBackground = false;

        static void Init()
        {
            isInitied = true;

        }

        public static string GetBackgroundLink()
        {
            if (!useWindowsBackground) return DIRTheme + "fon.gif";
            else return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData\\Roaming\\Microsoft\\Windows\\Themes\\TranscodedWallpaper.jpg");
        }

        public static string GetThemeLink()
        {
            return DIRTheme;
        }

        public static string GetDefaultIconImageLink()
        {
            return DIRTheme + "default.gif";
        }

    }
}
