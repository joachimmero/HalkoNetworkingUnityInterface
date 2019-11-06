using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Reflection;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;
using HalkoNetworking.RemoteMethod;

namespace HalkoNetworking
{
    public class HalkoNetwork : MonoBehaviour
    {
        //Public properties:
        public bool ConnectedToHalko
        {
            get
            {
                return connectedToHalko;
            }
        }

        public TcpClient Client
        {
            get
            {
                return client;
            }
        }

        //Public fields:
        public List<HalkoClass> halkoClasses;

        //Private fields:
        private uint clientId;
        private int mainThreadId;
        private bool connectedToHalko = false;
        private List<ClientInfo> clients;
        private List<KeyValuePair<string, MethodInfo>> halkoMethods;
        private TcpClient client;
        private NetworkStream stream;
        private HalkoClientHandler clientHandler;
        private HalkoAttributeHandler ah;

        [SerializeField] bool connectedToRoom = false;
        [SerializeField] HalkoPlayer localPlayer;

        [Header("Network Settings")]
        [SerializeField] string offlineScene;
        [SerializeField] string nextScene;

        [Header("Client Settings")]
        public string clientName = "Dummy";
        public GameObject player;
        
        //Public methods

        public void ConnectToHalko()
        {

            DontDestroyOnLoad(gameObject);

            halkoClasses = new List<HalkoClass>();

            string hostName = Dns.GetHostName();
            string ip = Dns.GetHostAddresses(hostName)[1].ToString();

            if (ip != "192.168.0.157")
                ip = "192.168.43.176";

            Connect(ip, 27015);
        }

        public void CreateRoom(string rName, int maxPlayers)
        {
            if(connectedToHalko && !connectedToRoom)
            {
                //Length of this clients name-string.
                int len1 = clientName.Length;
                //"0" + maxPlayers in characters(1 character) + nameLen (uint) in bytes + name.Length + roomName.Length
                int len2 = 6 + len1 + rName.Length;

                //Length of the "Create Room" -stream to be sent to the server
                byte[] strmLen = BitConverter.GetBytes((uint)len2);
                //Send the size of the next stream to the server.
                stream.Write(strmLen, 0, 4);

                //Create an array that holds the length of the clients name in bytes.
                byte[] nameLen = BitConverter.GetBytes((uint)len1);

                //Create an empty buffer for the stream to be sent.
                byte[] createRoomBuf = new byte[50];
                
                //Insert the "Create Room" -flag to the buffer
                createRoomBuf[0] = (byte)'c';
                //Insert the length of this clients name to the buffer.
                createRoomBuf[1] = nameLen[0];
                createRoomBuf[2] = nameLen[1];
                createRoomBuf[3] = nameLen[2];
                createRoomBuf[4] = nameLen[3];
                
                //Create a array that holds the rest of the info needed in the buffer
                byte[] rest = Encoding.ASCII.GetBytes(clientName + rName + maxPlayers.ToString());
                //Add the rest of the info to the buffer
                for (int i = 0; i < rest.Length; i++)
                {
                    createRoomBuf[5 + i] = rest[i];
                }
                //Send the "Create Room" -buffer to the server.
                stream.Write(createRoomBuf, 0, len2);

                Receive('c');
            }
            else
            {
                _OnCreateRoomFailed("Can't create a room. This client is not connected to the server...");
            }
        }

        public void JoinRoom(string rName)
        {
            if(connectedToHalko && !connectedToRoom)
            {
                //Length of this clients name-string.
                int len1 = clientName.Length;
                //"1" + nameLen (uint) in bytes + name.Length + roomName.Length
                int len2 = 5 + len1 + rName.Length;

                //Length of the "Create Room" -stream to be sent to the server
                byte[] strmLen = BitConverter.GetBytes((uint)len2);
                //Send the size of the next stream to the server.
                stream.Write(strmLen, 0, 4);

                //Create an array that holds the length of the clients name in bytes.
                byte[] nameLen = BitConverter.GetBytes((uint)len1);

                //Create an empty buffer for the stream to be sent.
                byte[] joinRoomBuf = new byte[50];

                //Insert the "Join Room" -flag to the buffer
                joinRoomBuf[0] = (byte)'j';
                //Insert the length of this clients name to the buffer.
                joinRoomBuf[1] = nameLen[0];
                joinRoomBuf[2] = nameLen[1];
                joinRoomBuf[3] = nameLen[2];
                joinRoomBuf[4] = nameLen[3];

                //Create a array that holds the rest of the info needed in the buffer
                byte[] rest = Encoding.ASCII.GetBytes(clientName + rName);
                //Add the rest of the info to the buffer
                for (int i = 0; i < rest.Length; ++i)
                {
                    joinRoomBuf[5 + i] = rest[i];
                }
                //Send the "Create Room" -buffer to the server.
                stream.Write(joinRoomBuf, 0, len2);

                Receive('c');
            }
            else
            {
                _OnJoinRoomFailed("Can't join a room. This client is not connected to the server...");
            }
        }

