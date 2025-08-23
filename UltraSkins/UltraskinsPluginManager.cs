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
    }
}
