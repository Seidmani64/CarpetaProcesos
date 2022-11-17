using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Unity.Netcode;

//Testing github change

public class PlayerController : NetworkBehaviour
{
    public float speed = 5f;
    public float jumpForce = 5f;
    public GameObject groundCheck;
    public bool hasJumped = false;
    public bool isGrounded = true;
    Rigidbody rb;
    public GameObject bulletPrefab;
    public Transform spawnBullet;
    public float rotationSpeed = 0.5f;
    public float fireCooldown = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        if(IsLocalPlayer)
        {
            GetComponent<MeshRenderer>().material.color = Color.green;
            rb = GetComponent<Rigidbody>();
            Vector3 spawnPos = new Vector3(Random.Range(-5.0f, 5.0f), 0.0f, Random.Range(-5.0f, 5.0f));
            Quaternion spawnRot = Quaternion.Euler(0.0f, Random.Range(0.0f, 360.0f), 0.0f);
            gameObject.transform.position = spawnPos;
            gameObject.transform.rotation = spawnRot;
        }
    }

    // Update is called once per frame
    void Update()
    {
        fireCooldown -= Time.deltaTime;
        if(!IsLocalPlayer)
        {
            return;
        }
        float xMove = Input.GetAxisRaw("Horizontal"); 
        float zMove = Input.GetAxisRaw("Vertical");
        Vector3 input = new Vector3(xMove, 0, zMove).normalized;
        rb.velocity = new Vector3(input.x * speed, rb.velocity.y, input.z*speed);
        transform.Rotate(0, (Input.GetAxis("Mouse X") * rotationSpeed), 0, Space.World);
        if(Input.GetKeyDown(KeyCode.Space) && isGrounded && !hasJumped)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            hasJumped = true;
        }
        if(Input.GetKeyDown(KeyCode.Mouse0) && fireCooldown <= 0)
        {
            FireServerRpc();
            fireCooldown = 2.0f;
        }
    }

    [ServerRpc]
    void FireServerRpc()
    {
        GameObject bullet = GameObject.Instantiate(bulletPrefab, spawnBullet.position, spawnBullet.rotation);
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 5.0f;
        bullet.GetComponent<NetworkObject>().Spawn();
        
        
    }

}
