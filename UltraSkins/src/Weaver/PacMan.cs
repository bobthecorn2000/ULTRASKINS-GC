using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BatonPassLogger;
using System.Linq;
namespace UltraSkins.Weaver
{
    public enum SkinProfileDirType
    {
        Other,
        Global,
        Version,
        R2modman,
        Thunderstore,
        Gale, //gale is unsupported but this is here for if its added later


    }

    public enum ModManagerType
    {
        Unknown,
        R2modman,
        Thunderstore,
        Gale, //gale is unsupported but this is here for if its added later

    }

    public enum FolderStructureType
    {

        Raw,
        Loose,
        MultiPack,
        CompressedZip,
        CompressedGCSKIN,
        Conflicting,
    }

    public class PacMan
    {





        public static FolderStructureType? DetermineStructureType(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                BatonPass.Warn("a folder structure could not be detected: Reason: Empty String");
                return null;
            }
            if (Directory.Exists(path))
            {
                return DetermineStructureType(new DirectoryInfo(path));
            }
            if (File.Exists(path))
            {
                return DetermineStructureType(new FileInfo(path));
            }
            BatonPass.Warn("a folder structure could not be detected: Reason: Unknown");
            return null;
        }
        public static FolderStructureType DetermineStructureType(DirectoryInfo dirInfo)
        {
            bool hasPackFile = false;
            bool hasMdFile = false;

            foreach (FileInfo file in dirInfo.EnumerateFiles())
            {
                ReadOnlySpan<char> fileName = file.Name.AsSpan();
                if (!hasPackFile && file.Name.Equals(USC.PACKFILE, StringComparison.OrdinalIgnoreCase))
                {
                    hasPackFile = true;
                }
                    

                if (!hasMdFile && file.Name.Equals(USC.MDFILE, StringComparison.OrdinalIgnoreCase))
                {
                    hasMdFile = true;
                }
                    

                
                if (hasPackFile && hasMdFile)
                {
                    return FolderStructureType.Conflicting;
                    
                }

                

            }
            if (hasPackFile)
            {
                return FolderStructureType.MultiPack;
            }
            if (hasMdFile)
            {
                return FolderStructureType.Loose;
            }

            return FolderStructureType.Raw;
        }
        public static FolderStructureType? DetermineStructureType(FileInfo fileInfo)
        {
             
            ReadOnlySpan<char> ext = fileInfo.Extension.AsSpan();
            if (ext.Equals(".zip", StringComparison.OrdinalIgnoreCase) || ext.Equals(".gczip", StringComparison.OrdinalIgnoreCase))
            {
                return FolderStructureType.CompressedZip;
            }
            if (ext.Equals(".gcskin", StringComparison.OrdinalIgnoreCase))
            {
                return FolderStructureType.CompressedGCSKIN;
            }
            BatonPass.Warn("a folder structure could not be detected for the File given: Reason: Not a valid file type");
            return null;
        }



    }



}

