
using UnityEngine;
using System.IO;
using GameConsole;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using Ionic.Zlib;
using System;

using PluginConfig.API.Functionals;
using PluginConfig.API;

namespace UltraSkins
{
	public class SkinEventHandler : MonoBehaviour
	{
        //static PluginConfigurator config;

        public GameObject Activator;
		public string path;
		public string pname;
		public ULTRASKINHand UKSH;

		private void Update()
		{
			if (Activator != null && Activator.activeSelf)
			{
				Activator.SetActive(false);
				string message = UKSH.ReloadTextures(false, path);
				string folder = GetModFolderPath();
				TextureOverWatch[] TOWS = GameObject.FindWithTag("MainCamera").GetComponentsInChildren<TextureOverWatch>(true);
				foreach (TextureOverWatch TOW in TOWS)
				{
					if (TOW && TOW.gameObject)
					{
						TOW.enabled = true;
                    }
				}
                MonoSingleton<HudMessageReceiver>.Instance.SendHudMessage(message, "", "", 0, false);
			}
		}
        public string GetModFolderPath()
        {
            // Get the path to the current directory where the game executable is located
            //string gameDirectory = Assembly.GetExecutingAssembly().Location;
            //string gameDirectory = Path.GetDirectoryName(Application.dataPath);
            string dlllocation = Assembly.GetExecutingAssembly().Location.ToString();
            string dir = Path.GetDirectoryName(dlllocation);
            string defloc = Path.Combine(dir + "\\OG-SKINS");
            // The mod folder is typically named "BepInEx/plugins" or similar
            //string modFolderName = "BepInEx\\plugins\\ultraskins\\custom"; // Adjust this according to your setup
            StringSerializer serializer = new StringSerializer();
            string filecheck = Path.Combine(dir + "\\data.USGC");
            if (!File.Exists(Path.Combine(dir + "\\data.USGC")))
            {
                serializer.SerializeStringToFile(defloc, filecheck);

            }
            //ExtractSkin("OG-SKINS.GCskin");
            if (!File.Exists(Path.Combine(dir + "\\OG-SKINS")) &&  File.Exists(Path.Combine(dir + "\\OG-SKINS.GCskin"))) {
                ExtractSkin(Path.Combine(dir + "\\OG-SKINS.GCskin"));
            }
            string deserializedData = serializer.DeserializeStringFromFile(filecheck);
            // Combine the game directory with the mod folder name to get the full path
            //return gameDirectory+modFolderName;
            return deserializedData;
        }
        public class StringSerializer
        {
            public void SerializeStringToFile(string data, string filePath)
            {
                //// Convert string to byte array
                //byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(data);

                //// Write byte array to file
                //using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                //{
                //    fileStream.Write(byteArray, 0, byteArray.Length);
                //}
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fileStream, data);
                }
            }

            public string DeserializeStringFromFile(string filePath)
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
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    return (string)formatter.Deserialize(fileStream);
                }
            }
        }
        public static void ExtractSkin(string skinFilePath)
        {
            string extractFolder = Path.Combine(Path.GetDirectoryName(skinFilePath), Path.GetFileNameWithoutExtension(skinFilePath));
            //config = PluginConfigurator.Create("UltraskinsDebug", "ultrakilldebug.ultraskins.bobthecorn");
            try
            {
                Directory.CreateDirectory(extractFolder);

                using (FileStream zipFileStream = File.OpenRead(skinFilePath))
                {
                    using (ZlibStream zlibStream = new ZlibStream(zipFileStream, Ionic.Zlib.CompressionMode.Decompress))
                    {
                        ExtractFromZlibStream(zlibStream, extractFolder);
                    }
                }

              //  Console.WriteLine($"Skin '{skinFilePath}' extracted successfully to '{extractFolder}'.");
            }
            catch (Exception ex)
            {
               // ButtonField exerror = new ButtonField(config.rootPanel, ex.ToString(), ex.ToString());
            }
        }

        static void ExtractFromZlibStream(ZlibStream zlibStream, string extractFolder)
        {
            try
            {
                byte[] buffer = new byte[4096];
                int bytesRead;
                string line;

                // Create the root extraction directory if it doesn't exist
                Directory.CreateDirectory(extractFolder);

                // Initialize a counter for debug output
                int fileCount = 0;

                while ((line = ReadLineFromZlibStream(zlibStream)) != null)
                {
                    // Trim any extraneous characters like newline or carriage return
                    line = line.Trim();

                    // Get the file name from the full path
                    string fileName = Path.GetFileName(line);

                    // Ensure valid file name by replacing invalid characters
                    fileName = ReplaceInvalidFileNameChars(fileName);

                    string filePath = Path.Combine(extractFolder, fileName);

                    try
                    {
                        // Read file size as a 4-byte integer
                        byte[] fileSizeBytes = new byte[sizeof(int)];
                        zlibStream.Read(fileSizeBytes, 0, fileSizeBytes.Length);
                        int fileSize = BitConverter.ToInt32(fileSizeBytes, 0);
                        if (!fileName.Contains(".png"))
                        {
                            return;
                        }
                        // Read file content
                        using (FileStream fileStream = File.Create(filePath))
                        {
                            long bytesRemaining = fileSize;
                            while (bytesRemaining > 0)
                            {
                                int toRead = (int)Math.Min(bytesRemaining, buffer.Length);
                                bytesRead = zlibStream.Read(buffer, 0, toRead);
                                if (bytesRead == 0)
                                {
                                    throw new Exception("Unexpected end of stream.");
                                }

                                fileStream.Write(buffer, 0, bytesRead);

                                bytesRemaining -= bytesRead;
                            }
                        }




                        // Output debug message for extracted file
                       // Console.WriteLine($"Extracted file '{filePath}' successfully.");
                        fileCount++;
                    }
                    catch (Exception ex)
                    {
                        //ButtonField exerror = new ButtonField(config.rootPanel, ex.ToString(), ex.ToString());
                        // Optionally handle or log the exception further
                    }
                }

                // Output total files processed for debug purposes
               // Console.WriteLine($"Total files processed: {fileCount}");
            }
            catch (Exception ex)
            {
                //ButtonField exerror = new ButtonField(config.rootPanel, ex.ToString(), ex.ToString());
            }
        }




        static string ReplaceInvalidFileNameChars(string fileName)
        {
            foreach (char invalidChar in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(invalidChar.ToString(), "_");
            }
            return fileName;
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

                return System.Text.Encoding.UTF8.GetString(ms.ToArray());
            }
            catch
            {
                return null;
            }
        }
    }
}
