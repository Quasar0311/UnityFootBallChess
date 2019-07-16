using UnityEngine;
using System.Collections;

public class GRScript : MonoBehaviour {

	public MoveBall moveBall;
	public Vector3 initPos;

	public virtual void Awake(){

		initPos = transform.position;
	}

	// Use this for initialization
	void Start () {

		//StartCoroutine(MoveTo(new Vector3(2,0,0),0.3f));
	}
	
	// Update is called once per frame
	public virtual void Update () {

		//moveBall = GameObject.FindGameObjectWithTag("Ball").GetComponent<MoveBall>();
		//print(mo);
		//transform.position = new Vector3(Mathf.PingPong(Time.time, 1), transform.position.y, transform.position.z);
	}

	public void MoveGR(Vector3 initialPos, Vector3 calculatedPos){


	}

	public virtual IEnumerator MoveTo(Vector3 position, float time)
	{	
		//Vector3 initPos = transform.position;
		Vector3 end = position;
		float t = 0;
		
		while(t < 1)
		{
			yield return null;
			t += Time.deltaTime / time;
			transform.position = Vector3.Lerp(initPos, end, t);
		}
		transform.position = end;
	}
}
