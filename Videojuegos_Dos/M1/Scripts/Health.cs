using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Unity.Netcode;

public class Health : NetworkBehaviour
{
    public const int totalHealth = 100;
    public NetworkVariable<int> currentHealth = new NetworkVariable<int>(totalHealth);
    public RectTransform healthBar;


    void Awake()
    {
        currentHealth.Value = totalHealth;
    }

    void ChangeHealth(int health)
    {
        healthBar.sizeDelta = new Vector2(health, healthBar.sizeDelta.y);
    }

    public void Damage(int amount)
    {
        if(!IsServer)
        {
            return;
        }
        currentHealth.Value -= amount;
        if(currentHealth.Value <= 0)
        {
            currentHealth.Value = totalHealth;
            RespawnClientRpc();
        }
    }

    public void AddHealth(int amount)
    {
        if(!IsServer)
        {
            return;
        }
        currentHealth.Value += amount;
    }

    [ClientRpc]
    void RespawnClientRpc()
    {
        if(IsLocalPlayer)
        {
            transform.position = Vector3.zero;
        }
    }
}
