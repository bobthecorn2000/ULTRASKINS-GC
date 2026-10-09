using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace UltraSkins.UI.Widgets
{
    public class PullOutShifter : MonoBehaviour
    {
        // Start is called before the first frame update
        public RectTransform PreviewBox;
        public GameObject PulloutBox;


        void Start()
        {
        
        }

        public void HidePulloutBox()
        {
            PulloutBox.SetActive(false);
            PreviewBox.anchoredPosition = new Vector2(0,0);
        }
        public void ShowPulloutBox()
        {
            PreviewBox.anchoredPosition = new Vector2(-130,0);
            PulloutBox.SetActive(true);
        }
    }
}
