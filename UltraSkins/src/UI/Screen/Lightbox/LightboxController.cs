using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using System.IO;
using System.Reflection;

namespace UltraSkins.UI.LightBox
{
    public class LightboxController : MonoBehaviour
    {
        // Start is called before the first frame update
        public TextMeshProUGUI textbox;
        string runtimelocation; 
        string licensecache;
        string Tlicensecache;

        public GameObject Lightbox;

        void Awake()
        {
            runtimelocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }


    public static string ReadTextFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
                return "License File Missing";
        }

        return File.ReadAllText(filePath);
    }
        public void OpenLightBox(int type)
        {
            OpenType opentype = (OpenType)type;
            switch (opentype)
            {
                case OpenType.FirstParty:
                    if (licensecache == null)
                    {
                        licensecache = ReadTextFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "LICENSE.txt"));
                    }
                    textbox.text = licensecache;
                    break;
                case OpenType.ThirdParty:
                    if (Tlicensecache == null)
                    {
                        Tlicensecache = ReadTextFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "THIRD-PARTY-LICENSE.txt"));
                    }
                    textbox.text = Tlicensecache;
                    break;
            }
            Lightbox.SetActive(true);
        }
        public void CloseLightbox()
        {
            Lightbox.SetActive(false);
            textbox.text = "";
        }
        public enum OpenType
        {
            FirstParty,
            ThirdParty
        }
    }
}
