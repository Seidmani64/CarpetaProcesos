using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

namespace Enemies
{
    /// <summary>
    /// Class of the vacuum enemy that inherits Enemy behaviour.
    /// </summary>
    public class ConductorEnemy : Enemy
    {
        [SerializeField] private GameObject bombPrefab;
        [SerializeField] private float throwStrength;
        [SerializeField] private GameObject fragment;
        // ----------------------------------------------------------------------------------------------- Unity Methods
        void Awake()
        {
            // Initialize private components
            player = GameObject.FindWithTag("Player").transform;
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            GameObject manager = GameObject.FindWithTag("Manager");
            if(manager!=null)
                hitStop = manager.GetComponent<HitStop>();
        }

        void Update()
        {
            if (!GameManager.isOnline || PhotonNetwork.IsMasterClient)
            {
                dazeTime -= Time.deltaTime;
                invFrames -= Time.deltaTime;
                if(invFrames <= 0f)
                    shield.SetActive(false);
                player = GameManager.GetClosestTarget(transform).transform;
                    if (player == null)
                        player = transform;

                
                playerInSights = Physics.CheckSphere(transform.position, sightRange, isPlayer);
                playerInRange = Physics.CheckSphere(transform.position, attackRange, isPlayer);

                if(playerInSights && !playerInRange)
                    Chasing();
                else if(playerInSights && playerInRange)
                    Attacking();
            }
        }

        void OnDestroy()
        {
            fragment.SetActive(true);
            fragment.transform.position =  new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
        }

        public override void Attacking()
        {
            Vector3 targetPosition = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
            transform.LookAt(targetPosition);

            if(!hasAttacked)
            {
                animator.SetTrigger("Attack");
                GameObject bomb;
                Vector3 bombpos = this.transform.position;
                bombpos.y = this.transform.position.y + 3;
                bomb = Instantiate(bombPrefab, bombpos, Quaternion.identity);
                Vector3 targetDirection = player.transform.position - bomb.transform.position;
                Vector3 throwOffset = new Vector3(Random.Range(-3f,3f), Random.Range(-3f,3f), Random.Range(-3f, 3f));
                float strengthOffset = targetDirection.magnitude/2;
                bomb.GetComponent<Rigidbody>().velocity = (targetDirection + throwOffset).normalized * (throwStrength * strengthOffset);
                hasAttacked = true;
                Invoke(nameof(ResetAttack), attackSpeed);
            }
        }
    }
}
