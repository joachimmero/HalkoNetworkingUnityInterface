using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HalkoNetworking;

public class Movement : MonoBehaviour
{
    //Public properties:
    public HalkoPlayer h;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        h.Translate(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));
    }
}
