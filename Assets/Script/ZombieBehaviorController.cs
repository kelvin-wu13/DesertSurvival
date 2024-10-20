using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieBehaviorController : MonoBehaviour
{
    [Header("Components")]
    private NavMeshAgent agent;
    private Animator animator;
    private Transform player;
    private HealthSystem healthSystem;

    [Header("Behavior Settings")]
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float wanderRadius = 10f;
    [SerializeField] private float idleTime = 3f;
    [SerializeField] private float attackDamage = 10f;
    

    [Header("Animation Parameters")]
    private readonly int IsWalkingParam = Animator.StringToHash("IsWalking");
    private readonly int IsAttackingParam = Animator.StringToHash("IsAttacking");
    private readonly int DieParam = Animator.StringToHash("Die");
    private readonly int SpeedParam = Animator.StringToHash("Speed");

    // State variables
    private bool isDead = false;
    private float nextAttackTime;
    private float idleTimer;
    private Vector3 startPosition;
    private ZombieState currentState;

    private enum ZombieState
    {
        Idle,
        Wandering,
        Chasing,
        Attacking
    }

    private void Start()
    {
        InitializeComponents();
        SetupInitialState();
    }

     private void InitializeComponents()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        healthSystem = GetComponent<HealthSystem>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        startPosition = transform.position;

    }

    private void SetupInitialState()
    {
        currentState = ZombieState.Idle;
        idleTimer = idleTime;
    }

    private void Update()
    {
        if (isDead || player == null) return;

        UpdateBehavior();
        UpdateAnimations();
    }

    private void UpdateBehavior()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange && CanAttack())
        {
            SetState(ZombieState.Attacking);
            PerformAttack();
        }
        else if (distanceToPlayer <= detectionRange)
        {
            SetState(ZombieState.Chasing);
            ChasePlayer();
        }
        else
        {
            HandleIdleAndWandering();
        }
    }

    private void HandleIdleAndWandering()
    {
        if (currentState == ZombieState.Idle)
        {
            idleTimer -= Time.deltaTime;
            if (idleTimer <= 0)
            {
                SetState(ZombieState.Wandering);
                SetNewWanderDestination();
            }
        }
        else if (currentState == ZombieState.Wandering)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                SetState(ZombieState.Idle);
                idleTimer = idleTime;
            }
        }
    }

    private void SetNewWanderDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += startPosition;
        
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1);
        agent.SetDestination(hit.position);
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private bool CanAttack()
    {
        return Time.time >= nextAttackTime;
    }

    private void PerformAttack()
    {
        transform.LookAt(player);
        agent.isStopped = true;
        
        animator.SetTrigger(IsAttackingParam);
        nextAttackTime = Time.time + attackCooldown;

        // Attack logic - you can implement this based on your game's requirements
        // For example: player.GetComponent<PlayerHealth>().TakeDamage(damageAmount);
    }

    private void SetState(ZombieState newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        
        switch (newState)
        {
            case ZombieState.Idle:
                agent.isStopped = true;
                break;
            case ZombieState.Wandering:
                agent.isStopped = false;
                agent.speed = 1f;
                break;
            case ZombieState.Chasing:
                agent.isStopped = false;
                agent.speed = 3f;
                break;
            case ZombieState.Attacking:
                agent.isStopped = true;
                break;
        }
    }

    private void UpdateAnimations()
    {
        animator.SetBool(IsWalkingParam, agent.velocity.magnitude > 0.1f);
        animator.SetFloat(SpeedParam, agent.velocity.magnitude);
    }

    public void HandleDeath()
    {
        if (isDead) return; // Prevent multiple calls

        isDead = true;
        animator.SetBool(DieParam, true);
        
        // Disable components
        agent.enabled = false;
        GetComponent<Collider>().enabled = false;
        
        // Optional: Destroy after animation
        Destroy(gameObject, 5f);
    }

    // Animation event methods (called from animation clips)
    public void OnAttackAnimationHit()
    {
        if (player != null && Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            HealthSystem playerHealth = player.GetComponent<HealthSystem>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}