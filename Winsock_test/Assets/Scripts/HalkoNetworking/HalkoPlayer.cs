using System;
using System.Collections;
using System.Net.Sockets;
using UnityEngine;

namespace HalkoNetworking
{
    public class HalkoPlayer : MonoBehaviour
    {

        //Public properties:

        [Header("Client Info")]
        public uint clientId;
        public string clientName;
        public bool isLocalPlayer; //True if this is the local player's player object.

        //This changes the transforms
        public Vector3 position
        {
            get
            {
                return transform.position;
            }
            set
            {
                if (isLocalPlayer && value != transform.position)
                {
                    transform.position = value;
                    transformChanged = true;
                }
            }
        }
        //This changes the transforms
        public Vector3 eulerAngles
        {
            get
            {
                return transform.eulerAngles;
            }
            set
            {
                
                if(isLocalPlayer && value != transform.eulerAngles)
                {
                    transform.eulerAngles = value;
                    transformChanged = true;
                }
            }
        }

        //Private properties:
        public bool transformChanged = false; //bool for checking if the transform has changed.
        private bool moving = false;
        private Vector3 lastPos = Vector3.zero;
        private Vector3 nextPosition = Vector3.zero;
        private Vector3 nextRotation = Vector3.zero;
        private TcpClient client;
        private NetworkStream stream;
        private HalkoNetwork halkoNetwork;


        // Start is called before the first frame update
        void Start()
        {
            halkoNetwork = FindObjectOfType<HalkoNetwork>();
            client = halkoNetwork.Client;
            stream = client.GetStream();
        }

        // Update is called once per frame
        void Update()
        { 
            //Move the remote client if the local client has moved.
            if (transformChanged && !isLocalPlayer)
            {
                SetTransform();
            }
        }

        private void LateUpdate()
        {
            if(transformChanged && isLocalPlayer)
            {
                if (client != null && client.Connected)
                {
                    SendTransform();
                    transformChanged = false;
                }
            }
        }

        //Public methods:

        public void Translate(Vector3 translation)
        {
            if(translation != Vector3.zero && isLocalPlayer)
            {
                transform.Translate(translation);
                transformChanged = true;
            }
        }

        public void Rotate(Vector3 rotation)
        {
            if(rotation != Vector3.zero && isLocalPlayer)
            {
                transform.Rotate(rotation);
                transformChanged = true;
            }
        }

        public void SetTransformProperties(Vector3 nextPos, Vector3 nextRot)
        {
            transformChanged = true;
            nextPosition = nextPos;
            nextRotation = nextRot;
        }

        //Private methods:

        private void SendTransform()
        {
            print("Sending transform");
            Package p = new Package();
            p.pos_x = transform.position.x;
            p.pos_y = transform.position.y;
            p.pos_z = transform.position.z;
            p.rot_x = transform.eulerAngles.x;
            p.rot_y = transform.eulerAngles.y;
            p.rot_z = transform.eulerAngles.z;
            Formatter f = new Formatter();
            byte[] id = BitConverter.GetBytes(clientId);
            byte[] data = f.SerializePackage(id, (byte)'t', p);
            stream.Write(data, 0, data.Length);
        }

        private void SetTransform()
        {
            transform.position = nextPosition;
            transform.eulerAngles = nextRotation;
            transformChanged = false;
        }
    }

    public struct ClientInfo
    {
        public uint clientId;
        public string clientName;
        public bool isLocalClient; //True if this is the local player's player object.

        public ClientInfo(uint id, string name, bool isLocal)
        {
            clientId = id;
            clientName = name;
            isLocalClient = isLocal;
        }
    }
}
