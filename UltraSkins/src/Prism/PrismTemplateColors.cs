using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltraSkins.Prism
{
    [CreateAssetMenu(fileName = "PrismColorSet", menuName = "Ultraskins/PrismColorSet", order = 1)]
    public class PrismTemplateColors : ScriptableObject
    {
        public string CSName1;
        public Color[] ColorSet1;
        public string CSName2;
        public Color[] ColorSet2;
        public string CSName3;
        public Color[] ColorSet3;
        public string CSName4;
        public Color[] ColorSet4;

    }
}
