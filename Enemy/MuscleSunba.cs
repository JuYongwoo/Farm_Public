using JYW.Game.Commons;
using JYW.Game.EventPlay;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace JYW.Game.Enemies
{
    public class MuscleSunba : MonoBehaviour, IEnemy
    {
        private GameObject player;
        private NavMeshAgent agent;
        [SerializeField]
        private float moveSpeed = 3f;

        private AudioSource audioSource;
        [SerializeField]
        private AudioClip audioClip;
        private Coroutine audioCoroutine = null;

        [SerializeField] private EventSO deathEvent;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            agent = GetComponent<NavMeshAgent>();
            if (agent != null) agent.speed = moveSpeed;

            player = GameObject.FindWithTag("Player");
        }

        private void Update()
        {
            if (player == null || agent == null) return;

            // 이동 잠금 시 에이전트 정지 및 로직 중단
            if (EventPlayManager.Instance != null && EventPlayManager.Instance.isLockMove)
            {
                agent.isStopped = true;
                agent.velocity = Vector3.zero;
                return;
            }

            agent.isStopped = false;
            agent.SetDestination(player.transform.position);
            if (audioCoroutine == null) audioCoroutine = StartCoroutine(soundCoroutine());
        }

        IEnumerator soundCoroutine()
        {
            // 잠금 동안 사운드 시작 대기
            while (EventPlayManager.Instance != null && EventPlayManager.Instance.isLockMove)
                yield return null;

            if (audioSource != null && audioClip != null)
                audioSource.PlayOneShot(audioClip, 0.5f);
            yield return new WaitForSeconds(2f);
            audioCoroutine = null;
        }

        public void GetHit()
        {
            Destroy(gameObject);
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