        public void LeaveRoom()
        {
            if(connectedToRoom)
            {
                //Send the server a stream that indicates, that a client left the server
                //intentionally.
                byte[] buf = { (byte)'l' };
                stream.Write(buf, 0, 1);

                Receive('c');
                /*
                byte[] callback = new byte[1];
                //Read a callback from the server.
                stream.Read(callback, 0, 1);
                //Check that the callback indicates that the player has succesfully left the room.
                if (callback[0] == (byte)'l')
                {
                    _OnLeftRoom();
                }
                */
            }
            else
            {
                print("Client is not connected to any room.");
            }
        }

        public List<HalkoPlayer> GetCurrentPlayers()
        {
            return clientHandler.connectedPlayers;
        }

        public List<Room> GetRooms()
        {
            //Send the size of the buffer to the server
            stream.Write(BitConverter.GetBytes((uint)1), 0, 4);

            //Send the info that you want to receive a list of all the rooms on the server.
            stream.Write(Encoding.ASCII.GetBytes("g"), 0, 1);

            //Create a list where the rooms can be stored.
            List<Room> rooms = new List<Room>();

            //Read data about how many rooms are on the server.
            byte[] data = new byte[4];
            stream.Read(data, 0, 4);
            uint roomsCount = BitConverter.ToUInt32(data, 0);
            print("Room count: " + roomsCount);
            
            //Read roomsCount of times info about a room on the server.
            for(int i = (int)roomsCount - 1; i >= 0; --i)
            {
                byte[] lenBuf = new byte[4];
                stream.Read(lenBuf, 0, 4);
                uint namelen = BitConverter.ToUInt32(lenBuf, 0);
                byte[] roomData = new byte[namelen + 8];
                stream.Read(roomData, 0, roomData.Length);

                uint playersInRoom = BitConverter.ToUInt32(roomData, 0);
                uint maxPlayers = BitConverter.ToUInt32(roomData, 4);
                string roomName = Encoding.ASCII.GetString(roomData, 8, (int)namelen);

                rooms.Add(new Room(
                    roomName,
                    playersInRoom,
                    maxPlayers
                    ));
            }
            
            return rooms;
        }

        public void DisconnectFromHalko()
        {
            //Create the byte-arrays to be sent to the server
            byte[] strmLength = new byte[] { 1, 0, 0, 0};
            byte[] strm = new byte[] { (byte)'d' };
            //Send the arrays
            stream.Write(strmLength, 0, 4);
            stream.Write(strm, 0, 1);
            //Wait for a callback
            Receive('c');
        }

        //Adds a HalkoClass to the halkoClasses-list and returns its index.
        public int AddHalkoClass(HalkoClass h)
        {
            halkoClasses.Add(h);
            return halkoClasses.Count - 1;
        }

        //Private methods

        private void Connect(string ip, int port)
        {
            clientHandler = this.gameObject.AddComponent<HalkoClientHandler>();
            mainThreadId = Thread.CurrentThread.ManagedThreadId;

            try
            {
                //Create a TCPClient
                client = new TcpClient(ip, port);

                //Get a clinet stream for reading and writing 
                stream = client.GetStream();

                clients = new List<ClientInfo>();

                Receive('c');
            } 
            catch (SocketException e)
            {
                print(e.Message);
                _OnFailedToConnectToHalko();
            }
        }

