using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using static UltraSkins.ULTRASKINHand;


namespace UltraSkins.Legacy
{
    //tempcomment5


    // Use fractal its faster and better and really only use this if you need auto detection on an object and are to lazy to just make a class that inherits from Fractal" +
    //even though its way easier then figuring out what this does
    [Obsolete("Use Fractal.")]
	public class TextureOverWatch : MonoBehaviour
    {


        [Obsolete]
        public Material[] cachedMaterials;
        [Obsolete]
        public Renderer renderer;
        [Obsolete]
        public bool forceswap;
        [Obsolete]
        string swapType = "weapon";
        [Obsolete]
        public string iChange;


        [Obsolete("Use Fractal. also why are you calling onEnable manually")]
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
                    coin.uselessMaterial.mainTexture = HoldEm.Call("coin01_3");
                }
                
            }
            if (hammerInstance != null) {
                //ULTRASKINHand.ReadOut.SwapTheDial(this);
               // ReadOut.updateMeter(hammerInstance, true);
            }
            if (GetComponentInParent<Grenade>())
            {
                swapType = GetComponentInParent<Grenade>().rocket ? "rocket": "grenade";
				if (swapType == "rocket" && GetComponent<ChangeMaterials>())
				{
					Material[] chargemats = GetComponent<ChangeMaterials>().materials;
                    Material newrocketmat = new Material(chargemats[0]);
					chargemats[0] = newrocketmat;
                    if (HoldEm.Check("skull2rocketcharge"))
						{
						chargemats[0].mainTexture = HoldEm.Call("skull2rocketcharge");
						}
                    if (HoldEm.Check("skull2rocketbonuscharge"))
                    {
                        chargemats[1].mainTexture = HoldEm.Call("skull2rocketbonuscharge");
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
                    if (!HoldEm.Instance.MaterialNames.ContainsKey(swapname))
                    {
                        string textureName = (mat.HasProperty("_MainTex") && mat.mainTexture != null) ? mat.mainTexture.name : null;
                        HoldEm.Instance.MaterialNames.Add(swapname, textureName);
                    }
                }
            }
            if (renderer.materials != cachedMaterials)
            {
                UpdateMaterials();
            }
			
		}



        [Obsolete("Dont use this. it duplicated materials every time it runs Use Fractal")]
        public void UpdateMaterials()
		{
            if (renderer && renderer.materials != cachedMaterials)
			{
				Material[] materials = renderer.materials;
				for (int i = 0; i < materials.Length; i++)
				{
					//ULTRASKINHand.PerformTheSwap(materials[i], forceswap, transform.GetComponent<TextureOverWatch>(), swapType);
				}
				cachedMaterials = renderer.materials;
            }
			transform.GetComponent<TextureOverWatch>().enabled = false;
		}


    }
    [Obsolete("I dont even remember what this does but im scared that removing this will break something")]
    public class TowStorage : MonoBehaviour
    {
        [Obsolete("I dont even remember what this does but im scared that removing this will break something")]
        [SerializeField]public List<TextureOverWatch> TOWS;
    }
}
