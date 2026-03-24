using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UltraSkins.Fractal
{
    internal class FractalWPIhandler
    {
        private static readonly AccessTools.FieldRef<WeaponIcon, Renderer[]> varcolor = AccessTools.FieldRefAccess<WeaponIcon, Renderer[]>("variationColoredRenderers");
        public static void AddToWPItracker(GameObject caller, WeaponIcon WPI)
        {
            if (caller.TryGetComponent<Renderer>(out Renderer rend))
            {
                if (!varcolor(WPI).Contains<Renderer>(rend))
                {
                    varcolor(WPI) = varcolor(WPI).AddToArray<Renderer>(rend);
                }

            }
        }
        public void getVarColorAsInt(GameObject caller, WeaponIcon WPI)
        {

        }


    }
}
