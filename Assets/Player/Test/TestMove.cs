using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMove : MonoBehaviour
{
    public CharacterController controller;
    public GameObject body;
    public Camera cam;

    public float speed = 12f;
    public float gravity = -9.81f;
    public float groundDistance = 0.4f;
    public float flySpeed = 3f;

    Vector3 velocity;
    bool isGrounded;

    void Start() {

    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right *x + transform.forward * z;

        controller.Move(move * speed  * Time.deltaTime);

        if (Input.GetKey(KeyCode.Space))
        {   
            transform.Translate(Vector3.up * flySpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {   
            transform.Translate(Vector3.down * flySpeed * Time.deltaTime);
        }
    }
}
