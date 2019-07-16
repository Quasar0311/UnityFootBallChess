﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraKeyboardController : MonoBehaviour
{
    float moveSpeed = 10;

    // Update is called once per frame
    void Update()
    {
        Vector3 translate = new Vector3
            (
                Input.GetAxis("Horizontal"),
                0,
                Input.GetAxis("Vertical")
            );

        this.transform.Translate( translate * moveSpeed * Time.deltaTime, Space.World);
    }
}
