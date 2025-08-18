using System;
using System.Collections.Generic;
using System.Text;

namespace BatonPassLogger.EX
{
    public class BPException : Exception
    {
        public BPException() { }

        public BPException(string message) : base(message) { }

        public BPException(string message, Exception inner) : base(message, inner) { }
    }



    /// <summary>
    /// Used when batonpass finds a file that exists but shouldnt yet
    /// </summary>
    public class BPFileExistsExc : BPException
    {
        public string TargetPath { get; }

        public BPFileExistsExc(string path) : base($"File already exists at: {path}")
        {
            TargetPath = path;
        }

        public BPFileExistsExc(string path, Exception inner) : base($"File already exists at: {path}", inner)

        {
            TargetPath = path;
        }
    }

    /// <summary>
    /// Used when BatonPass finds a settings value that doesnt make sense in the current context
    /// </summary>
    public class BPBadSettingsValue : BPException
    {
        public string SettingName { get; }
        public string SettingValue { get; }

        public BPBadSettingsValue(string settingsname, string settingvalue) : base($"SettingsValue ID \"{settingsname}\" is set to a value of {settingvalue} which is not allowed")
        {
            SettingName = settingsname;
            SettingValue = settingvalue;
        }

        public BPBadSettingsValue(string settingsname, string settingvalue, Exception inner) : base($"SettingsValue ID \"{settingsname}\" is set to a value of {settingvalue} which is not allowed", inner)

        {
            SettingName = settingsname;
            SettingValue = settingvalue;
        }
    }


    /// <summary>
    /// Used when we have a custom file format return unexpected data
    /// </summary>
    public class BPBadData : BPException
    {
        public string TargetPath { get; }

        public BPBadData(string path) : base($"File at \"{path}\" returned an unexpected value")
        {
            TargetPath = path;
        }

        public BPBadData(string path, Exception inner) : base($"File at \"{path}\" returned an unexpected value", inner)

        {
            TargetPath = path;
        }
    }
}
