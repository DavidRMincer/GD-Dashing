using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour {

	public GameObject toEnable;
	public GameObject toRotate;
    private ParticleSystem particle;
    public bool isOn = false;
    public bool onlyActivableOnce = true;
    public bool wasActivated = false;
	
	// Use this for initialization
	void Start () {
        particle = this.GetComponent<ParticleSystem>();
        particle.Stop();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnCollisionEnter(Collision collision)
	{ 
        if (collision.gameObject.tag == "Player")
        {
            particle.Play();
            if (onlyActivableOnce && !wasActivated || !onlyActivableOnce)
            {
                isOn = !isOn;
                toEnable.SetActive(!toEnable.activeSelf);
                toRotate.transform.rotation = Quaternion.Euler(0, 0, -toRotate.transform.eulerAngles.z);
                wasActivated = true;
            }
        }
	}
}
