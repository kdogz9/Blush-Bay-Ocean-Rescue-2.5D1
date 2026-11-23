using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class VisitorNavMeshSetup : MonoBehaviour
{
    private void Awake()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();

        // Keep the pixel-art sprite facing the camera.
        agent.updateRotation = false;
    }

    private void Start()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();

        // Check whether Unity generated a NavMesh.
        NavMeshTriangulation mesh = NavMesh.CalculateTriangulation();

        Debug.Log("Visitor NavMesh vertices: " + mesh.vertices.Length);
        Debug.Log("Visitor is on NavMesh: " + agent.isOnNavMesh);
    }
}