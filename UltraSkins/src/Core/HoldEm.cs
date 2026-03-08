using System.Collections.Generic;
using UnityEngine;
using BatonPassLogger;

//using UltraSkins.Prism;
//using UnityEngine.UIElements;
namespace UltraSkins
{
    public partial class ULTRASKINHand
    {
        /// <summary>
        /// The HoldEm system allows you to request and modify the textures Ultraskins currently keeps track of
        /// </summary>
        public class HoldEm
        {

            /// <summary>
            /// <b>one of either </b> 
            /// <list type="table">
            ///
            /// <term>ASC</term> <description>AUTOSWAPCACHE: the users loaded skins</description>
            /// <br/>
            /// <term>OGS</term> <description>OGSKINS: the master list of all supported skins, the secondary cache</description>
            /// <br/>
            /// <term>IC</term> <description>ICONCACHE: all skin icons for this session) </description>
            /// <br/>
            /// <term>SC</term> <description>SPRITECACHE: all currently known sprites</description>
            /// </list>
            /// </summary>
            public enum HoldemType
            {
                ASC = 0, OGS = 1, IC = 2, SC = 3
            }
            /// <summary>
            /// Returns the Texture If it exists in either Primary or Secondary Cache
            /// </summary>
            /// <param name="key">The KEY we are looking for</param>
            /// <returns>A <seealso cref="Texture">Texture</seealso></returns>
            public static Texture Call(string key)
            {
                if (autoSwapCache.TryGetValue(key, out Texture texture))
                {
                    BatonPass.Debug("ASC Call:" + key);
                    return texture;
                }
                else if (HandInstance.ogSkinsManager.OGSKINS.TryGetValue(key, out Texture originalTexture))
                {
                    BatonPass.Debug("OGS Call:" + key);
                    return originalTexture;
                }
                else
                {
                    BatonPass.Debug("Call NoKeyFound" + key);
                    // Not found anywhere
                    return null;
                }
            }
            /// <summary>
            /// Check if a KEY exists in either Primary Cache or Secondary Cache
            /// </summary>
            /// <param name="key">the KEY we are looking for</param>
            /// <returns>Bool "True" if we have it "False" if we don't</returns>
            public static bool Check(string key)
            {
                if (autoSwapCache.ContainsKey(key))
                {
                    BatonPass.Debug("ASC Check:" + key);
                    return true;
                }
                else if (HandInstance.ogSkinsManager.OGSKINS.ContainsKey(key))
                {
                    BatonPass.Debug("OGS Check:" + key);
                    return true;
                }
                else
                {
                    // Not found anywhere
                    BatonPass.Debug("Check NoKeyFound" + key);
                    return false;
                }
            }
            /// <summary>
            /// Removes a specific item from the Dictionary
            /// </summary>
            /// <param name="holdemType">The Type of Dictionary we are Changing</param>
            /// <param name="name">The KEY we are removing</param>
            public static void Discard(HoldemType holdemType, string name)
            {


                switch (holdemType)
                {
                    case HoldemType.ASC:
                        if (autoSwapCache.ContainsKey(name))
                        {
                            Texture workingfile = autoSwapCache[name];
                            UnityEngine.Object.Destroy(workingfile);
                            autoSwapCache.Remove(name);
                        }
                        break;
                    case HoldemType.OGS:
                        if (HandInstance.ogSkinsManager.OGSKINS.ContainsKey(name))
                        {
                            Texture workingfile = HandInstance.ogSkinsManager.OGSKINS[name];
                            UnityEngine.Object.Destroy(workingfile);
                            HandInstance.ogSkinsManager.OGSKINS.Remove(name);
                        }
                        break;
                    case HoldemType.IC:
                        if (HandInstance.IconCache.ContainsKey(name))
                        {
                         
                            HandInstance.IconCache.Remove(name);
                        }
                        break;
                    case HoldemType.SC:
                        if (HandInstance.SpriteCache.ContainsKey(name))
                        {

                            HandInstance.SpriteCache.Remove(name);
                        }
                        break;
                }
            }
            /// <summary>
            /// Adds a Key Value pair into the Holdem System, Placing a Bet on the table
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="holdemType">The Type of Dictionary we are Changing</param>
            /// <param name="TextureName">The KEY for the dict</param>
            /// <param name="texture2D">the VALUE for the dict, In the case of IC this is a byte[]</param>
            public static void Bet(HoldemType holdemType,string TextureName,Texture texture2D)
            {
                switch (holdemType)
                {
                    case HoldemType.ASC:
                        
                        autoSwapCache.Add(TextureName, texture2D);
                        break;
                    case HoldemType.OGS:
                        HandInstance.ogSkinsManager.OGSKINS.Add(TextureName, texture2D);
                        break;
                    case HoldemType.IC:
                        HandInstance.IconCache.Add(TextureName, texture2D as Texture2D);
                        break;
                    case HoldemType.SC:
                        texture2D.filterMode = FilterMode.Bilinear;
                        Sprite newsprite = Sprite.Create(texture2D as Texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
                        HandInstance.SpriteCache.Add(TextureName, newsprite);
                        break;

                }
            }
            /// <summary>
            /// Pulls a Value from a Key in the Holdem System, ,
            /// </summary>
            /// <typeparam name="T">Either a Texture or a Sprite</typeparam>
            /// <param name="holdemType">The Type of Dictionary we are Changing</param>
            /// <param name="name">The KEY we are asking for</param>
            /// <returns>Draw a Texture or byte[] into your hand</returns>
            public static T Draw<T>(HoldemType holdemType, string name) where T : class
            {

                switch (holdemType)
                {
                    case HoldemType.ASC:
                        autoSwapCache.TryGetValue(name, out Texture rawASCtexture);
                        Texture2D ASCtexture = rawASCtexture as Texture2D;
                        return ASCtexture as T;

                    case HoldemType.OGS:
                        HandInstance.ogSkinsManager.OGSKINS.TryGetValue(name, out Texture rawOGtexture);
                        Texture2D OGtexture = rawOGtexture as Texture2D;
                        return OGtexture as T;

                    case HoldemType.IC:
                        HandInstance.IconCache.TryGetValue(name, out Texture2D ICtexture);
                        return ICtexture as T;
                    case HoldemType.SC:
                        HandInstance.SpriteCache.TryGetValue(name, out Sprite SCtexture);
                        return SCtexture as T;
                    default:
                        return null;

                }

            }
            /// <summary>
            /// Check a specific holdem stash for a texture
            /// </summary>
            /// <param name="holdemType">The Type of Dictionary we are Changing</param>
            /// <param name="name">The KEY we are asking for</param>
            /// <returns></returns>
            public static bool Bluff(HoldemType holdemType, string name)
            {
                switch (holdemType)
                {
                    case HoldemType.ASC:
                        return (autoSwapCache.ContainsKey(name)) ? true : false; 


                    case HoldemType.OGS:
                        return (HandInstance.ogSkinsManager.OGSKINS.ContainsKey(name)) ? true : false;

                    case HoldemType.IC:
                        return (HandInstance.IconCache.ContainsKey(name)) ? true : false;
                    case HoldemType.SC:
                        return (HandInstance.SpriteCache.ContainsKey(name)) ? true : false;

                }
                return false;
            }
            /// <summary>
            /// Danger: Purges an entire Dictionary and resets it,
            /// </summary>
            /// <param name="holdemType">The Type of Dictionary we are Changing</param>
            public static void Fold(HoldemType holdemType)
            {


                switch (holdemType)
                {
                    case HoldemType.ASC:
                        foreach (KeyValuePair<string, Texture> kvp in autoSwapCache)
                        {

                            string name = kvp.Key;
                            BatonPass.Debug("Deleting " + name + " from Holdem ASC");
                            Texture workingfile = autoSwapCache[name];
                            UnityEngine.Object.Destroy(workingfile);

                        }
                        autoSwapCache.Clear();
                        break;
                    case HoldemType.OGS:
                        foreach (KeyValuePair<string, Texture> kvp in HandInstance.ogSkinsManager.OGSKINS)
                        {

                            string name = kvp.Key;
                            BatonPass.Debug("Deleting " + name + " from Holdem ASC");
                            Texture workingfile = HandInstance.ogSkinsManager.OGSKINS[name];
                            UnityEngine.Object.Destroy(workingfile);

                        }
                        HandInstance.ogSkinsManager.OGSKINS.Clear();
                        break;
                    case HoldemType.IC:

                        HandInstance.IconCache.Clear();
                        break;
                    case HoldemType.SC:
                        foreach (KeyValuePair<string, Sprite> kvp in HandInstance.SpriteCache)
                        {

                            string name = kvp.Key;
                            BatonPass.Debug("Deleting " + name + " from Holdem SC");
                            Sprite workingfile = HandInstance.SpriteCache[name];
                            UnityEngine.Object.Destroy(workingfile);

                        }
                        HandInstance.SpriteCache.Clear();
                        break;


                }
            }

        }


    }
}
