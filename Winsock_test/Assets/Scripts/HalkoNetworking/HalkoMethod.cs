using System;
using System.Collections.Generic;
using System.Reflection;
using HalkoNetworking;

namespace HalkoNetworking.RemoteMethod
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class HalkoMethod : Attribute
    {
        //Private fields:
        private List<KeyValuePair<string, MethodInfo>> halkoMethods;
        private HalkoAttributeHandler ah;
        //Private methods:
        private void GetHalkoMethods()
        {
            halkoMethods = new List<KeyValuePair<string, MethodInfo>>();
            ah = new HalkoAttributeHandler();
            List<MethodInfo> tempMethods = ah.GetMethodsWithAttribute(this.GetType().GetTypeInfo(), typeof(RemoteMethod.HalkoMethod));

            for (int i = tempMethods.Count - 1; i >= 0; --i)
            {
                halkoMethods.Add(new KeyValuePair<string, MethodInfo>(tempMethods[i].Name, tempMethods[i]));
            }
        }
    }
}
