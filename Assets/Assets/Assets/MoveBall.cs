using UnityEngine;
using System.Collections;

public class MoveBall : MonoBehaviour {
	
	public float force;
	public Transform hitParticle;
	public AudioClip clip;
	private Vector3 distVector;
	public GameObject GR;
	public GRScript grScript;
	private bool isHit;
	
	// Use this for initialization
	void Start () {

		GR = GameObject.FindGameObjectWithTag("GR");
		//rigidbody.AddForce(Random.Range(3,4),Random.Range(3,4),0,ForceMode.Impulse);
		grScript = GR.GetComponent<GRScript>();
		if(this != null)
			Destroy(gameObject,6);
		isHit = false;
	}
	
	// Update is called once per frame
	void Update () {

		//print("sweet spot "+ this.transform.position.x);
//		if(GameObject.FindGameObjectWithTag("Ball") == null){
//			
//			StartCoroutine(grScript.MoveTo(grScript.initPos,0.2f));
//		}
	}
	
	void OnMouseDown(){
		
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;


		if(Physics.Raycast(ray, out hit, 100) && !isHit){

			isHit = true;
			distVector = (transform.position - hit.point).normalized;
			distVector.y = Mathf.Clamp(distVector.y,-0.1f,0.26f);
			distVector.x = Mathf.Clamp(distVector.x,-0.2f,0.2f);
			//print("Distancia clamped "+distVector);
			GetComponent<Rigidbody>().AddForceAtPosition(distVector*force, hit.point, ForceMode.Impulse);
			GetComponent<AudioSource>().PlayOneShot(clip);
			Instantiate(hitParticle,hit.point,Quaternion.identity);
			CalculateKeeper();
			//StartCoroutine(grScript.MoveTo(grScript.initPos,0.2f));
			Debug.DrawRay(ray.origin,ray.direction,Color.cyan);
		}
	}

	void CalculateKeeper(){

//		if(this.transform.position.x > -3.0f && this.transform.position.x < -1.5f || this.transform.position.x > 0.0f && this.transform.position.x < 1.5f){
//			
//			print("sweet spot");
//		}

		if(IsPositive(distVector.x)){

			if(this.transform.position.x > 0.0f && this.transform.position.x < 1.5f && distVector.x >= -0.2f && distVector.x <= 0.2f){
				print("sweet spot");
				StartCoroutine(grScript.MoveTo(new Vector3(grScript.initPos.x+1.8f,grScript.initPos.y,grScript.initPos.z),0.2f));
			}else{

			if(distVector.x > 0 && distVector.x <= 0.1f) 
				StartCoroutine(grScript.MoveTo(new Vector3(grScript.initPos.x+0.5f,grScript.initPos.y,grScript.initPos.z),0.2f));
			if(distVector.x > 0.1f && distVector.x <= 0.2f) 
				StartCoroutine(grScript.MoveTo(new Vector3(grScript.initPos.x+1.0f,grScript.initPos.y,grScript.initPos.z),0.3f));
			if(distVector.x > 0.2f && distVector.x <= 0.3f) 
				StartCoroutine(grScript.MoveTo(new Vector3(grScript.initPos.x+1.5f,grScript.initPos.y,grScript.initPos.z),0.4f));
			}
		}else if(IsNegative(distVector.x)){

			if(this.transform.position.x > -3.0f && this.transform.position.x < 0.0f && distVector.x >= -0.2f && distVector.x <= 0.2f){
				print("sweet spot2");
				StartCoroutine(grScript.MoveTo(new Vector3(grScript.initPos.x-1.8f,grScript.initPos.y,grScript.initPos.z),0.2f));
			}else{

			if(distVector.x < 0 && distVector.x >= -0.1f) 
				StartCoroutine(grScript.MoveTo(new Vector3(grScript.initPos.x-0.5f,grScript.initPos.y,grScript.initPos.z),0.2f));
			if(distVector.x < -0.1f && distVector.x >= -0.2f) 
				StartCoroutine(grScript.MoveTo(new Vector3(grScript.initPos.x-1.0f,grScript.initPos.y,grScript.initPos.z),0.32f));
			if(distVector.x < -0.2f && distVector.x >= -0.3f) 
				StartCoroutine(grScript.MoveTo(new Vector3(grScript.initPos.x-1.5f,grScript.initPos.y,grScript.initPos.z),0.4f));
		}
		}
	}

	public static bool IsPositive(float number)
	{
		return number > 0;
	}
	
	public static bool IsNegative(float number)
	{
		return number < 0;
	}
}
