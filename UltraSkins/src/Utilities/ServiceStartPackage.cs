using System;
using System.Collections.Generic;
using System.Text;

namespace UltraSkins.Utils
{
    public class ServiceStartPackage
    {
        bool Successful { get;}
        string Message { get;}

        public ServiceStartPackage(bool success,string message)
        {
            Successful = success;
            Message = message;
        }
    }
}
