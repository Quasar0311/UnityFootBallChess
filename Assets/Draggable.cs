using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    private Vector3 mOffset;

    private float mZCoord;

    private Vector3 yOffset = new Vector3(0,1f,0);
     
    Vector3 oldPosition;
    Vector3 newPosition;

    Vector3 currentVelocity = Vector3.zero;
    float smoothTime = 0.1f;

    void OnMouseDown()
    {
        oldPosition = this.transform.position;
        newPosition = this.transform.position + yOffset;

        Debug.Log("OnMouseDown");
        mZCoord = Camera.main.WorldToScreenPoint(
            gameObject.transform.position).z;

        // Store offset = gameobject world pos - mouse world pos
        mOffset = gameObject.transform.position - GetMouseAsWorldPoint();
    }

    private Vector3 GetMouseAsWorldPoint()
    {
        // Pixel coordinates of mouse (x,y)
        Vector3 mousePoint = Input.mousePosition;

        // z coordinate of game object on screen
        mousePoint.z = mZCoord;

        // Convert it to world points
        Vector3 convertedMouse = Camera.main.ScreenToWorldPoint(mousePoint);
        convertedMouse.y = 0;
        return convertedMouse;
    }

    void OnMouseDrag()
    {
        transform.position = GetMouseAsWorldPoint() + mOffset;
    }

    private void OnMouseUp() {
        oldPosition.y = 0;
        newPosition = oldPosition;
    }

    void Update()
    {
        this.transform.position = Vector3.SmoothDamp(this.transform.position, newPosition, ref currentVelocity, smoothTime);
    }
}