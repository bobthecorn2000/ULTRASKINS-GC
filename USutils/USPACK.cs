using System;
using System.IO;
using System.Text;
using Ionic.Zlib;
using System.Text.Json;

namespace UltraSkinsPacker
{
    public class Skinpacker
    {
        private const string CurrentFileVersion = "2.0";
        private static readonly byte[] SyncBuffer = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF };
        public class Metadata
        {
            public string FileVersion { get; set; }
            public string FileName { get; set; }
            public string FileDescription { get; set; }
        }

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Drag a folder onto this executable to compress it, or a GCskin file to extract it.");
                



            }
            if (args.Length == 1) { 
            string path = args[0];

            // Verify if the argument is a folder or a GCskin file
            if (Directory.Exists(path))
            {
                CompressFolder(path);
            }
            else if (File.Exists(path) && path.EndsWith(".GCskin", StringComparison.OrdinalIgnoreCase))
            {
                    string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    string AppDataLoc = "bobthecorn2000\\ULTRAKILL\\ultraskinsGC";
                    string dir = Path.Combine(appDataPath, AppDataLoc);
                    bool loop = true;
                    while (loop == true)
                    {
                        Console.WriteLine("enter the number of your selected option, then press enter");
                        Console.WriteLine("1. Install to Ultrakill");
                        Console.WriteLine("2. Extract file to current directory");
                        string response = Console.ReadLine();
                        switch (response) {
                            case "1":
                                loop = false;
                                ExtractSkin(path, dir, true);
                                
                                break;
                            case "2": 
                                loop = false; 
                                ExtractSkin(path,path, false);
                                break;
                            default: continue;
                        }
                    }
                
            }
            else
            {
                return;
            }
            }
            Console.WriteLine("Operation complete. Press any key to exit.");
            Console.ReadKey();
        }

        static void CompressFolder(string folderPath)
        {
            try
            {
                // Collect metadata from the user
                Console.Write("Name: ");
                string name = Console.ReadLine()?.Trim() ?? "Unnamed";
                Console.Write("Description: ");
                string description = Console.ReadLine()?.Trim() ?? "No description.";

                // Create metadata object
                var metadata = new Metadata
                {
                    FileVersion = CurrentFileVersion,
                    FileName = name,
                    FileDescription = description
                };

                // Serialize metadata to JSON
                string metadataJson = JsonSerializer.Serialize(metadata);
                string zipFileName = $"{name}.GCskin";
                if (File.Exists(zipFileName))
                {
                    bool loop = true;
                    while (loop == true)
                    {
                        Console.WriteLine("A File with this name already exists");
                        Console.WriteLine("1. Cancel Compressing");
                        Console.WriteLine("2. Overwrite Existing File");
                        Console.WriteLine("3. Rename the Output");
                        string response = Console.ReadLine();
                        try
                        {
                            int responsevalue = int.Parse(response);
                            switch (responsevalue)
                            {
                                default:
                                    Console.WriteLine(responsevalue + "is not a valid option");
                                    continue;
                                case 1: throw new Exception("Cancelled");
                                case 2:
                                    loop = false;
                                    break;
                                case 3:
                                    Console.WriteLine("Name the new file:");
                                    zipFileName = Directory.GetParent(zipFileName) + "\\" + Console.ReadLine() + ".GCskin";
                                    loop = false;
                                    break;

                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            Console.ReadLine();
                            System.Environment.Exit(1);

                        }

                    }
                }
                   
                using (FileStream zipFileStream = File.Create(zipFileName))
                {
                    // Write metadata
                    byte[] metadataBytes = Encoding.UTF8.GetBytes(metadataJson + Environment.NewLine);
                    
                    
                    // Create a ZlibStream to compress files
                    using (ZlibStream zlibStream = new ZlibStream(zipFileStream,CompressionMode.Compress, CompressionLevel.Default))
                    {
                        
                        zlibStream.Write(metadataBytes, 0, metadataBytes.Length);
                        //zlibStream.Write(new byte[]{ 0xDE, 0xAD, 0xBE, 0xEF, 0xDE, 0xAD, 0xBE, 0xEF}, 0, 8);
                        CompressDirectory(folderPath, zlibStream, folderPath);
                    }
                }

                Console.WriteLine($"Folder '{folderPath}' compressed successfully with metadata.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error compressing folder: {ex.Message}");
            }
        }

        static void CompressDirectory(string directoryPath, ZlibStream zlibStream, string rootFolderPath)
        {
            string[] files = Directory.GetFiles(directoryPath);

            foreach (string file in files)
            {
                Console.WriteLine($"Compressing: {file}");
                CompressFile(file, zlibStream, rootFolderPath);
            }

            string[] subDirectories = Directory.GetDirectories(directoryPath);
            foreach (string subDir in subDirectories)
            {
                CompressDirectory(subDir, zlibStream, rootFolderPath); // Recursively process subdirectories
            }
        }

        static void CompressFile(string filePath, ZlibStream zlibStream, string rootFolderPath)
        {
            try
            {
                // Get relative path
                string relativePath = Path.GetRelativePath(rootFolderPath, filePath);

                // Write relative path
                byte[] pathBytes = Encoding.UTF8.GetBytes(relativePath + Environment.NewLine);
                zlibStream.Write(pathBytes, 0, pathBytes.Length);

                // Write file size
                long fileSize = new FileInfo(filePath).Length;
                byte[] fileSizeBytes = BitConverter.GetBytes(fileSize);
                zlibStream.Write(fileSizeBytes, 0, fileSizeBytes.Length);

                // Write file data
                using (FileStream fileStream = File.OpenRead(filePath))
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        zlibStream.Write(buffer, 0, bytesRead);
                    }
                }

                Console.WriteLine($"Compressed: {relativePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error compressing file '{filePath}': {ex.Message}");
            }
        }

        public static void ExtractSkin(string skinFilePath, string dir, bool installing)
        {
            string extractFolder = null;
            if (installing == false) {
                extractFolder = Path.Combine(Path.GetDirectoryName(dir), Path.GetFileNameWithoutExtension(skinFilePath));
            }
            else if (installing == true)
            {
                extractFolder = Path.Combine(dir, Path.GetFileNameWithoutExtension(skinFilePath));
            }
             

            try
            {
                if (Directory.Exists(extractFolder))
                {
                    bool loop = true;
                    while (loop == true)
                    {
                        Console.WriteLine("A folder with this name already exists");
                        Console.WriteLine("1. Cancel Extracting");
                        Console.WriteLine("2. Overwrite Existing Files");
                        Console.WriteLine("3. Rename the Output");
                        string response = Console.ReadLine();
                        try
                        {
                            int responsevalue = int.Parse(response);
                            switch (responsevalue)
                            {
                                default:
                                    Console.WriteLine(responsevalue + "is not a valid option");
                                    continue;
                                case 1: throw new Exception("Cancelled");
                                case 2:
                                    loop = false;
                                    break;
                                case 3:
                                    Console.WriteLine("Name the new folder:");
                                    extractFolder = Directory.GetParent(extractFolder) + "\\" + Console.ReadLine();
                                    loop = false;
                                    break;

                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            Console.ReadLine();
                            System.Environment.Exit(1);

                        }

                    }
                }
                    Directory.CreateDirectory(extractFolder);
                
                

                using (FileStream zipFileStream = File.OpenRead(skinFilePath))
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

                Console.WriteLine($"Skin '{skinFilePath}' extracted successfully to '{extractFolder}'.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting skin: {ex.Message}");
            }
        }

        

        static void ExtractFromZlibStream(ZlibStream zlibStream, string extractFolder)
        {
            byte[] buffer = new byte[4096];
            int bytesRead;
            bool MDread = false;

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
                    Console.WriteLine($"Extracted: {relativePath}");
                }
                else
                {
                    Metadata metadata = JsonSerializer.Deserialize<Metadata>(relativePath);
                    if (metadata.FileVersion != CurrentFileVersion)
                    {
                        Console.WriteLine($"Warning: This file was made with version '{metadata.FileVersion}' is may not compatible with the current version '{CurrentFileVersion}'.");
                        Console.WriteLine("Extraction may have issues");
                        Console.WriteLine("Continue Y/N");
                        string response = Console.ReadLine();
                        response = response.ToLower();
                        if (response == "y" || response == "yes") 
                        {
                            
                        }
                        else if (response == "n" || response == "no") {
                            throw new Exception("Cancelled");
                        }
                        else
                        {
                            Console.WriteLine("Interpreting vague answer as YES.");
                            
                        }

                    }

                    Console.WriteLine("Metadata:");
                    Console.WriteLine($"  File Version: {metadata.FileVersion}");
                    Console.WriteLine($"  Name: {metadata.FileName}");
                    Console.WriteLine($"  Description: {metadata.FileDescription}");
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
                Console.WriteLine(e.ToString());
                return null;
            }
        }
        private static void SeekToSyncBuffer(FileStream zipFileStream)
        {
            byte[] buffer = new byte[SyncBuffer.Length];
            int bytesRead;

            // Loop through the file and look for the sync buffer
            while ((bytesRead = zipFileStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                // Check if we have found the sync buffer in the current read
                for (int i = 0; i < bytesRead - SyncBuffer.Length + 1; i++)
                {
                    bool foundSyncBuffer = true;
                    for (int j = 0; j < SyncBuffer.Length; j++)
                    {
                        if (buffer[i + j] != SyncBuffer[j])
                        {
                            foundSyncBuffer = false;
                            break;
                        }
                    }

                    if (foundSyncBuffer)
                    {
                        // If the sync buffer is found, seek to the position right after it
                        zipFileStream.Seek(i + SyncBuffer.Length, SeekOrigin.Current);
                        Console.WriteLine("Sync buffer found, continuing extraction.");
                        return;
                    }
                }
            }

            throw new Exception("Sync buffer not found in file.");
        }
    }
}
