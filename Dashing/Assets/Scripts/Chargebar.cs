using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chargebar : MonoBehaviour {

    Vector3 startScale;

	// Use this for initialization
	void Start () {
        startScale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void changeScale(float percentage)
    {
        transform.localScale = startScale + (new Vector3(0, 0, percentage * 0.9f));
    }

    public void resetScale()
    {
        transform.localScale = startScale;
    }
}
