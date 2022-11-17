using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
	internal FlockController controller;
    private new Rigidbody rigidbody;
    private Vector3 randomize;

    public float avoidAngle;
    public float avoidDistance;

    public List<Vector3> vectors;
    public int avoidVectorIdx = -1;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        for(float i = 90; i <= 360+90-avoidAngle; i += avoidAngle)
        {
            Vector3 rayCheck = Vector3.zero;
            rayCheck.z = Mathf.Sin(Mathf.Deg2Rad*(i)) * avoidDistance;
            rayCheck.y = 0;
            rayCheck.x = Mathf.Cos(Mathf.Deg2Rad*(i)) * avoidDistance;
            vectors.Add(rayCheck);
        }
    }

    void Update()
    {
        foreach(Vector3 vector in vectors)
        {
            if(vectors.IndexOf(vector) == avoidVectorIdx)
                Debug.DrawRay(transform.position, vector, Color.red);
            else
                Debug.DrawRay(transform.position, vector, Color.green);
        }
        transform.LookAt(controller.target);
    }
    void FixedUpdate()
    {
        if (controller)
        {
            Vector3 relativePos = Steer() * Time.deltaTime;

            if(relativePos != Vector3.zero)
                rigidbody.velocity = relativePos;

            float speed = rigidbody.velocity.magnitude;

            if (speed > controller.maxVelocity)
                rigidbody.velocity = rigidbody.velocity.normalized * controller.maxVelocity;
                
            else if (speed < controller.minVelocity) 
                rigidbody.velocity = rigidbody.velocity.normalized * controller.minVelocity;
        }
    }

    //Calculate flock steering Vector based on the Craig Reynold's algorithm (Cohesion, Alignment, Follow leader and Seperation)
    private Vector3 Steer() 
    {
        Vector3 center = controller.flockCenter - transform.localPosition;          // cohesion
        Vector3 velocity = controller.flockVelocity - rigidbody.velocity;           // alignment
        Vector3 follow = controller.target.localPosition - transform.localPosition; // follow leader
        Vector3 separation = Vector3.zero; 											// separation
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 avoid = Vector3.zero;
        int layerMask =  1 << 6;
        RaycastHit rayHitStraight;
        RaycastHit rayHitSide;

        
        //Debug.DrawRay(transform.position, forward*50f, Color.red);
        if(Physics.Raycast(transform.position, vectors[0], out rayHitStraight, 50, layerMask))
        {
            foreach(Vector3 vector in vectors)
            {
                if(!(Physics.Raycast(transform.position, vector, out rayHitSide, 30, layerMask)))
                {
                    avoidVectorIdx = vectors.IndexOf(vector);
                    avoid = ((vector + rayHitStraight.normal)*controller.repulsionWeight);
                    break;
                }
            }
        }
        

        

        foreach (Flock flock in controller.flockList) 
        {
            if (flock != this) 
            {
                Vector3 relativePos = transform.localPosition - flock.transform.localPosition;
                separation += relativePos.normalized;
            }
        }

        // Randomize the direction every 2 seconds
        if(Time.time % 2==0)
        {
            randomize = new Vector3((Random.value * 2) - 1, (Random.value * 2) - 1, (Random.value * 2) - 1);
            randomize.Normalize();
        }


        return (controller.centerWeight * center +
                controller.velocityWeight * velocity +
                controller.separationWeight * separation +
                controller.followWeight * follow +
                controller.randomizeWeight * randomize +
                controller.avoidWeight * avoid);
    }	
}
