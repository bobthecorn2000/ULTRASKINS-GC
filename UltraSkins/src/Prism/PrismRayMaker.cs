
using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using UltraSkins.UI;
using UnityEngine.UIElements;
using UltraSkins.Configuration;


namespace UltraSkins.Prism
{

    public class RayMaker
    {
        /* 
        FORMAT DOC STUFF
        byte 00-02:MAGIC [RAY]
        byte 03:VERSION [an Int]
        byte 04:Mode (an int, 0 for off, 1 for custom, 2 for preset)
        byte 05:Preset (an int, 1-4 or more. ignored if mode is anything other then 2)
        byte 06-15:RESERVED
        byte 16-19:Color1
        byte 20-23:Color2
        byte 24-27:Color3
        
         */

        private const string Magic = "RAY";
        private const byte Version = 1;
        private const byte Mode = 0;
        private const byte Preset = 0;
        private static readonly byte[] Reserved = new byte[10];

        // Default Data as to not cause read errors later on
        // we use FF for color1.R and then loop 0s for the next 3 bytes of the rgba color. then another 0 to the first byte of the color2.R
        // we then write FF to color2.G
        // a 00 to color2.B
        // a FF to color2.A
        // 2 00s for color3.R and color3.G
        // ff for color3.B
        // 00 for color3.A
        private static readonly byte[] WeirdDefaultData = new byte[] { 255, 0, 0, 0, 0, 255, 0, 255, 0, 0, 255, 0 };


        //IGNORE THIS, might come back to it
        [Obsolete]
        enum PrismWep
        {
            MainArm,
            Feedbacker,
            KnuckleBlaster,
            Whiplash,
            YellowArm
        }

        /// <summary>
        /// Returns the correct offset byte for a ray file
        /// </summary>
        /// <param name="slot">the slot offset, ZERO INDEXED </param>
        /// <returns>a int that represents a byte offset </returns>
        private int GetOffsetBySlot(int slot)
        {

            return (slot * 4) + 16;
        }

        private void writeDefaults(BinaryWriter bw)
        {
            // this could probably be its own function if need be later. but for now it works here i guess    
            bw.Write(System.Text.Encoding.ASCII.GetBytes(Magic));
            bw.Write(Version);
            bw.Write(Mode);
            bw.Write(Preset);
            bw.Write(Reserved);

            bw.Write(WeirdDefaultData);

            bw.Seek(0, SeekOrigin.Begin);
        }
        public enum flags
        {
            Mode,
            Preset
        }

