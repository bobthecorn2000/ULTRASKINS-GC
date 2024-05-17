
using UnityEngine;
using System.IO;
using GameConsole;

namespace UltraSkins
{
	public class SkinEventHandler : MonoBehaviour
	{
		public GameObject Activator;
		public string path;
		public string pname;
		public ULTRASKINHand UKSH;

		private void Update()
		{
			if (Activator != null && Activator.activeSelf)
			{
				Activator.SetActive(false);
				string message = UKSH.ReloadTextures(false, path);
				string folder = GetModFolderPath();
				TextureOverWatch[] TOWS = GameObject.FindWithTag("MainCamera").GetComponentsInChildren<TextureOverWatch>(true);
				foreach (TextureOverWatch TOW in TOWS)
				{
					if (TOW && TOW.gameObject)
					{
						TOW.enabled = true;
                    }
				}
                MonoSingleton<HudMessageReceiver>.Instance.SendHudMessage(message, "", "", 0, false);
			}
		}
        public string GetModFolderPath()
        {
            // Get the path to the current directory where the game executable is located
            string gameDirectory = Path.GetDirectoryName(Application.dataPath);

            // The mod folder is typically named "BepInEx/plugins" or similar
            string modFolderName = "BepInEx/plugins"; // Adjust this according to your setup

			// Combine the game directory with the mod folder name to get the full path
			return "C:\\Program Files (x86)\\Steam\\steamapps\\common\\ULTRAKILL\\BepInEx\\plugins\\ULTRASKINS\\Custom";
           // return Path.Combine(gameDirectory, modFolderName);
        }
    }
}
