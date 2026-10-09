using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Reflection;
namespace UltraSkins.Configuration
{
    public class SettingsButtonScripts : MonoBehaviour
    {
        // Start is called before the first frame update
        string UltraskinsPath;
        //string dlllocation;
        void Start()
        {
            UltraskinsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "bobthecorn2000", "ULTRAKILL", "ultraskinsGC-V2", "GlobalSkins");
            //dlllocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        
        public void OpenUltraskinsGlobalFolder()
        {
            if (Directory.Exists(UltraskinsPath))
            {
                var psi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = UltraskinsPath,
                    UseShellExecute = true
                };
                System.Diagnostics.Process.Start(psi);
            }
        }


    }
}
