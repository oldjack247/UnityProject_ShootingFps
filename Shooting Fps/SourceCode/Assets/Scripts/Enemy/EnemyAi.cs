using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent agent;
    public MeshRenderer meshRenderer;
    public Transform player;
    public LayerMask whatIsGround;
    public LayerMask whatIsPlayer;
    public float health;
    public float dissolveRate;
    public float refreshRate;
    public GameManager gameManager;
    public GameObject[] enemies;

    [Header("Patroling")]
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    [Header("Attacking")]
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;
    public Transform attackPoint;

    [Header("States")]
    public float sightRange;
    public float attackRange;
    public bool playerInSightRange;
    public bool playerInAttackRange;
    private Material[] meshMats;
    private GenerateEnemies generateEnemies;


    private void Start()
    {
        player = GameObject.Find("PlayerObj").transform;
        generateEnemies = GetComponent<GenerateEnemies>();
        agent = GetComponent<NavMeshAgent>();

        if (meshRenderer != null)
            meshMats = meshRenderer.materials;
    }

    private void Update()
    {
        //check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange)
            Patroling();

        if (playerInSightRange && !playerInAttackRange)
            ChasePlayer();

        if (playerInAttackRange && playerInSightRange)
            AttackPlayer();

        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if(enemies.Length == 0)
        {
            FindObjectOfType<GameManager>().CompleteLevel();
        }
    }

    private void Patroling()
    {
        if (!walkPointSet)
            SearchWalkPoint();

        if(walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 directionToWalkPoint = transform.position - walkPoint;

        //walkpoint reached
        if (directionToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        //calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        //make enemy stop moving
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if(!alreadyAttacked)
        {
            //attack code
            Rigidbody rb = Instantiate(projectile, attackPoint.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * 40f, ForceMode.Impulse);
            rb.AddForce(transform.up, ForceMode.Impulse);

            //end of attack
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            StartCoroutine(DissolveCo());
            Invoke(nameof(DestroyEnemy), 0.5f);
        }           
    }

    public void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    IEnumerator DissolveCo()
    {
        if (meshMats.Length > 0)
        {
            float counter = 0;
            while (meshMats[0].GetFloat("_DissolveAmount") < 1)
            {
                counter += dissolveRate;
                for (int i = 0; i < meshMats.Length; i++)
                {
                    meshMats[i].SetFloat("_DissolveAmount", counter);
                }

                yield return new WaitForSeconds(refreshRate);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

}
