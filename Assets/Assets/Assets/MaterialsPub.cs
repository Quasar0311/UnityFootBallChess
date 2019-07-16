using UnityEngine;
using System.Collections;

public class MaterialsPub : MonoBehaviour {

	public Material material1;
	public Material material2;
	float fadingTime = 2.0f;
	float timer;

	// Use this for initialization
	void Start () {

		//renderer.material = matFindmore;
		StartCoroutine(FadeMaterials(transform,material1,material2,fadingTime));
	}
	
	// Update is called once per frame
	void Update () {

//		timer += Time.deltaTime;
//		//var lerp = timer  / duration;
//
//		float lerp = Mathf.PingPong(timer, duration) / 3.0f; 
//		
//		renderer.material.SetFloat( "_Blend", lerp );

	}

	public static IEnumerator FadeMaterials(Transform obj,Material mat1,Material mat2,float fadeTime){
		//Assign both materials to the object
		Material[] fadeMats = new Material[2];
		fadeMats[0]=mat2;
		fadeMats[1]=mat1;
		obj.GetComponent<Renderer>().materials = fadeMats;
		
		Color mat0Color = obj.GetComponent<Renderer>().materials[0].color;
		Color mat1Color = obj.GetComponent<Renderer>().materials[1].color;
		
		//Set opacity for both materials to 1
		obj.GetComponent<Renderer>().materials[0].color = new Color(mat0Color.r,mat0Color.g,mat0Color.b,1);
		obj.GetComponent<Renderer>().materials[1].color = new Color(mat1Color.r,mat1Color.g,mat1Color.b,1);
		
		//Fade material to 0 opacity over fadeTime
		while(obj.GetComponent<Renderer>().materials[1].color.a>0){
			obj.GetComponent<Renderer>().materials[1].color = new Color(mat1Color.r,mat1Color.g,mat1Color.b,obj.GetComponent<Renderer>().materials[1].color.a-(Time.deltaTime/fadeTime));
			yield return null;
		}
		yield return new WaitForSeconds(1);

		//Remove old material from the object
		Material[] newMat = new Material[1];
		newMat[0]=mat2;
		obj.GetComponent<Renderer>().materials = newMat;

		fadeMats[0]=mat2;
		fadeMats[1]=mat1;
		obj.GetComponent<Renderer>().materials = fadeMats;
	}
}
