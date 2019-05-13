﻿using UnityEngine;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    //Private properties:
    [Header("Buttons")]
    [SerializeField] Button setNameBtn;
    [SerializeField] Button createRoomBtn;
    [SerializeField] Button joinRoomBtn;

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
        connectionStatus.text = "NOT CONNECTED";
        connectionStatus.color = Color.red;
        myNetworking = FindObjectOfType<MyNetworking>();
        setNameBtn.onClick.AddListener(() => OpenMenu(1));
        createRoomBtn.onClick.AddListener(() => myNetworking.CreateRoom(createRoomName.text, createMaxPlayers.value - 1));
        joinRoomBtn.onClick.AddListener(() => myNetworking.JoinRoom(joinRoomName.text));
    }
    
    //Public methods:
    public void SetConnectionStatus(string text, Color color)
    {
        connectionStatus.text = text;
        connectionStatus.color = color;
    }

    //Private methods:
    private void OpenMenu(int index)
    {
        if(index == 1 && playerNameField.text == "")
        {
            return;
        }
        else
        {
            myNetworking.clientName = playerNameField.text;
            myNetworking.ConnectToHalko();
        }

        for (int i = 0; i < menus.Length; i++)
        {
            if(index != i)
            {
                menus[i].SetActive(false);
            }
            else
            {
                menus[i].SetActive(true);
            }
        }
    }
}
