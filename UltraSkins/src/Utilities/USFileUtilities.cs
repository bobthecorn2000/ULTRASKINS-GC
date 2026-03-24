using BatonPassLogger;
using BatonPassLogger.EX;
using Newtonsoft.Json;
using System;
using System.IO;


namespace UltraSkins.Utils
{
    internal class USFileUtilities
    {




        public static ModManagerInfo GetThunderstoreProfileName()
        {

            string[] parts = USC.MODPATH.Split(Path.DirectorySeparatorChar);

            bool isInsideThunderstore = false;
            bool isInsideR2Modman = false;
            string Dirtype = null;
            int keyint = 0;
            for (int i = 0; i < parts.Length - 1; i++)
            {
                if (parts[i].Equals("Thunderstore Mod Manager", StringComparison.OrdinalIgnoreCase))
                {
                    isInsideThunderstore = true;
                    Dirtype = "Thunderstore";
                    keyint = i;
                    break;
                }
                if (parts[i].Equals("r2modmanPlus-local", StringComparison.OrdinalIgnoreCase))
                {
                    isInsideR2Modman = true;
                    Dirtype = "r2modman";
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
            if (!Directory.Exists(ProfileService.Instance.ProfileDirectory))

            {
                BatonPass.Warn("The Current Profile's Directiory is missing. Fixing");
                Directory.CreateDirectory(ProfileService.Instance.ProfileDirectory);
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


    
}
