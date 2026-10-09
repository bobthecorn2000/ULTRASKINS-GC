using BatonPassLogger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace UltraSkins.Weaver
{
    public class SearchableSkinProfileInfo
    {
        public SkinProfileDirType DirectoryType { get; private set; }
        public string ProfileName { get; private set; }

        public DirectoryInfo ProfileLocation { get; private set; }

        public List<SkinPackData> ProfileSkinPacks { get; private set; } = new List<SkinPackData>();
        public List<SkinMultiPackData> ProfileMultiSkinPacks { get; private set; } = new List<SkinMultiPackData>();

        public bool ShouldSearch { get; set; }

        public SearchableSkinProfileInfo(SkinProfileDirType dirtype, string profName, string profLocation, bool searchable = true)
        {
            BatonPass.Debug($"Making Search Location for {dirtype.ToString()}:{profName} at {profLocation} ");
            ProfileLocation = new DirectoryInfo(profLocation);
            if (!ProfileLocation.Exists)
            {
                throw new DirectoryNotFoundException();
            }
            ProfileName = profName;
            ShouldSearch = searchable;
            DirectoryType = dirtype;
            RefreshSubFolders();
            //SkinPackFolders = ProfileLocation.GetDirectories().ToList();
        }

        public void RefreshSubFolders()
        {
            ProfileSkinPacks.Clear();
            ProfileMultiSkinPacks.Clear();
            DirectoryInfo[] subDirs = ProfileLocation.GetDirectories();

            foreach (DirectoryInfo subfolder in subDirs)
            {

                FolderStructureType foldstruct = PacMan.DetermineStructureType(subfolder);

                if (foldstruct == FolderStructureType.Loose)
                {
                    SkinPackData skinpackdata = new SkinPackData(subfolder, DirectoryType,foldstruct);
                    ProfileSkinPacks.Add(skinpackdata);
                }
                if (DirectoryType != SkinProfileDirType.Thunderstore || DirectoryType != SkinProfileDirType.R2modman){
                    if (foldstruct == FolderStructureType.Raw)
                    {
                        SkinPackData skinpackdata = new SkinPackData(subfolder, DirectoryType, foldstruct);
                        ProfileSkinPacks.Add(skinpackdata);
                    }
                    if (foldstruct == FolderStructureType.MultiPack)
                    {
                      
                    }
                }
                else if (foldstruct == FolderStructureType.MultiPack)
                {
                    BatonPass.Warn("Using MultiPacks outside of the Global Folder is unsupported");
                }
                

            }
            //SkinPackFolders = ProfileLocation.GetDirectories().ToList();
        }

        private void DetermineFolderType()
        {

        }
    }
}
