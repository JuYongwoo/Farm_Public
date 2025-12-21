using JYW.Game.Commons;
using JYW.Game.EventPlay;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PassportSunba : MonoBehaviour, IEnemy
{
    public enum BehaviorType
    {
        Stop,
        TwoSided,
        Patrol
    }

    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float flipInterval = 3f;       // 앞뒤 뒤집기 간격 (TwoSided)
    [SerializeField] private float chaseSpeed = 3f;         // 추적 속도 (NavMeshAgent.speed)
    [SerializeField] private float stopChaseDistance = 12f; // 플레이어가 멀어지면 추적 종료
    [SerializeField] private float censorDistance = 4f;     // 이 거리내에서만 플레이어 감지
    [SerializeField] private float censorAngle;
    [SerializeField] private EventSO deathEvent;

    // 동작 선택(직렬화)
    [SerializeField] private BehaviorType behavior = BehaviorType.TwoSided;

    // Patrol 전용: 두 포인트 (월드 좌표)
    [Tooltip("Patrol 선택 시 이동할 지점 A (월드 좌표)")]
    [SerializeField] private Transform patrolPointA;
    [Tooltip("Patrol 선택 시 이동할 지점 B (월드 좌표)")]
    [SerializeField] private Transform patrolPointB;
    [Tooltip("Patrol 속도 (NavMeshAgent.speed)")]
    [SerializeField] private float patrolSpeed = 1.5f;

    // 리바이브 지연 시간
    [SerializeField] private float reviveDelay = 5f;

    private AudioSource audiosource;
    [SerializeField] private AudioClip chaseSound;

    [SerializeField] private Light visionLight;

    // NavMeshAgent
    private NavMeshAgent agent;

    private Coroutine behaviourCoroutine;
    private Transform playerTransform;
    private bool wasInSpotlight = false;

    // patrol 상태
    private bool patrolGoingToA = false;

    // 상태 플래그
    private bool isDead = false;

    // 이동 잠금 상태 변화를 감지하기 위한 플래그
    private bool wasLockMove = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
        }
        audiosource = GetComponent<AudioSource>();
        if (audiosource == null)
        {
            audiosource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Start()
    {
        var pgo = GameObject.FindGameObjectWithTag(playerTag);
        playerTransform = pgo ? pgo.transform : null;

        ConfigureAgentForBehavior(behavior);

        switch (behavior)
        {
            case BehaviorType.TwoSided:
                behaviourCoroutine = StartCoroutine(ActFlip());
                break;
            case BehaviorType.Patrol:
                patrolGoingToA = false;
                behaviourCoroutine = StartCoroutine(ActPatrol());
                break;
            case BehaviorType.Stop:
            default:
                behaviourCoroutine = null;
                agent.isStopped = true;
                break;
        }

        EventPlayManager.Instance.AddAction(gameObject, StartChase);
    }

    private void Update()
    {
        if (isDead) return;

        bool isLock = EventPlayManager.Instance != null && EventPlayManager.Instance.isLockMove;

        // 이동 잠금: 에이전트 및 행동 일시 정지
        if (isLock)
        {
            if (agent != null)
            {
                agent.isStopped = true;
                agent.velocity = Vector3.zero;
            }
            wasLockMove = true;
            return;
        }

        // 잠금 해제 직후 에이전트 재가동 안전 조치
        if (wasLockMove)
        {
            wasLockMove = false;
            if (agent != null)
            {
                agent.isStopped = false;
                // Patrol 중이면 현재 목표를 다시 설정해 재개
                if (behavior == BehaviorType.Patrol)
                {
                    Vector3 currentTarget = (patrolGoingToA && patrolPointA != null)
                        ? patrolPointA.position
                        : (patrolPointB != null ? patrolPointB.position : agent.destination);
                    agent.SetDestination(currentTarget);
                }
            }
        }

        if (playerTransform != null)
        {
            bool inSpot = IsPlayerInAssignedSpotlight(playerTransform.position);
            if (inSpot && !wasInSpotlight)
            {
                StartChase();
            }
            wasInSpotlight = inSpot;
        }
    }

    public void GetHit()
    {
        /*



        if (isDead) return; // 중복 방지
        isDead = true;

        visionLight.enabled = false;
        audiosource.Stop();
        if (behaviourCoroutine != null)
        {
            StopCoroutine(behaviourCoroutine);
            behaviourCoroutine = null;
        }
        StopAllCoroutines();

        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
            agent.velocity = Vector3.zero;
            agent.updatePosition = false;
            agent.updateRotation = false;
        }

        var colliders = GetComponentsInChildren<Collider>(true);
        foreach (var col in colliders) { if (col != null) col.enabled = false; }

        var renderers = GetComponentsInChildren<Renderer>(true);
        foreach (var rend in renderers) { if (rend != null) rend.enabled = false; }

        StartCoroutine(ReviveRoutine(reviveDelay, colliders, renderers));



        */
    }

    private IEnumerator ReviveRoutine(float delay, Collider[] colliders, Renderer[] renderers)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        visionLight.enabled = true;

        if (colliders != null)
        {
            foreach (var col in colliders) { if (col != null) col.enabled = true; }
        }
        if (renderers != null)
        {
            foreach (var rend in renderers) { if (rend != null) rend.enabled = true; }
        }

        if (agent != null)
        {
            ConfigureAgentForBehavior(behavior);
        }

        isDead = false;

        switch (behavior)
        {
            case BehaviorType.TwoSided:
                behaviourCoroutine = StartCoroutine(ActFlip());
                break;
            case BehaviorType.Patrol:
                behaviourCoroutine = StartCoroutine(ActPatrol());
                break;
            case BehaviorType.Stop:
            default:
                behaviourCoroutine = null;
                if (agent != null) agent.isStopped = true;
                break;
        }
    }

    private void ConfigureAgentForBehavior(BehaviorType b)
    {
        if (agent == null) return;

        switch (b)
        {
            case BehaviorType.TwoSided:
                agent.updatePosition = false;
                agent.updateRotation = false;
                agent.isStopped = true;
                break;
            case BehaviorType.Patrol:
            case BehaviorType.Stop:
            default:
                agent.updatePosition = true;
                agent.updateRotation = true;
                agent.isStopped = false;
                break;
        }
    }

    private void StartChase()
    {
        if (isDead) return;

        if (audiosource != null && chaseSound != null)
        {
            audiosource.loop = false;
            audiosource.clip = chaseSound;
            audiosource.Play();
        }

        if (behaviourCoroutine != null)
        {
            StopCoroutine(behaviourCoroutine);
            behaviourCoroutine = null;
        }

        ConfigureAgentForBehavior(BehaviorType.Patrol);
        if (agent != null) agent.speed = chaseSpeed;

        var pgo = playerTransform ?? GameObject.FindGameObjectWithTag(playerTag)?.transform;
        if (pgo != null)
            behaviourCoroutine = StartCoroutine(ActChase(pgo));
    }

    private IEnumerator ActFlip()
    {
        while (!isDead)
        {
            // 이동 잠금 동안 대기
            while (EventPlayManager.Instance != null && EventPlayManager.Instance.isLockMove)
                yield return null;

            yield return new WaitForSeconds(flipInterval);
            transform.Rotate(0f, 180f, 0f);
        }
    }

    private IEnumerator ActPatrol()
    {
        if ((patrolPointA == null || patrolPointB == null) || agent == null)
            yield break;

        agent.speed = patrolSpeed;
        agent.stoppingDistance = 0.1f;
        agent.isStopped = false;
        agent.updatePosition = true;
        agent.updateRotation = true;

        patrolGoingToA = false;
        Vector3 currentTarget = patrolGoingToA ? patrolPointA.position : patrolPointB.position;
        agent.SetDestination(currentTarget);

        while (!isDead)
        {
            // 이동 잠금 동안 에이전트 정지 및 대기
            if (EventPlayManager.Instance != null && EventPlayManager.Instance.isLockMove)
            {
                agent.isStopped = true;
                agent.velocity = Vector3.zero;
                yield return null;
                continue;
            }

            // 잠금 해제 후 즉시 재개 보장: 목적지 재설정
            if (agent.isStopped)
            {
                agent.isStopped = false;
                agent.SetDestination(currentTarget);
            }

            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.05f)
            {
                patrolGoingToA = !patrolGoingToA;
                currentTarget = patrolGoingToA ? patrolPointA.position : patrolPointB.position;
                agent.isStopped = false;
                agent.SetDestination(currentTarget);
            }
            yield return null;
        }
    }

    private IEnumerator ActChase(Transform target)
    {
        if (agent == null) yield break;

        agent.isStopped = false;
        agent.updatePosition = true;
        agent.updateRotation = true;
        agent.stoppingDistance = 0.1f;
        agent.speed = chaseSpeed;

        while (!isDead && target != null)
        {
            // 이동 잠금 동안 에이전트 정지 및 대기
            if (EventPlayManager.Instance != null && EventPlayManager.Instance.isLockMove)
            {
                agent.isStopped = true;
                agent.velocity = Vector3.zero;
                yield return null;
                continue;
            }

            // 잠금 해제 후 즉시 재개 보장
            if (agent.isStopped)
            {
                agent.isStopped = false;
            }

            float distToPlayer = Vector3.Distance(target.position, transform.position);
            if (distToPlayer > stopChaseDistance) break;

            agent.SetDestination(target.position);
            yield return null;
        }

        if (isDead) yield break;

        switch (behavior)
        {
            case BehaviorType.TwoSided:
                if (agent != null) agent.isStopped = true;
                ConfigureAgentForBehavior(BehaviorType.TwoSided);
                behaviourCoroutine = StartCoroutine(ActFlip());
                break;
            case BehaviorType.Patrol:
                behaviourCoroutine = StartCoroutine(ActPatrol());
                break;
            case BehaviorType.Stop:
            default:
                if (agent != null) agent.isStopped = true;
                behaviourCoroutine = null;
                break;
        }
    }

    private bool IsPlayerInAssignedSpotlight(Vector3 playerPos)
    {
        Vector3 origin = transform.position;
        Vector3 toPlayer = playerPos - origin;
        float dist = toPlayer.magnitude;
        if (dist > censorDistance) return false;

        // 시야 콘 검사
        Vector3 localDir = transform.InverseTransformDirection(toPlayer.normalized);
        float yawDeg = Mathf.Abs(Mathf.Atan2(localDir.x, localDir.z) * Mathf.Rad2Deg);
        float pitchDeg = Mathf.Abs(Mathf.Atan2(localDir.y, new Vector2(localDir.x, localDir.z).magnitude) * Mathf.Rad2Deg);

        float hHalf = Mathf.Max(0f, censorAngle);
        float vHalf = Mathf.Max(0f, censorAngle);
        if (yawDeg > hHalf || pitchDeg > vHalf) return false;

        // 가림 체크: 가장 먼저 맞은 것이 플레이어면 true, 아니면 false
        var dir = toPlayer.normalized;
        var hits = Physics.RaycastAll(origin, dir, dist, ~0, QueryTriggerInteraction.Ignore);
        if (hits == null || hits.Length == 0)
            return false;

        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        for (int i = 0; i < hits.Length; i++)
        {
            var h = hits[i];
            if (h.collider == null) continue;

            var tr = h.collider.transform;

            // 자기 자신(적 오브젝트) 충돌 무시
            if (tr == transform || tr.IsChildOf(transform))
                continue;

            // 플레이어(혹은 자식) 먼저 맞으면 가시
            if (h.collider.gameObject.CompareTag(playerTag) ||
                (playerTransform != null && tr.IsChildOf(playerTransform)))
                return true;

            // 그 외 오브젝트가 먼저 맞으면 가려진 것
            return false;
        }

        return false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (isDead) return; // 죽은 상태면 트리거 무시
        if (other.CompareTag("Player"))
        {
            EventPlayManager.Instance.PlayEvent(deathEvent, gameObject);
        }
    }

    private void OnDisable()
    {
        if (behaviourCoroutine != null)
        {
            StopCoroutine(behaviourCoroutine);
            behaviourCoroutine = null;
        }
    }
}