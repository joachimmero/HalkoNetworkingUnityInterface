using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HalkoNetworking
{
    public class HalkoPlayer : MonoBehaviour
    {

        //Public properties:

        [Header("Client Info")]
        public uint id;
        public string clientName;
        public bool IsLocalPlayer; //True if this is the local player's player object.

        //This changes the transforms
        public Vector3 Position
        {
            get
            {
                return t.position;
            }
            set
            {
                t.position = value;
                //HalkoNetwork.Send(t.position);
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
                t.eulerAngles = value;
                //HalkoNetwork.Send(t.eulerAngles);
            }
        }

        //Private properties:
        public bool positionChanged = false;
        private Vector3 lastPos = Vector3.zero;
        private Transform t;
        private HalkoNetwork halkoNetwork;
        [SerializeField] static Vector3 nextPos = Vector3.zero;


        // Start is called before the first frame update
        void Start()
        {
            t = GetComponent<Transform>();
        }

        // Update is called once per frame
        void Update()
        {
            if (!IsLocalPlayer && nextPos != Vector3.zero)
            {
                _Move();
            }
            else
            {
                positionChanged = false;
                //Check if player has moved during frame change
                if (lastPos != transform.position)
                {
                    positionChanged = true;
                    lastPos = transform.position;
                }
            }
        }

        //Public methods:

        public void Translate(Vector3 translation)
        {
            t.Translate(translation);
            //HalkoNetwork.Send(t.position);
        }

        public void SetNextPosition(Vector3 next)
        {
            nextPos = next;
            print(nextPos);
        }

        private void _Move()
        {
            transform.position = nextPos;
            nextPos = Vector3.zero;
        }
    }

}
