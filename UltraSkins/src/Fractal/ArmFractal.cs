using System;
using System.Collections.Generic;
using System.Text;
using BatonPassLogger;
using UnityEngine;

namespace UltraSkins.Fractal
{
    public class ArmFractal : BaseFractal
    {


        public void Init(Punch P)
        {
            swapType = SwapType.Arm;
            switch (P.type)
            {
                case FistType.Standard:
                    subType = SubType.FB;
                    break;
                case FistType.Heavy:
                    subType = SubType.KB;
                    break;
            }

        }
        public void Init(HookArm HA)
        {
            swapType = SwapType.Arm;
            subType = SubType.WL;
        }


        public override Color GetVarationColor()
        {
            Color VariantColor = new Color(0, 0, 0, 0);

            if (subType == SubType.FB)
            {
                VariantColor = new Color(ColorBlindSettings.Instance.variationColors[0].r,
                ColorBlindSettings.Instance.variationColors[0].g,
                ColorBlindSettings.Instance.variationColors[0].b, 1f);

            }
            else if (subType == SubType.KB)
            {
                VariantColor = new Color(ColorBlindSettings.Instance.variationColors[2].r,
                ColorBlindSettings.Instance.variationColors[2].g,
                ColorBlindSettings.Instance.variationColors[2].b, 1f);
            }
            else if (subType == SubType.WL)
            {
                VariantColor = new Color(ColorBlindSettings.Instance.variationColors[1].r,
                    ColorBlindSettings.Instance.variationColors[1].g,
                    ColorBlindSettings.Instance.variationColors[1].b, 1f);
            }
            return VariantColor;
        }
        protected override void DoSwapLogic(Material mat, string texturename)
        {
            base.DoSwapLogic(mat, texturename);
            base.DoEmissiveSwap(mat);
        }



    }
}
