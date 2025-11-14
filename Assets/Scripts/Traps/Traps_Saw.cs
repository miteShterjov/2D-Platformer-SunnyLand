using System;
using System.Collections;
using UnityEngine;

public class Traps_Saw : MonoBehaviour
{
    [Header("Saw Movement Settings")]
    [SerializeField] private int moveSpeed = 3;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private int currentWaypointIndex = 1;
    [SerializeField] private bool canMove = true;
    [Header("Saw Movement Options")]
    [SerializeField] private bool moveInCircle = true;
    [SerializeField] private bool moveBackAndForth = false;
    [SerializeField] private bool waitAtWaypoints = false;
    [SerializeField] private bool waitAtEndpointsOnly = false;
    [SerializeField] private float waitDuration = 0.5f;

    private static readonly int anim_param_active = Animator.StringToHash("isActive");
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector3[] waypointsPosition;

    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        ConvertWaypointsToPositions();
        transform.position = waypointsPosition[0];
    }

    void Update()
    {
        if (canMove)
        {
            MoveSaw();
            UpdateActiveAnim();
            UpdateSpriteDirection();
        }
    }

    private void OnValidate()
    {
        if (moveInCircle)
        {
            moveBackAndForth = false;
            waitAtWaypoints = true;
        }
        if (moveBackAndForth) moveInCircle = false;
    }

    private void MoveSaw()
    {
        Vector3 targetPosition = waypointsPosition[currentWaypointIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            if (waitAtWaypoints) StartCoroutine(WaitAtWaypoint());
            if (waitAtEndpointsOnly)
            {
                if (currentWaypointIndex == 0 || currentWaypointIndex == waypointsPosition.Length - 1)
                {
                    StartCoroutine(WaitAtWaypoint());
                }
            }

            if (currentWaypointIndex == waypointsPosition.Length - 1)
            {
                if (moveInCircle)
                {
                    currentWaypointIndex = 0;
                }
                else if (moveBackAndForth)
                {
                    Array.Reverse(waypointsPosition);
                    currentWaypointIndex = 1;
                }
            }
            else
            {
                currentWaypointIndex++;
            }
        }
    }


    private IEnumerator WaitAtWaypoint()
    {
        canMove = false;
        yield return new WaitForSeconds(waitDuration);
        canMove = true;
    }

    private void UpdateActiveAnim()
    {
        animator.SetBool(anim_param_active, canMove);
    }

    private void UpdateSpriteDirection()
    {
        if (waypointsPosition.Length < 2) return;

        Vector2 direction = waypointsPosition[currentWaypointIndex] - transform.position;
        if (direction.x > 0) spriteRenderer.flipX = true;
        else if (direction.x < 0) spriteRenderer.flipX = false;
    }

    private void ConvertWaypointsToPositions()
    {
        waypointsPosition = new Vector3[waypoints.Length];
        for (int i = 0; i < waypoints.Length; i++)
        {
            waypointsPosition[i] = waypoints[i].position;
            waypoints[i].gameObject.SetActive(false);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < waypointsPosition.Length; i++)
        {
            if (waypointsPosition[i] != null)
            {
                Gizmos.DrawSphere(waypointsPosition[i], 0.2f);
                if (i + 1 < waypoints.Length && waypointsPosition[i + 1] != null)
                {
                    Gizmos.DrawLine(waypointsPosition[i], waypointsPosition[i + 1]);
                }
                else if (i + 1 == waypointsPosition.Length && waypointsPosition[0] != null)
                {
                    Gizmos.DrawLine(waypointsPosition[i], waypointsPosition[0]);
                }
            }
        }

        Gizmos.color = Color.red;
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] != null)
            {
                Gizmos.DrawSphere(waypoints[i].position, 0.2f);
                if (i + 1 < waypoints.Length && waypoints[i + 1] != null)
                {
                    Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                }
                else if (i + 1 == waypoints.Length && waypoints[0] != null)
                {
                    Gizmos.DrawLine(waypoints[i].position, waypoints[0].position);
                }
            }
        }
    }
}
