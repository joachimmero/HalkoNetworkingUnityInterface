using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneInstantiator : MonoBehaviour
{
    [SerializeField] GameObject networkingObject;
    [SerializeField] GameObject canvasObject;

    // Start is called before the first frame update
    void Start()
    {
        if(FindObjectOfType<MyNetworking>() == null)
        {
            Instantiate(networkingObject);
        }
        if(FindObjectOfType<MenuScript>() == null)
        {
            Instantiate(canvasObject);
        }
    }
}
