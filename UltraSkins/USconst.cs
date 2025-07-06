using System;

using System.IO;
using System.Reflection;


namespace UltraSkins
{
    internal static class USC
    {
        // MAIN STUFF
        public const string VERSION = "7.0.0";
        public static string MODPATH => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string GCDIR => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "bobthecorn2000", "ULTRAKILL", "ultraskinsGC-V2");

        //Folder names
        public const string SAVEDATA = "SaveData";
        public const string UNISKIN = "GlobalSkins";
        public const string VERNAME = "Versions";
#if RELEASE
        public const string BUILDTYPE = "Standalone";
#elif DEBUG
        public const string BUILDTYPE = "Debug";
#elif CANARY
        public const string BUILDTYPE = "Canary";
#elif ALPHA
        public const string BUILDTYPE = "Alpha";
#endif

        // File types 

        public const string DATAFILE = "Data.USGC";
        public const string SETTINGSFILE = "Settings.USGC";
        public const string DEFAULTSETTINGS = "DefaultSettings.USGC";
    }
}
