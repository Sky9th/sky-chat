using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(PlayerPrefs.GetString(Store.SESSION_KEY));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
