using System.Collections;
using System.Collections.Generic;
using TMPro;
using UltraSkins.Configuration;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.Rendering;
using UnityEngine.UI;


namespace UltraSkins.ExMan
{


    public class ExMan : MonoBehaviour
    {
        // Start is called before the first frame update
        string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string downloads = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string pictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        string music = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        string videos = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
        string UltraskinsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "bobthecorn2000", "ULTRAKILL");


        private string currentdir;

        public enum Types
        {
            Free,
            Restricted
        }

        public Types Type;

        public GameObject buttonholder;
        public GameObject nofilefoundblocker;
        public TMP_InputField pathArea;
        [SerializeField] private ToggleGroup fileToggleGroup;
        public List<string> filter;
        public GameObject content;
        public GameObject fileButton;
        public GameObject loadblocker;
        public Button upbutton;
        public string directoryabove;
        public Scrollbar scrollbar;
        public OpenFileExplorer sourcebutton;

        public GameObject errorModel;
        public TextMeshProUGUI errorModelText;

        void Start()
        {
            switch (Type)
            {
                case Types.Free:
                    ChangeDir(desktop);
                    break;
                case Types.Restricted:
                    buttonholder.SetActive(false);
                    pathArea.interactable = false;
                    pathArea.transform.Find("Button").gameObject.SetActive(false);
                    break;

            }

        }
        public void ChangeDir(string path)
        {
            if (path != currentdir)
            {
                StartCoroutine(ChangeDirCo(path));
            }

        }
        public void ChangeDirFolder(string path)
        {
            if (path != currentdir)
            {
                StartCoroutine(FolderSelector(path));
            }

        }

        public void ChangeWithBar()
        {
            ChangeDir(pathArea.text);
        }

        public void CreateErrorModel(string text)
        {
            errorModel.SetActive(true);
            errorModelText.text = text;
        }

        public void upOneDir() {
            ChangeDir(directoryabove);
        }
        
        public IEnumerator ChangeDirCo(string dir)
        {
            if (!Directory.Exists(dir))
            {
                string error = "SmileOS can't find \"" + dir + "\". Check the spelling and try again.";
                CreateErrorModel(error);
                yield break;
            }

            loadblocker.SetActive(true);
            yield return null;
            scrollbar.interactable = false;
            pathArea.interactable = false;
            pathArea.text = dir;
            currentdir = dir;
            foreach (Transform child in content.transform)
            {
                Destroy(child.gameObject);
            }

            DirectoryInfo dirinfo = new DirectoryInfo(dir);
            if (dirinfo.Parent != null)
            {
                upbutton.interactable = true;
                directoryabove = dirinfo.Parent.FullName;
                /*                 GameObject upButton = Instantiate(fileButton, content.transform);
                                SmileFileButton up = upButton.GetComponent<SmileFileButton>();
                                upButton.SetActive(false);
                                up.FileName = "..";
                                up.FullPath = dirinfo.Parent.FullName;
                                up.isfolder = true;
                                up.icontype = SmileFileButton.IconType.Folder;

                                up.button.GetComponent<Button>().onClick.AddListener(() =>
                                {
                                    ChangeDir(up.FullPath);
                                });
                                upButton.SetActive(true); */
            }
            else
            {
                upbutton.interactable = false;
                directoryabove = "";
            }
            foreach (DirectoryInfo subDir in dirinfo.GetDirectories())
            {
                if ((subDir.Attributes & (FileAttributes.Hidden | FileAttributes.System)) != 0)
                {
                    continue;
                }
                GameObject folderButton = Instantiate(fileButton, content.transform);
                SmileFileButton btn = folderButton.GetComponent<SmileFileButton>();
                folderButton.SetActive(false);
                btn.FileName = subDir.Name;
                btn.FullPath = subDir.FullName;
                btn.isfolder = true;
                btn.icontype = SmileFileButton.IconType.Folder;

                btn.button.GetComponent<Button>().onClick.AddListener(() =>
                {
                    ChangeDir(btn.FullPath);
                });
                folderButton.SetActive(true);
            }
            int overcount = 0;
            bool hardcap = false;
            foreach (FileInfo file in dirinfo.GetFiles())
            {
                if ((file.Attributes & (FileAttributes.Hidden | FileAttributes.System)) != 0)
                {
                    continue;
                }
                if (filter != null && filter.Count > 0)
                {
                    if (!filter.Contains(file.Extension.ToLower()))
                        continue;
                }
                if ((file.Extension == ".png" || file.Extension == ".jpg" || file.Extension == ".jpeg" || file.Extension == ".tga" || file.Extension == ".gif") && hardcap == false)
                {
                    overcount++;
                    if (overcount >  SettingsManager.Instance.GetSettingValue<int>("TN_MaxAmount") )
                    {
                        hardcap = true;
                    }
                }
                GameObject filebox = Instantiate(fileButton, content.transform);
                SmileFileButton btn = filebox.GetComponent<SmileFileButton>();
                filebox.SetActive(false);
                btn.FileName = file.Name;
                btn.FullPath = file.FullName;
                btn.hardcap = hardcap;
                btn.isfolder = false;
                btn.icontype = GetIconType(file.Extension); 
                Toggle toggle = btn.toggletype.GetComponent<Toggle>();
                toggle.group = fileToggleGroup;
                filebox.SetActive(true);
            }

            if (content.transform.childCount == 0)
            {
                nofilefoundblocker.SetActive(true);
            }
            else
            {
                nofilefoundblocker.SetActive(false);
            }
            loadblocker.SetActive(false);
            scrollbar.interactable = true;
            pathArea.interactable = true;
        }

