using UnityEngine;
using Unity.Netcode;

public class CustomID : NetworkBehaviour
{
    public NetworkVariable<float> id = new NetworkVariable<float>();
}
