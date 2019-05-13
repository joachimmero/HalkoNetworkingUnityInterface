using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HalkoNetworking;

public class MyNetworking : HalkoNetwork
{
    //Public fields:
    private MenuScript ms;

    private void Start()
    {
        ms = FindObjectOfType<MenuScript>();    
    }

    // Update is called once per frame
    void Update()
    {
    }

    //Public methods:

    public void Connect()
    {
        ConnectToHalko();
    }

    //Overrided methods:

    public override void OnConnectedToHalko()
    {
        base.OnConnectedToHalko();
        ms.SetConnectionStatus("CONNECTED TO SERVER", Color.green);
    }
    public override void OnCreatedRoom()
    {
       // ms.SetConnectionStatus("CONNECTED TO ROOM: " + RoomName, Color.green);
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
