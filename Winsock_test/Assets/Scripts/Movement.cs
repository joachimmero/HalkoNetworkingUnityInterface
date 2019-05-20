﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HalkoNetworking;

public class Movement : MonoBehaviour
{
    //Public properties:
    public HalkoPlayer h;
    public float moveSpeed;
    public float rotateSpeed;

    private void Start()
    {
        h = GetComponent<HalkoPlayer>();    
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        /*
        h.Translate(new Vector3(Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime, 0, Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime));
        h.Rotate(new Vector3(0, Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime, 0));
        */
        h.position = new Vector3(h.position.x + (Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime), h.position.y, h.position.z + (Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime));
        h.eulerAngles = new Vector3(h.eulerAngles.x, h.eulerAngles.y + (Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime), h.eulerAngles.z);
    }
}
