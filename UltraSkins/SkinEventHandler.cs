
using UnityEngine;
using System.IO;
using GameConsole;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using Ionic.Zlib;
using System;

using BatonPassLogger;
using System.Text.Json;
using System.Text;
using UnityEngine.Profiling.Memory.Experimental;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace UltraSkins.Utils
{
	public class SkinEventHandler : MonoBehaviour
	{
        //static PluginConfigurator config;
        
        
        
        public GameObject Activator;
		public string path;
		public string pname;
		public static ULTRASKINHand UKSH = ULTRASKINHand.HandInstance;
        public class Metadata
        {
            public string FileVersion { get; set; }
            public string FileName { get; set; }
            public string FileDescription { get; set; }
        }
        public class saveinfo
        {
            public string ModVersion { get; set; }
            public string[] SkinLocation { get; set; }
        }
/*        private void Update()
		{
			if (Activator != null && Activator.activeSelf)
			{
				Activator.SetActive(false);
				UKSH.ReloadTextures(false, path);
				string[] folder = GetModFolderPath();
				TextureOverWatch[] TOWS = GameObject.FindWithTag("MainCamera").GetComponentsInChildren<TextureOverWatch>(true);
                ULTRASKINHand.BatonPass("update ran");
				foreach (TextureOverWatch TOW in TOWS)
				{
					if (TOW && TOW.gameObject)
					{
						TOW.enabled = true;
                    }
				}
                
			}
		}*/

        public static string getDataFile()
        {
            
            
            string[] profileInfo = UKSH.ThunderProfInfo;
            string ProfDir = null;
            
            if (profileInfo != null && profileInfo.Length == 2)
            {
                string profileName = profileInfo[0];
                string profileType = profileInfo[1];
                ProfDir = Path.Combine(USC.GCDIR, USC.SAVEDATA, profileType, profileName, USC.DATAFILE);
                
            }
            else
            {
                ProfDir = Path.Combine(USC.GCDIR, USC.SAVEDATA, USC.BUILDTYPE, USC.DATAFILE);
            }
                return ProfDir;
        }
        public static string getUserSettingsFile()
        {


            string[] profileInfo = UKSH.ThunderProfInfo;
            string ProfDir = null;

            if (profileInfo != null && profileInfo.Length == 2)
            {
                string profileName = profileInfo[0];
                string profileType = profileInfo[1];
                ProfDir = Path.Combine(USC.GCDIR, USC.SAVEDATA, profileType, profileName, USC.SETTINGSFILE);

            }
            else
            {
                ProfDir = Path.Combine(USC.GCDIR, USC.SAVEDATA, USC.BUILDTYPE, USC.SETTINGSFILE);
            }
            return ProfDir;
        }

        public string[] GetModFolderPath()
        {
            BatonPass.Debug("BATON PASS: GETMODFOLDERPATH()");
            // Get the path to the current directory where the game executable is located
            //string gameDirectory = Assembly.GetExecutingAssembly().Location;
            //string gameDirectory = Path.GetDirectoryName(Application.dataPath);
            

            

            string VerDir = Path.Combine(USC.GCDIR, USC.VERNAME, USC.VERSION);
            
            string ProfDir = null;
            string[] profileInfo = UKSH.ThunderProfInfo;
            if (profileInfo != null && profileInfo.Length == 2)
            {
                string profileName = profileInfo[0];
                string profileType = profileInfo[1];
                ProfDir = Path.Combine(USC.GCDIR, USC.SAVEDATA, profileType, profileName);
            }
            else
            {
              ProfDir = Path.Combine(USC.GCDIR, USC.SAVEDATA, USC.BUILDTYPE);
            }
            string GlobalDir = Path.Combine(USC.GCDIR, USC.UNISKIN);
            string[] parts = USC.MODPATH.Split(Path.DirectorySeparatorChar);
            string savelocation = null;






            BatonPass.Debug("Checking Directories");
            if (!Directory.Exists(USC.GCDIR))
            {
                BatonPass.Warn("The AppData Directiory is missing. Fixing");
                Directory.CreateDirectory(USC.GCDIR);
                BatonPass.Success("Fixed");
                
                
            }
            if (!Directory.Exists(VerDir))
            
                {
                BatonPass.Warn("The Versions Directiory is missing. Fixing");
                Directory.CreateDirectory(VerDir);
                BatonPass.Success("Fixed");
            }
            if (ProfDir != null)
            {
                if (!Directory.Exists(ProfDir))

                {
                    BatonPass.Warn("The Current Profile's Directiory is missing. Fixing");
                    Directory.CreateDirectory(ProfDir);
                    BatonPass.Success("Fixed");
                }
            }
            if (!Directory.Exists(GlobalDir)) {
                BatonPass.Warn("The Global SaveData Directiory is missing. Fixing");
                Directory.CreateDirectory(GlobalDir);
                BatonPass.Success("Fixed");
            }
            BatonPass.Debug("Done");


            string[] defloc = new string[0];
            if (ProfDir != null) {
                savelocation = ProfDir;
            }
            


                StringSerializer serializer = new StringSerializer();
            string filecheck = Path.Combine(savelocation, USC.DATAFILE);
            if (!File.Exists(Path.Combine(savelocation, USC.DATAFILE)))
            {
                serializer.SerializeStringToFile(defloc, filecheck);

            }
            if (!File.Exists(Path.Combine(savelocation, USC.SETTINGSFILE)))
            {
                File.Create(Path.Combine(savelocation, USC.SETTINGSFILE));

            }

            string[] deserializedData = serializer.DeserializeStringFromFile(filecheck);
            // Combine the game directory with the mod folder name to get the full path
            //return gameDirectory+modFolderName;
            if (deserializedData[0] == "deserializedData Failed") {
                serializer.SerializeStringToFile(defloc, filecheck);
                deserializedData = serializer.DeserializeStringFromFile(filecheck);
            }
            if (deserializedData[0] == "Wrong Version")
            {
                deserializedData = serializer.DeserializeStringFromFile(filecheck);
            }

                    return deserializedData;
        }

        public static Dictionary<String,String> GetCurrentLocations()
        {


            string parentDir = Directory.GetParent(USC.MODPATH).FullName;

            string[] profileInfo = UKSH.ThunderProfInfo;
            string profileName = null;
            string profileType = null;
            
            string GlobalDir = Path.Combine(USC.GCDIR, USC.UNISKIN);
            //string VerDir = Path.Combine(USC.GCDIR, USC.VERNAME,USC.VERSION);

            Dictionary<String, String> Locations = new Dictionary<String,String>();


            Locations.Add("Global", GlobalDir);
            //Locations.Add("Version", VerDir);
            BatonPass.Info("Location Found: " + "Global" + " " + GlobalDir);
            if (profileInfo != null && profileInfo.Length == 2)
            {
                profileName = profileInfo[0];
                profileType = profileInfo[1];
                BatonPass.Info("Location Found: " +  profileType + " " + parentDir);
                Locations.Add(profileType, parentDir);
                

            }
            return Locations;
        } 

        public class StringSerializer
        {
            public void SerializeStringToFile(string[] data, string filePath)
            {
                //// Convert string to byte array
                //byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(data);

                //// Write byte array to file
                //using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                //{
                //    fileStream.Write(byteArray, 0, byteArray.Length);
                //}
                saveinfo saveinfo = new saveinfo();
                saveinfo.SkinLocation = data;
                saveinfo.ModVersion = USC.VERSION;
                string jsonData = JsonConvert.SerializeObject(saveinfo);
                BatonPass.Info("Encoding " + jsonData );
                File.WriteAllText(filePath, jsonData);
            }

            public string[] DeserializeStringFromFile(string filePath)
            {
                // Read byte array from file
                //byte[] byteArray;
                //using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                //{
                //    byteArray = new byte[fileStream.Length];
                //    fileStream.Read(byteArray, 0, byteArray.Length);
                //}

                //// Convert byte array to string
                //string data = System.Text.Encoding.UTF8.GetString(byteArray);
                //return data;
                // Read the JSON data from the file
                string jsonData = File.ReadAllText(filePath);
                string[] data = new string[] { "deserializedData Failed" };
                // Deserialize the JSON data into a saveinfo object
                try { 
                saveinfo deserializedData = JsonConvert.DeserializeObject<saveinfo>(jsonData);
                    data = deserializedData?.SkinLocation;
                    if (deserializedData.ModVersion != USC.VERSION)
                    {
                        SerializeStringToFile(deserializedData?.SkinLocation, filePath);
                        BatonPass.Warn("Wrong version, correcting");
                        return new string[] { "Wrong Version" };

                    }
                
                }
                catch
                {
                    BatonPass.Error("deserializedData Failed");
                    return new string[] { "deserializedData Failed" };
                }

                // Return the SkinLocation property from the deserialized object
                //BatonPass.Error("returning " + data.ToString());
                return data;
            }
        }
        public static void ExtractSkin(string skinFilePath, string storage)
        {
            string extractFolder = Path.Combine(skinFilePath, Path.GetFileNameWithoutExtension(storage));

                Directory.CreateDirectory(extractFolder);
            


                using (FileStream zipFileStream = File.OpenRead(storage))
                {

                    /*if (metadata.FileVersion != CurrentFileVersion)
                    {
                        Console.WriteLine($"Warning: File version '{metadata.FileVersion}' is not compatible with the current version '{CurrentFileVersion}'.");
                        Console.WriteLine("Extraction aborted.");
                        return;
                    }

                    Console.WriteLine("Metadata:");
                    Console.WriteLine($"  File Version: {metadata.FileVersion}");
                    Console.WriteLine($"  Name: {metadata.FileName}");
                    Console.WriteLine($"  Description: {metadata.FileDescription}");*/

                    // Decompress the remaining content
                    using (ZlibStream zlibStream = new ZlibStream(zipFileStream, CompressionMode.Decompress))
                    {
                        ExtractFromZlibStream(zlibStream, extractFolder);
                    }
                }

                
            }

        



        static void ExtractFromZlibStream(ZlibStream zlibStream, string extractFolder)
        {
            
            byte[] buffer = new byte[4096];
            int bytesRead;
            bool MDread = false;
            int i = 0;
            while (true)
            {
                string relativePath = ReadLineFromZlibStream(zlibStream)?.Trim();
                if (string.IsNullOrEmpty(relativePath))
                    break;
                
                // Recreate directory structure
                if (MDread == true)
                {
                    string targetPath = Path.Combine(extractFolder, relativePath);
                    string targetDir = Path.GetDirectoryName(targetPath);
                
                    if (!Directory.Exists(targetDir))
                    {
                        Directory.CreateDirectory(targetDir);
                    }
                
                // Read file size
                byte[] fileSizeBytes = new byte[sizeof(long)];
                zlibStream.Read(fileSizeBytes, 0, fileSizeBytes.Length);
                long fileSize = BitConverter.ToInt64(fileSizeBytes, 0);

                // Read and write file content
                
                    using (FileStream fileStream = File.Create(targetPath))
                    {

                        long remainingBytes = fileSize;
                        while (remainingBytes > 0)
                        {
                            int toRead = (int)Math.Min(remainingBytes, buffer.Length);
                            bytesRead = zlibStream.Read(buffer, 0, toRead);
                            if (bytesRead == 0)
                                throw new Exception("Unexpected end of stream.");

                            fileStream.Write(buffer, 0, bytesRead);
                            remainingBytes -= bytesRead;
                        }
                    }
                    
                }
                else
                {
                    //Metadata metadata = JsonSerializer.Deserialize<Metadata>(relativePath);


                    MDread = true;
                }


            }
        }

        static string ReadLineFromZlibStream(ZlibStream zlibStream)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                int currentByte;

                while ((currentByte = zlibStream.ReadByte()) != -1)
                {
                    if (currentByte == '\n')
                        break;
                    ms.WriteByte((byte)currentByte);
                }

                return Encoding.UTF8.GetString(ms.ToArray());
            }
            catch (Exception e)
            {
                
                return null;
            }
        }

        public static string[] GetThunderstoreProfileName()
        {
            string dllLocation = Assembly.GetExecutingAssembly().Location;
            string[] parts = dllLocation.Split(Path.DirectorySeparatorChar);

            bool isInsideThunderstore = false;
            bool isInsideR2Modman = false;
            string Dirtype = null;
            foreach (string part in parts)
            {
                if (part.Equals("Thunderstore Mod Manager", StringComparison.OrdinalIgnoreCase))
                {
                    isInsideThunderstore = true;
                    Dirtype = "Thunderstore";
                    break;
                }
                if (part.Equals("r2modmanPlus-local", StringComparison.OrdinalIgnoreCase))
                {
                    isInsideR2Modman = true;
                    Dirtype = "r2modman";
                    break;
                }
            }

            if (!isInsideThunderstore && !isInsideR2Modman)
                return null;

            for (int i = 0; i < parts.Length - 1; i++)
            {
                if (parts[i].Equals("profiles", StringComparison.OrdinalIgnoreCase))
                {
                    string[] InfoObject = new string[2]; 
                    InfoObject[0] = parts[i+1];
                    InfoObject[1] = Dirtype;
                    
                    return InfoObject;
                }
            }

            return null;
        }


    }
}

