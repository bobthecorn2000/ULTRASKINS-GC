using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
namespace UltraSkins
{
    public class OGSkinsManager : MonoBehaviour
    {
        // Start is called before the first frame update


        [SerializeField]
        public List<OGTexturePair> RawSkins; 

        public List<Texture> RawTex;
        public List<string> RawNames;
        public Dictionary<string, Texture> OGSKINS = new Dictionary<string, Texture>();
        void Awake()
        {
            Debug.Log($"OGSkinsManager attached to GameObject: {gameObject.name}");
    Debug.Log($"RawSkins is {(RawSkins == null ? "NULL" : "NOT NULL")}, count: {(RawSkins?.Count ?? -1)}");
            BuildDictionary();
        }

        // Update is called once per frame
    public void BuildDictionary()
    {
        //OGSKINS.Clear();

        for (int i = 0; i < Mathf.Min(RawNames.Count, RawTex.Count); i++)
        {
            if (!string.IsNullOrEmpty(RawNames[i]) && RawTex[i] != null)
            {
                if (!OGSKINS.ContainsKey(RawNames[i]))
                    OGSKINS.Add(RawNames[i], RawTex[i]);
                else
                    Debug.LogWarning($"Duplicate key found in OGSkinsManager: {RawNames[i]}");
            }
        }
    }
    }
    [System.Serializable]
    public class OGTexturePair
    {
        public string key;
        public Texture texture;
    }
}

 