using JYW.Game.Commons;
using JYW.Game.EventPlay;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace JYW.Game.Enemies
{
    public class MegaSunba : MonoBehaviour, IEnemy
    {
        private GameObject player;
        private NavMeshAgent agent;
        private Coroutine moveCoroutine;
        [SerializeField]
        private float moveSpeed = 2f;
        [SerializeField]
        private AudioClip moveSound;
        private AudioSource audioSource;

        [SerializeField] private EventSO deathEvent;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            player = GameObject.FindWithTag("Player");
            audioSource = GetComponent<AudioSource>();
            if (agent != null) agent.speed = moveSpeed;
        }

        private void Update()
        {
            // 잠금 중이면 코루틴 시작을 막고 에이전트 정지
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
        }

        public void GetHit()
        {
            StopAllCoroutines();
            Destroy(gameObject);
        }

        private IEnumerator MoveToPlayer()
        {
            // 잠금 해제까지 대기
            while (EventPlayManager.Instance != null && EventPlayManager.Instance.isLockMove)
                yield return null;

            float timer = 0f;
            if (agent != null) agent.isStopped = false;
            if (audioSource != null && moveSound != null)
                audioSource.PlayOneShot(moveSound, 0.6f);

            while (timer < 3f)
            {
                // 잠금 중이면 루프 대기
                if (EventPlayManager.Instance != null && EventPlayManager.Instance.isLockMove)
                {
                    if (agent != null)
                    {
                        agent.isStopped = true;
                        agent.velocity = Vector3.zero;
                    }
                    yield return null;
                    continue;
                }

                timer += Time.deltaTime;
                if (agent != null && player != null)
                    agent.destination = player.transform.position;
                yield return null;
            }
            if (agent != null) agent.isStopped = true;
            yield return new WaitForSeconds(3f);
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