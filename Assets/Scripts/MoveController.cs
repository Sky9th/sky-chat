using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController : MonoBehaviour
{
    public GameObject player;

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            print("鼠标左键");
        }
        else if(Input.GetMouseButton(1))
        {
            print("鼠标右键");
            Vector3 mousePosition = Input.mousePosition;
            print(mousePosition);
            Ray ray = new Ray(mousePosition);
        }
    }
}
