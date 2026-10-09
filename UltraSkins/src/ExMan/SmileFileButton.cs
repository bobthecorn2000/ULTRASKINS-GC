using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UltraSkins.Configuration;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace UltraSkins.ExMan
{
    public class SmileFileButton : MonoBehaviour
    {
        // Start is called before the first frame update

        public string FileName;
        public string FullPath;

        public enum IconType
        {
            Unknown,
            Folder,
            Image,
            Zip,
            GCskin,
            Music,
            Video,
            Text,
            Internal,
        };
        public bool isfolder;
        public TextMeshProUGUI displayname;
        public IconType icontype;
        public GameObject button;
        public GameObject toggletype;
        public GameObject FolderIcon;
        public GameObject ImageIcon;
        public GameObject ZipIcon;
        public Image ThumbnailIcon;
        public GameObject GCskinIcon;
        public GameObject UnknownIcon;
        public GameObject InternalIcon;
        public GameObject VideoIcon;
        public GameObject TextIcon;
        public GameObject MusicIcon;

        public bool hardcap = false;
        static int height = 64;
        static int width = 64;
        Sprite ThumbnailSprite;
        Texture ThumbnailTexture;

        void OnEnable()
        {
            displayname.text = FileName;
            iconselector(icontype);
            if (isfolder)
            {
                button.SetActive(true);
                toggletype.SetActive(false);
            }
            else
            {
                button.SetActive(false);
                toggletype.SetActive(true);
            }
           
        }
        public async Task<Sprite> LoadTextureFromFile(string path)
        {
        // Load raw image bytes
        byte[] data = await File.ReadAllBytesAsync(path);
        if (data == null || data.Length == 0)
            return null;

        // Load texture from bytes
        Texture2D original = new Texture2D(2, 2);
        if (!original.LoadImage(data))
        {
            Destroy(original);
            return null;
        }

        // Resize texture using RenderTexture
        RenderTexture rt = RenderTexture.GetTemporary(width, height);
        Graphics.Blit(original, rt);

        RenderTexture.active = rt;
        Texture2D resized = new Texture2D(width, height, original.format, false);
        resized.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        resized.Apply();

        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);

        // Cleanup original texture
        Destroy(original);

        // Create sprite from resized texture
        Sprite sprite = Sprite.Create(resized, new Rect(0, 0, resized.width, resized.height), new Vector2(0.5f, 0.5f));
        if (this == null)
        {
    // Clean up resources since nobody will use them
            if (sprite != null)
            Destroy(sprite.texture);
            if (sprite != null)
            Destroy(sprite);
        return null;
        }
        // Return sprite (note: keep reference to resized texture if you want to clean up later)
        return sprite;
            
        }

        private void OnDestroy()
        {
            if (ThumbnailSprite != null)
            {
                Destroy(ThumbnailSprite);
                ThumbnailSprite = null;
            }
            if (ThumbnailTexture != null)
            {
                Destroy(ThumbnailTexture);
                ThumbnailTexture = null;
            }
            if (ThumbnailIcon != null && ThumbnailIcon.sprite != null)
            {
                Destroy(ThumbnailIcon.sprite.texture);
                Destroy(ThumbnailIcon.sprite);
                ThumbnailIcon.sprite = null;
            }
}

        
        private async Task iconselector(IconType icontype)
        {
            FolderIcon.SetActive(false);
            ImageIcon.SetActive(false);
            ZipIcon.SetActive(false);
            GCskinIcon.SetActive(false);
            UnknownIcon.SetActive(false);
            VideoIcon.SetActive(false);
            TextIcon.SetActive(false);
            InternalIcon.SetActive(false);
            MusicIcon.SetActive(false);
            ThumbnailIcon.gameObject.SetActive(false);

            switch (icontype)
            {
                case IconType.Unknown:
                    UnknownIcon.SetActive(true);
                    break;
                case IconType.Folder:
                    FolderIcon.SetActive(true);
                    break;
                case IconType.Image:
                    if (SettingsManager.Instance.GetSettingValue<bool>("TN_In_FileViewer") &&  hardcap == false)
                    {
                        ImageIcon.SetActive(true);
                        Sprite thumbsprite = await LoadTextureFromFile(FullPath);
                        if (thumbsprite != null )
                        {
                            ImageIcon.SetActive(false);
                            ThumbnailIcon.gameObject.SetActive(true);
                            ThumbnailSprite = thumbsprite;
                            ThumbnailTexture = thumbsprite.texture;
                            ThumbnailIcon.sprite = thumbsprite;
                        }


                    }
                    else
                    {
                        ImageIcon.SetActive(true);
                    }

                    break;
                case IconType.Zip:
                    ZipIcon.SetActive(true);
                    break;
                case IconType.GCskin:
                    GCskinIcon.SetActive(true);
                    break;
                case IconType.Internal:
                    InternalIcon.SetActive(true);
                    break;
                case IconType.Video:
                    VideoIcon.SetActive(true);
                    break;
                case IconType.Text:
                    TextIcon.SetActive(true);
                    break;
                case IconType.Music:
                    MusicIcon.SetActive(true);
                    break;
            }
        }
        // Update is called once per frame

    }
}
