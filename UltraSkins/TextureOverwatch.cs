using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using static UltraSkins.ULTRASKINHand;


namespace UltraSkins
{
    //tempcomment5
	public class TextureOverWatch : MonoBehaviour
	{
		public Material[] cachedMaterials;
		public Renderer renderer;
		public bool forceswap;
		string swapType = "weapon";
        public string iChange;
		void OnEnable()
		{
            ShotgunHammer hammerInstance = GetComponentInParent<ShotgunHammer>();
            Coin coin = GetComponentInParent<Coin>();

            if (GetComponentInParent<Nail>())
			{
				swapType = "projectile";
			}
            if (coin != null)
            {
                swapType = "projectile";
                if (HoldEm.Check("coin01_3")){
                    coin.uselessMaterial.mainTexture = ULTRASKINHand.HoldEm.Call("coin01_3");
                }
                
            }
            if (hammerInstance != null) {
                ULTRASKINHand.ReadOut.SwapTheDial(this);
                ReadOut.updateMeter(hammerInstance, true);
            }
            if (GetComponentInParent<Grenade>())
            {
                swapType = GetComponentInParent<Grenade>().rocket ? "rocket": "grenade";
				if (swapType == "rocket" && GetComponent<ChangeMaterials>())
				{
					Material[] chargemats = GetComponent<ChangeMaterials>().materials;
                    Material newrocketmat = new Material(chargemats[0]);
					chargemats[0] = newrocketmat;
                    if (ULTRASKINHand.HoldEm.Check("skull2rocketcharge"))
						{
						chargemats[0].mainTexture = ULTRASKINHand.HoldEm.Call("skull2rocketcharge");
						}
                    if (ULTRASKINHand.HoldEm.Check("skull2rocketbonuscharge"))
                    {
                        chargemats[1].mainTexture = ULTRASKINHand.HoldEm.Call("skull2rocketbonuscharge");
                    }

                }
            }

            if (!renderer)
            {
                renderer = GetComponent<Renderer>();
                string swapname;
                
                foreach (Material mat in renderer.materials)
                {
/*                    if (mat.name == "Pistol New (Instance)")
                    {
                        renderer.SetMaterial(PrismManager.PrismMan.toon);
                    }*/
                    iChange = (mat.HasProperty("_MainTex") && mat.mainTexture != null) ? mat.mainTexture.name : null;
                    swapname = "Swapped_" + swapType + "_" + mat.name;
                    if (!ULTRASKINHand.HandInstance.MaterialNames.ContainsKey(swapname))
                    {
                        string textureName = (mat.HasProperty("_MainTex") && mat.mainTexture != null) ? mat.mainTexture.name : null;
                        ULTRASKINHand.HandInstance.MaterialNames.Add(swapname, textureName);
                    }
                }
            }
            if (renderer.materials != cachedMaterials)
            {
                UpdateMaterials();
            }
			
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

public class TowStorage : MonoBehaviour
    {
        [SerializeField]public List<TextureOverWatch> TOWS;
    }
}
