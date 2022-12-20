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

    float moveSpeed = 5f;

    Vector2 moveDir;
    Vector2 moveFinalDir;
    Vector2 newPos;

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

    private void onAllReceived(All data)
    {
        Dictionary<string, GameObject> onlinePlayerList = GameObject.Find("GameController").GetComponent<GameController>().onlinePlayerList;
        Dictionary<String, Player> playerList = data.playerList;
        foreach (KeyValuePair<string, Player> p in playerList)
        {
            if (p.Key == PlayerPrefs.GetString(Store.NETWORK_IDENTIFY)) continue;
            if (onlinePlayerList.ContainsKey(p.Key))
            {
                GameObject player;
                onlinePlayerList.TryGetValue(p.Key,out player);
                if (player && networkPlayer.ready && !p.Value.Equals(null))
                {
                    newPos = new Vector2(float.Parse(p.Value.positionX), float.Parse(p.Value.positionY));
                    moveDir = new Vector2(player.transform.position.x, player.transform.position.y) - newPos;
                    animator.SetFloat("Horizontal", moveDir.x);
                    animator.SetFloat("Vertical", moveDir.y);
                    animator.SetFloat("Speed", moveDir.sqrMagnitude);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!UIController.getIsInputChat() && networkPlayer.IsLocalPlayer())
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
        if (!networkPlayer.IsLocalPlayer())
        {
            transform.position = Vector2.Lerp(transform.position, newPos, 1f);
        } else
        {
            rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);
        }
    }
}
