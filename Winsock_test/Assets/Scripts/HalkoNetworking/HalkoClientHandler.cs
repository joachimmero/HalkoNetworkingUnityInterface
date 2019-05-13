using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HalkoNetworking
{
    public class HalkoClientHandler : MonoBehaviour
    {
        //Private fields:

        private HalkoNetwork _halkoNetwork;

        private GameObject _player;

        //Clients that have joined the room, but haven't yet been instantiated for other clients in the room.
        List<HalkoPlayer> _instantiationList;

        //Clients that have left the room, but haven't yet been destroyed from the other clients.
        List<uint> _deletionList;

        //Public properties:

        public HalkoPlayer LastJoinedPlayer
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
            _instantiationList = new List<HalkoPlayer>();
            _deletionList = new List<uint>();

            _halkoNetwork = FindObjectOfType<HalkoNetwork>();
            _player = _halkoNetwork.player;
        }

        // Update is called once per frame
        void Update()
        {
            if(_instantiationList.Count > 0)
            {
                foreach(HalkoPlayer h in _instantiationList)
                {
                    InstantiatePlayer(h.id, h.clientName, h.IsLocalPlayer);
                }
                _instantiationList.Clear();
            }
            if (_deletionList.Count > 0)
            {
                foreach (uint id in _deletionList)
                {
                    foreach (HalkoPlayer h in FindObjectsOfType<HalkoPlayer>())
                    {
                        if (id == h.id)
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
            h.id = id;
            h.clientName = name;
            g.name = name;
            if (IsLocal)
            {
                h.IsLocalPlayer = true;
                g.AddComponent<Movement>().h = h;
            }
            _halkoNetwork.connectedPlayers.Add(h);
        }
    }

}
