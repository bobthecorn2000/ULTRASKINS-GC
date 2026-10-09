using System.Collections;
using System.Collections.Generic;
using UltraSkins.UI;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace UltraSkins.Prism
{
    public class PrismColorSetter : MonoBehaviour
    {
        [Header("ID")]
        public int ID;
        [Header("Icon Thingys")]
        public Image colorIcon;
        public Image metalIcon;
        [Header("Slider Set")]

        public Slider sliderR;
        public Slider sliderG;
        public Slider sliderB;
        public Slider sliderM;
        public PrismColorTypeGetter PCTG;

        private float red;
        private float green;
        private float blue;
        private float metal;

        void Awake()
        {
            sliderR.onValueChanged.AddListener((float thing) => SetRed(thing));
            sliderG.onValueChanged.AddListener((float thing) => SetGreen(thing));
            sliderB.onValueChanged.AddListener((float thing) => SetBlue(thing));
            sliderM.onValueChanged.AddListener((float thing) => SetMetal(thing));
        }


        public void Init()
        {
            Color[] colors = PrismManager.Instance.colorGet(PCTG.WeaponName);
            sliderR.value = colors[ID].r;
            sliderG.value = colors[ID].g;
            sliderB.value = colors[ID].b;
            sliderM.value = colors[ID].a;
            red = colors[ID].r;
            green = colors[ID].g;
            blue = colors[ID].b;
            metal = colors[ID].a;
            colorIcon.color = new Color(colors[ID].r,colors[ID].g,colors[ID].b);
            metalIcon.color = new Color(1,1,1,colors[ID].a);

        }


        public void SetRed(float amount)
        {
            red = amount;
            UpdateColor();
        }
        public void SetGreen(float amount)
        {
            green = amount;
            UpdateColor();
        }
        public void SetBlue(float amount)
        {
            blue = amount;
            UpdateColor();
        }
        public void SetMetal(float amount)
        {
            metal = amount;
            UpdateColor();
        }
        public void UpdateColor()
        {
            Color iconcolor = new Color(red,green,blue);
            Color metalcolor = new Color(1,1,1,metal);
            colorIcon.color = iconcolor;
            metalIcon.color = metalcolor;
            Color cachecolor = new Color(red,green,blue,metal);
            PrismManager.Instance.Cache(PCTG.WeaponName,ID,cachecolor);
            PCTG.UpdatePreview();
        }

    }
}
