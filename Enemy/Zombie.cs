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
        [SerializeField]
        private MoveStat moveStat = MoveStat.Stand;
        private GameObject player;
        private NavMeshAgent agent;

        private Animator animator;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            player = GameObject.FindWithTag("Player");

            animator = GetComponent<Animator>();
            SetAnimStat(moveStat);
        }

        private void Update()
        {
            agent.destination = player.transform.position;
        }

        private void SetAnimStat(MoveStat statnum)
        {
            animator.SetInteger("MoveStat", (int)statnum);
        }

    }
}