using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Script : MonoBehaviour
{
    public GameObject directionalArrow,
        directionPivot;
    public Camera followingCamera;
    public ParticleSystem dust;
    public float dashForce,
        rotationSpeed,
        cameraSpeed,
		cameraShakeDuration,
		cameraShakeIntensity;
    public Vector3 direction;

    private Rigidbody rb;
    private bool moving;
    private Vector3 cameraOffset;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        moving = false;
        cameraOffset = followingCamera.transform.position - transform.position;
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
