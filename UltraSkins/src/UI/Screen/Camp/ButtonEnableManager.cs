using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
namespace UltraSkins.UI.Camp

{
    public class ButtonEnableManager : MonoBehaviour
    {
        [SerializeField] private GameObject Exclaim;
        [SerializeField] private bool isEnabled = true;
        [SerializeField] public bool thunderstore = false;
        public Image border;
        public SkinDetails skinDetails;
      
        public Texture2D RawIcon;
        private Sprite Icon;
/*         public bool warning;
        public bool error; */

        public ErrorType ET = ErrorType.None;

        public string filePath;

        [Header("Data")]
        public string SkinName;
        public string SkinDescription = null;
        public string VerNum;
        public string warningmessage;
        public string Author;
        public bool isplugin;
        public DetailObject DETOB = new DetailObject();

        public enum ErrorType
        {
            None,
            Error,
            Warning,
        }

/*         public bool IsEnabled
        {
            get => isEnabled;
            set
            {
                isEnabled = value;
                if (componentToToggle != null)
                    componentToToggle.enabled = isEnabled;
            }
        } */



        public void DecideColor()
        {
            switch (ET)
            {
                case ErrorType.Error:
                Exclaim.SetActive(true);
                Exclaim.GetComponent<Image>().color = Color.red;
                break;
                case ErrorType.Warning:
                Exclaim.SetActive(true);
                Exclaim.GetComponent<Image>().color = Color.yellow;
                break;
                case ErrorType.None:
                Exclaim.SetActive(false);
                break;
            }
            if (thunderstore)
            {
                border.color = new Color32(23, 255, 176, 255);
            }
            else
            {
                border.color = Color.white;
            }
        }
        void OnDestroy()
        {
            if (Icon)
            {
                Destroy(Icon);
            }
            

        }
        public void Toggle()
        {
/*             isEnabled = !isEnabled;
            if (componentToToggle != null)
                componentToToggle.enabled = isEnabled; */
        }
        void Start()
        {
     
                            Icon = Sprite.Create(RawIcon, new Rect(0, 0, RawIcon.width, RawIcon.height), new Vector2(0.5f, 0.5f));
/*             if (componentToToggle != null)
            {
                componentToToggle.enabled = isEnabled;
            } */
            makeObject();
            


        }


        public void callRefresh()
        {
            if (skinDetails != null)
            {

                skinDetails.OnRefreshDetails(DETOB);


            }

        }
        public void makeObject()
        {
            if (SkinDescription != null)
                DETOB.SkinDescription = SkinDescription;

            if (Icon != null)
                DETOB.icon = Icon;

            if (Author != null)
                DETOB.Author = Author;

            if (warningmessage != null)
                DETOB.warningmessage = warningmessage;

            if (VerNum != null)
                DETOB.versionNumber = VerNum;

            DETOB.isPlugin = isplugin;  

            if (SkinName != null)
                DETOB.SkinName = SkinName;
        }

        void OnValidate()
        {
/*             if (componentToToggle != null)
                componentToToggle.enabled = isEnabled; */
        }
    }
}