        private bool CheckValid(string path, FileStream fs)
        {

            using var br = new BinaryReader(fs, Encoding.UTF8, true);
            bool valid = true;
            // Validate header
            var magicBytes = br.ReadBytes(3);
            try
            {
                if (System.Text.Encoding.ASCII.GetString(magicBytes) != Magic)
                {
                    valid = false;
                    throw new Exception("Invalid ray file");

                }


                if (br.ReadByte() != Version)
                {
                    valid = false;
                    throw new Exception("Unsupported ray version");
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
                return false;
            }


        }
        bool HealthCheck(string path, ref FileStream fs)
        {
            bool newfile = false;
            if (!File.Exists(path))
            {
                newfile = true;
                fs?.Dispose();
                fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
            }
            if (newfile)
            {
                using var bw = new BinaryWriter(fs, Encoding.UTF8, true);
                writeDefaults(bw);
            }
            if (!CheckValid(path, fs) && newfile != true)
            {
                return false;
            }
            fs.Seek(0, SeekOrigin.Begin);
            return true;
        }

        string MakePath(string wep)
        {
            return Path.Combine(SettingsManager.Instance.SaveSettingsLocation, "Prism", wep + ".ray");
        }

        public void SetFlag(string wep, flags flag, int value)
        {
            string path = MakePath(wep);
            FileStream fs = null;
            try
            {
                if (File.Exists(path))
                {
                    fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
                }


                if (!HealthCheck(path, ref fs))
                {
                    Debug.LogError($"{path} Failed HealthCheck: Could not set flag");
                    return;
                }



                using var bw = new BinaryWriter(fs);
                // Header

                switch (flag)
                {
                    case flags.Mode:
                        bw.Seek(4, SeekOrigin.Begin);
                        break;

                    case flags.Preset:
                        bw.Seek(5, SeekOrigin.Begin);
                        break;
                }

                bw.Write((byte)value);
            }
            finally
            {
                fs?.Dispose();
            }
        }



        public int GetFlag(string wep, flags flag)
        {
            string path = MakePath(wep);

            FileStream fs = null;
            try
            {
                if (File.Exists(path))
                {
                    fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
                }

                if (!HealthCheck(path, ref fs))
                {
                    Debug.LogError($"{path} Failed HealthCheck: Could not get flag. defaulting to 0");
                    return 0;
                }
                using var br = new BinaryReader(fs);
                switch (flag)
                {
                    case flags.Mode:
                        br.BaseStream.Seek(4, SeekOrigin.Begin);
                        break;

                    case flags.Preset:
                        br.BaseStream.Seek(5, SeekOrigin.Begin);
                        break;
                }

                return br.ReadByte();
            }

            finally
            {
                fs?.Dispose();
            }
        }


        public void Save(string Weapon, Color color, int slot)
        {
            string path = MakePath(Weapon);
            FileStream fs = null;
            try
            {
                if (File.Exists(path))
                {
                    fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
                }

                if (!HealthCheck(path, ref fs))
                {
                    Debug.LogError($"{path} Failed HealthCheck: Could not Save file");
                    return;
                }

                using var bw = new BinaryWriter(fs);




                // Datasets
                WriteSet(bw, color, slot);
            }
            finally
            {
                fs?.Dispose();
            }



        }

        public void SaveTriColor(string Weapon, Color[] colors)
        {
            string path = MakePath(Weapon);
            FileStream fs = null;
            try
            {
                if (File.Exists(path))
                {
                    fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
                }

                if (!HealthCheck(path, ref fs))
                {
                    Debug.LogError($"{path} Failed HealthCheck: Could not set TriColor");
                    return;
                }


                using var bw = new BinaryWriter(fs);
                // Header

                WriteSet(bw, colors[0], 0);
                WriteSet(bw, colors[1], 1);
                WriteSet(bw, colors[2], 2);
            }
            finally
            {
                fs?.Dispose();
            }

        }




        public Color[] Load(string Weapon)
        {
            FileStream fs = null;
            try
            {


                string path = MakePath(Weapon);

                if (File.Exists(path))
                {
                    fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
                }
                if (!HealthCheck(path, ref fs))
                {
                    Debug.LogError($"{path} Failed HealthCheck: Could not Load colors, Returing a standard RGB set");
                    return new Color[] { Color.red, Color.green, Color.blue };
                }

                using var br = new BinaryReader(fs);


                // Skip reserved bytes
                br.ReadBytes(16);

                // Load datasets
                Color32[] colors = ReadSet(br);

                return new Color[] {
                new Color(colors[0].r / 255f, colors[0].g / 255f, colors[0].b / 255f, colors[0].a / 255f),
                new Color(colors[1].r / 255f, colors[1].g / 255f, colors[1].b / 255f, colors[1].a / 255f),
                new Color(colors[2].r / 255f, colors[2].g / 255f, colors[2].b / 255f, colors[2].a / 255f)
            };
            }
            finally
            {
                fs?.Dispose();
            }
        }





        private void WriteSet(BinaryWriter bw, Color32 c, int slot)
        {

            bw.Seek(GetOffsetBySlot(slot), SeekOrigin.Begin);



            bw.Write(c.r);
            bw.Write(c.g);
            bw.Write(c.b);
            bw.Write(c.a);

        }

        private Color32[] ReadSet(BinaryReader br, int slotstart = 0, int looptimes = 3)
        {


            br.BaseStream.Seek(GetOffsetBySlot(slotstart), SeekOrigin.Begin);
            Color32[] set = new Color32[looptimes];
            for (int i = 0; i < looptimes; i++)
            {
                set[i] = new Color32(br.ReadByte(), br.ReadByte(), br.ReadByte(), br.ReadByte());
            }
            ;
            return set;
        }

    }
}

