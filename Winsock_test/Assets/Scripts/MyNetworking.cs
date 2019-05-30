using UnityEngine;
using HalkoNetworking;
using HalkoNetworking.RemoteMethod;
using System.Collections.Generic;

public class MyNetworking : HalkoNetwork
{
    //Public fields:
    private MenuScript ms;
    private HalkoAttributeHandler ah;

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

    [HalkoMethod]
    public void PrintMsg(string msg, int i, bool b, float f, double d, char c, uint u)
    {
        print(msg + i.ToString() + i.ToString() + b.ToString() + f.ToString() + d.ToString() + c + u.ToString());
    }

    [HalkoMethod]
    public void ChangePlayerColor()
    {
        print("kakkostesti");
    }

    [HalkoMethod]
    public void PrintVector(Vector2 v2, Vector3 v3, Vector4 v4)
    {
        print("VECTORS");
        print("Vector2: " + v2.ToString());
        print("Vector3: " + v3.ToString());
        print("Vector4: " + v4.ToString());
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

    public override void OnLeftRoom()
    {
        ms.OpenMenu(1);
    }
}
