using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NavMeshAgent))]
public class VisitorNavMeshSetup : MonoBehaviour
{
    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        // Prevent the pixel-art character from rotating.
        agent.updateRotation = false;
    }

    private void Update()
    {
        // Press P to check the visitor's navigation.
        if (Keyboard.current == null ||
            !Keyboard.current.pKey.wasPressedThisFrame)
        {
            return;
        }

        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning("Visitor is not on the NavMesh!");
            return;
        }

        Debug.Log("Visitor Path Status: " + agent.pathStatus);
        Debug.Log("Visitor Has Path: " + agent.hasPath);
        Debug.Log("Visitor Path Pending: " + agent.pathPending);
        Debug.Log("Visitor Remaining Distance: " + agent.remainingDistance);
        Debug.Log("Visitor Destination: " + agent.destination);
        Debug.Log("Visitor Position: " + transform.position);

        GameObject tank = GameObject.Find("Tank_3");

        if (tank != null)
        {
            Transform standPoint =
                tank.transform.Find("VisitorStandPoint");

           
            if (standPoint != null)
            {
                Debug.Log("Tank 3 StandPoint Position: " +
                          standPoint.position);

                // Check whether the standing point is near the NavMesh.
                bool foundNavMesh = NavMesh.SamplePosition(
                    standPoint.position,
                    out NavMeshHit hit,
                    0.75f,
                    NavMesh.AllAreas
                );

                Debug.Log("Tank 3 StandPoint near NavMesh: " + foundNavMesh);

                if (foundNavMesh)
                {
                    Debug.Log("Nearest NavMesh Position: " + hit.position);

                    // Check whether the visitor can reach that point.
                    NavMeshPath testPath = new NavMeshPath();

                    bool pathFound = agent.CalculatePath(
                        hit.position,
                        testPath
                    );

                    Debug.Log("Path to Tank 3 possible: " + pathFound);
                    Debug.Log("Path to Tank 3 status: " + testPath.status);
                }
            }

        }
    }
}