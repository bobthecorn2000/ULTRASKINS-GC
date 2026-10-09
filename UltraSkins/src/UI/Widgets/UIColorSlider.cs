using System.Collections;
using System.Collections.Generic;
using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace UltraSkins.UI.Widgets
{
    public class UIColorSlider : MonoBehaviour
    {
        // Start is called before the first frame update
        [SerializeField] Slider sliderR;
        [SerializeField] Slider sliderG;
        [SerializeField] Slider sliderB;
        [SerializeField] TMP_Dropdown dropdown;
        [SerializeField] Toggle toggle;
        [SerializeField] Image toggleimage;
        [SerializeField] TMP_InputField HexCode;
        private bool Guard = false;
        private bool hexGuard = false;

        void Start()
        {
            
            sliderR.onValueChanged.AddListener((float thing) => SliderBroadcast());
            sliderG.onValueChanged.AddListener((float thing) => SliderBroadcast());
            sliderB.onValueChanged.AddListener((float thing) => SliderBroadcast());
            dropdown.onValueChanged.AddListener((int thing) => dropValueChange(thing));
            toggle.onValueChanged.AddListener((bool thing) => modechange(thing));
            HexCode.onValueChanged.AddListener((string thing) => SetColorFromHex(thing));
        }
        void OnEnable()
        {
            
        }

        public void SliderBroadcast()
        {
            if (!Guard)
            {
                Color color = new Color(sliderR.value, sliderG.value, sliderB.value);
                switch (dropdown.value)
                {
                    case 0:

                        Shader.SetGlobalColor("_US_ECP", color);
                        break;
                    case 1:
                        Shader.SetGlobalFloat("_US_Pmode", 1);
                        Shader.SetGlobalColor("_US_CCP1", color);
                        break;
                    case 2:
                        Shader.SetGlobalColor("_US_CCP2", color);
                        break;
                    case 3:
                        Shader.SetGlobalColor("_US_CCP3", color);
                        break;
                }
                hexGuard = true;
                HexCode.text = gethexcolor(color.r, color.g, color.b);
                HexCode.ForceLabelUpdate();
                hexGuard = false;
            }

        }
        public void dropValueChange(int selected)
        {
            Color color = new Color();
            switch (selected)
            {
                case 0:

                    color = Shader.GetGlobalColor("_US_ECP");
                    break;
                case 1:

                    color = Shader.GetGlobalColor("_US_CCP1");
                    break;
                case 2:
                    color = Shader.GetGlobalColor("_US_CCP2");
                    break;
                case 3:
                    color = Shader.GetGlobalColor("_US_CCP3");

                    break;
            }
            Guard = true;

            //sliderR.value = color.r;
            //sliderG.value = color.g;
            // sliderB.value = color.b;


            if (this.gameObject.activeInHierarchy)
            {
                StartCoroutine(SlideToNewPosition(color.r, color.g, color.b));
            }
            else
            {
                MoveToPosInstant(color.r, color.g, color.b);
            }

            hexGuard = true;
            HexCode.text = gethexcolor(color.r, color.g, color.b);
            HexCode.ForceLabelUpdate();
            hexGuard = false;   
            
        }
        public void SetColorFromHex(string input) {
            if (input.Length == 6)
            {
                if (ColorUtility.TryParseHtmlString("#" + input, out Color color))
                {

                    switch (dropdown.value)
                    {
                        case 0:

                            Shader.SetGlobalColor("_US_ECP", color);
                            break;
                        case 1:
                            Shader.SetGlobalFloat("_US_Pmode", 1);
                            Shader.SetGlobalColor("_US_CCP1", color);
                            break;
                        case 2:
                            Shader.SetGlobalColor("_US_CCP2", color);
                            break;
                        case 3:
                            Shader.SetGlobalColor("_US_CCP3", color);
                            break;
                    }
                    
                    //  sliderR.value = color.r;
                    // sliderG.value = color.g;
                    // sliderB.value = color.b; 
                    if (!hexGuard)
                    {
                        Guard = true;
                        StartCoroutine(SlideToNewPosition(color.r, color.g, color.b));
                    }                   
                    
                    Debug.Log("started coruteneeee");
                }
            }
        }
        private string gethexcolor(float r, float g, float b)
        {
            int ri = Mathf.RoundToInt(r * 255f);
            int gi = Mathf.RoundToInt(g * 255f);
            int bi = Mathf.RoundToInt(b * 255f);
            return $"{ri:X2}{gi:X2}{bi:X2}";
        }

        IEnumerator SlideToNewPosition(float r, float g, float b)
        {
            float start = Time.realtimeSinceStartup;
            float duration = 0.1f;
            float startr = sliderR.value;
            float startg = sliderG.value;
            float startb = sliderB.value;
        
            while (Time.realtimeSinceStartup - start < duration)
            {
                float t = (Time.realtimeSinceStartup - start) / duration;
                sliderR.value = Mathf.Lerp(startr, r, t);
                sliderG.value = Mathf.Lerp(startg, g, t);
                sliderB.value = Mathf.Lerp(startb, b, t);
                yield return null; // wait one frame
            }
            sliderB.value = b;
            sliderG.value = g;
            sliderR.value = r;
            Guard = false;
        
        }
        private void MoveToPosInstant(float r, float g, float b)
        {
            sliderB.value = b;
            sliderG.value = g;
            sliderR.value = r;
            Guard = false;
        }


        public void modechange(bool change)
        {
            if (change)
            {
                Shader.SetGlobalInt("_US_Pmode", 1);
                toggleimage.color = Color.red;
                dropdown.interactable = true;

            }
            else
            {
                Shader.SetGlobalInt("_US_Pmode", 0);
                dropdown.interactable = false;
                dropdown.value = 0;
                dropdown.RefreshShownValue();
                toggleimage.color = Color.white;
            }
            if (toggle.isOn != change){
                toggle.isOn = change;
            }
        }

        public void SupportID(bool supported)
        {
            if (supported == true)
            {
                toggle.interactable = true;
            }
            else
            {
                toggle.interactable = false;
            }
        }
    }
}
