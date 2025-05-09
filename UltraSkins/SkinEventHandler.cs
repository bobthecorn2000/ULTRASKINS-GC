﻿
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

namespace UltraSkins.Utils
{
	public class SkinEventHandler : MonoBehaviour
	{
        //static PluginConfigurator config;
        public const string CurrentVersion = "6.0.2";
        public GameObject Activator;
		public string path;
		public string pname;
		public ULTRASKINHand UKSH;
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
        public string[] GetModFolderPath()
        {

            // Get the path to the current directory where the game executable is located
            //string gameDirectory = Assembly.GetExecutingAssembly().Location;
            //string gameDirectory = Path.GetDirectoryName(Application.dataPath);
            

            string dlllocation = Assembly.GetExecutingAssembly().Location.ToString();
            string moddir = Path.GetDirectoryName(dlllocation);
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string AppDataLoc = "bobthecorn2000\\ULTRAKILL\\ultraskinsGC";
            string dir = Path.Combine(appDataPath, AppDataLoc);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
                
                
            }
            string[] defloc = new string[] { Path.Combine(dir, "OG-SKINS") };

            // The mod folder is typically named "BepInEx/plugins" or similar
            //string modFolderName = "BepInEx\\plugins\\ultraskins\\custom"; // Adjust this according to your setup
            StringSerializer serializer = new StringSerializer();
            string filecheck = Path.Combine(dir + "\\data.USGC");
            if (!File.Exists(Path.Combine(dir + "\\data.USGC")))
            {
                serializer.SerializeStringToFile(defloc, filecheck);

            }

            //ExtractSkin("OG-SKINS.GCskin");
            if (!Directory.Exists(Path.Combine(dir + "\\OG-SKINS")) &&  File.Exists(Path.Combine(moddir + "\\OG-SKINS.GCskin"))) {
                ExtractSkin(dir, Path.Combine(moddir + "\\OG-SKINS.GCskin"));
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
                
                DirectoryInfo skindir = new DirectoryInfo(Path.Combine(dir + "\\OG-SKINS"));

                foreach (FileInfo fi in skindir.GetFiles())
                {
                    try
                    {
                        fi.Delete();
                    }
                    catch (Exception) { } // Ignore all exceptions
                }

                ExtractSkin(dir, Path.Combine(moddir + "\\OG-SKINS.GCskin"));
                deserializedData = serializer.DeserializeStringFromFile(filecheck);
            }

                    return deserializedData;
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
                saveinfo.ModVersion = CurrentVersion;
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
                    if (deserializedData.ModVersion != CurrentVersion)
                    {
                        SerializeStringToFile(deserializedData?.SkinLocation, filePath);
                        BatonPass.Info("Wrong version, correcting");
                        return new string[] { "Wrong Version" };

                    }
                
                }
                catch
                {
                    BatonPass.Error("deserializedData Failed");
                    return new string[] { "deserializedData Failed" };
                }

                // Return the SkinLocation property from the deserialized object
                BatonPass.Error("returning " + data.ToString());
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
        
    }
}

