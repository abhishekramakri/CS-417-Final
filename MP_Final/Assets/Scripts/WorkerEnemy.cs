using UnityEngine;

// Companion to WorkerPatrol and VisionCone.
// When VisionCone detects the player, OnDetected() switches this worker to
// attack mode: it stops patrolling and walks straight to the dam to deal damage.
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
    float _damageTimer;

    void Awake()
    {
        _patrol = GetComponent<WorkerPatrol>();
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

        if (workerRenderer != null)
            workerRenderer.material.color = alertColor;
    }

    void MoveAndAttackDam()
    {
        if (damTarget == null) return;

        Vector3 dir = (damTarget.position - transform.position);
        dir.y = 0f;
        float dist = dir.magnitude;
        dir.Normalize();

        if (dist > reachDistance)
        {
            transform.position += dir * alertMoveSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(dir), 10f * Time.deltaTime);
        }
        else
        {
            _damageTimer += Time.deltaTime;
            if (_damageTimer >= damDamageInterval)
            {
                _damageTimer = 0f;
                // ForestIntegrityManager is Chris's component; null-safe call
                ForestIntegrityManager.Instance?.TakeDamage(damDamageAmount);
            }
        }
    }

    public WorkerState State => _state;
}
