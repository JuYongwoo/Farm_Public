using JYW.Game.Commons;
using JYW.Game.EventPlay;
using System.Collections;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    private Animator animator;

    private Coroutine currentCoroutine = null;
    public float attackBeforeDelay = 0.6f;
    public float attackAfterDelay = 0.4f;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 2.0f;
    [SerializeField] private float attackRadius = 0.6f;
    [SerializeField] private LayerMask attackLayer = ~0;


    [SerializeField]
    private AudioClip eliminateSound;
    [SerializeField]
    private AudioClip attackSound;
    private AudioSource audioSource;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !EventPlayManager.Instance.isLockMove)
        {
            if (currentCoroutine == null)
            {
                currentCoroutine = StartCoroutine(Attack());
            }
        }
    }

    IEnumerator Attack()
    {
        if (animator != null)
            animator.SetTrigger("Attack");

        yield return new WaitForSeconds(attackBeforeDelay);

        Vector3 origin = transform.root.position;
        Vector3 center = origin + transform.root.forward * Mathf.Max(0.01f, attackRange);

        Collider[] hits = Physics.OverlapSphere(center, Mathf.Max(0.01f, attackRadius), attackLayer);

        audioSource.PlayOneShot(attackSound, 0.1f);

        foreach (var hit in hits)
        {
            if (hit == null) continue;
            GameObject target = hit.gameObject;
            if (target == this.gameObject) continue;

            var enemy = hit.GetComponentInParent<IEnemy>();
            if (enemy != null)
            {
                audioSource.PlayOneShot(eliminateSound);
                enemy.GetHit();
                continue;
            }
        }

        yield return new WaitForSeconds(attackAfterDelay);

        currentCoroutine = null;
    }
}