using UnityEngine;
using UnityEngine.AI;

public enum MaleState
{
    Patrolling,
    Chasing,
    Returning
}
public class Male : MonoBehaviour
{
    public Transform target;
    public NavMeshAgent agent;
    public Transform[] patrolPoints;
    public int currentPatrolIndex = 0;
    public MaleState currentState = MaleState.Patrolling;

    public float chaseRange = 10f;
    public float returnRange = 20f;
    public Vector3 initialPosition; 
    void Start()
    {
        initialPosition = transform.position;
        agent = GetComponent<NavMeshAgent>();
    }
    void Update()
    {
        // tính khoảng cách đến người chơi
        var distanceToPlayer = Vector3.Distance(transform.position, target.position);
        // tính khoảng cách từ vị trí ban đầu đến vị trí hiện tại
        var distanceFromInitial = Vector3.Distance(transform.position, initialPosition);
        // logic trạng thái con chó
        switch(currentState)
        {
            case MaleState.Patrolling:
                HandlePatrolling(distanceToPlayer);
                break;
            case MaleState.Chasing:
                HandleChasing(distanceToPlayer, distanceFromInitial);
                break;
            case MaleState.Returning:
                HandleReturning();
                break;
            default: break;
        }
    }
    void HandleReturning()
    {
        agent.SetDestination(initialPosition);
        if(!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentState = MaleState.Patrolling;
            return;
        }
    }
    void HandleChasing(float distanceToTarget, float distanceToInitial)
    {
        if(distanceToTarget > returnRange)
        {
            currentState = MaleState.Returning;
            return;
        }
        agent.SetDestination(target.position);
        if(distanceToTarget > chaseRange + 2f)
        {
            currentState = MaleState.Patrolling;
            return;
        }
    }
    void HandlePatrolling(float distanceToTarget)
    {
        if(distanceToTarget < chaseRange)
        {
            currentState = MaleState.Chasing;
            return;
        }
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        if(!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }
}
