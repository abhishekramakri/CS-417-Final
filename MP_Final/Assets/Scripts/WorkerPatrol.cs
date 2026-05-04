using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class WorkerPatrol : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform[] waypoints;
    public float waypointReachDistance = 0.5f;

    NavMeshAgent _agent;
    int _currentWaypoint;
    bool _patrolling = true;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        if (_patrolling && waypoints != null && waypoints.Length > 0)
            _agent.SetDestination(waypoints[0].position);
    }

    void Update()
    {
        if (!_patrolling || waypoints == null || waypoints.Length == 0) return;

        if (!_agent.pathPending && _agent.remainingDistance < waypointReachDistance)
        {
            _currentWaypoint = (_currentWaypoint + 1) % waypoints.Length;
            _agent.SetDestination(waypoints[_currentWaypoint].position);
        }
    }

    public void SetWaypoints(Transform[] wps)
    {
        waypoints = wps;
        _currentWaypoint = 0;
        if (_agent != null && _patrolling && wps != null && wps.Length > 0)
            _agent.SetDestination(wps[0].position);
    }

    public void StopPatrol()
    {
        _patrolling = false;
        _agent.ResetPath();
    }

    public void ResumePatrol()
    {
        _patrolling = true;
        if (waypoints != null && waypoints.Length > 0)
            _agent.SetDestination(waypoints[_currentWaypoint].position);
    }
}
