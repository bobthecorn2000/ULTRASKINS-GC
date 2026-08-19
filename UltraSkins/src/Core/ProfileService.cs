using BatonPassLogger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UltraSkins.Utils;

namespace UltraSkins
{
    internal class ProfileService
    {
        //ServiceLocation
        public static ProfileService Instance { get; private set; }



        /// <summary>
        /// The Directory to the Profile subfolder. at ultraskinsGC-V2/SaveData/buildtype or ultraskinsGC-V2/SaveData/profiletype/profilename
        /// </summary>
        public string SaveDataDirectory { get; private set; }


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


        /// <summary>
        /// an object containing the current Mod managers type
        /// </summary>
        public ModManagerInfo MMI { get; private set; }


        private List<SearchableSkinProfileInfo> SkinSearchDirectories = new List<SearchableSkinProfileInfo>();

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
            SelfObject.MMI = USFileUtilities.GetCurrentModManagerInfo();
            if (SelfObject.MMI == null)
            {
                BatonPass.Debug("none found");
                SelfObject.SaveDataDirectory = Path.Combine(USC.GCDIR, USC.SAVEDATA, USC.BUILDTYPE);

            }
            else
            {
                BatonPass.Info($"The Mod Manager {SelfObject.MMI.DirectoryType} was found. Setting Profile Name to {SelfObject.MMI.ProfileName}");
                SelfObject.SaveDataDirectory = Path.Combine(USC.GCDIR, USC.SAVEDATA, SelfObject.MMI.DirectoryType.ToString(), SelfObject.MMI.ProfileName);
            }
            SelfObject.DataFile = Path.Combine(SelfObject.SaveDataDirectory, USC.DATAFILE);


            Instance = SelfObject;
            return new ServiceStartPackage(true, "ProfileInfo was started Correctly");
        }


        public static void AddDirToSearch(SkinProfileDirType ProfType,string ProfName,string profPath)
        {
            throw new NotImplementedException();
        }

        public static Dictionary<string, string> GetActiveSearchPaths()
        {
            throw new NotImplementedException();
        }

    }




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


    public class SearchableSkinProfileInfo
    {
        public SkinProfileDirType DirectoryType { get; private set; }
        public string ProfileName { get; private set; }

        public DirectoryInfo ProfileLocation { get; private set; }

        public List<SkinPackData> ProfileSkinPacks { get; private set; } = new List<SkinPackData>();

        public bool ShouldSearch { get; set; }

        public SearchableSkinProfileInfo(SkinProfileDirType dirtype, string profName, string profLocation,bool searchable = true)
        {
            BatonPass.Debug($"Making Search Location for {dirtype.ToString()}:{profName} at {profLocation} ");
            ProfileLocation = new DirectoryInfo(profLocation);
            if (!ProfileLocation.Exists)
            {
                throw new DirectoryNotFoundException();
            }
            //SkinPackFolders = ProfileLocation.GetDirectories().ToList();
        }

        public void RefreshSubFolders()
        {
            //SkinPackFolders = ProfileLocation.GetDirectories().ToList();
        }

    }

    internal class ModManagerInfo
    {
        public ModManagerType DirectoryType { get; private set; }
        public string ProfileName { get; private set; }

        public ModManagerInfo(ModManagerType dirtype, string proftype)
        {
            DirectoryType = dirtype;
            ProfileName = proftype;
        }
    }
}
