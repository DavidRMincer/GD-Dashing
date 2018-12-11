using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_Script : MonoBehaviour
{
    public GameObject directionalArrow,
        directionPivot,
        rotateHint,
        dashHint,
        chargebar,
        chargeBarMax,
        cameraPosition,
        clippingCameraPosition;
    public float startDashForce,
        maxDashForce,
        addDashSpeed,
        rotationSpeed,
		cameraShakeDuration,
		cameraShakeIntensity,
        cameraShakeIntensityCharging,
        lerpMaxTime,
        hintWaitTime;
    public Text text;
    public Camera followingCamera;
    public ParticleSystem dust;
    public Vector3 direction;

    private int dashesAmount;
    private Rigidbody rb;
    private ParticleSystem particle;
    private bool moving,
        rotated = false,
        dashed = false,
        isDashRising = true,
        isCharging = false;
    private Vector3 cameraOffset;
    private float currentDashForce,
        lerpCounter,
        hintCounter;
    private QueryTriggerInteraction layerMask;

    //Audio vars
    [SerializeField]
    private AudioSource source;
    [SerializeField]
    private AudioClip chargeSound;
    [SerializeField]
    private AudioClip REVchargeSound;
    [SerializeField]
    private AudioClip dashSound;

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

        //init audio
        source.GetComponent<AudioSource>();
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

        if (!rotated)
        {
            hintCounter += Time.deltaTime;

            if (hintCounter >= hintWaitTime)
            {
                rotateHint.SetActive(true);
                hintCounter = 0.0f;
            }
        }
        else
        {
            rotateHint.SetActive(false);

            if (!dashed)
            {
                hintCounter += Time.deltaTime;

                if (hintCounter >= hintWaitTime)
                {
                    dashHint.SetActive(true);
                    hintCounter = 0.0f;
                }
            }
            else dashHint.SetActive(false);
        }
    }

    private void updateCameraPosition()
    {
        Vector3 NewPos,
            camToPlayer = transform.position - cameraPosition.transform.position,
            NewCam;
        RaycastHit hit;

        if (Physics.Raycast(cameraPosition.transform.position, camToPlayer, out hit, 100.0f)
            && hit.transform.tag != "Player") NewCam = clippingCameraPosition.transform.position;
        else NewCam = cameraPosition.transform.position;

        if (moving) NewPos = Vector3.Lerp(followingCamera.transform.position, NewCam, lerpCounter / lerpMaxTime);
        else NewPos = NewCam;
        
        followingCamera.transform.position = NewPos;

        followingCamera.transform.eulerAngles = new Vector3(followingCamera.transform.eulerAngles.x, transform.eulerAngles.y, followingCamera.transform.eulerAngles.z);

        if (lerpCounter >= lerpMaxTime) lerpCounter = 0.1f;
    }

    private void GetInput()
    {
        if (Input.GetButton("Dash"))
        {
            if (!isCharging)
            {
                particle.Play();
                isCharging = true;

                chargebar.GetComponent<MeshRenderer>().enabled = true;
                chargeBarMax.GetComponent<MeshRenderer>().enabled = true;
            }

            if (isDashRising)
            {
                currentDashForce = currentDashForce + (addDashSpeed * Time.deltaTime);
                if (!source.isPlaying)
                    source.PlayOneShot(chargeSound);
            }
            else
            {
                currentDashForce = currentDashForce - (addDashSpeed * Time.deltaTime);
                if (!source.isPlaying)
                    source.PlayOneShot(REVchargeSound);
                
            }

            if (currentDashForce >= maxDashForce)
            {
                isDashRising = false;
            }
            else if (currentDashForce <= 0)
            {
                isDashRising = true;
            }
        }
        else if (Input.GetButtonUp("Dash"))
        {
            Dash();
            source.Stop();
            source.Stop();
            source.PlayOneShot(dashSound);
            dashed = true;
        }

        else
        {
            transform.Rotate(Vector3.up * Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime);
            if (Input.GetAxis("Horizontal") != 0.0f) rotated = true;

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
