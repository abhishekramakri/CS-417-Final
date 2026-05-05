using UnityEngine;

[RequireComponent(typeof(WorkerPatrol))]
public class WorkerEnemy : MonoBehaviour, IDamageable
{
    public enum WorkerState { Patrolling, AttackingDam }

    [Header("Health")]
    public float maxHealth = 100f;
    float _health;

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
    WorkerHealthBar _healthBar;
    float _damageTimer;

    void Awake()
    {
        _patrol = GetComponent<WorkerPatrol>();
        _healthBar = GetComponentInChildren<WorkerHealthBar>();
        _health = maxHealth;

        if (workerRenderer == null)
            workerRenderer = GetComponentInChildren<Renderer>();
    }

    void Start()
    {
        _healthBar?.SetFill(1f);
    }

    void Update()
    {
        if (_state == WorkerState.AttackingDam)
            MoveAndAttackDam();
    }

    public void TakeHit(float force)
    {
        _health -= force;
        _healthBar?.SetFill(_health / maxHealth);

        if (_health <= 0f)
            Die();
    }

    void Die()
    {
        Destroy(gameObject);
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

        Vector3 dir = damTarget.position - transform.position;
        dir.y = 0f;
        float dist = dir.magnitude;
        dir.Normalize();

        if (dist > reachDistance)
        {
            transform.position += dir * alertMoveSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(dir),
                10f * Time.deltaTime);

            Vector3 origin = transform.position + Vector3.up * 2f;
            if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 10f, _patrol.groundLayers))
                transform.position = new Vector3(transform.position.x, hit.point.y + _patrol.groundSnapOffset, transform.position.z);
        }
        else
        {
            _damageTimer += Time.deltaTime;
            if (_damageTimer >= damDamageInterval)
            {
                _damageTimer = 0f;
                // TODO: ForestIntegrityManager.Instance?.TakeDamage(damDamageAmount);
            }
        }
    }

    public WorkerState State => _state;
    public float HealthFraction => _health / maxHealth;
}
