using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HalkoNetworking;

public class Movement : MonoBehaviour
{
    //Public properties:
    public HalkoPlayer h;
    public float moveSpeed;
    public float rotateSpeed;

    //Private fields:
    private MyNetworking myNetworking;
    private void Start()
    {
        h = GetComponent<HalkoPlayer>();
        myNetworking = FindObjectOfType<MyNetworking>();
        if(!GetComponent<HalkoPlayer>().isLocalPlayer)
        {
            this.enabled = false;
        }
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.H))
        {
            myNetworking.InvokeMethod1("PrintVector", new object[] { new Vector2(10f, 10f), new Vector3(20f, 20f, 20f), new Vector4(30f, 30f, 30f, 30f)});
        }
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
