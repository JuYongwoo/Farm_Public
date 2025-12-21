using JYW.Game.Commons;
using JYW.Game.EventPlay;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace JYW.Game.Enemies
{
    public class ZombieSunba : MonoBehaviour, IEnemy
    {
        private GameObject player;
        private NavMeshAgent agent;
        private Coroutine moveCoroutine;
        private Coroutine audioCoroutine;
        [SerializeField]
        private float moveSpeed = 1f;

        [SerializeField]
        private AudioClip keepSound;
        private AudioSource audioSource;

        [Header("Respawn")]
        [SerializeField] private float respawnDelay = 20f;

        private bool isDead = false;

        // 원래 상태 복원용 컬렉션
        private Collider[] ownColliders;
        private Renderer[] ownRenderers;
        private bool agentWasEnabled;
        private bool audioWasEnabled;
        private bool[] collidersWasEnabled;
        private bool[] renderersWasEnabled;

        [SerializeField] private EventSO deathEvent;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            if (agent != null) agent.speed = moveSpeed;

            player = GameObject.FindWithTag("Player");
            audioSource = GetComponent<AudioSource>();

            ownColliders = GetComponents<Collider>();
            ownRenderers = GetComponentsInChildren<Renderer>(true);

            // 상태 배열 초기화
            collidersWasEnabled = new bool[ownColliders.Length];
            for (int i = 0; i < ownColliders.Length; i++) collidersWasEnabled[i] = ownColliders[i].enabled;

            renderersWasEnabled = new bool[ownRenderers.Length];
            for (int i = 0; i < ownRenderers.Length; i++) renderersWasEnabled[i] = ownRenderers[i].enabled;

            agentWasEnabled = agent != null && agent.enabled;
            audioWasEnabled = audioSource != null && audioSource.enabled;
        }

        private void OnEnable()
        {
            isDead = false;
        }

        private void Update()
        {
            if (isDead) return;

            // 이동 잠금 체크: NavMeshAgent 정지 및 로직 중단
            if (EventPlayManager.Instance != null && EventPlayManager.Instance.isLockMove)
            {
                if (agent != null)
                {
                    agent.isStopped = true;
                    agent.velocity = Vector3.zero;
                }
                return;
            }

            if (moveCoroutine == null) moveCoroutine = StartCoroutine(MoveToPlayer());
            if (audioCoroutine == null) audioCoroutine = StartCoroutine(PlayAudio());
        }

        public void GetHit()
        {
            if (isDead) return;
            StartCoroutine(HandleDeathAndRespawn());
        }

        private IEnumerator HandleDeathAndRespawn()
        {
            isDead = true;

            // 중지 및 상태 저장
            if (audioCoroutine != null) { StopCoroutine(audioCoroutine); audioCoroutine = null; }
            if (moveCoroutine != null) { StopCoroutine(moveCoroutine); moveCoroutine = null; }

            if (ownColliders != null)
            {
                for (int i = 0; i < ownColliders.Length; i++)
                {
                    collidersWasEnabled[i] = ownColliders[i].enabled;
                    ownColliders[i].enabled = false;
                }
            }
            if (ownRenderers != null)
            {
                for (int i = 0; i < ownRenderers.Length; i++)
                {
                    renderersWasEnabled[i] = ownRenderers[i].enabled;
                    ownRenderers[i].enabled = false;
                }
            }

            if (agent != null)
            {
                agentWasEnabled = agent.enabled;
                agent.enabled = false;
            }

            if (audioSource != null)
            {
                audioWasEnabled = audioSource.enabled;
                audioSource.Stop();
                audioSource.enabled = false;
            }

            float t = 0f;
            while (t < respawnDelay)
            {
                t += Time.deltaTime;
                yield return null;
            }

            // 컴포넌트 복원
            if (ownColliders != null)
            {
                for (int i = 0; i < ownColliders.Length; i++)
                {
                    if (ownColliders[i] != null)
                        ownColliders[i].enabled = collidersWasEnabled[i];
                }
            }
            if (ownRenderers != null)
            {
                for (int i = 0; i < ownRenderers.Length; i++)
                {
                    if (ownRenderers[i] != null)
                        ownRenderers[i].enabled = renderersWasEnabled[i];
                }
            }

            if (agent != null)
            {
                agent.enabled = true;
                agent.Warp(transform.position);
                var pl = GameObject.FindWithTag("Player");
                if (pl != null)
                {
                    agent.destination = pl.transform.position;
                }
            }

            if (audioSource != null)
            {
                audioSource.enabled = audioWasEnabled;
            }

            isDead = false;
        }

        private IEnumerator PlayAudio()
        {
            // 잠금 동안 오디오 시작 대기
            while (EventPlayManager.Instance != null && EventPlayManager.Instance.isLockMove)
                yield return null;

            if (audioSource != null && keepSound != null)
            {
                audioSource.PlayOneShot(keepSound, 2.0f);
            }
            yield return new WaitForSeconds(4.5f);
            audioCoroutine = null;
        }

        private IEnumerator MoveToPlayer()
        {
            // 잠금 동안 이동 대기
            while (EventPlayManager.Instance != null && EventPlayManager.Instance.isLockMove)
                yield return null;

            if (agent != null && player != null)
            {
                agent.isStopped = false;
                agent.destination = player.transform.position;
            }
            yield return new WaitForSeconds(0.1f);
            moveCoroutine = null;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                EventPlayManager.Instance.PlayEvent(deathEvent, gameObject);
            }
        }
    }
}