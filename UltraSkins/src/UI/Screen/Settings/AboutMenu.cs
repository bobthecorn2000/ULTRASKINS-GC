using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace UltraSkins.UI.Settings
{
    public class AboutMenu : MonoBehaviour
    {
        // Start is called before the first frame update
        [Header("GameObjects for the about page")]
        public TextMeshProUGUI verblock;
        public TextMeshProUGUI GCblock;
        public TextMeshProUGUI SPFblock;

        public GameObject HPtag;
        public GameObject MDtag;
        public GameObject TStag;
        void Start()
        {
        
        }
        public void SetAboutVersionInfo(string version, string buildtype, string ogskinsverson, string[] supportedformats)
        {
            verblock.text = version + "-" + buildtype;
            GCblock.text = ogskinsverson;
            SPFblock.text = string.Join(", ", supportedformats);
        }
        // Update is called once per frame
        

        void Update()
        {
        
        }
    }
}
