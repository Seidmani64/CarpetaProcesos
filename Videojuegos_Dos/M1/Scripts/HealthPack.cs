using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Unity.Netcode;

public class HealthPack : NetworkBehaviour
{
    void OnCollisionEnter(Collision col)
    {
        GameObject hit = col.gameObject;
        Health currentHealth = hit.GetComponent<Health>();
        if(currentHealth != null)
        {
            currentHealth.AddHealth(5);
            Destroy(gameObject);
        }
    }
}
