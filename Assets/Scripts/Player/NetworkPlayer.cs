using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    public string networkIdentify;
    public bool ready = false;

    void Start()
    {
        StartCoroutine(setPlayerActive());
    }

    // Update is called once per frame
    void Update()
    {

    }
    private IEnumerator setPlayerActive()
    {
        yield return new WaitForSeconds(2);
        ready = true;
        if (IsLocalPlayer()) GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>().m_Follow = gameObject.transform;
    }

    public bool IsLocalPlayer ()
    {
        return networkIdentify == PlayerPrefs.GetString(Store.NETWORK_IDENTIFY);
    }
}
