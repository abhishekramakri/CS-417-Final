using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class WorkerEnemy : MonoBehaviour
{
    public enum WorkerState { Patrolling, AttackingDam }

    [Header("Dam Attack")]
    public Transform damTarget;
    public float alertMoveSpeed = 3.5f;
    public float reachDistance = 1.5f;
    public float damDamageAmount = 5f;
    public float damDamageInterval = 2f;

    [Header("Alert Visual")]
    public Renderer workerRenderer;
    public Color alertColor = new Color(1f, 0.3f, 0.3f);

    WorkerState _state = WorkerState.Patrolling;
    WorkerPatrol _patrol;
    NavMeshAgent _agent;
    float _damageTimer;

    void Awake()
    {
        _patrol = GetComponent<WorkerPatrol>();
        _agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (_state == WorkerState.AttackingDam)
            MoveAndAttackDam();
    }

    public void OnDetected()
    {
        if (_state == WorkerState.AttackingDam) return;
        _state = WorkerState.AttackingDam;
        _patrol?.StopPatrol();

        _agent.speed = alertMoveSpeed;

        if (damTarget != null)
            _agent.SetDestination(damTarget.position);

        if (workerRenderer != null)
            workerRenderer.material.color = alertColor;
    }

    void MoveAndAttackDam()
    {
        if (damTarget == null) return;

        float dist = Vector3.Distance(transform.position, damTarget.position);

        if (dist <= reachDistance)
        {
            _agent.ResetPath();
            _damageTimer += Time.deltaTime;
            if (_damageTimer >= damDamageInterval)
            {
                _damageTimer = 0f;
                // TODO: ForestIntegrityManager.Instance?.TakeDamage(damDamageAmount);
            }
        }
    }

    public WorkerState State => _state;
}
