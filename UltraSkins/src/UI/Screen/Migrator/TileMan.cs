using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UltraSkins.UI.Migrator
{
    public class TileMan : MonoBehaviour
    {
        // Start is called before the first frame update
        public TextMeshProUGUI Text;
        public Image selected;
        public Image normal;
        public string FolderPath;
        public Toggle toggle;

        public void AddToGroup(ToggleGroup togglegroup)
        {
            toggle.group = togglegroup;
        } 

        public void ChangeSelectedColor(Color color)
        {
            selected.color = color;
        }
        public void ChangeNormalColor(Color color)
        {
            normal.color = color;
        }
        public void IsToggle(bool value)
        {
            toggle.interactable = value;
        }
        public void ChangeText(string newtext)
        {
            Text.text = newtext;
        }
       
    }
}
