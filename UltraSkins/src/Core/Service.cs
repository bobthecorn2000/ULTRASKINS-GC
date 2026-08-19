using BatonPassLogger;
using System;
using System.Collections.Generic;
using System.Text;
using UltraSkins.Utils;
using UnityEngine;

namespace UltraSkins
{
    // this design is heavily inspired by the MonoSingleton class used in the base game. it has been rewritten and altered for our purposes here.
    //It is currently unused as to focus on other parts of the project but will replace the redundent code in a future version
    public abstract class ServiceSingleton : MonoBehaviour {
        private protected static Dictionary<Type, ServiceSingleton> serviceRegistry = new Dictionary<Type, ServiceSingleton>();
        public static ServiceSingleton GetService(Type type)
        {
            ServiceSingleton gotService;
            ServiceSingleton.serviceRegistry.TryGetValue(type, out gotService);
            return gotService;
        }
        
    }
    public abstract class ServiceSingleton<T> : ServiceSingleton where T : ServiceSingleton<T>
    {
        public static T? Instance;
        public static readonly string ServiceName = typeof(T).Name;

        public ServiceStartPackage StartService()
        {
            if (Instance != null)
            {
                BatonPass.Warn($"{ServiceName} has already started and cannot be started again!");
                return new ServiceStartPackage(false, $"{ServiceName} has already started and cannot be started again");
            }

            BatonPass.Info($"{ServiceName} Service has started");
            Instance = (T)this;
            return new ServiceStartPackage(true, $"{ServiceName} was started Correctly");
        }


    }
}
