using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class button : MonoBehaviour {

    public GameObject toEnable;
    public bool isOn = false;
    public Material onMaterial;
    public Material offMaterial;
    public Light light;
    public float pingMax;
    public float speedLight = 1;

    private MeshRenderer meshRenderer;
    private Color offColor;
    private Color onColor;
    private Color emissionColor;

    // Use this for initialization
    void Start ()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        emissionColor = meshRenderer.material.color;
        offColor = offMaterial.color;
        onColor = onMaterial.color;
    }
	
	// Update is called once per frame
	void Update ()
    {
        light.intensity = Mathf.PingPong(Time.time * speedLight * pingMax, pingMax);

        if (isOn)
        {
            emissionColor.r = Mathf.PingPong(Time.time * speedLight * onColor.r, onColor.r);
            emissionColor.g = Mathf.PingPong(Time.time * speedLight * onColor.g, onColor.g);
            emissionColor.b = Mathf.PingPong(Time.time * speedLight * onColor.b, onColor.b);
        }
        else
        {
            emissionColor.r = Mathf.PingPong(Time.time * speedLight * offColor.r, offColor.r);
            emissionColor.g = Mathf.PingPong(Time.time * speedLight * offColor.g, offColor.g);
            emissionColor.b = Mathf.PingPong(Time.time * speedLight * offColor.b, offColor.b);
        }
        meshRenderer.material.SetColor("_EmissionColor", emissionColor);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            isOn = !isOn;

            if (isOn)
            {
                meshRenderer.material = onMaterial;
                light.color = onMaterial.color;
            }
            else
            {
                meshRenderer.material = offMaterial;
                light.color = offMaterial.color;
            }

            emissionColor = meshRenderer.material.color;
            toEnable.SetActive(!toEnable.activeSelf);
        }
    }
}
