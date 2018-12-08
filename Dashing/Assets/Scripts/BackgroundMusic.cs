using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour {

    [SerializeField]
    private AudioClip track1;
    [SerializeField]
    private AudioClip track2;
    [SerializeField]
    private AudioClip track3;

    [SerializeField]
    private AudioSource source;

	// Use this for initialization
	void Start () {
        source.GetComponent<AudioSource>();

         int rand = Random.Range(0, 100);
        if (0 <= rand && rand <= 33) rand = 0;
        else if (33 < rand && rand <= 66) rand = 1;
        else if (66 < rand && rand <= 99) rand = 2;
        switch (rand)
        {
            case 0: source.clip = track1;
                source.Play();
                break;
            case 1:
                source.clip = track2;
                source.Play();
                break;
            case 2:
                source.clip = track3;
                source.volume = .5f;
                source.Play();
                break;
            default: source.clip = track1;
                source.Play();
                break;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
