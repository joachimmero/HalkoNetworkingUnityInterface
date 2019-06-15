using UnityEngine;
using HalkoNetworking.RemoteMethod;

public class TestAbility : HalkoClass
{
    [HalkoMethod]
    private void ChangeColor()
    {
        GetComponent<MeshRenderer>().material.color = Color.red;
    }
}
