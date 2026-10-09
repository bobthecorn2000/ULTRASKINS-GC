using System;
using System.Collections.Generic;
using System.Text;
using UltraSkins.UI;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace UltraSkins.Prism
{
    public class PrismManager : MonoBehaviour
    {
       
        public Material toon;
        RayMaker rayMaker = new RayMaker();
        public static readonly Dictionary<string, Dictionary<int, Color[]>> TemplateCache = new Dictionary<string, Dictionary<int, Color[]>>
        {
            {"MainArm", new Dictionary<int, Color[]> {  {1, new Color[] {new Color32(65,0,106,0), new Color32(245,130,26,0), new Color32(34,6,58,0)} }, //Root beer co
                                                        {2, new Color[] {new Color32(255,233,0,255), new Color32(123,123,123,0), new Color32(255,255,255,0)} },// Angelfish
                                                        {3, new Color[] {new Color32(142,25,0,0), new Color32(248,207,87,0), new Color32(0,0,0,0)} },// Synthetic
                                                        {4, new Color[] {new Color32(85,206,253,0), new Color32(240,167,182,0), new Color32(255,255,255,0)} }, } // Blahaj
            },
            {"Feedbacker", new Dictionary<int, Color[]> {  {1, new Color[] {new Color32(65,0,106,0), new Color32(245,130,26,0), new Color32(34,6,58,0)} }, //Root beer co
                                                           {2, new Color[] {new Color32(255,233,0,255), new Color32(123,123,123,0), new Color32(255,255,255,0)} },// Angelfish
                                                           {3, new Color[] {new Color32(142,25,0,0), new Color32(248,207,87,0), new Color32(0,0,0,0)} },// Synthetic
                                                           {4, new Color[] {new Color32(85,206,253,0), new Color32(240,167,182,0), new Color32(255,255,255,0)} }, } // Blahaj
            },
            {"KnuckleBlaster", new Dictionary<int, Color[]> {  {1, new Color[] {new Color32(255,0,0,0), new Color32(0,0,255,0), new Color32(0,22,157,0)} }, //Brennan Innovations
                                                               {2, new Color[] {new Color32(0,0,0,0), new Color32(255,0,0,255), new Color32(67,67,67,0)} },// Hush!
                                                               {3, new Color[] {new Color32(134,217,58,0), new Color32(217,103,159,0), new Color32(255,142,0,255)} },// Clash
                                                               {4, new Color[] {new Color32(214,2,112,0), new Color32(155,79,150,0), new Color32(0,56,168,0)} }, } // Mirage
            },
            {"Whiplash", new Dictionary<int, Color[]> {  {1, new Color[] {new Color32(255,255,0,0), new Color32(0,255,0,0), new Color32(120,120,120,0)} }, //GCorn Industries
                                                         {2, new Color[] {new Color32(36,59,113,0), new Color32(254,87,192,0), new Color32(170,204,239,0)} },// Webpunk
                                                         {3, new Color[] {new Color32(0,255,255,0), new Color32(91,91,91,0), new Color32(0,0,0,0)} },// Cold and Uncooked
                                                         {4, new Color[] {new Color32(155,89,208,0), new Color32(48,48,48,0), new Color32(255,255,0,0)} }, } // GigaByte
            }
        };
        public Dictionary<string, Dictionary<int, Color>> ColorCache = new Dictionary<string, Dictionary<int, Color>>();
        public Dictionary<string, int> ModeCache = new Dictionary<string, int>();
        public Dictionary<string, int> PresetCache = new Dictionary<string, int>();
        private Dictionary<string, Color[]> lastloaded = new Dictionary<string, Color[]>();
        public static PrismManager Instance { get; private set; }

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                UnityEngine.Object.Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

        }



        public void Cache(string weapon, int ID, Color color)
        {
            ColorCache[weapon][ID] = color;
        }

        public void TryInitCache(string weapon)
        {
            if (ColorCache.TryAdd(weapon, new Dictionary<int, Color>()))
            {
                Color[] colors = rayMaker.Load(weapon);
                ColorCache[weapon][0] = colors[0];
                ColorCache[weapon][1] = colors[1];
                ColorCache[weapon][2] = colors[2];
                lastloaded.TryAdd(weapon, colors);
            }


        }

        public Color[] colorGet(string weapon)
        {
            if (ColorCache.ContainsKey(weapon))
            {
                Color[] colors = new Color[]
                {
                    ColorCache[weapon][0],
                    ColorCache[weapon][1],
                    ColorCache[weapon][2]
                };
                
                return colors;
            }
            else
            {
                TryInitCache(weapon);
                Color[] colors = new Color[]
                {
                    ColorCache[weapon][0],
                    ColorCache[weapon][1],
                    ColorCache[weapon][2]
                };
                
                return colors;
                
            }
            



        }

        public Color[] RequestTemplateColors(string weapon)
        {
            int templateID = GetFlagByID(weapon,RayMaker.flags.Preset);

            if (templateID > 0 && templateID < 5)
            {
                return TemplateCache[weapon][templateID];
            }
            return new Color[] {new Color32(255,0,0,0), new Color32(0,255,0,0), new Color32(0,0,255,0)}; 
        }



        public void SaveColorsToDisk(string wep)
        {
            Color[] newcolors = new Color[] { ColorCache[wep][0], ColorCache[wep][1], ColorCache[wep][2] };
            PrismBeam.FireColorBeam(wep);
            if (!lastloaded.TryGetValue(wep, out Color[] cached) || !ColorChecker(newcolors, cached))
            {
                rayMaker.SaveTriColor(wep, newcolors);
            }

        }
        public void SaveFlagToDisk(string wep, RayMaker.flags flag)
        {
            try
            {
                int newflag = GetFlagByID(wep,flag);
                if (rayMaker.GetFlag(wep,flag) != newflag)
                {
                    rayMaker.SetFlag(wep,flag,newflag);
                }
            }
            catch(Exception ex)
            {
                Debug.LogError("Something has gone wrong saving a prism flag to disk: " + ex.ToString());
            }
            
        }
        public void SetFlagByID(string wep, RayMaker.flags flag, int value, bool SaveToDiskImmediate = false)
        {
            switch (flag)
            {
                case RayMaker.flags.Mode: 
                    ModeCache[wep] = value;
                    break;
                case RayMaker.flags.Preset:
                    PresetCache[wep] = value;
                    break;
            }
            if (SaveToDiskImmediate)
            {
                SaveFlagToDisk(wep,flag);
            }
        }
        public int GetFlagByID(string wep, RayMaker.flags flag)
        {

            switch (flag)
            {
                case RayMaker.flags.Mode:
                    if (ModeCache.TryGetValue(wep, out int mvalue))
                    {
                        return mvalue;
                    }
                    else
                    {
                        int readvalue = rayMaker.GetFlag(wep,flag);
                        ModeCache.Add(wep,readvalue);
                        return readvalue;
                    }
                
                case RayMaker.flags.Preset:
                    if (PresetCache.TryGetValue(wep, out int pvalue))
                    {
                        return pvalue;
                    }
                    else
                    {
                        int readvalue = rayMaker.GetFlag(wep,flag);
                        PresetCache.Add(wep,readvalue);
                        return readvalue;
                    }
                

            }
            return 0;



        }

        [Obsolete("use getflagbyID")]
        public int GetSelectedPreset()
        {
            return 0;
        }


        public static void GetAllMats()
        {

        }


        private bool ColorChecker(Color[] a, Color[] b)
        {
            if (a == null || b == null || a.Length != b.Length) return false;
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i]) return false;
            }
            return true;
        }

        void LoadMaterial(string address)
        {
            //Addressables.LoadAssetAsync<Material>(address).Completed += OnMaterialLoaded;
        }
        /*         void OnMaterialLoaded(AsyncOperationHandle<Material> handle)
                {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        Material mat = handle.Result;
                        Debug.Log("Material loaded: " + mat.name);

                        // Example: assign it to a renderer
                        toon = mat;

                    }
                    else
                    {
                        Debug.LogError("Failed to load material: " + handle.OperationException);
                    }
                } */
    }

    public static class PrismBeam
    {

        public static Action<string> OnRecieveColorBeam;
        public static void FireColorBeam(string wep)
        {
            OnRecieveColorBeam?.Invoke(wep);
        }
    }

}
