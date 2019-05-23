using UnityEngine;
using HalkoNetworking.RemoteMethod;

public class TestScript
{
    [HalkoMethod]
    private void PrintLine(string line)
    {
        Debug.Log(line);
    }
}
