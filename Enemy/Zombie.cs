using UnityEngine;
using UnityEngine.AI;

namespace JYW.Enemies.Enemies
{

    public class Zombie : MonoBehaviour
    {
        private enum MoveStat
        {
            Stand, //0
            Walk, //1
            Run, //2
            Attack //3
        }

        public enum BehaviorType
        {
            Stop,
            TwoSided,
            Patrol
        }

        [SerializeField]
        private MoveStat moveStat = MoveStat.Stand;

        [Header("Behavior Settings")]
        [SerializeField] private BehaviorType behavior = BehaviorType.Stop;
        [SerializeField] private float flipInterval = 3f; // TwoSided: flip every interval seconds

        [Header("Patrol Settings")]
        [SerializeField] private Transform[] patrolPoints;
        [SerializeField] private float patrolSpeed = 1.5f;
        [SerializeField] private float patrolStoppingDistance = 0.1f;

        private NavMeshAgent agent;
        private Animator animator;

        private Coroutine behaviourCoroutine;
        private int patrolGoingToIndex= 0 ;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            if (agent == null)
            {
                agent = gameObject.AddComponent<NavMeshAgent>();
            }

            animator = GetComponent<Animator>();
            SetAnimStat(moveStat);
        }

        private void OnEnable()
        {
            StartBehavior();
        }

        private void OnDisable()
        {
            if (behaviourCoroutine != null)
            {
                StopCoroutine(behaviourCoroutine);
                behaviourCoroutine = null;
            }
            if (agent != null)
            {
                agent.isStopped = true;
                agent.ResetPath();
            }
        }

        private void StartBehavior()
        {
            if (behaviourCoroutine != null)
            {
                StopCoroutine(behaviourCoroutine);
                behaviourCoroutine = null;
            }

            switch (behavior)
            {
                case BehaviorType.TwoSided:
                    ConfigureAgentForBehavior(false);
                    SetAnimStat(MoveStat.Stand);
                    behaviourCoroutine = StartCoroutine(ActFlip());
                    break;
                case BehaviorType.Patrol:
                    ConfigureAgentForBehavior(true);
                    SetAnimStat(MoveStat.Walk);
                    behaviourCoroutine = StartCoroutine(ActPatrol());
                    break;
                case BehaviorType.Stop:
                default:
                    ConfigureAgentForBehavior(false);
                    SetAnimStat(MoveStat.Stand);
                    break;
            }
        }

        private void ConfigureAgentForBehavior(bool enableMovement)
        {
            if (agent == null) return;
            agent.updatePosition = enableMovement;
            agent.updateRotation = enableMovement;
            agent.isStopped = !enableMovement;
            if (enableMovement)
            {
                agent.speed = patrolSpeed;
                agent.stoppingDistance = patrolStoppingDistance;
            }
        }

        private void SetAnimStat(MoveStat statnum)
        {
            if (animator != null)
                animator.SetInteger("MoveStat", (int)statnum);
        }

        private System.Collections.IEnumerator ActFlip()
        {
            while (true)
            {
                yield return new WaitForSeconds(flipInterval);
                transform.Rotate(0f, 180f, 0f);
            }
        }

        private System.Collections.IEnumerator ActPatrol()
        {
            if (agent == null || patrolPoints == null)
            {
                yield break;
            }

            agent.isStopped = false;
            agent.updatePosition = true;
            agent.updateRotation = true;
            agent.speed = patrolSpeed;
            agent.stoppingDistance = patrolStoppingDistance;


            Vector3 currentTarget = patrolPoints[patrolGoingToIndex].position;
            agent.SetDestination(currentTarget);

            while (true)
            {
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.05f)
                {
                    patrolGoingToIndex++;
                    currentTarget = patrolPoints[patrolGoingToIndex%patrolPoints.Length].position;
                    agent.isStopped = false;
                    agent.SetDestination(currentTarget);
                }
                yield return null;
            }
        }
    }
}