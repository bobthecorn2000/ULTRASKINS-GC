using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UltraSkins.UI.Camp
{
    public class ViewFinderSearch : MonoBehaviour
    {
        // Start is called before the first frame update
        public TMP_InputField inputfield;
        public Transform content;
        private List<ButtonEnableManager> items = new();

        void OnEnable()
        {
            RefreshItems();
            inputfield.onValueChanged.AddListener(OnSearchChanged);
        }

        void OnDisable()
        {
            inputfield.onValueChanged.RemoveListener(OnSearchChanged);
        }

        void Start()
        {

        }
        void OnSearchChanged(string query)
        {
            query = query.Trim().ToLowerInvariant();

            foreach (var item in items)
            {
                bool visible =
                    string.IsNullOrEmpty(query) ||
                    item.SkinName.ToLowerInvariant().Contains(query) || item.SkinDescription.ToLowerInvariant().Contains(query) || item.Author.ToLowerInvariant().Contains(query);

                item.gameObject.SetActive(visible);
            }
        }
        // Update is called once per frame
        void Update()
        {

        }

        public void RefreshItems()
        {
            items.Clear();
            items.AddRange(content.GetComponentsInChildren<ButtonEnableManager>(true));
        }
    }
}
