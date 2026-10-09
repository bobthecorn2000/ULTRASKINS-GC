using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace UltraSkins.UI.Migrator
{
    public class ThunderstoreHandle : MonoBehaviour
    {


        public GameObject SkinTilePrefab;
        public GameObject ContentHolder;
        public ToggleGroup toggleGroup;
        public GameObject PHnone;
        public Button ContinueButton;

        public Dictionary<Toggle, TileMan> SkinTiles = new Dictionary<Toggle, TileMan>();
        void Start()
        {

        }

        void OnEnable()
        {
            if (ContentHolder.transform.childCount > 0)
            {
                PHnone.SetActive(false);
            }
            else
            {
                PHnone.SetActive(true);
            }
        }
        public void AddOptionToUI(string file)
        {
            GameObject AddedTile = Instantiate(SkinTilePrefab, ContentHolder.transform);

            TileMan tileman = AddedTile.GetComponent<TileMan>();
            tileman.IsToggle(true);
            tileman.AddToGroup(toggleGroup);
            tileman.FolderPath = file;
            SkinTiles.Add(tileman.toggle, tileman);
            ColorBlock cb = tileman.toggle.colors;
            cb.normalColor = new Color32(31, 31, 66, 255);
            cb.selectedColor = new Color32(35, 255, 176, 255);
            cb.highlightedColor = new Color32(80, 217, 159, 255);
            cb.pressedColor = new Color32(28,163,245, 255);
            tileman.toggle.colors = cb;
            tileman.ChangeText(Path.GetFileName(file));
            tileman.toggle.onValueChanged.AddListener((_) => CheckIfAnySelected());
        }

        public void RemoveOptionFromUI(GameObject tile)
        {
            throw new NotImplementedException("RemoveOptionFromUI not yet implemented.");
        }
        public void ClearUI()
        {

            foreach (TileMan tile in SkinTiles.Values)
            {

                Destroy(tile.gameObject);
            }
            SkinTiles.Clear();
            PHnone.SetActive(true);
        }
        public string GetSelectedFile()
        {
            foreach (Toggle toggle in toggleGroup.ActiveToggles())
            {

                if (toggle.isOn)
                {
                    SkinTiles.TryGetValue(toggle, out TileMan tile);

                    return tile.FolderPath;
                }
            }
            return null;
        }

        public GameObject GetSelectedGameObject()
        {
            return null;
        }

        private void CheckIfAnySelected()
        {
            bool anySelected = false;

            foreach (var toggle in toggleGroup.ActiveToggles())
            {
                if (toggle.isOn)
                {
                    anySelected = true;
                    break;
                }
            }

            ContinueButton.interactable = anySelected;
        }

    }


}
