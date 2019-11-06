using System.Collections.Generic;
using System;
using System.Diagnostics;
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
    [SerializeField] Button disconnectBtn;

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
        DontDestroyOnLoad(this);
        connectionStatus.text = "NOT CONNECTED";
        connectionStatus.color = Color.red;
        myNetworking = FindObjectOfType<MyNetworking>();
        setNameBtn.onClick.AddListener(() => SetNameBtnClicked());
        createRoomBtn.onClick.AddListener(() => CreateRoomBtnClicked());
        joinRoomBtn.onClick.AddListener(() => JoinRoomBtnClicked());
        FetchRoomsBtn.onClick.AddListener(() => FetchRooms());
        leaveRoomBtn.onClick.AddListener(() => LeaveRoomBtnClicked());
        disconnectBtn.onClick.AddListener(() => DisconnectBtnClicked());
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

    private void DisconnectBtnClicked()
    {
        myNetworking.DisconnectFromHalko();
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
