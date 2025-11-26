using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Platforms : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Transform[] wayPoints;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float dropTrapDelay = 0.8f;
    [SerializeField] private float dropTrapSpeed = 3f;
    [SerializeField] private float destroyDelay = 5f;
    [SerializeField] private float waitAtPointDuration = 1f;
    [SerializeField] private Transform scanner;
    [SerializeField] private float scanRange = 1f;
    [SerializeField] private Vector3[] wayPointsPositions;
    [SerializeField] private int currentWayPointIndex = 0;
    [Space]
    [SerializeField] private bool loopInCircle;
    [SerializeField] private bool moveBackwardsAtEnd;
    [SerializeField] private bool waitAtPoint;
    [SerializeField] private bool platformMoves;
    [SerializeField] private bool platformTrap;
    [SerializeField] private bool isWaiting; // add this field


    void Start()
    {
        ConvertTransformsToVectors();
    }

    void Update()
    {
        if (platformTrap && PlayerIsInRange()) StartPlatformTrap();
        if (platformMoves) MovePlatform();
    }

    private void MovePlatform()
    {
        if (isWaiting) return; // don't move while waiting

        transform.position = Vector3.MoveTowards(
            transform.position,
            wayPointsPositions[currentWayPointIndex],
            moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, wayPointsPositions[currentWayPointIndex]) < 0.01f)
        {
            if (waitAtPoint)
            {
                if (!isWaiting) StartCoroutine(WaitAtPointRoutine());
            }
            else
            {
                UpdateWaypointIndex();
            }
        }
    }

    private void UpdateWaypointIndex()
    {
        if (loopInCircle)
        {
            currentWayPointIndex++;
            if (currentWayPointIndex >= wayPointsPositions.Length)
            {
                currentWayPointIndex = 0;
            }
        }
        else if (moveBackwardsAtEnd)
        {
            if (currentWayPointIndex == wayPointsPositions.Length - 1)
            {
                Array.Reverse(wayPointsPositions);
                currentWayPointIndex = 1;
            }
            else if (currentWayPointIndex == 0)
            {
                Array.Reverse(wayPointsPositions);
                currentWayPointIndex = 1;
            }
            else
            {
                currentWayPointIndex++;
            }
        }
        else
        {
            currentWayPointIndex++;
            if (currentWayPointIndex >= wayPointsPositions.Length)
            {
                currentWayPointIndex = wayPointsPositions.Length - 1; // stay at last point
            }
        }
    }

    private IEnumerator WaitAtPointRoutine()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitAtPointDuration);
        UpdateWaypointIndex();
        isWaiting = false;
    }

    private void StartPlatformTrap()
    {
        StartCoroutine(PlatformTrapRoutine());
    }

    private IEnumerator PlatformTrapRoutine()
    {
        yield return new WaitForSeconds(dropTrapDelay);
        transform.Translate(Vector3.down * Time.deltaTime * dropTrapSpeed);
        Destroy(gameObject, destroyDelay);
    }

    private bool PlayerIsInRange()
    {
        Collider2D hit = Physics2D.OverlapCircle(scanner.position, scanRange, LayerMask.GetMask("Player"));
        return hit != null;
    }

    private void ConvertTransformsToVectors()
    {
        wayPointsPositions = new Vector3[wayPoints.Length];
        for (int i = 0; i < wayPoints.Length; i++)
        {
            wayPointsPositions[i] = wayPoints[i].position;
            wayPoints[i].gameObject.SetActive(false);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(scanner.position, scanRange);

        Gizmos.color = Color.green;
        if (wayPointsPositions != null && wayPointsPositions.Length > 0)
        {
            for (int i = 0; i < wayPointsPositions.Length; i++)
            {
                Gizmos.DrawSphere(wayPointsPositions[i], 0.2f);
                if (i < wayPointsPositions.Length - 1)
                {
                    Gizmos.DrawLine(wayPointsPositions[i], wayPointsPositions[i + 1]);
                }
                else if (loopInCircle)
                {
                    Gizmos.DrawLine(wayPointsPositions[i], wayPointsPositions[0]);
                }
            }
        }
    }
}
