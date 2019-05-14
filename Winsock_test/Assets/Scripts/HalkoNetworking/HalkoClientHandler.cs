using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

namespace HalkoNetworking
{
    public class HalkoClientHandler : MonoBehaviour
    {
        //Private fields:

        private HalkoNetwork _halkoNetwork;

        private GameObject _player;

        //Clients that have joined the room, but haven't yet been instantiated for other clients in the room.
        private List<ClientInfo> _instantiationList;

        //Clients that have left the room, but haven't yet been destroyed from the other clients.
        private List<uint> _deletionList;

        //Public properties:

        public ClientInfo LastJoinedPlayer
        {
            set
            {
                _instantiationList.Add(value);
            }
        }

        public uint LastLeftPlayer
        {
            set
            {
                _deletionList.Add(value);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            _instantiationList = new List<ClientInfo>();
            _deletionList = new List<uint>();

            _halkoNetwork = FindObjectOfType<HalkoNetwork>();
            _player = _halkoNetwork.player;
        }

        // Update is called once per frame
        void Update()
        {
            if(_instantiationList.Count > 0)
            {
                for(int i = 0; i < _instantiationList.Count; i++)
                {
                    ClientInfo c = _instantiationList[i];
                    InstantiatePlayer(c.clientId, c.clientName, c.isLocalClient);
                }
                _instantiationList.Clear();
            }
            if (_deletionList.Count > 0)
            {
                foreach (uint id in _deletionList)
                {
                    foreach (HalkoPlayer h in FindObjectsOfType<HalkoPlayer>())
                    {
                        if (id == h.clientId)
                        {
                            Destroy(h.gameObject);
                        }
                    }
                }
                _deletionList.Clear();
            }
        }

        //Public methods:

        //Instantiate a player object.
        //If the object is the local player id -> 0
        //Else id is the joined player's id.
        public void InstantiatePlayer(uint id, string name, bool IsLocal)
        {
            GameObject g = Instantiate(_player);
            HalkoPlayer h = g.AddComponent<HalkoPlayer>();
            h.clientId = id;
            h.clientName = name;
            g.name = name;
            if (IsLocal)
            {
                h.isLocalPlayer = true;
                g.AddComponent<Movement>().h = h;
            }

            _halkoNetwork.connectedPlayers.Add(h);
        }
    }

}
