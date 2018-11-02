using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Script : MonoBehaviour
{
    public GameObject directionalArrow,
        directionPivot;
    public float currentDashForce, maxDashForce, addDashSpeed,
        rotationSpeed,cameraSpeed,
		cameraShakeDuration,
		cameraShakeIntensity;
    public GameObject chargebar, chargeBarMax, Marker;
    public Camera followingCamera;
    public ParticleSystem dust;
    public Vector3 direction;

    private Rigidbody rb;
    private ParticleSystem particle;
    private bool moving;
    private bool isDashRising = true;
    private bool isCharging = false;
    private Vector3 cameraOffset;

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
        cameraOffset = followingCamera.transform.position - transform.position;
    }

    private void FixedUpdate()
    {
        moving = !((Mathf.Abs(rb.velocity.x) < 0.1) &&
            (Mathf.Abs(rb.velocity.y) < 0.1) &&
            (Mathf.Abs(rb.velocity.z) < 0.1));

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
        updateCameraPosition();

        if (moving)
        {
            directionalArrow.SetActive(false);
        }
        else
        {
            directionalArrow.SetActive(true);
        }
    }

    private void updateCameraPosition()
    {
        Vector3 newCamPos = transform.position + cameraOffset;
        Vector3 camDirection = (newCamPos - followingCamera.transform.position) * Time.deltaTime * cameraSpeed;

        followingCamera.transform.position += camDirection;
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

        dust.Play();

		StartCoroutine(ShakeCamera());
    }

	public IEnumerator ShakeCamera()
	{
		Vector3 originalPos = followingCamera.transform.localPosition;

		float elapsed = 0.0f;

		while (elapsed < cameraShakeDuration)
		{
			float x = followingCamera.transform.position.x + (Random.Range(-1.0f, 1.0f) * cameraShakeIntensity);
			float y = followingCamera.transform.position.y + (Random.Range(-1.0f, 1.0f) * cameraShakeIntensity);

			followingCamera.transform.localPosition = new Vector3(x, y, originalPos.z);

			elapsed += Time.deltaTime;

			yield return null;
		}
	}
}
