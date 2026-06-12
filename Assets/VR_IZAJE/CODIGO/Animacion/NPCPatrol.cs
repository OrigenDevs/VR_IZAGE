using UnityEngine;

public class NPCPatrol : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float moveSpeed = 2f;
    public float rotationSpeed = 180f;
    public float minIdleTime = 1f;
    public float maxIdleTime = 3f;
    public Animator animator;

    private Transform target;
    private float idleTimer;
    private float currentIdleTime;

    void Start()
    {
        target = pointB;
        transform.position = pointA.position;
        FaceTarget();
        animator.SetBool("Caminar", true);
    }

    void Update()
    {
        if (target == null || pointA == null || pointB == null)
            return;

        FaceTarget();

        float dist = Vector3.Distance(transform.position, target.position);

        if (dist < 0.1f)
        {
            animator.SetBool("Caminar", false);
            if (currentIdleTime == 0f)
                currentIdleTime = Random.Range(minIdleTime, maxIdleTime);

            idleTimer += Time.deltaTime;
            if (idleTimer >= currentIdleTime)
            {
                idleTimer = 0f;
                currentIdleTime = 0f;
                target = target == pointA ? pointB : pointA;
                animator.SetBool("Caminar", true);
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
        }
    }

    void FaceTarget()
    {
        Vector3 dir = (target.position - transform.position).normalized;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dir), rotationSpeed * Time.deltaTime);
    }
}
