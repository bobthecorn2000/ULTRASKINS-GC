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
}
