using JYW.Game.Commons;
using JYW.Game.EventPlay;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace JYW.Game.Enemies
{
    public class SpeedSunba : MonoBehaviour, IEnemy
    {
        [Header("Target / Activation")]
        [SerializeField] private string playerTag = "Player";
        [SerializeField] private float activationDistance = 10f; // 활성화 거리

        [Header("Rotation / Charge")]
        [SerializeField] private float rotationSpeedDeg = 60f;    // 초당 회전 속도(도)
        [SerializeField] private float faceThresholdDeg = 2f;     // "완전히 바라봄" 판정 각도
        [SerializeField] private float chargeDuration = 3f;       // 돌진 시간(초)
        [SerializeField] private float restDuration = 3f;        // 대기 시간(초)
        [SerializeField] private float moveSpeed = 10f;           // 돌진 속도 (m/s)

        [SerializeField] private AudioClip attackSound; // 공격 사운드

        private AudioSource audioSource;

        private Transform player;
        private NavMeshAgent agent; // 있으면 회전 자동 업데이트 비활성화
        private Coroutine behaviourCoroutine;
        private bool isActive = false;

        [SerializeField] private EventSO deathEvent;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.updateRotation = false;
                agent.updatePosition = false;
            }

            audioSource = GetComponent<AudioSource>();

            var pgo = GameObject.FindWithTag(playerTag);
            player = pgo ? pgo.transform : null;
        }

        private void Update()
        {
            // 이동 잠금: 돌진/회전 로직을 일시 중지, NavMeshAgent가 있으면 정지
            if (EventPlayManager.Instance != null && EventPlayManager.Instance.isLockMove)
            {
                if (agent != null)
                {
                    agent.isStopped = true;
                    agent.velocity = Vector3.zero;
                }
                // 코루틴은 내부에서 잠금을 프레임 대기로 처리하므로 여기선 시작만 막음
                return;
            }

            // lazy player find
            if (player == null)
            {
                var pgo = GameObject.FindWithTag(playerTag);
                player = pgo ? pgo.transform : null;
            }

            if (!isActive)
            {
                if (player != null)
                {
                    float d = Vector3.Distance(player.position, transform.position);
                    if (d <= activationDistance)
                        isActive = true;
                }
            }

            if (isActive && behaviourCoroutine == null)
            {
                behaviourCoroutine = StartCoroutine(BehaviorCycle());
            }
        }

        private IEnumerator BehaviorCycle()
        {
            while (true)
            {
                // 잠금 중이면 한 프레임씩 대기
                while (EventPlayManager.Instance != null && EventPlayManager.Instance.isLockMove)
                    yield return null;

                // 1) 플레이어를 향해 수평(Y축 한정)으로 회전한다.
                if (player == null) { yield return null; continue; }

                // 계속 회전하면서 "완전히 바라봄" 판정 대기
                while (true)
                {
                    // 잠금 중이면 회전 잠시 중단
                    if (EventPlayManager.Instance != null && EventPlayManager.Instance.isLockMove)
                    {
                        yield return null;
                        continue;
                    }

                    if (player == null) break;

                    Vector3 toPlayer = player.position - transform.position;
                    toPlayer.y = 0f;
                    if (toPlayer.sqrMagnitude < 0.0001f) break;

                    Quaternion current = transform.rotation;
                    Quaternion target = Quaternion.LookRotation(toPlayer.normalized, Vector3.up);

                    float maxDegThisFrame = rotationSpeedDeg * Time.deltaTime;
                    transform.rotation = Quaternion.RotateTowards(current, target, maxDegThisFrame);

                    float angle = Quaternion.Angle(transform.rotation, target);
                    if (angle <= faceThresholdDeg)
                    {
                        break;
                    }

                    yield return null;
                }

                if (audioSource != null && attackSound != null)
                    audioSource.PlayOneShot(attackSound);

                // 2) 바라보던 쪽(현재 forward)를 기록한 뒤 그 방향으로 chargeDuration 동안 돌진
                Vector3 chargeDir = transform.forward;
                float t = 0f;
                while (t < chargeDuration)
                {
                    if (EventPlayManager.Instance != null && EventPlayManager.Instance.isLockMove)
                    {
                        // 잠금 중 돌진 중단하고 대기
                        yield return null;
                        continue;
                    }

                    float dt = Time.deltaTime;
                    transform.position += chargeDir * moveSpeed * dt;
                    t += dt;
                    yield return null;
                }

                // 3) rest
                float waited = 0f;
                while (waited < restDuration)
                {
                    if (EventPlayManager.Instance != null && EventPlayManager.Instance.isLockMove)
                    {
                        yield return null;
                        continue;
                    }

                    waited += Time.deltaTime;
                    yield return null;
                }

                // 반복
            }
        }

        public void GetHit()
        {
            /*
            if (behaviourCoroutine != null)
            {
                StopCoroutine(behaviourCoroutine);
                behaviourCoroutine = null;
            }
            StopAllCoroutines();
            Destroy(gameObject);
            */
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(playerTag))
            {
                EventPlayManager.Instance.PlayEvent(deathEvent, gameObject);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 0.6f, 0f, 0.2f);
            Gizmos.DrawWireSphere(transform.position, activationDistance);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2f);
        }
#endif
    }
}