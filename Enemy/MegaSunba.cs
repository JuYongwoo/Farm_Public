using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace JYW.Game.Enemies
{
    public class MegaSunba : MonoBehaviour
    {
        private GameObject player;
        private NavMeshAgent agent;
        private Coroutine moveCoroutine;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            player = GameObject.FindWithTag("Player");
        }

        private void Update()
        {
            if (moveCoroutine == null) moveCoroutine = StartCoroutine(MoveToPlayer());
        }

        private IEnumerator MoveToPlayer()
        {
            float timer = 0f;
            agent.isStopped = false;

            while (timer < 3f)
            {
                timer += Time.deltaTime;
                agent.destination = player.transform.position;
                yield return null;
            }
            agent.isStopped = true;
            yield return new WaitForSeconds(3f);
            moveCoroutine = null;
        }
    }
}