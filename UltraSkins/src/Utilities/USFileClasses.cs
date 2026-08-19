using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace UltraSkins.Utils
{
    public class Metadata
    {
        public string FileVersion { get; set; }
        public string FileName { get; set; }
        public string FileDescription { get; set; }
    }

    //Currently Applied skins
    public class AppliedSkinSaveInfo
    {
        public string ModVersion { get; set; }
        public string[] SkinLocation { get; set; }
    }
    public class TSjson
    {



        public string name { get; set; }
        public string description { get; set; }

        public string version_number { get; set; }
    }


    /// <summary>
    /// Metadata Object
    /// </summary>
    public class GCMD
    {
        public string SkinName { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? IconOveride { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Version { get; set; }
        public string PackFormat { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string>? SupportedPlugins { get; set; }
    }

    /// <summary>
    /// Pack Object
    /// </summary>
    public class GCPACK
    {
        public string PackName { get; set; }
        public string[] SubDirectories { get; set; }
    }
}
