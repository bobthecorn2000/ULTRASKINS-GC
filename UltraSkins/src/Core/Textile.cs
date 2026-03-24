using BatonPassLogger;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UltraSkins.API;
using UltraSkins.Harmonic;
using UltraSkins.Utils;
using UltraSkins.UI;
using UnityEngine;
using static UltraSkins.API.USAPI;
using static UltraSkins.HoldEm;
using static UltraSkins.ULTRASKINHand;



namespace UltraSkins
{
    internal class TextileService
    {
        public static TextileService Instance { get; private set; }

        public static ServiceStartPackage StartService(TextileService SelfObject)
        {
            if (Instance != null)
            {
                BatonPass.Warn("Textile has already started and cannot be started again!");
                return new ServiceStartPackage(false, "Textile has already started and cannot be started again");
            }

            BatonPass.Info("Textile Service has started");
            Instance = SelfObject;
            return new ServiceStartPackage(true, "Textile was started Correctly");
        }




        /// <summary>
        /// Loads textures from disk async
        /// </summary>
        /// <param name="fpaths">paths to load</param>
        /// <param name="firsttime"></param>
        /// <returns></returns>
        public async Task LoadTextures(string[] fpaths, bool firsttime = false)
        {
            float ProgressTotal = 0;
            float ProgressDone = 0;

            if (firsttime == false)
            {
                HandInstance.BPGUI.ShowGUI("Loading:Gathering Info");
                BatonPass.Info("Loading: Gathering Info");


            }
            ProgressTotal = HoldEm.Instance.autoSwapCache.Count;
            foreach (string fpath in fpaths)
            {
                DirectoryInfo dir = new DirectoryInfo(fpath);
                FileInfo[] Files = dir.GetFiles("*.png");
                ProgressTotal += Files.Length;
            }
            BatonPass.Debug("BATON PASS: WE ARE IN LOADTEXTURES() we have the variable FPATH");



            HandInstance.BPGUI.EnableTerminal(10);
            HandInstance.BPGUI.ShowProgressBar(ProgressTotal);
            HoldEm.Fold(HoldemType.ASC);
            HoldEm.Fold(HoldemType.SC);


            System.Array.Reverse(fpaths);
            BatonPass.Debug("starting ForEach");
            bool failed = false;
            Dictionary<string, string> pathbook = new Dictionary<string, string>();
            foreach (string fpath in fpaths)
            {

                switch (TypeDetection(fpath))
                {
                    case ArchiveType.folder:
                        pathbook = QueryTexturesInFolder(fpath, pathbook);
                        break;
                }

            }
            TexOpData texOpData = await LoadOpDataFromPathBook(pathbook);
            await ConvertToTextures(texOpData);
            if (!failed)
            {
                HandInstance.BPGUI.DisableTerminal();
                HandInstance.BPGUI.HideProgressBar();

                HandInstance.BPGUI.BatonPassAnnoucement(Color.green, "success");
            }
            if (firsttime == false)
            {
                HandInstance.BPGUI.HideGUI(2);
            }



            USAPI.BroadcastTextureFinished(new USAPI.TextureLoadEventArgs(failed));

        }

        public ArchiveType TypeDetection(string path)
        {
            string ext = Path.GetExtension(path);
            if (ext.Equals(".zip", StringComparison.OrdinalIgnoreCase))
            {
                return ArchiveType.zip;
            }
            else
            {
                return ArchiveType.folder;
            }
        }




        public enum ArchiveType
        {
            folder,
            zip,
            gcskin,
        }

        public class TexOpData
        {
            public List<(string name, byte[] data)> RawData = new List<(string name, byte[] data)>();
            public float ProgressState { get; set; }
            public bool FailState { get; set; }
        }
        public class UniOpData
        {

            public float ProgressState { get; set; }
            public bool FailState { get; set; }
        }


        public Dictionary<string, string> QueryTexturesInFolder(string fpath, Dictionary<string, string> pathbook)
        {
            BatonPass.Info("ULTRASKINS IS SEARCHING " + fpath.ToString());

            DirectoryInfo dir = new DirectoryInfo(fpath);

            if (!dir.Exists)
            {
                HandInstance.BPGUI.DisableTerminal();
                HandInstance.BPGUI.BatonPassAnnoucement(Color.red, "failed, CODE - \"USHAND-LOADTEXTURES-DIR_NOT_FOUND\" \n FILEPATH:" + fpath);
                BatonPass.Error("Dir does not exist, CODE - \"USHAND-LOADTEXTURES-DIR_NOT_FOUND\" ");
                //failed = true;
                HandInstance.BPGUI.HideGUI(5);
                return pathbook; // Exit early if the directory doesn't exist
            }

            FileInfo[] Files = dir.GetFiles("*.png");
            BatonPass.Debug("Beginning file swap loop");

            if (Files.Length > 0)
            {
                int totalFiles = Files.Length;



                foreach (FileInfo file in Files)
                {
                    if (file.Exists)
                    {
                        pathbook[file.Name] = file.FullName;
                        BatonPass.Debug("found " + file.FullName);
                    }
                }
            }
            return pathbook;
        }

