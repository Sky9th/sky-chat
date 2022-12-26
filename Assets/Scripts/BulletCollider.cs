using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCollider : MonoBehaviour
{
    [SerializeField]
    private float height = 0;
    [SerializeField]
    private float xWidth = 0;

    private void Start()
    {
        gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
        if (xWidth == 0)
        {
            xWidth = gameObject.GetComponent<BoxCollider2D>().bounds.size.y;
        }
        if (height == 0)
        {
            height = gameObject.GetComponent<BoxCollider2D>().bounds.size.y - xWidth;
        }
    }

    private void FixedUpdate()
    {
        Debug.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y + xWidth), Color.blue);
        Debug.DrawLine(new Vector2(transform.position.x + 0.01f, transform.position.y + xWidth), new Vector2(transform.position.x + 0.01f, transform.position.y + xWidth + height), Color.yellow);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6 && collision.GetComponent<Arrow>())
        {
            float y = collision.GetComponent<Arrow>().startY;
            if (transform.position.y < y && y < transform.position.y + xWidth)
            {
                Debug.Log("over its header");
                collision.gameObject.SendMessage("getHit");
            }
        }
    }

}
