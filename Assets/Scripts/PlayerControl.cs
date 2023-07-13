using System;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float acceleration = 10f;
    public float turnSpeed = 10f;

    public bool isGhost;
    
    public Rigidbody myRigidbody;
    public Transform controlDirection;
    public Transform displayObj;

    private bool[] keyStates = new bool[Enum.GetNames(typeof(KeyCode)).Length];
    
    void Start()
    {
        if (!isGhost)
        {
            GameObject.Find("GhostSystem")
                ?.GetComponent<GhostSystem>()
                .SetObservationTargets(myRigidbody.transform,
                                        controlDirection,
                                        myRigidbody,
                                        new KeyCode[] {
                                        KeyCode.W,
                                        KeyCode.S,
                                        KeyCode.D,
                                        KeyCode.A
                                        });
        }
    }

    void Update()
    {
        if(isGhost)
        {
            return;
        }

        keyStates[(int)KeyCode.W] = Input.GetKey(KeyCode.W);
        keyStates[(int)KeyCode.S] = Input.GetKey(KeyCode.S);
        keyStates[(int)KeyCode.D] = Input.GetKey(KeyCode.D);
        keyStates[(int)KeyCode.A] = Input.GetKey(KeyCode.A);

        ApplyKeyStates(keyStates);
    }

    public void ApplyKeyStates(bool[] keyStates)
    {
        if (keyStates[(int)KeyCode.W])
        {
            myRigidbody.velocity += controlDirection.forward.normalized * acceleration * Time.deltaTime;
        }

        if (keyStates[(int)KeyCode.S])
        {
            myRigidbody.velocity -= controlDirection.forward.normalized * acceleration * Time.deltaTime;
        }

        if (keyStates[(int)KeyCode.A])
        {
            controlDirection.RotateAround(controlDirection.position, Vector3.up, turnSpeed * -Time.deltaTime);
        }

        if (keyStates[(int)KeyCode.D])
        {
            controlDirection.RotateAround(controlDirection.position, Vector3.up, turnSpeed * Time.deltaTime);
        }

        displayObj.position = myRigidbody.transform.position;
        displayObj.forward = controlDirection.forward;
    }

    public void UpdateVelocity(Vector3 position, Vector3 direction, Vector3 velocity)
    {
        myRigidbody.transform.position = position;
        controlDirection.forward = direction;
        myRigidbody.velocity = velocity;
    }
}
