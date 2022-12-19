using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    public string networkIdentify { get; set; }

    void Start()
    {
        Debug.Log(networkIdentify);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsLocalPlayer ()
    {
        return networkIdentify == PlayerPrefs.GetString(Store.NETWORK_IDENTIFY);
    }
}
