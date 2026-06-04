using BatonPassLogger;
using System;
using System.Collections.Generic;
using System.Text;
using UltraSkins.Utils;
using UnityEngine;

namespace UltraSkins
{

    public class Resolver
    {
        public static Resolver Instance { get; private set; }

        private Dictionary<string, FracParamMap> MaterialParamMaps = new Dictionary<string, FracParamMap>();
        public static ServiceStartPackage StartService(Resolver SelfObject)
        {
            if (Instance != null)
            {
                BatonPass.Warn("Resolver has already started and cannot be started again!"); 
                return new ServiceStartPackage(false, "Resolver has already started and cannot be started again");
            }

            BatonPass.Info("Resolver Service has started");
            Instance = SelfObject;
            return new ServiceStartPackage(true, "Resolver was started Correctly");
        }

        public static void CacheMaterialState(Material mat,bool force = false)
        {
            BatonPass.Debug("Now Scanning " + mat.name);
            if (Instance.MaterialParamMaps.ContainsKey(mat.name) && force == false)
            {
                BatonPass.Warn("the material " + mat.name + " already exists. skipping....");
                return;
            }
            string[] texProp = mat.GetTexturePropertyNames();
            FracParamMap paramMap = new FracParamMap(mat.name);
            foreach (string prop in texProp)
            {
                Texture tex = mat.GetTexture(prop);
                if (tex != null)
                {
                    string name = tex.name;
                    BatonPass.Debug("found property: " + prop + " with name: " + name);
                    paramMap.Register(prop, name);
                }
            }
            Instance.MaterialParamMaps[mat.name] = paramMap;
        }

        public static string RecallSingle(string matName,string param)
        {
            
            if (Instance.MaterialParamMaps.TryGetValue(matName, out FracParamMap fpm))
            {
                if (fpm.TryResolve(param, out string texName))
                {
                    return texName;
                }
                else
                {
                    BatonPass.Warn("Found a material named: " + matName + "But failed to recall a param named: " + param);
                    return null;
                }
            }
            else
            {
                BatonPass.Warn("Failed to recall a material named: " + matName);
                return null;
            }
        }
        public static string[] Recall(string matName)
        {
            if (Instance.MaterialParamMaps.TryGetValue(matName, out FracParamMap fpm))
            {
                return fpm.ParamsKeysToArray();
            }
            else
            {
                BatonPass.Warn("Failed to recall a material named: " + matName);
                return null;
            }
        }

        public static bool CheckExist(string matName)
        {
            return Instance.MaterialParamMaps.ContainsKey(matName);
        }

        public static bool CheckDirty(string matName)
        {
            if (Instance.MaterialParamMaps.TryGetValue(matName, out FracParamMap fpm))
            {
                return fpm.dirty;
            }
            else
            {
                BatonPass.Warn("a value by the name " + matName + " does not exist");
                return false;

            }
        }

        public static void SetAllDirty(bool val)
        {
            foreach (FracParamMap FPM in Instance.MaterialParamMaps.Values)
            {
                FPM.dirty = val;
            }
        }


        public static void SetDirty(string matName, bool val)
        {
            if (Instance.MaterialParamMaps.TryGetValue(matName, out FracParamMap fpm))
            {
                fpm.dirty = val;
            }
            else
            {
                BatonPass.Warn("a value by the name " + matName + " does not exist");
                
            }
        }

    }

    internal class FracParamMap
    {
        public string matName { get; private set; }
        private Dictionary<string, string> Params = new Dictionary<string, string>();
        public bool dirty = true; 
        public FracParamMap(string matname)
        {
            matName = matname;
        }

        public void Register(string param, string texName)
        {
            Params[param] = texName;
        }

        public bool TryResolve(string param,out string texName)
        {
            return Params.TryGetValue(param, out texName);
        }

        public string[] ParamsKeysToArray()
        {
            List<string> list = new List<string>();
            foreach (KeyValuePair<string,string> kvp in Params)
            {
                list.Add(kvp.Key);
            }
            return list.ToArray();
        }

        public void Wipe()
        {
            Params.Clear();
            BatonPass.Debug(matName + " FracParamMap was wiped!!");
        }
    }
}
