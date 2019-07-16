using UnityEngine;
using System.Collections;

public class CrossAirFollow : MonoBehaviour {

	Vector2 mouse;
	int w = 32;
	int h = 32;
	public Texture2D cursor;

	// Use this for initialization
	void Start () {

		Cursor.visible = false;
	}
	
	// Update is called once per frame
	void Update () {

		mouse = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
	}

	void OnGUI()
	{
		GUI.DrawTexture(new Rect(mouse.x - (w / 2), mouse.y - (h ), w, h), cursor);
	}
}
