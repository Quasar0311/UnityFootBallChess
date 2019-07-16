using UnityEngine;
using System.Collections;

public class SwitchCamera : MonoBehaviour {

	public Camera flyCam;
	public Animation anim;
	//public

	// Use this for initialization
	void Start () {

		flyCam = GetComponent<Camera>();
		anim = GetComponent<Animation>();
		print("flyCam "+flyCam);
	}
	
	// Update is called once per frame
	void Update () {

		if(!anim.isPlaying){
			print("Ended playing");
			flyCam.enabled = false;
			//Camera.main.enabled = true;
		}
	}
}
