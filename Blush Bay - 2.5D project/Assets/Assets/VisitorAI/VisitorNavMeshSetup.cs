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

        // Keep the pixel-art visitor facing the camera.
        agent.updateRotation = false;
    }

    private void Update()
    {
        // Press P to check the exit navigation.
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

        GameObject exitPoint = GameObject.Find("VisitorExitPoint");

        if (exitPoint == null)
        {
            Debug.LogWarning("VisitorExitPoint not found!");
            return;
        }

        Debug.Log("Visitor Position: " + transform.position);
        Debug.Log("Exit Point Position: " + exitPoint.transform.position);

        // Check whether the exit is on or near the NavMesh.
        bool exitOnNavMesh = NavMesh.SamplePosition(
            exitPoint.transform.position,
            out NavMeshHit hit,
            1f,
            agent.areaMask
        );

        Debug.Log("Exit Point near NavMesh: " + exitOnNavMesh);


        if (NavMesh.SamplePosition(
                exitPoint.transform.position,
                out NavMeshHit nearestPoint,
                10f,
                agent.areaMask))
        {
            Debug.Log("Nearest walkable position: " + nearestPoint.position);

            Debug.Log("Distance from exit to NavMesh: " +
                      Vector3.Distance(
                          exitPoint.transform.position,
                          nearestPoint.position
                      ).ToString("F2"));
        }
        else
        {
            Debug.LogWarning("No nearby walkable NavMesh found.");
        }
    }
}