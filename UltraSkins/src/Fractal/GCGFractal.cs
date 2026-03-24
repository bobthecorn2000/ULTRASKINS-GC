using BatonPassLogger;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltraSkins.API;
using UnityEngine;

using static UltraSkins.ULTRASKINHand;

namespace UltraSkins.Fractal
{
    public class GCGFractal : BaseFractal
    {
        private int GCGnum = -1;
        private bool GCGAlt = false;
        GunColorGetter GCGref;
        WeaponIcon WPI;
        bool NoDynamicColor = false;


        
        public void Init(GunColorGetter GCG)
        {

            //not how i would have done it but whatever
            //1:Pistol 2:Shotgun 3:Nailgun 4:Railcannon 5:RocketLauncher
            //1+Alt:SlabRevolver 2+Alt:JackHammer 3+Alt:SawbladeLauncher
            GCGnum = GCG.weaponNumber;
            GCGAlt = GCG.altVersion;
            GCGref = GCG;
            swapType = SwapType.Weapon;
            WPI = transform.GetComponentInParent<WeaponIcon>();
            if (WPI != null)
            {
                FractalWPIhandler.AddToWPItracker(gameObject, WPI);
            }
            else
            {
                NoDynamicColor = true;
            }

        }
        


        protected override void setupRenderer()
        {
            base.setupRenderer();
            try
            {
                string swapname;
                foreach (Material mat in GCGref.defaultMaterials)
                {
                    swapname = "Swapped_" + swapType + "_" + mat.name;
                    if (!HoldEm.Instance.MaterialNames.ContainsKey(swapname))
                    {
                        string textureName = (mat.HasProperty("_MainTex") && mat.mainTexture != null) ? mat.mainTexture.name : null;
                        HoldEm.Instance.MaterialNames.Add(swapname, textureName);
                    }
                }
                foreach (Material mat in GCGref.coloredMaterials)
                {
                    swapname = "Swapped_" + swapType + "_" + mat.name;
                    if (!HoldEm.Instance.MaterialNames.ContainsKey(swapname))
                    {
                        string textureName = (mat.HasProperty("_MainTex") && mat.mainTexture != null) ? mat.mainTexture.name : null;
                        HoldEm.Instance.MaterialNames.Add(swapname, textureName);
                    }
                }
            }
            catch(Exception EX)
            {
                BatonPass.Error("Renderer could not be set up, Code-\"GCGFRACTAL-RENDERERSETUP-EX\" ");
                BatonPass.Error(EX.ToString());
            }

        }

        protected override void DoSwapLogic(Material mat, string texturename)
        {
            base.DoSwapLogic(mat, texturename);
            if (NoDynamicColor)
            {
                base.DoEmissiveSwap(mat);
            }
            
        }


        public override void UpdateMaterials()
        {
            for (int i = 0; i < GCGref.defaultMaterials.Length; i++)
            {
                PerformTheSwap(GCGref.defaultMaterials[i], forceswap);

            }
            for (int i = 0; i < GCGref.coloredMaterials.Length; i++)
            {
                PerformTheSwap(GCGref.coloredMaterials[i], forceswap);

            }

        }
    }
}