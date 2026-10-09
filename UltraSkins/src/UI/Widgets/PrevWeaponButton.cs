using System.Collections;
using System.Collections.Generic;
 
using UnityEngine;
using UnityEngine.UI;
using UltraSkins.UI.Layout;

namespace UltraSkins.UI.Widgets
{
    public class PrevWeaponButton : MonoBehaviour
    {
        // Start is called before the first frame update
        public ChildCycler.PWeaponModel changetotype;
       // public bool SupportsIDTex;
        //public bool SupportsEmissive;
        public ChildCycler childCycler;

        public UIColorSlider uiColorSlider;

        void Start()
        {
            Button button = GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(OnButtonClicked);
            }
            else
            {
                Debug.LogWarning($"No Button component found on {gameObject.name}");
            }
        
        }
        void OnButtonClicked()
        {
            childCycler.ShowChildAt(changetotype);
/*             if (SupportsIDTex == false)
            {
                uiColorSlider.modechange(false);
                uiColorSlider.SupportID(false);
            }
            else
            {
                uiColorSlider.SupportID(true);
            } */  
                
        }
        // Update is called once per frame

    }
}
