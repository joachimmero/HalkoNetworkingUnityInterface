using UnityEngine;
using HalkoNetworking.RemoteMethod;

public class TestAbility : HalkoClass
{
    [HalkoMethod]
    private void ChangeColor(int color)
    {
        switch(color)
        {
            case 0:
                GetComponent<MeshRenderer>().material.color = Color.red;
                break;
            case 1:
                GetComponent<MeshRenderer>().material.color = Color.green;
                break;
            case 2:
                GetComponent<MeshRenderer>().material.color = Color.blue;
                break;
            case 3:
                GetComponent<MeshRenderer>().material.color = Color.yellow;
                break;
            case 4:
                GetComponent<MeshRenderer>().material.color = Color.white;
                break;
            case 5:
                GetComponent<MeshRenderer>().material.color = Color.magenta;
                break;
            case 6:
                GetComponent<MeshRenderer>().material.color = Color.black;
                break;
        }
    }

    [HalkoMethod]
    private void Test()
    {

    }
}
