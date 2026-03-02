using BepInEx;
using System;
using System.Collections.Generic;
using System.Text;
using BatonPassLogger;
using System.Reflection;

namespace UltraSkins
{
    public class CompatLayer
    {
        // partial reconstruction of pluginconfig
        public Assembly PluginConfigASM;
        private Type Main;
        private MethodInfo GetConfigByGUID;
        private Type PCtype;
        

        public bool PluginConfigCompatBoot()
        {
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.TryGetValue("com.eternalUnion.pluginConfigurator", out var pluginInfo))
            {
                BaseUnityPlugin PluginConfig = pluginInfo.Instance;
                
                PluginConfigASM = PluginConfig.GetType().Assembly;
                BatonPass.Info("Found PluginConfig");
                bool bindsuccess = Bind();
                if (PluginConfigASM != null)
                {
                    return bindsuccess;
                }
                
            }
            return false;
        }

        private bool Bind()
        {
            try
            {
                Main = PluginConfigASM.GetType("PluginConfig.PluginConfiguratorController");
                GetConfigByGUID = Main?.GetMethod("GetConfig", BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            }
            catch (Exception ex) { 
             BatonPass.Error(ex.Message);
                return false;
            }
            return true;
            
            
        }

        public string RipConfigPathByGUID(string guid)
        {
            string path = null;
            if (GetConfigByGUID != null)
            {
                object configinfo = GetConfigByGUID.Invoke(null, new object[] { guid });
                if ( configinfo != null)
                {
                    var prop = configinfo.GetType().GetProperty("currentConfigFilePath", BindingFlags.Public | BindingFlags.Instance);
                    
                    path = prop.GetValue(configinfo) as string;
                 
                }
            }

            return null;
        }




    }
}
