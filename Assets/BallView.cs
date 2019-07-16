using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BallView : MonoBehaviour
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

    public void OnBallMoved( Hex oldHex, Hex newHex)
    {
        this.transform.position = oldHex.Position();
        newPosition = newHex.Position();
        currentVelocity = Vector3.zero;

    }

    void Update()
    {
        this.transform.position = Vector3.SmoothDamp(this.transform.position, newPosition, ref currentVelocity, smoothTime);
    }
}
