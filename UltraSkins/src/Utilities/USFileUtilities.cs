using BatonPassLogger;
using BatonPassLogger.EX;
using System;
using System.IO;
using UltraSkins.Utils;

namespace UltraSkins
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
    internal class ProfileService
    {
        //ServiceLocation
        public static ProfileService Instance { get; private set; }



        /// <summary>
        /// The Directory to the Profile subfolder. at ultraskinsGC-V2/SaveData/buildtype or profiletype/profilename
        /// </summary>
        public string ProfileDirectory { get; private set; }


        /// <summary>
        /// The Version specific Directory at ultraskinsGC-V2/Versions/versionum
        /// </summary>
        public readonly string VersionDirectory = Path.Combine(USC.GCDIR, USC.VERNAME, USC.VERSION);

        /// <summary>
        /// The GlobalSkins Directory at ultraskinsGC-V2/GlobalSkins
        /// </summary>
        public readonly string GlobalSkinsDirectory = Path.Combine(USC.GCDIR, USC.UNISKIN);


        /// <summary>
        /// CURRENTLY UNUSED
        /// The Prism Directory at ultraskinsGC-V2/SaveData/buildtype/Prism or profiletype/profilename/Prism
        /// </summary>
        public string PrismSaveDirectory { get; private set; }

        /// <summary>
        /// the path to the Data file with the last used mod version and the currently saved skin packs
        /// </summary>
        public string DataFile { get; private set; }

        public ModManagerInfo MMI { get; private set; }

        public static ServiceStartPackage StartService(ProfileService SelfObject)
        {
            //Early return if profile info already exists
            if (Instance != null)
            {
                BatonPass.Warn("ProfileInfo has already started and cannot be started again!");
                return new ServiceStartPackage(false, "ProfileInfo has already started and cannot be started again");
            }
            BatonPass.Info("ProfileInfo Service has started");

            //find if we are running in a mod manager
            BatonPass.Debug("scanning for a mod manager");
            SelfObject.MMI = USFileUtilities.GetThunderstoreProfileName();
            if (SelfObject.MMI == null)
            {
                BatonPass.Debug("none found");
                SelfObject.ProfileDirectory = Path.Combine(USC.GCDIR, USC.SAVEDATA, USC.BUILDTYPE);

            }
            else
            {
                BatonPass.Info($"The Mod Manager {SelfObject.MMI.DirectoryType} was found. Setting Profile Name to {SelfObject.MMI.ProfileName}");
                SelfObject.ProfileDirectory = Path.Combine(USC.GCDIR, USC.SAVEDATA, SelfObject.MMI.DirectoryType, SelfObject.MMI.ProfileName);
            }
            SelfObject.DataFile = Path.Combine(SelfObject.ProfileDirectory, USC.DATAFILE);


            Instance = SelfObject;
            return new ServiceStartPackage(true, "ProfileInfo was started Correctly");
        }

    }

    internal class ModManagerInfo
    {
        public string DirectoryType { get; private set; }
        public string ProfileName { get; private set; }

        public ModManagerInfo(string dirtype, string proftype)
        {
            DirectoryType = dirtype;
            ProfileName = proftype;
        }
    }
    
}
