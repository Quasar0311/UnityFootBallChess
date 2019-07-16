﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{
    public Animator animator;
    public Rigidbody rigidbody;


    private float h;
    private float v;

    private float moveX;
    private float moveZ;
    private float speedH = 50f;
    private float speedZ = 80f;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.Play("SLIDE00", -1, 0);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            animator.Play("JUMP00", -1, 0);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            animator.Play("DamageDown", -1, 0);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            animator.Play("Jab", -1, 0);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            animator.Play("Hikick", -1, 0);
        }

        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        animator.SetFloat("h", h);
        animator.SetFloat("v", v);

        moveX = h * speedH * Time.deltaTime;
        moveZ = v * speedZ * Time.deltaTime;

        if (moveZ <= 0)
        {
            moveX = 0;
        }

        rigidbody.velocity = new Vector3(moveX, 0, moveZ);

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "Cube")
        {
            Debug.Log("충돌 감지");
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if(collision.collider.tag == "Cube")
        {
            Debug.Log("충돌 유지");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.collider.tag == "Cube")
        {
            Debug.Log("충돌 종료");
        }
    }


}
