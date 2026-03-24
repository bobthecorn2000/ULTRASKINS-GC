using System;
using System.Collections.Generic;
using System.Text;
using BatonPassLogger;
using Sandbox.Arm;
using UnityEngine;

namespace UltraSkins.Fractal
{
    public class ArmFractal : IlluminatedGenericFractal
    {


        public void Init(Punch P)
        {
            swapType = SwapType.Arm;
            switch (P.type)
            {
                case FistType.Standard:
                    varcolor = 0;
                    subType = SubType.FB;
                    break;
                case FistType.Heavy:
                    varcolor = 2;
                    subType = SubType.KB;
                    break;
            }

        }
        public void Init(HookArm HA)
        {
            varcolor = 1;
            swapType = SwapType.Arm;
            subType = SubType.WL;
        }


        public void Init(Revolver rev)
        {
            WeaponIcon WPI = rev.GetComponent<WeaponIcon>();
            varcolor = (int)WPI.weaponDescriptor.variationColor;
            subType = SubType.RightArm;
            swapType = SwapType.Arm;
        }

        public void Init(SandboxArm sandarm)
        {
            WeaponIcon WPI = sandarm.GetComponent<WeaponIcon>();
            varcolor = (int)WPI.weaponDescriptor.variationColor;
            subType = SubType.SandBox;
            swapType = SwapType.Arm;
        }


        protected override void DoSwapLogic(Material mat, string texturename)
        {
            base.DoSwapLogic(mat, texturename);
            OnCBSUpdated();
        }



    }
}
