using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowFlyController : MonoBehaviour
{

    public float force;
    public Vector2 dir;
    // Start is called before the first frame update

    private Rigidbody2D rb;
    private bool startFly = false;
    private float gravity = 100;

    private float flyTime = 0;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (force > 0 && dir.magnitude != 0)
        {
            rb.AddForce(dir * force, ForceMode2D.Force);
            force = 0;
            startFly = true;
        }
        if (startFly)
        {
            Debug.Log(new Vector2(0, -1) * gravity * Time.fixedDeltaTime);
            rb.AddForce(new Vector2(0, -1) * gravity * Time.fixedDeltaTime * flyTime, ForceMode2D.Force);
            flyTime += Time.fixedDeltaTime;
        }
    }
}
