using System;
using System.Collections.Generic;
using System.Text;
using BatonPassLogger;

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



        protected override void DoSwapLogic(Material mat, string texturename)
        {
            base.DoSwapLogic(mat, texturename);
            OnCBSUpdated();
        }



    }
}
