using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Script : MonoBehaviour
{
    public GameObject directionalArrow,
        directionPivot;
    public float currentDashForce, maxDashForce, addDashSpeed,
        rotationSpeed;
    public GameObject chargebar, chargeBarMax, Marker;

    private Vector3 direction;
    private Rigidbody rb;
    private ParticleSystem particle;
    private bool moving;
    private bool isDashRising = true;
    private bool isCharging = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        particle = GetComponent<ParticleSystem>();
        particle.Stop();
        particle.Clear();

        moving = false;

        chargebar.GetComponent<MeshRenderer>().enabled = false;
        chargeBarMax.GetComponent<MeshRenderer>().enabled = false;

        //Dash();
    }

    private void FixedUpdate()
    {
        moving = !((Mathf.Abs(rb.velocity.x) < 0.001) &&
            (Mathf.Abs(rb.velocity.y) < 0.001) &&
            (Mathf.Abs(rb.velocity.z) < 0.001));

        if (!moving) GetInput();

        if(isCharging)
        {
            float percentage = currentDashForce / maxDashForce;

            particle.startSpeed = 1 + percentage;
            particle.emissionRate = 3 + (percentage * 7);

            chargebar.GetComponent<Chargebar>().changeScale(percentage);
        }

        var mousePosition = Input.mousePosition;
        mousePosition.z = 10.0f;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        Marker.transform.position = mousePosition;
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
        if (Input.GetButton("Dash"))
        {
            if(!isCharging)
            {
                particle.Play();
                isCharging = true;

                chargebar.GetComponent<MeshRenderer>().enabled =  true;
                chargeBarMax.GetComponent<MeshRenderer>().enabled =  true;
            }

            if(isDashRising)
                currentDashForce = currentDashForce + addDashSpeed * Time.deltaTime;
            else
                currentDashForce = currentDashForce - addDashSpeed * Time.deltaTime;

            if (currentDashForce >= maxDashForce)
                isDashRising = false;
            else if (currentDashForce <= 0)
                isDashRising = true;
        }
        else if (Input.GetButtonUp("Dash"))
            Dash();

        else
        {
            transform.Rotate(Vector3.up * Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime);
        }

    }

    public void Dash()
    {
        

        direction = (Marker.transform.position - transform.position).normalized;
        direction.y = 0.0f;

        rb.AddForce(direction * currentDashForce);

        currentDashForce = 0;

        particle.Stop();
        particle.Clear();
        isCharging = false;

        chargebar.GetComponent<Chargebar>().resetScale();
        chargebar.GetComponent<MeshRenderer>().enabled = false;
        chargeBarMax.GetComponent<MeshRenderer>().enabled = false;
    }
}
