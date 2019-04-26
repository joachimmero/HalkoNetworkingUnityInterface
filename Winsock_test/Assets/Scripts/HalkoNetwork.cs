using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System;
using System.Text;
using UnityEngine.UI;

namespace HalkoNetworking
{
    public class HalkoNetwork : MonoBehaviour
    {
        //Private properties

        private NetworkStream stream;
        private TcpClient client;
        private bool connectedToRoom = false;
        private static HalkoPlayer localPlayer;
        private static bool newClientJoined = false;
        private static string newClientName;
        private static uint newClientId;
        private int mainThreadId;
        private List<HalkoPlayer> instantiationList;
        private List<uint> leftPlayers;

        [Header("Client Settings")]
        public string clientName = "Dummy";
        [SerializeField] GameObject player;
        
        [Header("Current room")]
        [SerializeField] string roomName;
        [SerializeField] int maxPlayers;
        [SerializeField] List<HalkoPlayer> connectedPlayers;

        // Start is called before the first frame update
        void Start()
        {
            mainThreadId = Thread.CurrentThread.ManagedThreadId;
            instantiationList = new List<HalkoPlayer>();
            leftPlayers = new List<uint>();
        }

        private void FixedUpdate()
        {
            if (client == null || localPlayer == null || !localPlayer.positionChanged)
            {
                return;
            }
        }

        private void Update()
        {
            if (Thread.CurrentThread.ManagedThreadId == mainThreadId)
            {
                if (instantiationList.Count > 0)
                {
                    for (int i = 0; i < instantiationList.Count; i++)
                    {
                        _InstantiatePlayer(
                            instantiationList[i].id,
                            instantiationList[i].clientName,
                            instantiationList[i].IsLocalPlayer
                            );
                        instantiationList.RemoveAt(i);
                    }

                }
                if(leftPlayers.Count > 0)
                {
                    //Go through all the id's of players that left the room.
                    for(int i = 0; i < leftPlayers.Count; i++)
                    {
                        //Go through all the HalkoPlayers that are connected to the room.
                        for(int j = 0; j < connectedPlayers.Count; j++)
                        {
                            //Check if the id of the HalkoPlayer in index j of connectedPlayers mathes the id of the player that left the room.
                            if(connectedPlayers[j].id == leftPlayers[i])
                            {
                                //Remove the HalkoPlayer who left, from the connectedPlayers list and destroy the gameObject of the player who left.
                                Destroy(connectedPlayers[j].gameObject);
                                connectedPlayers.RemoveAt(j);
                                break;
                            }
                        }
                    }
                    //Clear the leftPlayers list.
                    leftPlayers.Clear();
                }
            }
        }

        //Public methods

        public void Send(uint id, Vector3 position)
        {
            _Send(id, position);
        }

        public void ConnectToServer()
        {
            string hostName = Dns.GetHostName();
            string ip = Dns.GetHostAddresses(hostName)[1].ToString();

            if (ip.ToString() != "10.206.28.40")
            {
                ip = "10.206.28.40";
            }
            _Connect(ip, 27015);
        }
        public void CreateRoom()
        {
            _CreateRoom("testi", 2);
        }

        public void JoinRoom()
        {
            _JoinRoom("testi");
        }

        public List<HalkoPlayer> GetCurrentPlayers()
        {
            return connectedPlayers;
        }

        public void MoveNonLocalClient(uint clientId, Vector3 pos)
        {
            _MoveNonLocalClient(clientId, pos);
        }

        //Private methods
        
        private void _Connect(string ip, int port)
        {
            try
            {
                //Create a TCPClient
                client = new TcpClient(ip, port);

                //Get a clinet stream for reading and writing 
                stream = client.GetStream();

                //Buffer to receive the TcpServer.response
                byte[] data = new byte[512];

                //String to store the response ASCII representation.
                string responseData = "";

                //Read the first batch of the TcpServer response bytes.
                int bytes = stream.Read(data, 0, data.Length);
                responseData = Encoding.ASCII.GetString(data, 0, bytes);
            } 
            catch (SocketException e)
            {
                print(e.Message);
            }
        }

        private void _MoveNonLocalClient(uint clientId, Vector3 nextPos)
        {
            if (client.Connected)
            {
                foreach (HalkoPlayer p in connectedPlayers)
                {
                    print(p.id);
                    if (p.id == clientId)
                    {
                        print("Client: " + clientId + ", nextPos: " + nextPos);
                        p.SetNextPosition(nextPos);
                        break;
                    }
                }
            }
        }

