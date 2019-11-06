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
    private TestAbility testAbility;
    private TestAbility2 testAbility2;

    private void Start()
    {
        h = GetComponent<HalkoPlayer>();
        myNetworking = FindObjectOfType<MyNetworking>();
        if(!GetComponent<HalkoPlayer>().isLocalPlayer)
        {
            this.enabled = false;
        }

        testAbility = FindObjectOfType<TestAbility>();
        testAbility2 = GetComponent<TestAbility2>();
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.R))
        {
            testAbility.InvokeMethod("ChangeColor", new object[] { 0 });
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            testAbility.InvokeMethod("ChangeColor", new object[] { 1 });
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            testAbility.InvokeMethod("ChangeColor", new object[] { 2 });
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            testAbility.InvokeMethod("ChangeColor", new object[] { 3 });
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            testAbility.InvokeMethod("ChangeColor", new object[] { 4 });
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            testAbility.InvokeMethod("ChangeColor", new object[] { 5 });
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            testAbility.InvokeMethod("ChangeColor", new object[] { 6 });
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            testAbility2.InvokeMethod("TeleportToRandomLocationWithinRadius", new object[] { 5 });
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
