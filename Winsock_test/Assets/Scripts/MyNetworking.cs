using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HalkoNetworking;

public class MyNetworking : HalkoNetwork
{

    //Public properties:
    public Text serverConnection;
    public InputField jNameField;
    public InputField cNameField;
    public Dropdown cMaxPlayers;
    
    //Private properties:

    // Start is called before the first frame update
    void Start()
    {

        serverConnection.text = "NOT CONNECTED";
        serverConnection.color = Color.red;
    }

    // Update is called once per frame
    void Update()
    {
        print(GetCurrentPlayers().Count);
    }

    //Public methods:

    public void Connect()
    {
        ConnectToHalko();
    }
    public void CreateRoom()
    {
        CreateRoom(cNameField.text, cMaxPlayers.value + 1);
    }

    public void JoinRoom()
    {
        JoinRoom(jNameField.text);
    }

    //Overrided methods:

    public override void OnConnectedToHalko()
    {
        base.OnConnectedToHalko();
        serverConnection.text = "CONNECTED TO SERVER";
        serverConnection.color = Color.green;
    }
    public override void OnCreatedRoom()
    {
        serverConnection.text = "CONNECTED TO ROOM: " + RoomName;
    }

    public override void OnCreateRoomFailed(string msg)
    {
    }

    public override void OnJoinedRoom()
    {
    }

    public override void OnJoinRoomFailed(string msg)
    {
    }
}
