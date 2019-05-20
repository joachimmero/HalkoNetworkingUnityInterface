using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Room
{
    //Public fields:
    public string roomName;
    public uint playerCount;
    public uint maxPlayers;

    public Room(string name, uint players, uint maxPlayers)
    {
        roomName = name;
        playerCount = players;
        //TEMP: Muuta tämä paremman nimiseksi!
        this.maxPlayers = maxPlayers;
    }
}