        public IEnumerator FolderSelector(string dir, bool detection = false)
        {
            if (!Directory.Exists(dir))
            {
                string error = "SmileOS can't find \"" + dir + "\". Check the spelling and try again.";
                CreateErrorModel(error);
                yield break;
            }
            loadblocker.SetActive(true);
            yield return null;
            currentdir = dir;
            DirectoryInfo dirinfo = new DirectoryInfo(dir);

            foreach (DirectoryInfo subDir in dirinfo.GetDirectories())
            {
                bool createflag = true;
                if (detection == true)
                {
                    foreach (FileInfo subFile in subDir.GetFiles())
                    {
                        if (subFile.Extension == ".gcmd" || subFile.Extension == ".gcpack")
                        {
                            createflag = false;
                            break;
                        }
                    }
                }
                if (createflag == true)
                {
                    GameObject folderButton = Instantiate(fileButton, content.transform);
                    SmileFileButton btn = folderButton.GetComponent<SmileFileButton>();
                    folderButton.SetActive(false);
                    btn.FileName = subDir.Name;
                    btn.FullPath = subDir.FullName;
                    btn.isfolder = false;
                    btn.icontype = SmileFileButton.IconType.Folder;

                    folderButton.SetActive(true);
                }

            }
            loadblocker.SetActive(false);
        }



        public void onsubmit()
        {
            foreach (Toggle toggle in fileToggleGroup.ActiveToggles())
            {
                SmileFileButton btn = toggle.GetComponentInParent<SmileFileButton>();
                if (btn != null && toggle.isOn)
                {
                    sourcebutton.onvalueget(btn.FullPath);
                    Destroy(gameObject);
                }
            }

        }
        public void oncancel()
        {


            sourcebutton.onvalueget("");
            Destroy(gameObject);



        }


        public void gotopreset(presets preset)
        {
            switch (preset)
            {
                case presets.Desktop:
                    ChangeDir(desktop);
                    break;
                case presets.Downloads:
                    ChangeDir(downloads);
                    break;
                case presets.Documents:
                    ChangeDir(documents);
                    break;
                case presets.Pictures:
                    ChangeDir(pictures);
                    break;
                case presets.Music:
                    ChangeDir(music);
                    break;
                case presets.Videos:
                    ChangeDir(videos);
                    break;
                case presets.Ultraskins:
                    ChangeDir(UltraskinsPath);
                    break;
            }
        }
        public void GoToDesktop() => gotopreset(presets.Desktop);
        public void GoToDownloads() => gotopreset(presets.Downloads);
        public void GoToDocuments() => gotopreset(presets.Documents);
        public void GoToPictures() => gotopreset(presets.Pictures);
        public void GoToMusic() => gotopreset(presets.Music);
        public void GoToVideos() => gotopreset(presets.Videos);
        public void GoToUltraskins() => gotopreset(presets.Ultraskins);
        public enum presets
        {
            Desktop,
            Downloads,
            Documents,
            Pictures,
            Music,
            Videos,
            Ultraskins
        }

        private SmileFileButton.IconType GetIconType(string extension)
        {
            switch (extension.ToLower())
            {
                case ".png":
                case ".jpg":
                case ".jpeg":
                case ".gif":
                case ".tga":
                    return SmileFileButton.IconType.Image;
                case ".zip":
                    return SmileFileButton.IconType.Zip;
                case ".gcskin":
                    return SmileFileButton.IconType.GCskin;
                case ".mp3":
                case ".wav":
                case ".ogg":
                    return SmileFileButton.IconType.Music;
                case ".mp4":
                case ".mov":
                case ".avi":
                case ".webm":
                    return SmileFileButton.IconType.Video;
                case ".txt":
                case ".json":
                case ".xml":
                case ".csv":
                case ".cs":
                case ".log":
                case ".cfg":
                    return SmileFileButton.IconType.Text;
                case ".usgc":
                case ".gcpack":
                case ".gcmd":
                    return SmileFileButton.IconType.Internal;

                default:
                    return SmileFileButton.IconType.Unknown;
            }
        }
    }
}
