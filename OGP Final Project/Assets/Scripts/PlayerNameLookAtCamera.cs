using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerNameLookAtCamera : NetworkBehaviour
{
    public Camera _camera;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        //if (_camera == null)
        //    _camera = Camera.main;
        if (_camera == null)
            return;

        transform.LookAt(_camera.transform);
    }
}
