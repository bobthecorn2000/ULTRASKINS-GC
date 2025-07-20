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
    public class BPBadSettingsValue: BPException
    {
        public string SettingName { get; }
        public string SettingValue{ get; }

        public BPBadSettingsValue(string settingsname, string settingvalue) : base($"SettingsValue ID \"{settingsname}\" is set to a value of {settingvalue} which is not allowed")
        {
            SettingName = settingsname;
            SettingValue = settingvalue;
        }

        public BPBadSettingsValue(string settingsname, string settingvalue, Exception inner) : base($"SettingsValue ID \"{ settingsname}\" is set to a value of {settingvalue} which is not allowed", inner)

        {
            SettingName = settingsname;
            SettingValue = settingvalue;
        }
    }
}
