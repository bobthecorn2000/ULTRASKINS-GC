using System.Collections;
using System.Collections.Generic;

using UltraSkins.Prism;

using UnityEngine;
using UnityEngine.UI;

namespace UltraSkins.Prism
{
    public class PrismColorTypeGetter : MonoBehaviour
    {
        // Start is called before the first frame update
        int currentmode = 0;
        int currentPreset = 0;

        private void Awake()
        {
            // do template stuff
            PrismManager.Instance.TryInitCache(WeaponName);

        }

        private void OnEnable()
        {
            foreach (PrismColorSetter PCS in prismColorSetters)
            {
                PCS.Init();
            }
            currentmode = PrismManager.Instance.GetFlagByID(WeaponName, RayMaker.flags.Mode);
            switch (currentmode)
            {
                case 0:
                    templateMenu.SetActive(true);
                    CustomMenu.SetActive(false);
                    OpenTemplateButton.interactable = false;
                    OpenCustomButton.interactable = true;
                    foreach (PrismColorGetter PCG in prismColorGetters)
                    {
                        PCG.DisableCustomColors();
                        PCG.UpdateStoredColors();
                    }
                    break;
                case 1:
                    OpenTemplateButton.interactable = true;
                    OpenCustomButton.interactable = false;
                    templateMenu.SetActive(false);
                    CustomMenu.SetActive(true);
                    foreach (PrismColorGetter PCG in prismColorGetters)
                    {

                        PCG.EnableCustomColors();
                        PCG.UpdateStoredColors();
                    }
                    break;
                case 2:
                    OpenTemplateButton.interactable = false;
                    OpenCustomButton.interactable = true;
                    templateMenu.SetActive(true);
                    CustomMenu.SetActive(false);
                    foreach (PrismColorGetter PCG in prismColorGetters)
                    {

                        PCG.EnableCustomColors();
                        PCG.UpdateStoredColors();
                    }
                    break;
            }
            syncTemplatePageLayout();
        }

        void syncTemplatePageLayout()
        {
            currentPreset = PrismManager.Instance.GetFlagByID(WeaponName, RayMaker.flags.Preset);
            if (currentPreset == 0)
            {
                StandardButton.interactable = false;
                foreach (Button btn in TemplateButtons)
                {
                    btn.interactable = true;
                }
            }
            else
            {
                StandardButton.interactable = true;
                for (int i = 0; i < TemplateButtons.Count; i++)
                {
                    if (currentPreset - 1 == i)
                    {
                        TemplateButtons[i].interactable = false;
                    }
                    else
                    {
                        TemplateButtons[i].interactable = true;
                    }

                }
            }
        }

        public void setTemplate(int index)
        {
            if (index == 0)
            {
                PrismManager.Instance.SetFlagByID(WeaponName, RayMaker.flags.Mode, 0);
                PrismManager.Instance.SetFlagByID(WeaponName, RayMaker.flags.Preset, 0);
                foreach (PrismColorGetter PCG in prismColorGetters)
                {
                    PCG.DisableCustomColors();

                }
            }
            else
            {

                PrismManager.Instance.SetFlagByID(WeaponName, RayMaker.flags.Mode, 2);
                PrismManager.Instance.SetFlagByID(WeaponName, RayMaker.flags.Preset, index);
                foreach (PrismColorGetter PCG in prismColorGetters)
                {
                    PCG.EnableCustomColors();

                }
            }
            syncTemplatePageLayout();
            UpdatePreview();
            //TODO soul orb behaviors
        }


        public void UpdatePreview()
        {
            foreach (PrismColorGetter PCG in prismColorGetters)
            {
                PCG.UpdateStoredColors();
            }
        }

        public void SetMode(bool custom)
        {
            templateMenu.SetActive(!custom);
            OpenTemplateButton.interactable = custom;
            //TODO sound logic for template button
            CustomMenu.SetActive(custom);
            OpenCustomButton.interactable = !custom;
            if (custom)
            {
                PrismManager.Instance.SetFlagByID(WeaponName, RayMaker.flags.Mode, 1);
                foreach (PrismColorGetter PCG in prismColorGetters)
                {
                    PCG.EnableCustomColors();

                }
            }
            else
            {
                int presetflag = PrismManager.Instance.GetFlagByID(WeaponName, RayMaker.flags.Preset);
                if (presetflag != 0)
                {
                    PrismManager.Instance.SetFlagByID(WeaponName, RayMaker.flags.Mode, 2);
                }
                else
                {
                    PrismManager.Instance.SetFlagByID(WeaponName, RayMaker.flags.Mode, 0);
                    foreach (PrismColorGetter PCG in prismColorGetters)
                    {
                        PCG.DisableCustomColors();

                    }
                }

            }
            //TODO sound logic for custom button
            UpdatePreview();

        }

        void OnDisable()
        {
            PrismBeam.FireColorBeam(WeaponName);
            PrismManager.Instance.SaveColorsToDisk(WeaponName);
            PrismManager.Instance.SaveFlagToDisk(WeaponName, RayMaker.flags.Mode);
            PrismManager.Instance.SaveFlagToDisk(WeaponName, RayMaker.flags.Preset);
        }

        void Start()
        {

        }

        public string WeaponName;
        public GameObject templateMenu;
        public GameObject CustomMenu;
        public GameObject previewModel;
        public PrismColorGetter[] prismColorGetters;
        public PrismColorSetter[] prismColorSetters;

        public Button StandardButton;
        public List<Button> TemplateButtons;

        public PrismTemplateColors ColorSet;
        public Button OpenTemplateButton;
        public Button OpenCustomButton;


    }
}
