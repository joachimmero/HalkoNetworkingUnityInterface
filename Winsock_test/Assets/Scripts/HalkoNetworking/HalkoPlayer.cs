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
                return t.position;
            }
            set
            {
                if (isLocalPlayer)
                {
                    t.position = value;
                    //HalkoNetwork.Send(t.position);
                }
            }
        }
        //This changes the transforms
        public Vector3 EulerAngles
        {
            get
            {
                return t.eulerAngles;
            }
            set
            {
                
                if(isLocalPlayer)
                {
                    t.eulerAngles = value;
                    //HalkoNetwork.Send(t.eulerAngles);
                }
            }
        }

        //Private properties:
        public bool positionChanged = false;
        private Vector3 lastPos = Vector3.zero;
        private Transform t;
        private HalkoNetwork halkoNetwork;
        [SerializeField] Vector3 nextPos = Vector3.zero;


        // Start is called before the first frame update
        void Start()
        {
            halkoNetwork = FindObjectOfType<HalkoNetwork>();
            t = GetComponent<Transform>();
        }

        // Update is called once per frame
        void Update()
        { 
            if (!isLocalPlayer && nextPos != Vector3.zero)
            {
                _Move();
            }
        }

        //Public methods:

        public void Translate(Vector3 translation)
        {
            if(translation != Vector3.zero && isLocalPlayer)
            {
                t.Translate(translation);
                halkoNetwork.Send(clientId, t.position);
            }
        }

        public void SetNextPosition(Vector3 next)
        {
            nextPos = next;
        }

        //Private methods:

        private void _Move()
        {
            transform.position = nextPos;
            nextPos = Vector3.zero;
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
