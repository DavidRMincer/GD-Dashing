using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_Script : MonoBehaviour
{
    public GameObject directionalArrow,
        directionPivot;
    public float startDashForce,
        maxDashForce,
        addDashSpeed,
        rotationSpeed,
		cameraShakeDuration,
		cameraShakeIntensity,
        cameraShakeIntensityCharging,
        lerpMaxTime;
    public GameObject chargebar,
        chargeBarMax,
        cameraPosition;
    public Text text;
    public Camera followingCamera;
    public ParticleSystem dust;
    public Vector3 direction;

    private int dashesAmount;
    private Rigidbody rb;
    private ParticleSystem particle;
    private bool moving;
    private bool isDashRising = true;
    private bool isCharging = false;
    private Vector3 cameraOffset;
    private float currentDashForce,
        lerpCounter;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        particle = GetComponent<ParticleSystem>();
        particle.Stop();
        particle.Clear();

        moving = false;

        chargebar.GetComponent<MeshRenderer>().enabled = false;
        chargeBarMax.GetComponent<MeshRenderer>().enabled = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        //Dash();
        //cameraOffset = followingCamera.transform.position - transform.position;
    }

    private void FixedUpdate()
    {
        moving = !((Mathf.Abs(rb.velocity.x) < 0.1) &&
            (Mathf.Abs(rb.velocity.y) < 0.1) &&
            (Mathf.Abs(rb.velocity.z) < 0.1));

        if (!moving) GetInput();
    }

    private void LateUpdate()
    {
        if (moving)
        {
            directionalArrow.SetActive(false);
            lerpCounter += Time.deltaTime;
        }
        else
        {
            directionalArrow.SetActive(true);
        }

        if (isCharging)
        {
            float percentage = currentDashForce / maxDashForce;

            particle.startSpeed = 1 + (percentage * 1.3f);
            particle.emissionRate = 3 + (percentage * 14);
            particle.startSize = 0.12f + (percentage * 0.5f);

            chargebar.GetComponent<Chargebar>().changeScale(percentage);
        }

        updateCameraPosition();
        
        if (isCharging)
            StartCoroutine(ShakeCameraOverTime());
    }

    private void updateCameraPosition()
    {
        Vector3 NewPos;

        if (moving) NewPos = Vector3.Lerp(followingCamera.transform.position, cameraPosition.transform.position, lerpCounter / lerpMaxTime);
        else NewPos = cameraPosition.transform.position;
        
        followingCamera.transform.position = NewPos;

        followingCamera.transform.eulerAngles = new Vector3(followingCamera.transform.eulerAngles.x, transform.eulerAngles.y, followingCamera.transform.eulerAngles.z);

        if (lerpCounter >= lerpMaxTime) lerpCounter = 0.1f;
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
                currentDashForce = currentDashForce + (addDashSpeed * Time.deltaTime);
            else
                currentDashForce = currentDashForce - (addDashSpeed * Time.deltaTime);

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

            //Vector3 oldCamPosition = followingCamera.transform.position;
            //followingCamera.transform.RotateAround(chargebar.transform.position, Vector3.up, Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime);
            //cameraOffset += followingCamera.transform.position - oldCamPosition;
        }

    }

    public void Dash()
    {
        direction = (directionalArrow.transform.position - transform.position).normalized;
        direction.y = 0.0f;

        rb.AddForce(direction * (currentDashForce + startDashForce));

        particle.Stop();
        particle.Clear();
        isCharging = false;

        chargebar.GetComponent<Chargebar>().resetScale();
        chargebar.GetComponent<MeshRenderer>().enabled = false;
        chargeBarMax.GetComponent<MeshRenderer>().enabled = false;

        dust.Play();

        currentDashForce = 0;

        dashesAmount++;

        StartCoroutine(changeText());

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

    public IEnumerator ShakeCameraOverTime()
    {
        float percentage = (currentDashForce + startDashForce) / maxDashForce;

        Vector3 originalPos = followingCamera.transform.localPosition;

        float elapsed = 0.0f;

        while (elapsed < cameraShakeDuration)
        {
            float x = followingCamera.transform.position.x + (Random.Range(-1.0f, 1.0f) * cameraShakeIntensityCharging * percentage);
            float y = followingCamera.transform.position.y + (Random.Range(-1.0f, 1.0f) * cameraShakeIntensityCharging * percentage);

            followingCamera.transform.localPosition = new Vector3(x, y, originalPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }
    }

    public IEnumerator changeText()
    {
        text.text = "Dashes: " + dashesAmount;

        int oldFontsize = text.fontSize;
        text.fontSize += 2;

        yield return new WaitForSeconds(0.2f);

        text.fontSize = oldFontsize;
    }
}
