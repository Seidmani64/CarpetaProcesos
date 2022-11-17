using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{

    [SerializeField] private GameObject explosionfx;
    [SerializeField] private Renderer render;
    [SerializeField] private float fuseTimer;
    [SerializeField] private float explosionDuration;
    [SerializeField] private BoxCollider explosionCol;
    private float elapsedTime;
    private SphereCollider originalCol;
    private Rigidbody rb;
    private bool hasExploded;
    

    void Awake()
    {
        elapsedTime = 0f;
        render = GetComponent<Renderer>();
        explosionCol = GetComponent<BoxCollider>();
        explosionCol.enabled = false;
        originalCol = GetComponent<SphereCollider>();
        rb = GetComponent<Rigidbody>();
        hasExploded = false;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        render.material.color = new Color(1, 1 - elapsedTime / fuseTimer, 1 - elapsedTime / fuseTimer);
        if(elapsedTime >= fuseTimer && !hasExploded)
        {
            hasExploded = true;
            Explode();
        }
            
    }

    void Explode()
    {
        explosionCol.enabled = true;
        originalCol.enabled = false;
        render.enabled = false;
        rb.isKinematic = true;
        GameObject deathvfx;
        Vector3 vfxpos = this.transform.position;
        vfxpos.y = this.transform.position.y + 1;
        deathvfx = Instantiate(explosionfx, vfxpos, Quaternion.identity);
        deathvfx.transform.localScale = new Vector3(3f, 3f, 3f);
        Destroy(deathvfx, explosionDuration);
        Destroy(gameObject, explosionDuration);
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "Player")
        {
            hasExploded = true;
            Explode();
        }
    }

}
