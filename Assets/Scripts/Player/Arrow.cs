using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 startPos;
    private float groudY;
    private bool isHit;
    private Transform shadow;

    public float height;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        startPos = transform.position;
        groudY = startPos.y - height;
        shadow = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isHit)
        {
            /*float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);*/
            transform.rotation = Quaternion.FromToRotation(Vector2.right, rb.velocity.normalized);

            shadow.transform.position = new Vector2(transform.position.x, groudY);
            shadow.transform.right = Vector2.right;

            float c = gameObject.GetComponent<SpriteRenderer>().bounds.size.x;
            float z = transform.rotation.eulerAngles.z;
            float b = 0;
            float angle = 0;
            if (0 < z && z < 90)
            {
                angle = 90 - z;
                b = Mathf.Sin(angle * Mathf.Deg2Rad) * c;
            } else
            {
                angle = 360 - z;
                b = Mathf.Cos(angle * Mathf.Deg2Rad) * c;
            }
            shadow.localScale = new Vector2(b / c, 1);
        }

        if (transform.position.y - groudY <= 0)
        {
            isHit = true;
        }

        if (isHit)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.tag);
        if (collision.tag == "Fence")
        {
            Debug.Log("Fence");
            if (transform.position.y - groudY < 0.5)
            {
                isHit = true;
            }
        }

    }
}
