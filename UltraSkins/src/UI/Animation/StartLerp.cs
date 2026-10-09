using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltraSkins.UI.Animation
{
    public class StartLerp : MonoBehaviour
    {
        // Start is called before the first frame update
        public LerpRectTransformFull USPreview;
        void OnEnable()
        {
            USPreview.MenuModeOn();
        }
        void OnDisable()
        {
            USPreview.MenuModeOff();
        }


    }
}
