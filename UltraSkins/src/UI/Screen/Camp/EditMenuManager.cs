using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UltraSkins.UI.Creator;
using UltraSkins.UI.Migrator;


namespace UltraSkins.UI.Camp
{
    public class EditMenuManager : MonoBehaviour
    {
        // Start is called before the first frame update

        public SkinDetails skindetails;
        public CampCreator campCreator;
        public GameObject Editor;
        public EditMenuInfoBox editMenuInfoBox;

        public ThunderstoreHandle thunderstoreHandle;
        public MigrationPage MigrationPage;

        public GameObject NoSkinsDetectedPage;

        [Header("Bottom Row Buttons")]
        public Button CreateButton;
        public Button ApplyButton;
        public Button PreviewButton;

        [Header("Special Buttons")]
        public Button RefreshButton;
        public Button SearchButton;
        public Button SettingsButton;

        [Header("Columns")]
        public GameObject disabledColumn;
        public GameObject EnabledColumn;

        [Header("Special Buttons for Versions")]
        public GameObject ThunderstoreFixButton;
        public GameObject MigrationToolButton;

        [Header("Audio Sources")]
        public AudioSource EnableMenuSound;
        public void returntomenu()
        {
            editMenuInfoBox.gameObject.SetActive(false);
            Editor.SetActive(true);
            campCreator.ClearAllEntrys();

        } 
        // Update is called once per frame

    }
}
