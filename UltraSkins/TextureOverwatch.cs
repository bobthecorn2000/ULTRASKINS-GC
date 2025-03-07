using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UltraSkins
{

	public class TextureOverWatch : MonoBehaviour
	{
		public Material[] cachedMaterials;
		public Renderer renderer;
		public bool forceswap;
		string swapType = "weapon";

		void OnEnable()
		{
            if (GetComponentInParent<Nail>() || GetComponent<Coin>())
			{
				swapType = "projectile";
			}

            if (GetComponentInParent<Grenade>())
            {
                swapType = GetComponentInParent<Grenade>().rocket ? "rocket": "grenade";
				if (swapType == "rocket" && GetComponent<ChangeMaterials>())
				{
					Material[] chargemats = GetComponent<ChangeMaterials>().materials;

					foreach (Material mat in chargemats)
					{
						if (ULTRASKINHand.autoSwapCache.ContainsKey(mat.mainTexture.name))
						{
							mat.mainTexture = ULTRASKINHand.autoSwapCache[mat.mainTexture.name];
						}
					}
                }
            }
            if (!renderer)
            {
                renderer = GetComponent<Renderer>();
            }
            if (renderer.materials != cachedMaterials)
			UpdateMaterials();
		}

		public void UpdateMaterials()
		{
            if (renderer && renderer.materials != cachedMaterials)
			{
				Material[] materials = renderer.materials;
				for (int i = 0; i < materials.Length; i++)
				{
					ULTRASKINHand.PerformTheSwap(materials[i], forceswap, transform.GetComponent<TextureOverWatch>(), swapType);
				}
				cachedMaterials = renderer.materials;
            }
			transform.GetComponent<TextureOverWatch>().enabled = false;
		}
    }
}
