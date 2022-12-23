using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShootController : MonoBehaviour
{
    [SerializeField]
    private GameObject arrorwPrefab;

    private Transform spawnObj;
    private GameObject arrow;
    [SerializeField]
    private float arrowForce = 50f;

    // Start is called before the first frame update
    void Start()
    {
        spawnObj = transform.Find("BulletSpawn").transform;
        createArrow();
    }

    void createArrow ()
    {
        arrow = Instantiate(arrorwPrefab, spawnObj.position, Quaternion.Euler(0, 0, -90));
        arrow.GetComponent<Rigidbody2D>().mass = 1;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Debug.Log(111);
            arrow.GetComponent<ArrowFlyController>().force = arrowForce;
            arrow.GetComponent<ArrowFlyController>().dir = new Vector2(1, 0);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.R)) {
            Destroy(arrow);
            createArrow();
        }
    }
}
