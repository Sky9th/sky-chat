using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCenter 
{
    private static PlayerCenter playerCenter;
    private static GameObject player;

    public static PlayerCenter getInstance ()
    {
        if (playerCenter == null)
        {
            Debug.Log("Game Center init");
            GameObject[] playerList = GameObject.FindGameObjectsWithTag("Player");
            for(int i = 0; i < playerList.Length; i++)
            {
                if (playerList[i].GetComponent<NetworkPlayer>().networkIdentify == PlayerPrefs.GetString(Store.NETWORK_IDENTIFY))
                {
                    player = playerList[i];
                }
            }
        }
        return playerCenter;
    }

    public static Vector2 position
    {
        get
        {
            return player.transform.position;
        }
    }

    public static float height
    {
        get {
            return player.GetComponent<SpriteRenderer>().bounds.size.y;
        }
    }

    public static float width
    {
        get { 
            return player.GetComponent<SpriteRenderer>().bounds.size.x;
        } 
    }


}
