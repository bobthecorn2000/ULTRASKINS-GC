using BatonPassLogger;
using BepInEx;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using TMPro;
using UltraSkins.UI;
using UltraSkins.Utils;
using UnityEngine;
using static System.Net.WebRequestMethods;

namespace UltraSkins.Utils
{
    public class SkinPackData
    {
        //Folder Info
        DirectoryInfo directoryInfo;
        string folderName;
        SkinProfileDirType skinProfType;
        List<SkinPackNotice> notices = new List<SkinPackNotice>();
        public int ImageCount { get; private set; } = 0;
        public List<FileInfo> Images { get; private set; } = new List<FileInfo>();

        //metadata info if applicable
        public string Name { get; private set; }
        public string Author { get; private set; } = "Unknown";
        public string Description { get; private set; } = "No Info";
        public string Version { get; private set; }

        Dictionary<string, string> plugins;
        public string PackFormat { get; private set; }

        string IconOverride;
        Texture2D packIcon;

        

        //Flags
        public bool HiddenInMenu { get; private set; } = false;
        public bool hasMetadata { get; private set; } = false;
        public bool hasThunderstoreManifest { get; private set; }  = false;
        public bool hasPlugins { get; private set; } = false;


        public SkinPackData(DirectoryInfo dirInfo, SkinProfileDirType sProfDirType)
        {
            BatonPass.Debug("Constructing Info for:" + dirInfo.Name);
            folderName = dirInfo.Name;
            if (!dirInfo.Exists)
            {
                BatonPass.Error($"{folderName} does not exist");
                throw new DirectoryNotFoundException();
            }
            directoryInfo = dirInfo;
            skinProfType = sProfDirType;
            SetupMetadata();
            FileInfo[] packs = directoryInfo.GetFiles(USC.PACKFILE);
            if (packs.Length > 0)
            {
                BatonPass.Error("The Pack {name} contains a pack.GCMD file which is currently unsupported.");
            }
            FileInfo[] allImages = directoryInfo.GetFiles("*.png");
            ImageCount = allImages.Length;
            
        }
        void SetupMetadata()
            {
                FileInfo[] files = directoryInfo.GetFiles(USC.MDFILE);
                if (files.Length > 0)
                {


                    FileInfo MDFileInfo = files.FirstOrDefault();
                    metadataReader MDR = new metadataReader();
                    try
                    {
                        GCMD MD = MDR.ReadMD(MDFileInfo.FullName);
                        if (MD != null)
                        {
                            if (MD.PackFormat.IsNullOrWhiteSpace())
                            {
                                notices.Add(new SkinPackNotice(USC.Severity.Warning, "Pack Format", "Pack Format is missing, Skin may have issues"));

                            }
                            else
                            {
                                if (!USC.SupportedPackFormats.Contains(MD.PackFormat))
                                {
                                    notices.Add(new SkinPackNotice(USC.Severity.Warning, "Pack Format", "Made for a different version of ultraskins"));
                                }

                            }
                            if (!MD.Description.IsNullOrWhiteSpace())
                            {
                                Description = MD.Description;
                            }
                            else
                            {
                                Description = "No Info";
                            }
                            if (!MD.SkinName.IsNullOrWhiteSpace())
                            {
                                Name = MD.SkinName;

                            }
                            else
                            {
                                Name = folderName;
                            }
                            if (!MD.Author.IsNullOrWhiteSpace())
                            {
                                Author = MD.Author;
                            }
                            else
                            {
                                Author = "Unknown";
                            }

                            if (MD.SupportedPlugins != null && MD.SupportedPlugins.Count >= 1)
                            {
                                hasPlugins = true;
                            }
                            if (!MD.Version.IsNullOrWhiteSpace())
                            {
                                Version = MD.Version;
                            }

                            if (!string.IsNullOrWhiteSpace(MD.IconOveride))
                            {
                                FileSafety.UnsafeNotice unsafeNotice = FileSafety.CheckIfUnsafe(MD.IconOveride);
                                if (unsafeNotice.IsSafe)
                                {
                                    //ive never done this, this way before. so hopefully this doesnt explode on me
                                    Iconic.ICFinder(directoryInfo.FullName, this, MD.IconOveride);
                                }
                                else
                                {
                                    BatonPass.Warn("Skipping Unsafe Icon for " + MD.IconOveride + "reason:" + unsafeNotice.Reason1 + " " + unsafeNotice.Reason2);
                                }
                            }
                            else
                            {
                                Iconic.ICFinder(directoryInfo.FullName, this);
                            }
                            hasMetadata = true;


                        }
                        else
                        {

                            Name = folderName;
                            notices.Add(new SkinPackNotice(USC.Severity.Warning, "Pack Metadata", "Metadata file is null"));
                            Iconic.ICFinder(directoryInfo.FullName, this);
                        }
                    }
                    catch (Exception ex)
                    {
                        BatonPass.Error("Something went wrong with the metadata formatting, falling back");


                        BatonPass.Error(ex.ToString());
                        notices.Add(new SkinPackNotice(USC.Severity.Error, ex.Message, ex.StackTrace));
                    }
                }
                else
                {
                    Name = folderName;

                }

            }
        void SetupTSMani()
        {

        }


        public void SetSkinPackIcon(Texture2D tex)
        {
            packIcon = tex;
        }
        public void SetupBEM(ButtonEnableManager BEM)
        {
            BatonPass.Debug("setting up BEM for " + Name);
            BEM.filePath = directoryInfo.FullName;
            BEM.SkinName = Name;
            BEM.SkinDescription = Description;
            BEM.Author = Author;
            if (packIcon != null)
            {
                BEM.RawIcon = packIcon;
                
            }
            if (skinProfType == SkinProfileDirType.Thunderstore || skinProfType == SkinProfileDirType.R2modman)
            {
                BEM.thunderstore = true;
            }
            if (hasPlugins)
            {
                BEM.isplugin = true;
            }

            //generate strings from issues
            if (notices.Count > 0)
            {
                
                StringBuilder builder = new StringBuilder();
                foreach (SkinPackNotice notice in notices)
                {
                    builder.AppendJoin("\n", notice.GetNoticeString());

                }
                BEM.ET = ButtonEnableManager.ErrorType.Warning;
                BEM.warningmessage = builder.ToString();
            }


        }

    }


    public class SkinPackNotice
    {
        USC.Severity severity;
        string title;
        string message;

        public SkinPackNotice(USC.Severity sev, string inputTitle, string inputMessage)
        {
            severity = sev;
            title = inputTitle;
            message = inputMessage;
        }

        public string GetNoticeString()
        {
            return $"[{severity}] {title}: {message}";
        }
    }





}

