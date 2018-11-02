using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableEffect : MonoBehaviour {

    public ParticleSystem particle;

    // Use this for initialization
    void Start () {
        particle.Stop();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnDisable()
    {
        particle.Clear();
        particle.Play();
    }
}
