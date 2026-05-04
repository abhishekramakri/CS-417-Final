using UnityEngine;

public class WorkerPatrol : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform[] waypoints;
    public float moveSpeed = 2f;
    public float waypointReachDistance = 0.5f;

    [Header("Ground Snapping")]
    public LayerMask groundLayers;
    public float groundSnapOffset = 0f;

    int _currentWaypoint;
    bool _patrolling = true;

    void Start()
    {
        SnapToGround();
    }

    void Update()
    {
        if (!_patrolling || waypoints == null || waypoints.Length == 0) return;
        MoveTowardsWaypoint();
        SnapToGround();
    }

    void MoveTowardsWaypoint()
    {
        Transform target = waypoints[_currentWaypoint];
        Vector3 toTarget = target.position - transform.position;
        toTarget.y = 0f;
        float dist = toTarget.magnitude;

        if (dist < waypointReachDistance)
        {
            _currentWaypoint = (_currentWaypoint + 1) % waypoints.Length;
            return;
        }

        Vector3 dir = toTarget / dist;
        transform.position += dir * moveSpeed * Time.deltaTime;

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(dir),
            8f * Time.deltaTime);
    }

    void SnapToGround()
    {
        Vector3 origin = transform.position + Vector3.up * 2f;
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 10f, groundLayers))
            transform.position = hit.point + Vector3.up * groundSnapOffset;
    }

    public void SetWaypoints(Transform[] wps)
    {
        waypoints = wps;
        _currentWaypoint = 0;
    }

    public void StopPatrol()  => _patrolling = false;
    public void ResumePatrol() => _patrolling = true;
}
