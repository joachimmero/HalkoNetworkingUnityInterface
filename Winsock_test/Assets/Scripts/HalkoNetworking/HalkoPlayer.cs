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
        public Vector3 Position
        {
            get
            {
                return transform.position;
            }
            set
            {
                if (isLocalPlayer)
                {
                    transform.position = value;
                    //HalkoNetwork.Send(t.position);
                }
            }
        }
        //This changes the transforms
        public Vector3 EulerAngles
        {
            get
            {
                return transform.eulerAngles;
            }
            set
            {
                
                if(isLocalPlayer)
                {
                    transform.eulerAngles = value;
                    //HalkoNetwork.Send(t.eulerAngles);
                }
            }
        }

        //Private properties:
        public bool positionChanged = false;
        private bool moving = false;
        private Vector3 lastPos = Vector3.zero;
        [SerializeField] Vector3 nextPos = Vector3.zero;
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
            if (positionChanged && !isLocalPlayer && nextPos != transform.position)
            {
                Move();
            }
        }

        //Public methods:

        public void Translate(Vector3 translation)
        {
            if(translation != Vector3.zero && isLocalPlayer)
            {
                transform.Translate(translation);
                if(client != null && client.Connected)
                {
                    SendTransform();
                }
            }
        }

        public void SetNextPosition(Vector3 next)
        {
            positionChanged = true;
            nextPos = next;
        }

        //Private methods:

        private void SendTransform()
        {
            Package p = new Package();
            p.pos_x = transform.position.x;
            p.pos_y = transform.position.y;
            p.pos_z = transform.position.z;
            Formatter f = new Formatter();
            byte[] id = BitConverter.GetBytes(clientId);
            byte[] data = f.Serialize(id, (byte)'t', p);
            stream.Write(data, 0, data.Length);
        }

        private void Move()
        {
            transform.position = nextPos;
            positionChanged = false;
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
