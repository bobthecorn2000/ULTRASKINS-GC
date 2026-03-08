using BatonPassLogger;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UltraSkins.API;
using UnityEngine;
using UnityEngine.UI;
using static UltraSkins.ULTRASKINHand;

namespace UltraSkins.Fractal
{
    public class DialFractal : BaseFractal
    {
        ShotgunHammer JackHammer;
        public void Init(ShotgunHammer ShotHam)
        {
            subType = SubType.Hammer;
            JackHammer = ShotHam;
        }


        public override void PrepareSwap(object sender, USAPI.FractalTextureUpdateArgs args)
        {
            if (args.doAll)
            {
                UpdateMaterials();
            }
        }
        public override void PrepareSwap(bool fs = false)
        {
            forceswap = fs;
            UpdateMaterials();
        }

        public override void UpdateMaterials()
        {
            ReadOut.SwapTheDial(this);
            ReadOut.updateMeter(JackHammer, true);
        }




        internal class ReadOut
        {

            private static readonly AccessTools.FieldRef<ShotgunHammer, Texture[]> meterEmissivesRef = AccessTools.FieldRefAccess<ShotgunHammer, Texture[]>("meterEmissives");

            private static readonly AccessTools.FieldRef<ShotgunHammer, Image> secondaryMeterRef = AccessTools.FieldRefAccess<ShotgunHammer, Image>("secondaryMeter");

            internal delegate void UpdateMeterDelegate(ShotgunHammer instance, bool forceUpdateTexture = false);

            static MethodInfo updateMeterMethod = AccessTools.Method(typeof(ShotgunHammer), "UpdateMeter");

            internal static UpdateMeterDelegate updateMeter = AccessTools.MethodDelegate<UpdateMeterDelegate>(updateMeterMethod);


            internal static void SwapTheDial(DialFractal dialFractal)
            {


                if (meterEmissivesRef == null)
                {
                    BatonPass.Error("Failed to find 'meterEmissives' field.");
                    return;
                }
                
                var meterEmissives = meterEmissivesRef(dialFractal.JackHammer);
                var meterMask = secondaryMeterRef(dialFractal.JackHammer);
                Texture glow1;
                Texture glow2;
                Texture glow3;
                if (HoldEm.Check("T_DialGlow1"))
                {
                    glow1 = HoldEm.Call("T_DialGlow1");
                }
                else
                {
                    glow1 = meterEmissives[0];
                }
                if (HoldEm.Check("T_DialGlow2"))
                {
                    glow2 = HoldEm.Call("T_DialGlow2");
                }
                else
                {
                    glow2 = meterEmissives[1];
                }
                if (HoldEm.Check("T_DialGlow3"))
                {
                    glow3 = HoldEm.Call("T_DialGlow3");
                }
                else
                {
                    glow3 = meterEmissives[2];
                }
                meterEmissivesRef(dialFractal.JackHammer) = new Texture[3]
                    {
                        glow1,
                        glow2,
                        glow3,
                    };

                if (HoldEm.Check("T_DialMask"))
                {
                    Texture dialmask = HoldEm.Call("T_DialMask");

                    Sprite masksprite = Sprite.Create((Texture2D)dialmask, new Rect(0, 0, dialmask.width, dialmask.height), new Vector2(0.5f, 0.5f));
                    meterMask.sprite = masksprite;
                }
            }
        }
    }

}
