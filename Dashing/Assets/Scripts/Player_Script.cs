using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Script : MonoBehaviour
{
    public GameObject directionalArrow,
        directionPivot;
    public float dashForce,
        rotationSpeed;

    private Vector3 direction;
    private Rigidbody rb;
    private bool moving;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        moving = false;

        //Dash();
    }

    private void FixedUpdate()
    {
        moving = !((Mathf.Abs(rb.velocity.x) < 0.001) &&
            (Mathf.Abs(rb.velocity.y) < 0.001) &&
            (Mathf.Abs(rb.velocity.z) < 0.001));

        if (!moving) GetInput();
    }

    private void LateUpdate()
    {
        if (moving)
        {
            directionalArrow.SetActive(false);
        }
        else
        {
            directionalArrow.SetActive(true);
        }
    }

    private void GetInput()
    {
        if (Input.GetButtonDown("Dash")) Dash();

        else
        {
            transform.Rotate(Vector3.up * Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime);
        }

    }

    public void Dash()
    {
        direction = directionalArrow.transform.position - transform.position;
        direction.y = 0.0f;

        rb.AddForce(direction * dashForce);
    }
}