        private void SetRemoteClientTransform(uint clientId, Vector3 nextPos, Vector3 nextRot)
        {
            if (client.Connected)
            {
                for (int i = 0; i < clientHandler.connectedPlayers.Count; i++)
                {
                    if(clientHandler.connectedPlayers[i].clientId == clientId)
                    {
                        clientHandler.connectedPlayers[i].SetTransformProperties(nextPos, nextRot);
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
                Formatter f = new Formatter();
                Package p = new Package();
                new Thread(() =>
                {
                    NetworkStream s = client.GetStream();
                    while (connectedToRoom)
                    {
                        //Read the length of the received data from the stream
                        byte[] datalen = new byte[4];
                        s.Read(datalen, 0, datalen.Length);
                        uint dataLength = BitConverter.ToUInt32(datalen, 0);

                        //Read dataLength-bytes from the stream.
                        byte[] data = new byte[dataLength];
                        s.Read(data, 0, data.Length);

                        //Take the first byte of the data and assign it to be the streamFlag.
                        string streamflag = Encoding.ASCII.GetString(data, 0, 1);

                        //If the received stream holds a clients transform information.
                        if (streamflag == "t")
                        {
                            uint clientId = BitConverter.ToUInt32(data, 1);
                            p = f.DeSerializePackage(data);
                            SetRemoteClientTransform(clientId, new Vector3(p.pos_x, p.pos_y, p.pos_z), new Vector3(p.rot_x, p.rot_y, p.rot_z));
                        }
                        //If the received stream holds information that 
                        //a method needs to be called.
                        else if(streamflag == "m")
                        {
                            //Get the classIndex of the halkoClass from the stream.
                            uint classIndex = BitConverter.ToUInt32(data, 1);

                            KeyValuePair<int, object[]> method = f.DeSerializeMethod(data);
                            print("Jeesi");
                            HalkoClass classWithMethod = halkoClasses[(int)classIndex];
                            classWithMethod.methodsWaitingForInvoke.Add(method);
                        }
                        //If the received stream holds information, that a new client has connected to the room.
                        else if (streamflag == "n")
                        {
                            print("New player has joined the room.");
                            //Handle the received data and instantiate the appropriate players.
                            clientHandler.LastJoinedPlayer = new ClientInfo(
                                BitConverter.ToUInt32(data, 1),
                                Encoding.ASCII.GetString(data, 5, data.Length - 5),
                                false
                            );
                        }
                        //If the received stream holds information, that a client has left the room.
                        else if (streamflag == "l")
                        {
                            uint leftPlayerId = BitConverter.ToUInt32(data, 1);
                            for(int i = 0; i < clients.Count; i++)
                            {
                                if(leftPlayerId == clients[i].clientId)
                                {
                                    clients.RemoveAt(i);
                                }
                            }
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
                    _OnCreatedRoom(BitConverter.ToUInt32(data, 1));
                }
                else if (streamflag == "j")
                {
                    _OnJoinedRoom(BitConverter.ToUInt32(data, 1));
                }
                else if (streamflag == "l")
                {
                    _OnLeftRoom();
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
                else if(streamflag == "d")
                {
                    _OnDisconnectedFromHalko();
                }
            }
        }
        /// <summary>
        /// Spawns every HalkoPlayer that is in the connectedPlayers-list.
        /// </summary>
        private void SpawnPlayers()
        {
            for(int i = 0; i < clients.Count; i++)
            {
                ClientInfo c = clients[i];
                HalkoPlayer h = clientHandler.InstantiatePlayer(
                    c.clientId,
                    c.clientName,
                    c.isLocalClient
                    );
                if (c.isLocalClient)
                {
                    localPlayer = h;
                }
            }

            Receive('r');
        }

        IEnumerator LoadScene(string scene)
        {
            AsyncOperation load = SceneManager.LoadSceneAsync(scene);
            while(!load.isDone)
            {
                yield return null;
            }
            if(clients.Count > 0)
            {
                SpawnPlayers();
            }
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

        private void _OnCreatedRoom(uint playerId)
        {
            connectedToRoom = true;
            print("Scene index: " + SceneManager.GetActiveScene().buildIndex);
            clientId = playerId;

            clients.Add(new ClientInfo(
                clientId,
                clientName,
                true
                ));

            StartCoroutine(LoadScene(nextScene));
            print("Room created!");
            OnCreatedRoom();
        }

        private void _OnJoinedRoom(uint playerId)
        {
            connectedToRoom = true;
            clientId = playerId;

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

                clients.Add(new ClientInfo(
                    BitConverter.ToUInt32(pInfo, 0),
                    Encoding.ASCII.GetString(pInfo, 4, (int)cNameSize),
                    false
                    ));
            }

            clients.Add(new ClientInfo(
                clientId,
                clientName,
                true
                ));

            StartCoroutine(LoadScene(nextScene));
            print("Room joined");
            OnJoinedRoom();
        }
        
        private void _OnLeftRoom()
        {
            print("Room left.");
            connectedToRoom = false;
            Destroy(localPlayer.gameObject);
            localPlayer = null;
            clients.Clear();
            clientHandler.connectedPlayers.Clear();
            StartCoroutine(LoadScene(offlineScene));
            OnLeftRoom();
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

        private void _OnDisconnectedFromHalko()
        {
            print("Disconnected from server.");
            stream.Close();
            client.Close();
            OnDisconnectedFromHalko();
        }

        //Virtual methods:

        public virtual void OnFailedToConnectToHalko(){}

        public virtual void OnConnectedToHalko(){}

        public virtual void OnCreatedRoom(){}

        public virtual void OnCreateRoomFailed(string msg){}

        public virtual void OnJoinedRoom(){}

        public virtual void OnJoinRoomFailed(string msg){}

        public virtual void OnLeftRoom(){}

        public virtual void OnDisconnectedFromHalko(){}
    }

}
