using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HalkoNetworking.RemoteMethod
{
    public class HalkoMethodHandler : MonoBehaviour
    {
        //Public fields:
        public List<KeyValuePair<int, object[]>> methodsWaitingForInvoke;
        public HalkoClass parentClass;

        private void Start()
        {
            methodsWaitingForInvoke = new List<KeyValuePair<int, object[]>>();
        }
        // Update is called once per frame
        void Update()
        {
            if (methodsWaitingForInvoke.Count > 0)
            {
                for (int i = 0; i < methodsWaitingForInvoke.Count; i++)
                {
                    KeyValuePair<int, object[]> method = methodsWaitingForInvoke[i];
                    parentClass.InvokeRemoteMethod(method.Key, method.Value);
                }
                methodsWaitingForInvoke.Clear();
            }


        }
    }
}
