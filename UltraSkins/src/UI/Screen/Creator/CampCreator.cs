using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UltraSkins.ExMan;

namespace UltraSkins.UI.Creator
{
    public class CampCreator : MonoBehaviour
    {
        // Start is called before the first frame update 

        public bool TemplateMode;

        public string SkinName;
        public string SkinDescription;

        public string SkinAuthor;
        public TMP_InputField tmpSkinName;

        public string skinIconPath;
        public string SkinIconPath
        {
            get => skinIconPath;
            set
            {
                if (skinIconPath == value) return;
                skinIconPath = value;
                if (!string.IsNullOrWhiteSpace(value))
                {
                    iconNoteField.text = "Selected: <color=green>" + Path.GetFileName(value) + "</color>";
                }
                else
                {
                    iconNoteField.text = "<color=red>NO VALUE</color> is currently selected";
                }
                
            }
        }
        public TextMeshProUGUI iconNoteField;
        public OpenFileExplorer IconSkinExp;

        public TMP_InputField tmpVer;
        public TMP_InputField tmpAuthor;

        public TMP_InputField tmpDesc;

        public Button CreateButton;

        // Update is called once per frame
        void Start()
        {
            IconSkinExp.OnPathSelected = path => SkinIconPath = path;
            tmpSkinName.onValueChanged.AddListener((string thing) => skinnamechange());
            tmpDesc.onValueChanged.AddListener((string thing) => Descnamechange());
            tmpSkinName.onEndEdit.AddListener((string thing) => validatetext());
            tmpDesc.onEndEdit.AddListener((string thing) => validatetext());
        }
        public void SetTemplateMode(bool value)
        {
            TemplateMode = value;
        }
        private void skinnamechange()
        {
            SkinName = tmpSkinName.text;
        }
        private void Descnamechange()
        {
            SkinDescription = tmpDesc.text;
        }
        private void Authnamechange()
        {
            SkinAuthor = tmpAuthor.text;
        }

        private void validatetext()
        {
            if (!string.IsNullOrWhiteSpace(SkinName) && !string.IsNullOrWhiteSpace(SkinDescription))
            {
                CreateButton.interactable = true;
            }
            else
            {
                CreateButton.interactable = false;
            }
        }
        /*         private string[] DepGet()
                                {
                                    string[] Dependencys = tmpDep.text.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();
                                    return Dependencys;
                                } */
        public TEMPGCMD GatherInfo()
        {
            TEMPGCMD tempGCMD = new TEMPGCMD();

            tempGCMD.Name = SkinName;
            tempGCMD.Description = SkinDescription;

            if (!string.IsNullOrWhiteSpace(SkinIconPath))
            {
                tempGCMD.iconpath = SkinIconPath;
            }
            if (!string.IsNullOrWhiteSpace(tmpVer.text))
            {
                tempGCMD.Version = tmpVer.text;
            }
            if (!string.IsNullOrWhiteSpace(tmpAuthor.text))
            {
                tempGCMD.Author = tmpAuthor.text;
            }
            return tempGCMD;
        }

        public void ClearAllEntrys()
        {
            tmpSkinName.text = "";
            tmpDesc.text = "";
            tmpVer.text = "";
            IconSkinExp.returnedValue = "";
            SkinIconPath = "";
        }
        public class TEMPGCMD
        {

            public string Name { get; set; }
            public string Description { get; set; }

            public string Author { get; set; }
            public string? iconpath { get; set; }
            public string? Version { get; set; }
            //public Dictionary<string, string>? SupportedPlugins { get; set; }

        }
    }
}
