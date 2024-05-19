using System;
using System.IO;
using System.IO.Pipes;
using Ionic.Zlib;

namespace UltraSkinsPacker
{
    public class Skinpacker
    {
        private static Stream zipFileStream;

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Drag a folder onto this executable to compress it, or a GCskin file to extract it.");
                return;
            }

            string path = args[0];

            // Verify if the argument is a folder or a GCskin file
            if (Directory.Exists(path))
            {
                CompressFolder(path);
            }
            else if (File.Exists(path) && path.EndsWith(".GCskin", StringComparison.OrdinalIgnoreCase))
            {
                ExtractSkin(path);
            }
            else
            {
                Console.WriteLine("Invalid input. Drag a folder to compress or a GCskin file to extract.");
            }

            Console.WriteLine("Operation complete. Press any key to exit.");
            Console.ReadKey();
        }

        static void CompressFolder(string folderPath)
        {
            string zipFileName = $"{folderPath}.GCskin"; // Example custom extension

            try
            {
                using (FileStream zipFileStream = File.Create(zipFileName))
                {
                    using (ZlibStream zlibStream = new ZlibStream(zipFileStream, Ionic.Zlib.CompressionMode.Compress))
                    {
                        CompressDirectory(folderPath, zlibStream);
                    }
                }

                Console.WriteLine($"Folder '{folderPath}' compressed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error compressing folder: {ex.Message}");
            }
        }

        static void CompressDirectory(string directoryPath, ZlibStream zlibStream)
        {
            string[] files = Directory.GetFiles(directoryPath);
            
            foreach (string file in files)
            {
                Console.WriteLine("compressing: "+file);
                CompressFile(file, zlibStream);
            }

            string[] subDirectories = Directory.GetDirectories(directoryPath);
            foreach (string subDir in subDirectories)
            {
                CompressDirectory(subDir, zlibStream);
            }
        }

        static void CompressFile(string filePath, ZlibStream zlibStream)
        {
            try
            {
                string relativePath = Path.GetRelativePath(Directory.GetCurrentDirectory(), filePath);

                // Write the relative path to the zlibStream
                byte[] pathBytes = System.Text.Encoding.UTF8.GetBytes(relativePath + Environment.NewLine);
                zlibStream.Write(pathBytes, 0, pathBytes.Length);

                // Write file size as a 4-byte integer (assuming file size fits within an int)
                byte[] fileSizeBytes = BitConverter.GetBytes((int)new FileInfo(filePath).Length);
                zlibStream.Write(fileSizeBytes, 0, fileSizeBytes.Length);

                // Write file content
                using (FileStream fileStream = File.OpenRead(filePath))
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {

                        zlibStream.Write(buffer, 0, bytesRead);
                    }
                }

                Console.WriteLine($"File '{filePath}' compressed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error compressing file '{filePath}': {ex.Message}");
            }
        }




       public static void ExtractSkin(string skinFilePath)
        {
            string extractFolder = Path.Combine(Path.GetDirectoryName(skinFilePath), Path.GetFileNameWithoutExtension(skinFilePath));

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

                Console.WriteLine($"Skin '{skinFilePath}' extracted successfully to '{extractFolder}'.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting skin: {ex.Message}");
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
                    Console.WriteLine($"Extracted file '{filePath}' successfully.");
                    fileCount++;
                }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing file '{filePath}': {ex.Message}");
                        // Optionally handle or log the exception further
                    }
                }

                // Output total files processed for debug purposes
                Console.WriteLine($"Total files processed: {fileCount}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting from zlib stream: {ex.Message}");
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