using JYW.Game.Commons;
using System.Linq;
using UnityEngine;

namespace JYW.Game.Players
{
    public class PlayerEnemyDetector : MonoBehaviour
    {
        [SerializeField] private float detectRadius = 5f;
        [SerializeField] private AudioClip alertClip;

        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            audioSource.playOnAwake = false;
            audioSource.loop = true;
            audioSource.clip = alertClip;
        }

        private void Update()
        {
            bool anyEnemyNearby = IsAnyEnemyWithinRadius(detectRadius);

            if (anyEnemyNearby)
            {
                // 감지되면 루프 재생 (이미 재생 중이면 그대로 유지)
                if (alertClip != null)
                {
                    if (audioSource.clip != alertClip)
                        audioSource.clip = alertClip;

                    if (!audioSource.isPlaying)
                        audioSource.Play();
                }
            }
            else
            {
                // 감지 안되면 중지
                if (audioSource.isPlaying)
                    audioSource.Stop();
            }
        }

        private bool IsAnyEnemyWithinRadius(float radius)
        {
            // OverlapSphere로 주변 콜라이더 중 IEnemy가 있는지 검사
            var hits = Physics.OverlapSphere(transform.position, radius, ~0, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < hits.Length; i++)
            {
                var col = hits[i];
                // 부모까지 포함하여 IEnemy 컴포넌트 탐색
                var enemy = col.GetComponentInParent<MonoBehaviour>();
                if (enemy is IEnemy)
                {
                    // 거리를 한 번 더 엄밀히 체크 (콜라이더 중심 기준)
                    float dist = Vector3.Distance(transform.position, enemy.transform.position);
                    if (dist <= radius)
                        return true;
                }
            }

            // 콜라이더가 없는 적 대비: 씬 내 IEnemy 전수 검사(최후 수단)
            var enemies = FindObjectsOfType<MonoBehaviour>().OfType<IEnemy>();
            foreach (var e in enemies)
            {
                var mb = e as MonoBehaviour;
                if (mb == null) continue;
                float dist = Vector3.Distance(transform.position, mb.transform.position);
                if (dist <= radius)
                    return true;
            }

            return false;
        }
    }
}