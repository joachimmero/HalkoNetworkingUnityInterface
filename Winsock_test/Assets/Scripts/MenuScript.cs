using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;


public class MenuScript : MonoBehaviour
{
    //Private properties:
    [Header("Buttons")]
    [SerializeField] Button setNameBtn;
    [SerializeField] Button createRoomBtn;
    [SerializeField] Button joinRoomBtn;
    [SerializeField] Button FetchRoomsBtn;
    [SerializeField] Button leaveRoomBtn;

    [Header("Input Fields")]
    [SerializeField] InputField playerNameField;
    [SerializeField] InputField createRoomName;
    [SerializeField] InputField joinRoomName;

    [Header("Dropdowns")]
    [SerializeField] Dropdown createMaxPlayers;

    [Header("Texts")]
    [SerializeField] Text connectionStatus;

    [Header("Menus")]
    [SerializeField] GameObject[] menus;

    private MyNetworking myNetworking;

    // Start is called before the first frame update
    void Start()
    {
        object[] objs = new object[]
        {
            (object)"ookei",
            (object)true,
            (object)152,
            (object)'w',
            (object)(uint)132,
            (object)1.52f,
            (object)1.35
        };
        for(int i = objs.Length - 1; i >= 0; --i)
        {
            if(objs[i].GetType() == typeof(bool))
            {
                bool temp = (bool)objs[i];
                print("Bool: " + temp);
            }
            else if(objs[i].GetType() == typeof(uint))
            {
                uint temp = (uint)objs[i];
                print("UInt: " + temp);
            }
            else if (objs[i].GetType() == typeof(string))
            {
                string temp = (string)objs[i];
                print("String: " + temp);
            }
            else if (objs[i].GetType() == typeof(char))
            {
                char temp = (char)objs[i];
                print("Char: " + temp);
            }
            else if (objs[i].GetType() == typeof(double))
            {
                double temp = (double)objs[i];
                print("Double: " + temp);
            }
            else if (objs[i].GetType() == typeof(float))
            {
                float temp = (float)objs[i];
                print("Float: " + temp);
            }
            else if (objs[i].GetType() == typeof(int))
            {
                int temp = (int)objs[i];
                print("Int: " + temp);
            }
        }

        DontDestroyOnLoad(this);
        connectionStatus.text = "NOT CONNECTED";
        connectionStatus.color = Color.red;
        myNetworking = FindObjectOfType<MyNetworking>();
        setNameBtn.onClick.AddListener(() => SetNameBtnClicked());
        createRoomBtn.onClick.AddListener(() => CreateRoomBtnClicked());
        joinRoomBtn.onClick.AddListener(() => JoinRoomBtnClicked());
        FetchRoomsBtn.onClick.AddListener(() => FetchRooms());
        leaveRoomBtn.onClick.AddListener(() => LeaveRoomBtnClicked());

    }

    //Public methods:
    public void SetConnectionStatus(string text, Color color)
    {
        connectionStatus.text = text;
        connectionStatus.color = color;
    }

    public void OpenMenu(int index)
    {
        for (int i = 0; i < menus.Length; i++)
        {
            menus[i].SetActive(false);
        }
        menus[index].SetActive(true);
    }
    //Private methods:

    private void SetNameBtnClicked()
    {
        if (playerNameField.text != "")
        {
            myNetworking.clientName = playerNameField.text;
            myNetworking.ConnectToHalko();
            OpenMenu(1);
        }
    }
    private void CreateRoomBtnClicked()
    {
        myNetworking.CreateRoom(createRoomName.text, createMaxPlayers.value + 1);
        OpenMenu(2);
    }
    private void JoinRoomBtnClicked()
    {
        myNetworking.JoinRoom(joinRoomName.text);
        OpenMenu(2);
    }
    private void LeaveRoomBtnClicked()
    {
        print("clicked");
        myNetworking.LeaveRoom();
    }
    
    private void FetchRooms()
    {
        List<Room> rooms = myNetworking.GetRooms();
        for(int i = rooms.Count - 1; i >= 0; --i)
        {
            print("Room name: " + rooms[i].roomName + ", Players in room: " + rooms[i].playerCount + ", Max players: " + rooms[i].maxPlayers);
        }
    }
}
