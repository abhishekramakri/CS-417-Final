using UnityEngine;

// Moves the worker between a list of waypoints in a loop.
// Other components call StopPatrol/ResumePatrol to interrupt for alert states.
public class WorkerPatrol : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform[] waypoints;
    public float moveSpeed = 2f;
    public float waypointReachDistance = 0.5f;

    int _currentWaypoint;
    bool _patrolling = true;

    void Update()
    {
        if (!_patrolling || waypoints == null || waypoints.Length == 0) return;
        MoveTowardsWaypoint();
    }

    void MoveTowardsWaypoint()
    {
        Transform target = waypoints[_currentWaypoint];
        Vector3 toTarget = target.position - transform.position;
        toTarget.y = 0f;
        Vector3 dir = toTarget.normalized;

        transform.position += dir * moveSpeed * Time.deltaTime;

        if (dir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(dir), 8f * Time.deltaTime);

        if (toTarget.magnitude < waypointReachDistance)
            _currentWaypoint = (_currentWaypoint + 1) % waypoints.Length;
    }

    public void StopPatrol() => _patrolling = false;
    public void ResumePatrol() => _patrolling = true;
}
