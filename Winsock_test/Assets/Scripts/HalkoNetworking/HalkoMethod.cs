using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Net.Sockets;

namespace HalkoNetworking.RemoteMethod
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class HalkoMethod : Attribute
    {
    }
    public class HalkoClass : MonoBehaviour
    {
        //Public fields: 
        public HalkoMethodHandler halkoMethodHandler;

        //Private fields:
        private int classIndex;
        private List<KeyValuePair<string, MethodInfo>> halkoMethods;
        private NetworkStream stream;
        private HalkoNetwork halkoNetwork;
        private HalkoAttributeHandler ah;
        private Formatter formatter;
        
        private void Awake()
        {
            try
            {
                halkoNetwork = FindObjectOfType<HalkoNetwork>();
            }
            catch(Exception e)
            {
                Debug.Log("No HalkoNetwork-object found in the scene. " + e.Message);
            }

            halkoMethodHandler = this.gameObject.AddComponent<HalkoMethodHandler>();
            halkoMethodHandler.parentClass = this;
            classIndex = halkoNetwork.AddHalkoClass(this);
            stream = halkoNetwork.Client.GetStream();
            formatter = new Formatter();
            GetHalkoMethods();
        }
        //Public methods:

        /// <summary>
        /// Used to call a method locally and remotely.
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="parameters"></param>
        public void InvokeMethod(string methodName, object[] parameters)
        {
            bool methodFound = false;
            //If the methodName matches the key of a key-value -pair
            //in the dictionary.
            for (int i = halkoMethods.Count - 1; i >= 0; --i)
            {
                if (halkoMethods[i].Key == methodName)
                {
                    print(i);
                    methodFound = true;
                    //Call the method locally.
                    halkoMethods[i].Value.Invoke(this, parameters);

                    byte[] methodData = formatter.SerializeMethod((byte)'m', classIndex, i, parameters);
                    stream.Write(methodData, 0, methodData.Length);
                }
            }
            //If method wasn't found, throw an error.
            if (!methodFound)
            {
                throw new Exception("No method with with this name was found.");
            }
        }

        //Adds the index and parameters of the method, that needs to be invoked
        //to a list.
        public void InvokeRemoteMethod(int methodIndex, object[] parameters)
        {
            halkoMethods[methodIndex].Value.Invoke(this, parameters);
        }
        
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
