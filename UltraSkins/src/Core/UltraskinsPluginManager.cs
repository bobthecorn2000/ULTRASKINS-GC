using System;
using System.Collections.Generic;
using System.Text;

namespace UltraSkins.API
{
    public class USAPI
    {
        public class TextureLoadEventArgs : EventArgs
        {
            public bool Failed { get; }


            public TextureLoadEventArgs(bool failed)
            {
                Failed = failed;

            }
        }
        public static event Action<TextureLoadEventArgs?> OnTexLoadFinished;
        internal static void BroadcastTextureFinished(TextureLoadEventArgs TLEA)
        {
            OnTexLoadFinished?.Invoke(TLEA);
        }



        public class FractalTextureUpdateArgs : EventArgs
        {
            public bool doAll { get; }
            public Fractal.BaseFractal.SwapType? swaptype{ get; }
            public int? GCGgroup { get; }
            public bool? GCGalt { get; }


            public FractalTextureUpdateArgs(bool doall) {
                doAll = doall;
            }

            public FractalTextureUpdateArgs (Fractal.BaseFractal.SwapType type)
            {
                swaptype = type;
            }
            public FractalTextureUpdateArgs (int group,bool alt)
            {
                swaptype = Fractal.BaseFractal.SwapType.Weapon;
                GCGgroup = group;
                GCGalt = alt;
            }
        }


        public static event EventHandler<FractalTextureUpdateArgs?> RefreshFractals;

        public static void BroadcastTextureUpdate(object sender, FractalTextureUpdateArgs args)
        {
            RefreshFractals?.Invoke(sender, args);
        }


        public static event Action DoDynamicEmissiveSwap;

        public static void BroadcastDynEmSwap()
        {
            DoDynamicEmissiveSwap?.Invoke();
        }

    }
}
