using BatonPassLogger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UltraSkins.Utils;

namespace UltraSkins
{
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
