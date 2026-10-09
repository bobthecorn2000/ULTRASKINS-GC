using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;  
namespace UltraSkins.ExMan
{
    public class OpenFileExplorer : MonoBehaviour
    {
        // Start is called before the first frame update
        Button button;
        public TextMeshProUGUI NoteField;
        public List<string> filter = new List<string>();
        public Action<string> OnPathSelected;

        public string returnedValue;
        public GameObject ExplororPrefab;
        void Start()
        {
            button = GetComponent<Button>();
            if (button != null)
            {
            button.onClick.AddListener(createExplorer);
            }
        }

        public void createExplorer()
        {
            GameObject explorer = Instantiate(ExplororPrefab);
            ExMan exman = explorer.GetComponent<ExMan>();
            exman.filter = filter;
            exman.sourcebutton = this;
        }

        public void onvalueget(string value)
        {
            if (value != null)
            {
                returnedValue = value;
                OnPathSelected?.Invoke(value);
                if (NoteField)
                {
                    NoteField.text = "Selected: <color=green>" + returnedValue + "</color>";

                }
            }
            else
            {
                if (NoteField)
                {
                    NoteField.text = "<color=red>NO VALUE</color> is currently selected";
                }
            }
        }
        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
