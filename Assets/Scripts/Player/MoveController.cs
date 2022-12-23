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
    Player playerInfo = null;

    float moveSpeed = 5f;

    public Vector2 recMoveDir;
    public Vector2 sendMoveDir;
    public Vector2 moveFinalDir;
    public Vector2 syncDir;
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
        if (obj.playerList.ContainsKey(networkPlayer.networkIdentify)) playerInfo = obj.playerList[networkPlayer.networkIdentify];
    }

    // Update is called once per frame
    void Update()
    {
        if (!UIController.getIsInputChat() && playerInfo != null)
        {
            sendMoveDir.x = Input.GetAxisRaw("Horizontal");
            sendMoveDir.y = Input.GetAxisRaw("Vertical");

            recMoveDir.x = playerInfo.moveDirX;
            recMoveDir.y = playerInfo.moveDirY;
            newPos = new Vector2(playerInfo.positionX, playerInfo.positionY);

            syncDir = new Vector2(transform.position.x, transform.position.y) - newPos;
            
            if (recMoveDir.sqrMagnitude > 0 && sendMoveDir.x != 0)
            {
                moveFinalDir = recMoveDir;
                sprite.flipX = recMoveDir.x < 0;
            }
            animator.SetFloat("Horizontal", recMoveDir.x);
            animator.SetFloat("Vertical", recMoveDir.y);
            animator.SetFloat("Speed", recMoveDir.sqrMagnitude);
        }
    }

    private void FixedUpdate()
    {
        if (syncDir.magnitude > moveSpeed * 0.2f)
        {
            transform.position = Vector2.Lerp(transform.position, newPos, 1f);
        }
        rb.MovePosition(rb.position + recMoveDir * moveSpeed * Time.fixedDeltaTime);
    }
}
