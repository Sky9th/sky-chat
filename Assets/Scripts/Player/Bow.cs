using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    [SerializeField]
    private GameObject arrowPrefab;
    [SerializeField]
    private float force;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 bowPos = transform.position;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        Vector2 dir = mousePos - bowPos;
        Debug.DrawLine(bowPos, mousePos);
        transform.right = dir;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot(dir);
        }
    }

    void Shoot (Vector2 dir)
    {
        GameObject newArrow = Instantiate(arrowPrefab, transform.position, transform.rotation);
        newArrow.transform.Rotate(0, 0, -90);
        newArrow.GetComponent<Rigidbody2D>().velocity = transform.right * force;
    }
}
