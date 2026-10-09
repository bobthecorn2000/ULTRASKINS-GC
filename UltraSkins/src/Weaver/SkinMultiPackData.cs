using BatonPassLogger;
using GameConsole.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UltraSkins.Utils;

namespace UltraSkins.Weaver
{
    public class SkinMultiPackData
    {
        //Folder Info
        DirectoryInfo directoryInfo;
        string folderName;
        SkinProfileDirType skinProfType;
        public FolderStructureType StructType { get; private set; }
        List<SkinPackNotice> notices = new List<SkinPackNotice>();
        public List<SkinPackData> SubPacks = new List<SkinPackData>();
        public string PackName { get; private set; }


        public SkinMultiPackData(DirectoryInfo dirInfo, SkinProfileDirType sProfDirType, FolderStructureType FST)
        {
            BatonPass.Debug("Constructing MultiPack Info for:" + dirInfo.Name);
            folderName = dirInfo.Name;
            if (!dirInfo.Exists)
            {
                BatonPass.Error($"{folderName} does not exist");
                throw new DirectoryNotFoundException();
            }
            StructType = FST;
            directoryInfo = dirInfo;
            skinProfType = sProfDirType;
        }

        void SetupMultiPack()
        {

            FileInfo[] files = directoryInfo.GetFiles(USC.PACKFILE);
            if (files.Length > 0)
            {

                FileInfo PackFileInfo = files.FirstOrDefault();
                metadataReader MDR = new metadataReader();

                try
                {
                    GCPACK PD = MDR.ReadPack(PackFileInfo.FullName);
                    foreach (string path in PD.SubDirectories)
                    {

                        FileSafety.UnsafeNotice safe = FileSafety.CheckIfUnsafe(path);
                        if (safe.IsSafe)
                        {
                            string subsubfolder = Path.Combine(directoryInfo.FullName, path);
                            if (Directory.Exists(subsubfolder) && safe.IsSafe)
                            {
                                DirectoryInfo newDir = new DirectoryInfo(subsubfolder);
                                FolderStructureType Structure = PacMan.DetermineStructureType(newDir);
                                if (Structure == FolderStructureType.Loose)
                                {
                                    SkinPackData skinpack = new SkinPackData(newDir, skinProfType, Structure);
                                }
                                else
                                {
                                    switch (Structure)
                                    {
                                        case FolderStructureType.Raw:
                                            BatonPass.Warn($"MultiPacks do not support SubPacks without declaired MetaData. {subsubfolder} will not be generated");
                                            break;

                                        case FolderStructureType.CompressedGCSKIN:
                                        case FolderStructureType.CompressedZip:
                                            BatonPass.Warn($"MultiPacks do not support compressed types. {subsubfolder} will not be generated");
                                            //TODO notification
                                            break;

                                        case FolderStructureType.MultiPack:
                                            BatonPass.Warn($"MultiPacks do not support nested MultiPacks. {subsubfolder} will not be generated");
                                            //TODO notification
                                            break;
                                        case FolderStructureType.Conflicting:
                                            BatonPass.Warn($"SubPack contains. {subsubfolder} will not be generated");
                                            break;


                                    }
                                    // TODO notification alert logic
                                }

                            }
                            else
                            {
                                BatonPass.Warn("\"" + subsubfolder + "\" does not exist in the Pack. This is most likely an incorrect folder name in the Pack.GCMD file located in \"" + PackFileInfo.FullName + "\". Code -\"SMPD-PACKPATH_MISSING_LOCATION\"");
                                // TODO notification alert logic
                            }
                        }
                        else
                        {
                            BatonPass.Warn("Refusing to load Subdirectory at " + path);
                            BatonPass.Warn(safe.Reason1);
                            BatonPass.Warn(safe.Reason2);
                            // TODO notification alert logic
                        }

                    }



                }
                catch (Exception ex)
                {

                    BatonPass.Error($"Could not reach subdirectories due to a possible failed deserialization,this error should be accompanied by \"MDR-READPACK-PACK_READ_WARNING\". Pack {PackFileInfo.Name} will be skipped. CODE -\"MMAN-MAINMENU-PACK_READ_FAILURE\"");
                    BatonPass.Error(ex.Message);
                }






            }
        }
    }
}
