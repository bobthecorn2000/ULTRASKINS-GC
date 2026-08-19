using BatonPassLogger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UltraSkins.UI;
using UltraSkins.Utils;
using UnityEngine;

namespace UltraSkins
{
    /// <summary>
    /// Hash based icon manager thing to lazily return icons make icons whenever they finish
    /// </summary>
    public class Iconic
    {
        public static async void ICFinder(string subfolder,SkinPackData skinpackdata, string IconName = "icon.png")
        {
            try
            {
                string subhash = subfolder.GetHashCode().ToString();
                if (HoldEm.Instance.IconCache.TryGetValue(subhash, out Texture2D icon))
                {
                    skinpackdata.SetSkinPackIcon(icon);

                }
                else
                {
                    string path = Path.Combine(subfolder, IconName);
                    if (File.Exists(path))
                    {
                        BatonPass.Debug("Searching for icon " + path);
                        byte[] image = await LoadSingleIcon(path);
                        Texture2D texture2D = new Texture2D(2, 2);
                        texture2D.name = IconName;

                        BatonPass.Debug("Creating " + texture2D.name);
                        
                        texture2D.filterMode = FilterMode.Point;
                        texture2D.LoadImage(image);
                        texture2D.Apply();
                        HoldEm.Bet(HoldEm.HoldemType.IC, subhash, texture2D);
                        BatonPass.Debug($"skinpack {skinpackdata.Name}: icon has been set");
                        skinpackdata.SetSkinPackIcon(texture2D);
                    }
                    else
                    {
                        BatonPass.Debug($"skinpack {skinpackdata.Name}: did not return a valid icon");
                    }


                }
                
            }
            catch (Exception ex)
            {

                BatonPass.Error($"{ex.Message}, Code -\"ICONIC-ICFINDER-EX\"");
                BatonPass.Error(ex.ToString());
                
            }

        }

        public static async Task<byte[]> LoadSingleIcon(string path)
        {
            if (File.Exists(path))
            {
                byte[] data = await File.ReadAllBytesAsync(path);
                return data;
            }
            return null;
        }
    }
}
