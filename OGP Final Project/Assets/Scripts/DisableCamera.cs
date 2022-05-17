using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DisableCamera : NetworkBehaviour
{   
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
            GameObject.Find("MenuCamera").SetActive(false);
        if (!IsOwner)
        {
            gameObject.GetComponentInChildren<Camera>().enabled = false;
            gameObject.GetComponentInChildren<AudioListener>().enabled = false;
        }
    }
}
