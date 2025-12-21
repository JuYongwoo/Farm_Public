using JYW.Game.Commons;
using JYW.Game.EventPlay;
using UnityEngine;
using UnityEngine.AI;

namespace JYW.Game.Enemies
{
    public class MasSunba : MonoBehaviour, IEnemy
    {
        private GameObject player;
        private NavMeshAgent agent;
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private AudioClip stopSound;
        private AudioSource audioSource;

        // 시야 체크 파라미터
        [Tooltip("시야각(도, 전체 FOV). 예: 60")]
        [SerializeField] private float viewAngle = 60f;
        [SerializeField] private float viewDistance = 20f;
        [SerializeField] private LayerMask obstructionMask;   // (선택) 장애물 레이어
        [SerializeField] private bool debugSight = false;

        [SerializeField] private EventSO deathEvent;

        // 리스폰 지연 시간
        [SerializeField] private float respawnDelay = 5f;

        // 상태 플래그
        private bool isDead = false;

        // 정지 상태에서 stopSound를 한 번만 재생하기 위한 플래그
        private bool hasPlayedStopSound = false;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            if (agent != null) agent.speed = moveSpeed;

            player = GameObject.FindWithTag("Player");
            audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            // 이동 잠금 시 모든 이동 로직 중단
            if (EventPlayManager.Instance != null && EventPlayManager.Instance.isLockMove)
            {
                if (agent != null)
                {
                    agent.isStopped = true;
                    agent.velocity = Vector3.zero;
                }
                return;
            }

            // 죽은 상태면 모든 동작 차단
            if (isDead || player == null || agent == null) return;

            bool playerSeesMe = PlayerIsLookingAtMe();

            // 플레이어가 보면 정지, 안 보면 이동
            bool isStopped = playerSeesMe;
            agent.isStopped = isStopped;

            if (!agent.isStopped)
            {
                // 이동 중이면 목적지 계속 설정
                agent.SetDestination(player.transform.position);
                // 이동으로 전환되면 다음 정지 사이클에서 다시 1회 재생 가능
                hasPlayedStopSound = false;
            }
            else
            {
                // 정지 상태에 진입했을 때 한 번만 재생
                if (!hasPlayedStopSound)
                {
                    if (stopSound != null && audioSource != null && !audioSource.isPlaying)
                    {
                        audioSource.PlayOneShot(stopSound);
                    }
                    hasPlayedStopSound = true;
                }
            }
        }

        private bool PlayerIsLookingAtMe()
        {
            if (player == null) return false;

            // 우선 Player의 카메라(있으면)를 사용, 없으면 Camera.main 사용
            Camera cam = player.GetComponentInChildren<Camera>() ?? Camera.main;
            if (cam == null) return false;

            Vector3 origin = cam.transform.position;
            Vector3 toMe = transform.position - origin;
            float dist = toMe.magnitude;
            if (dist > viewDistance)
            {
                return false;
            }

            // viewAngle은 전체 FOV로 취급 -> 반각 비교
            float halfFov = viewAngle * 0.5f;
            float angle = Vector3.Angle(cam.transform.forward, toMe.normalized);
            if (angle > halfFov)
            {
                return false;
            }

            // 단순화: 거리/각도만 판단
            if (debugSight)
            {
                Debug.DrawRay(origin, toMe.normalized * Mathf.Min(dist, viewDistance), Color.green, 0.1f);
            }
            return true;
        }

        public void GetHit()
        {
            if (isDead) return; // 중복 호출 방지
            isDead = true;

            // 모든 동작 중지
            StopAllCoroutines();

            // 에이전트 정지
            if (agent != null)
            {
                agent.isStopped = true;
                agent.ResetPath();
                agent.velocity = Vector3.zero;
            }

            // 오디오 정지
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }

            // 콜라이더/렌더러 비활성화
            var colliders = GetComponentsInChildren<Collider>(true);
            foreach (var col in colliders) col.enabled = false;

            var renderers = GetComponentsInChildren<Renderer>(true);
            foreach (var rend in renderers) rend.enabled = false;

            // 정지 사운드 재생 플래그 초기화
            hasPlayedStopSound = false;

            // 리스폰
            StartCoroutine(RespawnRoutine(respawnDelay, colliders, renderers));
        }

        private System.Collections.IEnumerator RespawnRoutine(float delay, Collider[] colliders, Renderer[] renderers)
        {
            if (delay > 0f)
                yield return new WaitForSeconds(delay);

            // 콜라이더/렌더러 재활성화
            if (colliders != null)
            {
                foreach (var col in colliders)
                {
                    if (col != null) col.enabled = true;
                }
            }
            if (renderers != null)
            {
                foreach (var rend in renderers)
                {
                    if (rend != null) rend.enabled = true;
                }
            }

            // 에이전트 재가동
            if (agent != null)
            {
                agent.isStopped = false;
                agent.speed = moveSpeed;
            }

            // 상태 복구
            isDead = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isDead) return; // 죽은 상태면 트리거 무시
            if (other.CompareTag("Player"))
            {
                EventPlayManager.Instance.PlayEvent(deathEvent, gameObject);
            }
        }
    }
}