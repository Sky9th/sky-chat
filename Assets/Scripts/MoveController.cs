using RecEvent;
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
    NetworkPlayer networkPlayer;
    Player playerInfo;

    float moveSpeed = 5f;

    public Vector2 moveDir;
    public Vector2 moveFinalDir;
    public Vector2 newPos;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        UIController = GameObject.Find("UIDocument").GetComponent<MainUIController>();
        GameObject.Find("NetworkController").GetComponent<NetworkController>().AllReceived += onAllReceived;
        networkPlayer = GetComponent<NetworkPlayer>();
    }

    private void onAllReceived(All obj)
    {
        playerInfo = obj.playerList[networkPlayer.networkIdentify];
    }

    // Update is called once per frame
    void Update()
    {
        if (!UIController.getIsInputChat())
        {
            if (networkPlayer.IsLocalPlayer())
            {
                moveDir.x = Input.GetAxisRaw("Horizontal");
                moveDir.y = Input.GetAxisRaw("Vertical");
            } else
            {
                moveDir.x = playerInfo.moveDirX;
                moveDir.y = playerInfo.moveDirY;
            }

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
