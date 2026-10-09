using System.Collections;
using System.Collections.Generic;

using UnityEngine;


namespace UltraSkins.Prism
{
    public class PrismColorGetter : MonoBehaviour
    {
        // Start is called before the first frame update

        Renderer rend;
        public string weaponName;
        MaterialPropertyBlock prismcolors = new MaterialPropertyBlock();
        Color[] colors = new Color[] {new Color(1f, 0f, 0f, 1f), new Color(0f, 1f, 0f, 0f), new Color(0f, 0f, 1f, 1f) };
        private int mode = 0;
        private int preset = 0;

        void Start()
        {
            rend = GetComponent<Renderer>();
            PrismBeam.OnRecieveColorBeam += RecieveBeam;
            if (!string.IsNullOrEmpty(weaponName))
            {
                SyncCurrentState();
                UpdateStoredColors();
            }

        }
        void OnDestroy()
        {
            PrismBeam.OnRecieveColorBeam -= RecieveBeam;
        }
        public void ForceStart()
        {
                SyncCurrentState();
                UpdateStoredColors();
        }
        private void RecieveBeam(string wep)
        {
            if (wep == weaponName)
            {
                SyncCurrentState();
                UpdateStoredColors();
            }
        }

        void OnEnable()
        {
            
        }
        public void SyncCurrentState() {
            mode = PrismManager.Instance.GetFlagByID(weaponName,RayMaker.flags.Mode);
            switch (mode)
            {
                case 0: DisableCustomColors(); break;
                case 1: EnableCustomColors(); break;
                case 2: 
                EnableCustomColors();
                preset = PrismManager.Instance.GetFlagByID(weaponName,RayMaker.flags.Preset);
                break;
                default: break;
            }

        }

        public void SyncColorsForUse()
        {
            if (mode == 1)
            {
                colors = PrismManager.Instance.colorGet(weaponName);
            }
            else if (mode == 2)
            {
                colors = PrismManager.Instance.RequestTemplateColors(weaponName);
            }
            
        }

        public void UpdateStoredColors()
        {

            if (rend != null) {
            SyncColorsForUse();
              
            rend.GetPropertyBlock(prismcolors);
            prismcolors.SetColor("_CustomColor1", colors[0]);
            prismcolors.SetColor("_CustomColor2", colors[1]);
            prismcolors.SetColor("_CustomColor3", colors[2]);
            rend.SetPropertyBlock(prismcolors);
            }
        }
        public void EnableCustomColors()
        {
            if (rend != null)
            {
                
                rend.sharedMaterial.EnableKeyword("CUSTOM_COLORS");
                rend.sharedMaterial.SetFloat("_ReflectionStrength", 0.5f);
                rend.sharedMaterial.SetFloat("_ReflectionFalloff", 7f);
            }
        }
        public void DisableCustomColors()
        {
            if (rend != null)
            {
                
                rend.sharedMaterial.DisableKeyword("CUSTOM_COLORS");
                
            }
        }
        // Update is called once per frame

    }
}
