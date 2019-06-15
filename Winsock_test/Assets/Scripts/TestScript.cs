using UnityEngine;
using HalkoNetworking.RemoteMethod;

public class TestScript : HalkoClass
{
    [HalkoMethod]
    private void PrintLine(string line)
    {
        Debug.Log(line);
    }
}
