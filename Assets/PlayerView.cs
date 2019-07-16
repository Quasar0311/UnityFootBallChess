using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerView : MonoBehaviour
{
    private void Start() {
        oldPosition = newPosition = this.transform.position;
    }
     
    private Vector3 mOffset;
    private float mZCoord;
    private Vector3 yOffset = new Vector3(0,1f,0);
    
    Vector3 oldPosition;
    Vector3 newPosition;

    Vector3 currentVelocity;
    float smoothTime = 0.1f;

    public void OnPlayerMoved( Hex oldHex, Hex newHex)
    {
        this.transform.position = oldHex.Position();
        newPosition = newHex.Position();
        currentVelocity = Vector3.zero;

    }

    void Update()
    {
        this.transform.position = Vector3.SmoothDamp(this.transform.position, newPosition, ref currentVelocity, smoothTime);
    }

    //     void OnMouseDown()
    // {
    //     Debug.Log("OnMouseDown");
    //     mZCoord = Camera.main.WorldToScreenPoint(
    //         gameObject.transform.position).z;
    // }

    // private Vector3 GetMouseAsWorldPoint()
    // {
    //     // Pixel coordinates of mouse (x,y)
    //     Vector3 mousePoint = Input.mousePosition;

    //     // z coordinate of game object on screen
    //     mousePoint.z = mZCoord;

    //     // Convert it to world points
    //     Vector3 convertedMouse = Camera.main.ScreenToWorldPoint(mousePoint);
    //     convertedMouse.z += convertedMouse.y;
    //     convertedMouse.x *= 1.15f;
    //     convertedMouse.y = 0;

    //     return convertedMouse;
    // }

    // void OnMouseDrag()
    // {
    //     newPosition = GetMouseAsWorldPoint()+yOffset;
    // }

    // private void OnMouseUp() {
    //     newPosition = oldPosition;      
    // }
}
