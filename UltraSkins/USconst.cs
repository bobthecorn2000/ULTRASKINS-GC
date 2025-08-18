using System;

using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace UltraSkins
{
    internal static class USC
    {
        // Versions
        public const string VERSION = "7.0.0";
        public static readonly string[] SupportedPackFormats = { "4.0" };
        public const string GCSKINVERSION = "4.0";


        // MAIN STUFF
        
        public static string MODPATH => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string GCDIR => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "bobthecorn2000", "ULTRAKILL", "ultraskinsGC-V2");
        public static string LEGACYGCDIR => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "bobthecorn2000", "ULTRAKILL", "ultraskinsGC");
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
        public const string MDFILE = "metadata.GCMD";
        public const string PACKFILE = "pack.GCMD";
        public const string TSMANIFEST = "manifest.json";


        // Resticted info
        public static readonly Dictionary<string, string> ReservedNameJokes = new(StringComparer.OrdinalIgnoreCase)
        {
            { "CON", "SmileOS doesnt support that kind of CONSOLE" },
            { "PRN", "Try naming it 'homework' instead." },
            { "AUX", "I'm still mad Apple removed those, guess you need to remove it from your skin name too" },
            { "NUL", "Its so empty in here, I'm cold" },
            { "COM1", "Do people still even use COM ports" },
            { "COM2", "Do people still even use COM ports" },
            { "COM3", "Really? your still trying to name your skin COM?" },
            { "COM4", "So you're still trying to use COM" },
            { "COM5", "wow, it worked, oh wait, no" },
            { "COM6", "Sooooooo, while you'rehere, maybe star the project on github," },
            { "COM7", "Why specifically COM7" },
            { "COM8", "you're either reading the source or just trying everything, arnt you" },
            { "COM9", "hmm, i guess you're really set on using COM, COM10 should work" },
            {"COM\u00B9", "How did you even get that character? this font doesnt even support superscript"},
            {"COM\u00B2", "I can't calculate COM squared" },
            { "COM\u00B3", "I could calculate COM cubed, i just dont feel like it" },
            { "LPT1", "I'm not going to print your skins" },
            { "LPT2", "I'm not going to print your skins" },
            { "LPT3", "does the L stand for ligma, no.... ok" },
            { "LPT4", "Who just has 4 printers" },
            { "LPT5", "Is this funny yet" },
            { "LPT6", "Really think, When was the last time you used a Parallel port" },
            { "LPT7", "hmmm i wonder if V1 has a Parallel port" },
            { "LPT8", "If you really need to print your skins, Try using Photoshop file > print" },
            { "LPT9", "Yes i really did spend my sunday morning writing dialog for each windows reserved name" },
            { "LPT\u00B9", "wow thats a fancy lookin unicode character, please dont use it" },
            { "LPT\u00B2", "i dont want to square your printer, you only need one" },
            { "LPT\u00B3", "Most printers are already cubes" },
            {"OG-SKINS", "Very funny" }


        };
    }
}
