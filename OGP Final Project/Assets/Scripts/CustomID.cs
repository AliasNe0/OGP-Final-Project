using UnityEngine;
using Unity.Netcode;

public class CustomID : NetworkBehaviour
{
    public NetworkVariable<float> playerID = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone);
}
