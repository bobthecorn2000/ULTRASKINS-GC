using BatonPassLogger;
using BatonPassLogger.EX;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace UltraSkins.Utils
{
    internal class USFileUtilities
    {




        public static ModManagerInfo GetCurrentModManagerInfo()
        {

            string[] parts = USC.MODPATH.Split(Path.DirectorySeparatorChar);

            bool isInsideThunderstore = false;
            bool isInsideR2Modman = false;
            ModManagerType Dirtype = ModManagerType.Unknown;
            int keyint = 0;
            for (int i = 0; i < parts.Length - 1; i++)
            {
                if (parts[i].Equals("Thunderstore Mod Manager", StringComparison.OrdinalIgnoreCase))
                {
                    isInsideThunderstore = true;
                    Dirtype = ModManagerType.Thunderstore;
                    keyint = i;
                    break;
                }
                if (parts[i].Equals("r2modmanPlus-local", StringComparison.OrdinalIgnoreCase))
                {
                    isInsideR2Modman = true;
                    Dirtype = ModManagerType.R2modman;
                    keyint = i;
                    break;
                }
            }

            if (!isInsideThunderstore && !isInsideR2Modman)
                return null;

            for (int i = keyint; i < parts.Length - 1; i++)
            {
                if (parts[i].Equals("profiles", StringComparison.OrdinalIgnoreCase))
                {
                    return new ModManagerInfo(Dirtype, parts[i + 1]);
                }
            }

            return null;
        }


        internal void DirectorySetupChecker()
        {
            BatonPass.Info("Checking Directory Health");

            if (ProfileService.Instance == null)
            {
                throw new BPServiceNotStarted("ProfileService", "Directory Health cannot continue");

            }
            //TODO check for various creation issues like permissions
            if (!Directory.Exists(USC.GCDIR))
            {
                BatonPass.Warn("The AppData Directiory is missing. Fixing");
                Directory.CreateDirectory(USC.GCDIR);
                BatonPass.Success("Fixed");


            }
            if (!Directory.Exists(ProfileService.Instance.VersionDirectory))

            {
                BatonPass.Warn("The Versions Directiory is missing. Fixing");
                Directory.CreateDirectory(ProfileService.Instance.VersionDirectory);
                BatonPass.Success("Fixed");
            }
            if (!Directory.Exists(ProfileService.Instance.SaveDataDirectory))

            {
                BatonPass.Warn("The Current Profile's Directiory is missing. Fixing");
                Directory.CreateDirectory(ProfileService.Instance.SaveDataDirectory);
                BatonPass.Success("Fixed");
            }

            if (!Directory.Exists(ProfileService.Instance.GlobalSkinsDirectory))
            {
                BatonPass.Warn("The Global SaveData Directiory is missing. Fixing");
                Directory.CreateDirectory(ProfileService.Instance.GlobalSkinsDirectory);
                BatonPass.Success("Fixed");
            }
            BatonPass.Info("Done");
        }








    }


    public class AppliedSkinSaveSerialization
    {
        public AppliedSkinSaveSerialization()
        {
            if (ProfileService.Instance == null)
            {
                throw new BPServiceNotStarted("ProfileService", "Cannot Create Serializer");
            }
        }

        public void Save(string[] data)
        {
            AppliedSkinSaveInfo AppliedSkinSave = new AppliedSkinSaveInfo();
            AppliedSkinSave.ModVersion = USC.VERSION;
            AppliedSkinSave.SkinLocation = data;
            File.WriteAllText(ProfileService.Instance.DataFile, JsonConvert.SerializeObject(AppliedSkinSave));
        }

        public string[] Load()
        {
            try
            {
                string jsonData = File.ReadAllText(ProfileService.Instance.DataFile);
                AppliedSkinSaveInfo AppliedSkinSave = JsonConvert.DeserializeObject<AppliedSkinSaveInfo>(jsonData);
                if (AppliedSkinSave.ModVersion != USC.VERSION)
                {
                    BatonPass.Warn($"This file was saved with {AppliedSkinSave.ModVersion}. Current Version is {USC.VERSION}. It is possible it may not load correctly");
                }
                string[] SkinPaths = AppliedSkinSave?.SkinLocation;
                return SkinPaths;
            }
            catch (FileNotFoundException ex)
            {
                BatonPass.Error("the Applied skin file doesnt exist, Code - \"APPLIEDSKINSAVE-SAVE-FILENOTFOUND\"");
                BatonPass.Error(ex.ToString());
                return null;
            }
            catch (Exception ex)
            {
                BatonPass.Error("Something has gone wrong reading the Applied skin file, Code - \"APPLIEDSKINSAVE-SAVE-EX\"");
                BatonPass.Error(ex.ToString());
                return null;
            }



        }



    }



    public class metadataReader
    {
        public GCMD ReadMD(string file)
        {

            string GCMDreader = File.ReadAllText(file);
            try
            {
                GCMD gcmd = JsonConvert.DeserializeObject<GCMD>(GCMDreader);
                return gcmd;
            }
            catch (JsonReaderException ex)
            {
                BatonPass.Warn($"The metadata.GCMD File located at \"{file}\" could not be read, We think the error happened around line: {ex.LineNumber} character: {ex.LinePosition} . Code -\"MDR-READMD-METADATA_READ_WARNING\"");
                return null;
            }

        }
        public GCPACK ReadPack(string file)
        {

            string GCMDreader = File.ReadAllText(file);
            try
            {
                GCPACK gcPack = JsonConvert.DeserializeObject<GCPACK>(GCMDreader);
                return gcPack;
            }
            catch (JsonReaderException ex)
            {
                BatonPass.Warn($"The Pack.GCMD File located at \"{file}\" could not be read, We think the error happened around line: {ex.LineNumber} character: {ex.LinePosition} . Code -\"MDR-READPACK-PACK_READ_WARNING\"");
                return null;


            }
;
        }
        public TSjson ReadTSmani(string file)
        {
            string Jsonreader = File.ReadAllText(file);
            try
            {
                TSjson tsjson = JsonConvert.DeserializeObject<TSjson>(Jsonreader);
                return tsjson;
            }
            catch (JsonReaderException ex)
            {
                BatonPass.Warn($"The manifest.json File located at \"{file}\" could not be read, We think the error happened around line: {ex.LineNumber} character: {ex.LinePosition} . Code -\"MDR-TSMANI-THUNDERSTORE_MANIFEST_READ_WARNING\"");
                return null;
            }
        }
    }
    class metadataWriter
    {
        public async Task<MDWriteReturn> WriteMD(string Folder, GCMD gcmd)
        {

            string filepath = Path.Combine(Folder, USC.MDFILE);
            try
            {



                if (File.Exists(filepath))
                {
                    throw new BPFileExistsExc(filepath);
                }
                var MD = JsonConvert.SerializeObject(gcmd);
                await File.WriteAllTextAsync(filepath, MD);

                return MDWriteReturn.good();
            }
            catch (JsonException ex)
            {
                BatonPass.Error($"\"{filepath}\" failed to serialize . Code -\"MDW-WRITEMD-METADATA_WRITE_FAILURE\"");
                BatonPass.Error(ex.ToString());
                return MDWriteReturn.bad($"\"{filepath}\" failed to serialize .");
            }
            catch (BPFileExistsExc ex)
            {
                BatonPass.Error($"\"{filepath}\" already exists . Code -\"MDW-WRITEMD-METADATA_ALREADY_EXISTS\"");
                BatonPass.Error(ex.ToString());
                return MDWriteReturn.bad($"\"{filepath}\" already exists .");
            }
            catch (Exception ex)
            {

                BatonPass.Error($"an error occurred while trying to create {filepath}. Code -\"MDW-WRITEMD-EX\"");
                BatonPass.Error(ex.ToString());
                return MDWriteReturn.bad($"an error occurred while trying to create {filepath}");
            }



        }
    }

    public class FileSafety
    {
        public static UnsafeNotice CheckIfUnsafe(string input)
        {
            string normalized = input.Replace('\\', '/');


            if (normalized.Contains("../"))
            {
                return UnsafeNotice.Unsafe("I know what you're trying to do (._.)", "Please do not try and escape the confines of the Ultraskins folder (../).");
            }
            if (normalized.StartsWith("/"))
            {
                return UnsafeNotice.Unsafe("Save root folder access for another day", "Slashes are not permitted");

            }
            try
            {
                if (Path.IsPathRooted(input))
                {
                    return UnsafeNotice.Unsafe("Unfortunately we dont support your... \"unique\" name", "Skins may only be created in the Ultraskins folder");
                }
            }
            catch (ArgumentException) { }  // invalid chars caught below



            char[] invalidChars = Path.GetInvalidFileNameChars();
            char[] foundInvalid = input.Where(c => invalidChars.Contains(c)).Distinct().ToArray();
            if (foundInvalid.Length > 0)
            {
                if (foundInvalid.Length == 1)
                {
                    return UnsafeNotice.Unsafe("Hmm I dont recognize that symbol", $"Invalid character: {string.Join(" ", foundInvalid)}");
                }
                return UnsafeNotice.Unsafe("Sorry Ultraskins names may not contain ancient runes", $"Invalid characters: {string.Join(" ", foundInvalid)}");
            }
            string cleaned = new string(input.Where(c => !char.IsWhiteSpace(c)).ToArray());
            if (USC.ReservedNameJokes.TryGetValue(cleaned, out string value))
            {
                if (cleaned == "OG-SKINS")
                {
                    return UnsafeNotice.Unsafe(value, $"The name \"{cleaned}\" may not be used because I said so");
                }
                return UnsafeNotice.Unsafe(value, $"The name \"{cleaned}\" may not be used because microsoft said so");
            }




            return UnsafeNotice.Safe();
        }

        public struct UnsafeNotice
        {
            public bool IsSafe;
            public string Reason1;
            public string Reason2;
            public UnsafeNotice(bool isSafe, string reason1, string reason2 = "")
            {
                IsSafe = isSafe;
                Reason1 = reason1;
                Reason2 = reason2;

            }

            public static UnsafeNotice Safe() => new UnsafeNotice(true, "", "");
            public static UnsafeNotice Unsafe(string reason1, string reason2 = "") => new UnsafeNotice(false, reason1, reason2);
        }
    }



    internal struct MDWriteReturn
    {
        bool worked;
        string message;
        internal MDWriteReturn(bool Worked, string Message)
        {
            worked = Worked;
            message = Message;

        }
        public static MDWriteReturn good() => new MDWriteReturn(true, "");
        public static MDWriteReturn bad(string message) => new MDWriteReturn(false, message);
    }

    

}
