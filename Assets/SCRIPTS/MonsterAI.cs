using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    [Header("Vision Settings")]
    public float visionRange = 10f;
    public float attackRange = 2f;
    public float fieldOfView = 60f;
    public float proximityRange = 4f;
    
    [Header("Hearing Settings")]
    public float hearingRange = 15f;
    public float hearingMemoryTime = 5f;
    
    [Header("Patrol Settings")]
    public float patrolRadius = 20f;
    public float waitTimeAtPoint = 2f;
    public float patrolSpeed = 2f;
    
    [Header("Components")]
    public Transform player;
    private NavMeshAgent agent;
    private Vector3 currentPatrolPoint;
    private float waitTimer = 0f;
    private bool isWaiting = false;
    private bool isChasing = false;
    private float chaseTimer = 0f;
    private float chaseDuration = 3f;
    
    private Vector3 lastHeardPosition;
    private float timeSinceLastSound = 999f;
    private bool hasHeardSound = false;
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
        
        GetNewPatrolPoint();
    }
    
    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool canSeePlayer = CanSeePlayer();
        bool isPlayerClose = distanceToPlayer <= proximityRange;
        
        if (isPlayerClose || canSeePlayer)
        {
            TurnTowardsPlayer();
            
            if (distanceToPlayer <= attackRange)
            {
                Attack();
            }
            else if (canSeePlayer)
            {
                isChasing = true;
                chaseTimer = chaseDuration;
                agent.isStopped = false;
                agent.SetDestination(player.position);
            }
            else
            {
                isChasing = false;
                agent.isStopped = true;
            }
        }
        else if (hasHeardSound && timeSinceLastSound < hearingMemoryTime)
        {
            isChasing = true;
            agent.isStopped = false;
            agent.SetDestination(lastHeardPosition);
            
            if (Vector3.Distance(transform.position, lastHeardPosition) < 1f)
            {
                hasHeardSound = false;
                isChasing = false;
            }
        }
        else
        {
            isChasing = false;
            Patrol();
        }
        
        if (hasHeardSound)
        {
            timeSinceLastSound += Time.deltaTime;
            if (timeSinceLastSound >= hearingMemoryTime)
                hasHeardSound = false;
        }
    }
    
    void Patrol()
    {
        agent.speed = patrolSpeed;
        
        if (!agent.hasPath || agent.remainingDistance < 0.5f)
        {
            if (!isWaiting)
            {
                isWaiting = true;
                waitTimer = waitTimeAtPoint;
                agent.isStopped = true;
            }
            else
            {
                waitTimer -= Time.deltaTime;
                if (waitTimer <= 0f)
                {
                    GetNewPatrolPoint();
                    isWaiting = false;
                    agent.isStopped = false;
                }
            }
        }
        else
        {
            agent.isStopped = false;
        }
    }
    
    void GetNewPatrolPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;
        
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas))
        {
            currentPatrolPoint = hit.position;
            agent.SetDestination(currentPatrolPoint);
        }
        else
        {
            GetNewPatrolPoint();
        }
    }
    
    void TurnTowardsPlayer()
    {
        agent.isStopped = true;
        
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0;
        
        if (directionToPlayer != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }
    
    bool CanSeePlayer()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > visionRange) return false;
        
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        if (angle > fieldOfView / 2) return false;
        
        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToPlayer, out hit, visionRange))
        {
            if (hit.collider.CompareTag("Player"))
                return true;
        }
        return false;
    }
    
    public void HearSound(Vector3 soundPosition, float soundLoudness = 1f)
    {
        float effectiveRange = hearingRange * soundLoudness;
        float distance = Vector3.Distance(transform.position, soundPosition);
        
        if (distance <= effectiveRange && !isChasing)
        {
            lastHeardPosition = soundPosition;
            timeSinceLastSound = 0f;
            hasHeardSound = true;
            Debug.Log("Monster heard sound at distance: " + distance);
        }
    }
    
    void Attack()
    {
        Debug.Log("Monster attacks!");
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, hearingRange);
        
        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawWireSphere(transform.position, proximityRange);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(currentPatrolPoint, 0.5f);
    }
}