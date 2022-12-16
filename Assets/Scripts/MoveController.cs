using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public class MoveController : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer sprite;
    MainUIController UIController;

    float moveSpeed = 5f;

    Vector2 moveDir;
    Vector2 moveFinalDir;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        UIController = GameObject.Find("UIDocument").GetComponent<MainUIController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!UIController.getIsInputChat())
        {
            moveDir.x = Input.GetAxisRaw("Horizontal");
            moveDir.y = Input.GetAxisRaw("Vertical");

            if (moveDir.sqrMagnitude > 0)
            {
                moveFinalDir = moveDir;
                sprite.flipX = moveDir.x < 0;
            }

            animator.SetFloat("Horizontal", moveDir.x);
            animator.SetFloat("Vertical", moveDir.y);
            animator.SetFloat("Speed", moveDir.sqrMagnitude);
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);
    }
}
