using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boost : MonoBehaviour {

    Rigidbody rb;
    float power = 15f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player")
        { 
        rb = other.GetComponent<Rigidbody>();
        var some = other.GetComponent<Player_Script>();

        
        
         rb.AddForce((some.direction + new Vector3(0,1,0))*power, ForceMode.Force);
         
        }
       
    }
}
