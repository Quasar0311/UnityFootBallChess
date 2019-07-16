using UnityEngine;
using System.Collections;

public class TriggerDestroyBall : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other){

		if(other.GetComponent<Collider>().tag == "Ball")
			Destroy(other.gameObject, 2);
	}
}