        private void _Receive(char flag)
        {
            //Starts continuously receiveing data (Client has joined a room).
            if (flag == 'r')
            {
                Package p = new Package();
                new Thread(() =>
                {
                    while (client.Connected)
                    {
                        //Read the length of the received data from the stream
                        byte[] datalen = new byte[4];
                        stream.Read(datalen, 0, datalen.Length);
                        uint dataLength = BitConverter.ToUInt32(datalen, 0);

                        //Read dataLength-bytes from the stream.
                        byte[] data = new byte[dataLength];
                        stream.Read(data, 0, data.Length);
                        //Take the first byte of the data and assign it to be a flag
                        string streamflag = Encoding.ASCII.GetString(data, 0, 1);

                        //If the received stream holds a clients transform information.
                        if (streamflag == "t")
                        {
                            uint clientId = BitConverter.ToUInt32(data, 1);
                            Formatter f = new Formatter();
                            p = f.DeSerialize(data);
                            MoveNonLocalClient(clientId, new Vector3(p.pos_x, p.pos_y, p.pos_z));
                        }
                        //If the received stream holds information, that a new client has connected to the room.
                        else if (streamflag == "n")
                        {
                            //Handle the received data and instantiate the appropriate players.
                            HalkoPlayer c = new HalkoPlayer
                            {
                                id = BitConverter.ToUInt32(data, 1),
                                clientName = Encoding.ASCII.GetString(data, 5, data.Length - 5),
                                IsLocalPlayer = false
                            };
                            instantiationList.Add(c);
                        }
                        //If the received stream holds information, that a client has left the room.
                        else if (streamflag == "l")
                        {
                            uint leftPlayerId = BitConverter.ToUInt32(data, 1);
                            leftPlayers.Add(leftPlayerId);
                        }
                    }
                }).Start();
            }

            //If the client is receiving a callback 'c'
            else if (flag == 'c')
            {
                byte[] data = new byte[5];
                int bytes = stream.Read(data, 0, data.Length);
                string responseData = Encoding.ASCII.GetString(data, 0, bytes);

                if (responseData.Substring(0, 1) == "c")
                {
                    _OnCreatedRoom(BitConverter.ToUInt32(data, 1));
                }
                else if (responseData.Substring(0, 1) == "j")
                {
                    _OnJoinedRoom(BitConverter.ToUInt32(data, 1));
                }
            }
        }

        private void _Send(uint clientId, Vector3 pos)
        {
            if (client.Connected)
            {
                if (client != null)
                {
                    Package p = new Package();
                    p.pos_x = pos.x;
                    p.pos_y = pos.y;
                    p.pos_z = pos.z;
                    Formatter f = new Formatter();
                    byte[] id = BitConverter.GetBytes(clientId);
                    byte[] data = f.Serialize(id, (byte)'t', p);
                    stream.Write(data, 0, data.Length);
                }
            }
        }

        //Creates a room with name = roomName and size = maxPlayers
        private void _CreateRoom(string roomName, int maxPlayers)
        {
            //Length of this clients name-string.
            int len1 = clientName.Length;
            //"0" + maxPlayers in characters(1 character) + name.Length + roomName.Length
            int len2 = 2 + len1 + roomName.Length;

            //Length of the "Create Room" -stream to be sent to the server
            byte[] nameLen = BitConverter.GetBytes(len1);
            byte[] strmLen = BitConverter.GetBytes(len2);
            byte[] infoStrm = { strmLen[0], nameLen[0] };
            //Send the size of the next stream to the server.
            stream.Write(infoStrm, 0, 2);
            byte[] createRoomStrm = Encoding.ASCII.GetBytes("0" + clientName + roomName + maxPlayers.ToString());
            //Send the "Create Room" -stream to the server.
            stream.Write(createRoomStrm, 0, len2);

            _Receive('c');
        }

        private void _OnCreatedRoom(uint playerId)
        {
            print("Room created!");
            _InstantiatePlayer(
                playerId,
                clientName,
                true
                );
            connectedToRoom = true;
            _Receive('r');
        }

        private void _JoinRoom(string roomName)
        {
            //Length of this clients name-string.
            int len1 = clientName.Length;
            //"1" + roomName.Length
            int len2 = 1 + len1 + roomName.Length;
            //Length of the "Create Room" -stream to be sent to the server
            byte[] nameLen = BitConverter.GetBytes(len1);
            byte[] strmLen = BitConverter.GetBytes(len2);
            byte[] infoStrm = { strmLen[0], nameLen[0] };
            //Send the size of the next stream to the server.
            stream.Write(infoStrm, 0, 2);
            byte[] joinRoomStrm = Encoding.ASCII.GetBytes("1" + clientName + roomName);
            //Send the "Join Room" -stream to the server.
            stream.Write(joinRoomStrm, 0, len2);
            _Receive('c');
        }

        private void _OnJoinedRoom(uint playerId)
        {
            print("Room joined!");
            _InstantiatePlayer(
                playerId,
                clientName,
                true
                );

            connectedToRoom = true;

            //Receive data about how many players have to be instantiated to the room.
            byte[] data = new byte[4];
            int bytes = stream.Read(data, 0, 4);
            uint pCount = BitConverter.ToUInt32(data, 0);
            print("Players in room: " + pCount);
            for (uint i = 0; i < pCount; i++)
            {
                byte[] cNameInfo = new byte[4];
                stream.Read(cNameInfo, 0, 4);
                uint cNameSize = BitConverter.ToUInt32(cNameInfo, 0);
                byte[] pInfo = new byte[4 + (int)cNameSize];
                int pInfoSize = stream.Read(pInfo, 0, pInfo.Length);

                //Handle the received data and instantiate the appropriate players.
                
                _InstantiatePlayer(
                    BitConverter.ToUInt32(pInfo, 0),
                    Encoding.ASCII.GetString(pInfo, 4, pInfoSize - 4),
                    false
                    );
            }

            _Receive('r');
        }

        //Instantiate a player object.
        //If the object is the local player id -> 0
        //Else id is the joined player's id.
        private void _InstantiatePlayer(uint id, string name, bool IsLocal)
        {
            GameObject g = Instantiate(player);
            HalkoPlayer h = g.AddComponent<HalkoPlayer>();
            h.id = id;
            h.clientName = name;
            g.name = name;
            if (IsLocal)
            {
                h.IsLocalPlayer = true;
                GameObject.Find("localclient").GetComponent<Text>().text = "Local client: " + id;
                g.AddComponent<Movement>().h = h;
            }
            connectedPlayers.Add(h);
        }
    }

}
