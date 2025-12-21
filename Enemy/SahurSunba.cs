using JYW.Game.Commons;
using JYW.Game.EventPlay;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SahurSunba : MonoBehaviour, IEnemy
{
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip speakAudioClip;

    private Coroutine dieCoroutine = null;
    private Coroutine audioCoroutine = null;
    private Coroutine followCoroutine = null;
    private GameObject player;
    private NavMeshAgent agent;
    private bool isOn = false;

    [SerializeField] private EventSO deathEvent;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
    }
    private void Start()
    {
        EventPlayManager.Instance.AddAction(gameObject, MoveStart);
    }

    public void MoveStart()
    {
        isOn = true;
        dieCoroutine = StartCoroutine(Die());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EventPlayManager.Instance.PlayEvent(deathEvent, gameObject);
        }
    }

    public void GetHit()
    {

    }

    private void Update()
    {
        // 이동 잠금: 에이전트 정지 및 코루틴 시작 방지
        if (EventPlayManager.Instance != null && EventPlayManager.Instance.isLockMove)
        {
            if (agent != null)
            {
                agent.isStopped = true;
                agent.velocity = Vector3.zero;
            }
            return;
        }

        if (followCoroutine == null && isOn)
        {
            followCoroutine = StartCoroutine(Follow());
        }
        if (audioCoroutine == null && isOn)
        {
            audioCoroutine = StartCoroutine(Speak());
        }
    }

    private IEnumerator Follow()
    {
        // 잠금 중이면 대기
        while (EventPlayManager.Instance != null && EventPlayManager.Instance.isLockMove)
            yield return null;

        if (agent != null && player != null)
        {
            agent.isStopped = false;
            agent.destination = player.transform.position;
        }
        yield return new WaitForSeconds(0.1f);
        followCoroutine = null;
    }

    private IEnumerator Die()
    {
        yield return new WaitForSeconds(15f);
        Destroy(gameObject);
    }

    private IEnumerator Speak()
    {
        // 잠금 중이면 대기
        while (EventPlayManager.Instance != null && EventPlayManager.Instance.isLockMove)
            yield return null;

        if (audioSource != null && speakAudioClip != null)
            audioSource.PlayOneShot(speakAudioClip, 0.2f);
        yield return new WaitForSeconds(5f);
        audioCoroutine = null;
    }
}