using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spike : MonoBehaviour {

    public ParticleSystem particle;

    // Use this for initialization
    void Start () {
        particle = this.GetComponent<ParticleSystem>();
        particle.Stop();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnEnable()
    {
        particle = this.GetComponent<ParticleSystem>();
        particle.Stop();
    }
	
	void OnCollisionEnter(Collision collision)
	{

        if (collision.gameObject.tag == "Player")
        {
            particle.Play();
            Destroy(collision.gameObject);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
	}
}
