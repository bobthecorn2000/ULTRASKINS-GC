using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

namespace UltraSkins.UI.Migrator
{
    public class MigrationPage : MonoBehaviour
    {
        // Start is called before the first frame update

        public GameObject SkinTilePrefab;

        public GameObject UI;
        public GameObject NoDataBlocker;

        [Header("Good parent")]
        public GameObject GoodParent;
        public GameObject GParNone;
        [Header("Bad parent")]
        public GameObject BadParent;
        public GameObject BParNone;

        public Button continuebutton;

        public List<string> migratableFolders = new List<string>();

        void Start()
        {

        }
        void OnEnable()
        {
            if (GoodParent.transform.childCount > 0)
            {
                GParNone.SetActive(false);
                continuebutton.interactable = true;
            }
            else
            {
                GParNone.SetActive(true);
                continuebutton.interactable = false;
            }
            if (BadParent.transform.childCount > 0)
            {
                BParNone.SetActive(false);
            }
            else
            {
                BParNone.SetActive(true);
            }
        }
        public void ActivateMigratorTool()
        {
            NoDataBlocker.SetActive(false);
            UI.SetActive(true);
        }
        public void AddOptionToSupported(string file)
        {
            GameObject AddedTile = Instantiate(SkinTilePrefab, GoodParent.transform);

            TileMan tileman = AddedTile.GetComponent<TileMan>();

            tileman.FolderPath = file;
            if (!migratableFolders.Contains(file))
            {
                migratableFolders.Add(file);
            }


            tileman.ChangeText(Path.GetFileName(file));

        }

        public void RemoveOptionFromSupported()
        {

        }

        public void AddOptionToUnsupported(string file)
        {
            GameObject AddedTile = Instantiate(SkinTilePrefab, BadParent.transform);

            TileMan tileman = AddedTile.GetComponent<TileMan>();

            tileman.FolderPath = file;

            tileman.ChangeText(Path.GetFileName(file));
        }

        public void ClearMenu()
        {

            int childCount = GoodParent.transform.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                GameObject child = GoodParent.transform.GetChild(i).gameObject;
                GameObject.Destroy(child);
            }
            migratableFolders.Clear();
            GParNone.SetActive(true);
            childCount = BadParent.transform.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                GameObject child = BadParent.transform.GetChild(i).gameObject;
                GameObject.Destroy(child);
            }
            BParNone.SetActive(true);
        }
    }


}