        public async Task<TexOpData> LoadOpDataFromPathBook(Dictionary<string, string> pathbook, float ProgressDone = 0, bool failed = false)
        {
            TexOpData texOpData = new TexOpData();
            foreach (KeyValuePair<string, string> kvp in pathbook)
            {
                try
                {

                    // Read file asynchronously
                    byte[] data = await File.ReadAllBytesAsync(kvp.Value);
                    string name = Path.GetFileNameWithoutExtension(kvp.Value);


                    BatonPass.Debug("Reading " + kvp.Value);
                    texOpData.RawData.Add((name, data));
                    HandInstance.BPGUI.AddTermLine("Creating " + name);

                }
                catch (Exception ex)
                {
                    BatonPass.Error("Error reading or processing texture file: " + kvp.Value + " Error: " + ex.Message);
                    failed = true;
                    HandInstance.BPGUI.DisableTerminal();
                    HandInstance.BPGUI.BatonPassAnnoucement(Color.red, "failed");
                }
            }
            return texOpData;
        }






        

        public async Task<UniOpData> ConvertToTextures(TexOpData texOpData)
        {
            UniOpData uniOpData = new UniOpData();
            bool failed = texOpData.FailState;
            float progress = texOpData.ProgressState;
            foreach ((string name, byte[] data) in texOpData.RawData)
            {
                Texture2D texture2D = new Texture2D(2, 2);
                texture2D.name = name;

                BatonPass.Debug("Creating " + texture2D.name);

                texture2D.filterMode = FilterMode.Point;
                texture2D.LoadImage(data);
                BatonPass.Debug("Loading Image Data");
                texture2D.Apply();


                // Cache the texture
                Texture texture = texture2D;
                BatonPass.Debug("Setting texture");
                HoldEm.Bet(HoldemType.ASC,name, texture);
                BatonPass.Debug("Adding to Cache " + texture.name + " " + name);
                progress++;
            }
            uniOpData.ProgressState = progress;
            uniOpData.FailState = failed;
            return uniOpData;
        }




        public string[] filepathArray;







        public void RefreshSkins(string[] clickedButtons)
        {
            BatonPass.Debug("BATON PASS: WE ARE IN REFRESHSKINS()");

            AppliedSkinSaveSerialization serializer = new AppliedSkinSaveSerialization();
            BatonPass.Debug("Created The Serializer");
            BatonPass.Debug("looking for the dll, appdata paths and merging the directory strings");

            

            List<string> filepath = new List<string>();
            foreach (string clickedButton in clickedButtons)
            {
                filepath.Add(Path.Combine(clickedButton));
                BatonPass.Debug("added " + clickedButton);
            }
            filepathArray = filepath.ToArray();
            foreach (string thing in filepathArray)
            {
                BatonPass.Debug(thing);
            }

            BatonPass.Debug("folderis: " + filepath);
            serializer.Save(filepathArray);
            BatonPass.Success("Saved to data.USGC");
            BatonPass.Debug("INIT BATON PASS: RELOADTEXTURES(TRUE," + filepath + ")");

            ReloadTextures(filepathArray, false);

            BatonPass.Debug("BATON PASS: WELCOME BACK TO REFRESHSKINS()");
            BatonPass.Info("Closing panel");

            //LoadTextures(filepath);

        }
        public void RefreshSkins()
        {


            AppliedSkinSaveSerialization serializer = new AppliedSkinSaveSerialization();
            BatonPass.Debug("Created The Serializer");
            BatonPass.Debug("looking for the dll, appdata paths and merging the directory strings");
            



            filepathArray = serializer.Load();
            BatonPass.Info("Read data.USGC from " + filepathArray);
            ReloadTextures(filepathArray, true);

            //LoadTextures(filepath);

        }


        /// <summary>
        /// Disables Buttons in the menu and awaits texture loading then dispatches them to their respective fractals
        /// </summary>
        /// <param name="path">the array of paths to load</param>
        /// <param name="firsttime">whether or not the menu should disable buttons</param>
        public async void ReloadTextures(string[] path, bool firsttime = false)
        {
            BatonPass.Debug("BATON PASS: WE ARE IN ReloadTextures() We have variables \n FIRSTTIME:" + firsttime + "\n PATH:" + path);

            if (!firsttime)
            {
                MenuManager.MMinstance.DisableButtons();
                await LoadTextures(path, firsttime);
                MenuManager.MMinstance.EnableButtons();
            }
            else
            {
                await LoadTextures(path, firsttime);
            }
            USAPI.BroadcastTextureUpdate(this, new FractalTextureUpdateArgs(true));

        }
    }
}
