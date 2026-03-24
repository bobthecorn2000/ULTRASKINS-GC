using System;
using System.Collections.Generic;
using System.Text;
using UltraSkins.API;
using UltraSkins.Fractal;
using UnityEngine;

namespace UltraSkins.Fractal
{
    public class IlluminatedGenericFractal : BaseFractal
    {
        public int varcolor = 0;

        public void OnEnable()
        {
            OnCBSUpdated();
        }


        protected override void Awake()
        {
            base.Awake();
            USAPI.DoDynamicEmissiveSwap += OnCBSUpdated;
        }

        protected void OnCBSUpdated()
        {
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            if (renderer)
            {
                renderer.GetPropertyBlock(mpb);
                if (renderer.sharedMaterial.HasProperty("_EmissiveColor"))
                {
                    mpb.SetColor("_EmissiveColor", MonoSingleton<ColorBlindSettings>.Instance.variationColors[varcolor]);
                }
                renderer.SetPropertyBlock(mpb);
            }
        }

        protected override void OnDestroy()
        {
            USAPI.DoDynamicEmissiveSwap -= OnCBSUpdated;
            base.OnDestroy();
            
        }

    }
}
