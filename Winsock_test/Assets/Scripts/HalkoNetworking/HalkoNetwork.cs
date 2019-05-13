using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HalkoNetworking
{
    public class HalkoNetwork : MonoBehaviour
    {
        //Private properties:
        public bool ConnectedToHalko
        {
            get
            {
                return connectedToHalko;
            }
        }

        //Public properties:
        public string RoomName
        {
            get
            {
                return roomName;
            }
        }

        //Public fields:


        //Private fields:

        private HalkoClientHandler clientHandler;
        private NetworkStream stream;
        private TcpClient client;
        private bool connectedToHalko = false;
        //private bool connectedToRoom = false;
        private static HalkoPlayer localPlayer;
        private static string newClientName;
        private static uint newClientId;
        private int mainThreadId;

        [Header("Network Settings")]
        [SerializeField] string nextScene;
        [Header("Client Settings")]
        public string clientName = "Dummy";
        public GameObject player;

        [SerializeField] uint clientId;

        [Header("Current room")]
        [SerializeField] string roomName;
        [SerializeField] int maxPlayers;
        public List<HalkoPlayer> connectedPlayers;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        //Public methods

        public void Send(uint id, Vector3 position)
        {
            _Send(id, position);
        }

        public void ConnectToHalko()
        {
            string hostName = Dns.GetHostName();
            string ip = Dns.GetHostAddresses(hostName)[1].ToString();

            if (ip.ToString() != "10.69.10.187")
            {
                ip = "10.69.10.187";
            }
            Connect(ip, 27015);
        }

        public void CreateRoom(string rName, int maxPlayers)
        {
            if(connectedToHalko)
            {
                roomName = rName;
                //Length of this clients name-string.
                int len1 = clientName.Length;
                //"0" + maxPlayers in characters(1 character) + name.Length + roomName.Length
                int len2 = 2 + len1 + roomName.Length;

                //Length of the "Create Room" -stream to be sent to the server
                byte[] nameLen = BitConverter.GetBytes((uint)len1);
                byte[] strmLen = BitConverter.GetBytes((uint)len2);
                byte[] infoStrm = { strmLen[0], nameLen[0] };
                //Send the size of the next stream to the server.
                stream.Write(infoStrm, 0, 2);
                byte[] createRoomStrm = Encoding.ASCII.GetBytes("0" + clientName + roomName + maxPlayers.ToString());
                //Send the "Create Room" -stream to the server.
                stream.Write(createRoomStrm, 0, len2);

                this.maxPlayers = maxPlayers;
                Receive('c');
            }
            else
            {
                _OnCreateRoomFailed("Can't create a room. This client is not connected to the server...");
            }
        }

        public void JoinRoom(string roomName)
        {
            if(connectedToHalko)
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
                Receive('c');
            }
            else
            {
                _OnJoinRoomFailed("Can't join a room. This client is not connected to the server...");
            }
        }

        public List<HalkoPlayer> GetCurrentPlayers()
        {
            return connectedPlayers;
        }

        //Private methods
        
        private void Connect(string ip, int port)
        {
            clientHandler = this.gameObject.AddComponent<HalkoClientHandler>();
            mainThreadId = Thread.CurrentThread.ManagedThreadId;
            //instantiationList = new List<HalkoPlayer>();
            //leftPlayers = new List<uint>();

            try
            {
                //Create a TCPClient
                client = new TcpClient(ip, port);

                //Get a clinet stream for reading and writing 
                stream = client.GetStream();

                Receive('c');
                /*
                //Buffer to receive the TcpServer.response
                byte[] data = new byte[512];

                //String to store the response ASCII representation.
                string responseData = "";

                //Read the first batch of the TcpServer response bytes.
                int bytes = stream.Read(data, 0, data.Length);
                responseData = Encoding.ASCII.GetString(data, 0, bytes);
                print(responseData);*/
            } 
            catch (SocketException e)
            {
                print(e.Message);
                _OnFailedToConnectToHalko();
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

        private void Receive(char flag)
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
                            _MoveNonLocalClient(clientId, new Vector3(p.pos_x, p.pos_y, p.pos_z));
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
                            clientHandler.LastJoinedPlayer = c;
                        }
                        //If the received stream holds information, that a client has left the room.
                        else if (streamflag == "l")
                        {
                            uint leftPlayerId = BitConverter.ToUInt32(data, 1);
                            clientHandler.LastLeftPlayer = leftPlayerId;
                        }
                    }
                }).Start();
            }

            //If the client is receiving a callback 'c'
            else if (flag == 'c')
            {
                //Read the length of the received data from the stream
                byte[] datalen = new byte[4];
                stream.Read(datalen, 0, datalen.Length);
                uint dataLength = BitConverter.ToUInt32(datalen, 0);
                print(dataLength);
                //Read dataLength-bytes from the stream.
                byte[] data = new byte[dataLength];
                stream.Read(data, 0, data.Length);
                //Take the first byte of the data and assign it to be a flag
                string streamflag = Encoding.ASCII.GetString(data, 0, 1);

                if (streamflag == "c")
                {
                    clientId = BitConverter.ToUInt32(data, 1);
                    StartCoroutine(LoadScene(_OnCreatedRoom));
                    //_OnCreatedRoom(BitConverter.ToUInt32(data, 1));
                }
                else if (streamflag == "j")
                {
                    clientId = BitConverter.ToUInt32(data, 1);
                    StartCoroutine(LoadScene(_OnJoinedRoom));
                    //_OnJoinedRoom(BitConverter.ToUInt32(data, 1));
                }
                //If the client has connected to the server.
                else if(streamflag == "s")
                {
                    _OnConnectedToHalko();
                }
                //If the room creation or the joining failed.
                else if(streamflag == "f")
                {
                    //The flag that indicates if the action that failed was room creation or joining.
                    string failedflag = Encoding.ASCII.GetString(data, 1, 1);

                    if (failedflag == "j")
                    {
                        _OnJoinRoomFailed(Encoding.ASCII.GetString(data, 2, (int)dataLength - 2));
                    }
                    else if (failedflag == "c")
                    {
                        _OnCreateRoomFailed(Encoding.ASCII.GetString(data, 2, (int)dataLength - 2));
                    }
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

        IEnumerator LoadScene(Func<uint, int> nextFunc)
        {
            AsyncOperation load = SceneManager.LoadSceneAsync(nextScene);
            while(!load.isDone)
            {
                yield return null;
            }
            nextFunc(clientId);
        }

        //Callbacks:

        
        private void _OnConnectedToHalko()
        {
            print("Connected to the server!");
            connectedToHalko = true;
            OnConnectedToHalko();
        }

        private void _OnFailedToConnectToHalko()
        {
            print("Failed to connect to the server...");
            connectedToHalko = false;
            OnFailedToConnectToHalko();
        }

        private int _OnCreatedRoom(uint playerId)
        {
            print("Scene index: " + SceneManager.GetActiveScene().buildIndex);
 //           clientId = playerId;
            //connectedToRoom = true;

            Receive('r');

            clientHandler.InstantiatePlayer(
            clientId,
            clientName,
            true
            );

            print("Room created!");
            OnCreatedRoom();
            return 0;
        }

        private int _OnJoinedRoom(uint playerId)
        {
           // clientId = playerId;
            //connectedToRoom = true;

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

                clientHandler.InstantiatePlayer(
                    BitConverter.ToUInt32(pInfo, 0),
                    Encoding.ASCII.GetString(pInfo, 4, pInfoSize - 4),
                    false
                    );
            }

            Receive('r');

            clientHandler.InstantiatePlayer(
                clientId,
                clientName,
                true
                );

            print("Room joined");
            OnJoinedRoom();
            return 0;
        }

        private void _OnCreateRoomFailed(string msg)
        {
            print(msg);
            OnCreateRoomFailed(msg);
        }

        private void _OnJoinRoomFailed(string msg)
        {
            print(msg);
            OnJoinRoomFailed(msg);
        }

        //Virtual methods:

        public virtual void OnFailedToConnectToHalko()
        {
        }

        public virtual void OnConnectedToHalko()
        {
        }

        public virtual void OnCreatedRoom()
        {
        }

        public virtual void OnCreateRoomFailed(string msg)
        {
        }

        public virtual void OnJoinedRoom()
        {
        }

        public virtual void OnJoinRoomFailed(string msg)
        {
        }

    }

}
