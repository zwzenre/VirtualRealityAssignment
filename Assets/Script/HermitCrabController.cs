using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class HermitCrabController : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public NavMeshAgent agent;
    public Transform rockCenter;
    public XRGrabInteractable grabInteractable;

    [Header("Wander")]
    public float wanderRadius = 2f;
    public float minIdleTime = 1.5f;
    public float maxIdleTime = 3.5f;
    public float arriveDistance = 0.2f;

    [Header("Hide")]
    public float unhideDelay = 2f;

    private bool isHidden;
    private bool isGrabbed;
    private float idleTimer;
    private float unhideTimer;

    void Awake()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (grabInteractable == null) grabInteractable = GetComponent<XRGrabInteractable>();
    }

    void OnEnable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrabbed);
            grabInteractable.selectExited.AddListener(OnReleased);
        }
    }

    void OnDisable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            grabInteractable.selectExited.RemoveListener(OnReleased);
        }
    }

    void Start()
    {
        idleTimer = Random.Range(minIdleTime, maxIdleTime);
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsHidden", false);
    }

    void Update()
    {
        if (isGrabbed)
        {
            Hide();
            return;
        }

        if (isHidden)
        {
            unhideTimer -= Time.deltaTime;
            if (unhideTimer <= 0f)
            {
                Unhide();
            }
            return;
        }

        UpdateWander();
    }

    void UpdateWander()
    {
        if (agent.pathPending)
            return;

        if (agent.remainingDistance <= arriveDistance)
        {
            animator.SetBool("IsWalking", false);
            idleTimer -= Time.deltaTime;

            if (idleTimer <= 0f)
            {
                Vector3 target;
                if (TryGetRandomPointOnRock(out target))
                {
                    agent.isStopped = false;
                    agent.SetDestination(target);
                    animator.SetBool("IsWalking", true);
                }

                idleTimer = Random.Range(minIdleTime, maxIdleTime);
            }
        }
    }

    void Hide()
    {
        if (isHidden)
            return;

        isHidden = true;
        agent.isStopped = true;
        agent.ResetPath();

        animator.SetBool("IsWalking", false);
        animator.SetBool("IsHidden", true);
        animator.SetTrigger("Hide");
    }

    void Unhide()
    {
        isHidden = false;
        animator.SetBool("IsHidden", false);
        animator.SetTrigger("Unhide");
        idleTimer = Random.Range(minIdleTime, maxIdleTime);
    }

    bool TryGetRandomPointOnRock(out Vector3 result)
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 random2D = Random.insideUnitCircle * wanderRadius;
            Vector3 candidate = rockCenter.position + new Vector3(random2D.x, 0f, random2D.y);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(candidate, out hit, 1f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }

        result = transform.position;
        return false;
    }

    void OnGrabbed(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        agent.enabled = false;
        Hide();
    }

    void OnReleased(SelectExitEventArgs args)
    {
        isGrabbed = false;
        agent.enabled = true;
        unhideTimer = unhideDelay;
    }
}