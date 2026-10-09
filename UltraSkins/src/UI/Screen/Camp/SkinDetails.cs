using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UltraSkins.UI.Animation;

namespace UltraSkins.UI.Camp
{
    public class SkinDetails : MonoBehaviour
    {
        // Start is called before the first frame update
        public TMP_Text skinNameHolder;
        public TMP_Text skinDescHolder;
        public TypeWriter TP;
        public GameObject container;
        public Animator animator;
        public Sprite FallBackIcon;
        public bool warningmode = false;
        public TMP_Text warningmessage;
        public TMP_Text AuthorHolder;

        public Image IconImage;
        public GameObject PluginLogo;

        public TMP_Text versionNumberHolder;
        

        

        public void OnRefreshDetails(DetailObject detob)
        {
            //container.SetActive(false);
            versionNumberHolder.text = "";

            if (detob.isPlugin == true)
            {
                PluginLogo.SetActive(true);
            }
            else
            {
                PluginLogo.SetActive(false);
            }
            skinNameHolder.text = detob.SkinName;
            TP.gameObject.SetActive(false);
            if (!string.IsNullOrEmpty(detob.SkinDescription))
            {
                
                skinDescHolder.text = detob.SkinDescription;
                TP.RefreshFullText(detob.SkinDescription);
            }
            else
            {
                skinDescHolder.text = "No Info";
                TP.RefreshFullText("No Info");
            }
            TP.gameObject.SetActive(true);
            if (detob.icon != null)
            {
                IconImage.sprite = detob.icon;
            }
            else
            {
                IconImage.sprite = FallBackIcon;
            }

            if (!string.IsNullOrEmpty(detob.Author))
            {
                AuthorHolder.text = detob.Author;
            }
            else
            {
                AuthorHolder.text = "";
            }

            if (!string.IsNullOrEmpty(detob.versionNumber))
            {
                versionNumberHolder.text = "Ver: " + detob.versionNumber;
            }
            else
            {
                versionNumberHolder.text = "Ver: ???";
            }

            if (!string.IsNullOrEmpty(detob.warningmessage))
            {
                if (!warningmode)
                {
                    animator.SetBool("IsWarningActive", true);
                }
                warningmode = true;
                warningmessage.text = detob.warningmessage;

            }
            else
            {
                if (warningmode)
                {
                    animator.SetBool("IsWarningActive", false);
                }
                warningmode = false;
                warningmessage.text = "";
            }

            //container.SetActive(true);
        }

        void Start()
        {
            container.SetActive(false);
            skinNameHolder.text = "";
            skinDescHolder.text = "";
            AuthorHolder.text = "";
            TP.RefreshFullText("");
            PluginLogo.SetActive(false);
            versionNumberHolder.text = "";
            container.SetActive(true);
            
        }



        // Update is called once per frame
    }

    public class DetailObject
    {
        public string SkinName { get; set; }
        public string SkinDescription { get; set; }
        public bool isPlugin { get; set; }

        public Sprite icon { get; set; }

        public string versionNumber { get; set; }

        public string warningmessage { get; set; }

        public string Author { get; set; }
    }
}
