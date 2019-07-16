using UnityEngine;
using System.Collections;

public class GRSmarterScript : GRScript {

//	int area1_1 = 128;
//	int area1_2 = 384;
//	int area2_1 = 640;
//	int area2_2 = 896;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override IEnumerator MoveTo (Vector3 position, float time)
	{	
		//print("sweet spot "+moveBall.transform.position.x);

		return base.MoveTo (position, time);

	}
}
