using UnityEngine;
using System.Collections;

namespace JYW.Game.Particles
{
    public class WoodParticle : MonoBehaviour
    {
        [SerializeField]
        private GameObject woodPlankPrefab;

        // 소환 갯수
        [SerializeField]
        private int spawnCount = 10;

        // 퍼뜨릴 최대 반경 (1초 동안 이 반경 내로 퍼짐)
        [SerializeField]
        private float spreadRadius = 2f;

        // 수직 편차 (초기 스폰 시 약간의 높이 차)
        [SerializeField]
        private float verticalVariance = 0.2f;

        // 퍼뜨리는 시간(초)
        [SerializeField]
        private float spreadDuration = 1f;

        // 회전 속도(도/초) 최소/최대: 판자가 튀면서 회전하도록 임의 값 사용
        [SerializeField]
        private float minAngularSpeed = 180f;
        [SerializeField]
        private float maxAngularSpeed = 720f;

        // 자동 소환 원하면 Start에서 호출하거나 외부에서 SpawnAndSpread 호출
        void Start()
        {
            // 필요 시 자동 호출:
            SpawnAndSpread(transform.position);
        }

        // 지정한 위치를 중심으로 나무판자들을 생성한 뒤
        // 각 판자를 spreadDuration 동안 주변으로 퍼뜨리고 제거합니다.
        public void SpawnAndSpread(Vector3 center, int count = -1, float maxRadius = -1f)
        {
            if (woodPlankPrefab == null)
            {
                Debug.LogWarning("[WoodParticle] woodPlankPrefab이 할당되지 않았습니다.");
                return;
            }

            int useCount = (count <= 0) ? spawnCount : count;
            float useRadius = (maxRadius <= 0f) ? spreadRadius : maxRadius;

            for (int i = 0; i < useCount; i++)
            {
                // 생성은 중심 근처(약간의 수직편차만 줌)
                Vector3 spawnPos = new Vector3(center.x,
                                               center.y + Random.Range(-verticalVariance, verticalVariance),
                                               center.z);

                // 초기 회전은 임의로 줌
                Quaternion rot = Quaternion.Euler(Random.Range(-30f, 30f),
                                                   Random.Range(0f, 360f),
                                                   Random.Range(-30f, 30f));

                GameObject go = Instantiate(woodPlankPrefab, spawnPos, rot);
                go.name = woodPlankPrefab.name;

                // 목표 위치: 3D 상에서 반구(위쪽 반구) 방향으로 퍼짐
                // 방향은 Random.onUnitSphere로 뽑고 y>0 인 경우만 허용하여 모두 위쪽으로 향하게 함
                Vector3 dir3;
                int safety = 0;
                do
                {
                    dir3 = Random.onUnitSphere;
                    safety++;
                } while (dir3.y <= 0f && safety < 16);

                // 드물게 실패할 경우 위쪽으로 고정
                if (dir3.y <= 0f)
                    dir3 = Vector3.up;

                float distance = Random.Range(0.0f, useRadius);

                Vector3 targetPos = center + dir3 * distance;

                // 목표 Y가 중심 이하가 되지 않도록 보정 (거리 0 등 예외 방지)
                if (targetPos.y <= center.y)
                    targetPos.y = center.y + Mathf.Max(0.05f, verticalVariance);

                // 각 판자에 대해 퍼트기 + 회전 코루틴 시작
                StartCoroutine(SpreadAndDestroyRoutine(go, spawnPos, targetPos, spreadDuration));
            }
        }

        // 판자를 duration 동안 start -> target 위치로 보간하면서 회전한 뒤 파괴합니다.
        private IEnumerator SpreadAndDestroyRoutine(GameObject go, Vector3 start, Vector3 target, float duration)
        {
            if (go == null)
                yield break;

            // 안전 장치: duration이 0이면 즉시 이동 후 파괴
            if (duration <= 0f)
            {
                go.transform.position = target;
                Destroy(go);
                yield break;
            }

            // 회전 축과 속도 결정 (월드 공간)
            Vector3 rotAxis = Random.onUnitSphere;
            // 약간의 편향: 회전축이 완전 수평이거나 수직이면 자연스럽지 않으므로 보정 가능
            if (Mathf.Abs(rotAxis.y) < 0.1f)
            {
                // 수평에 너무 가깝다면 약간 위쪽 성분 추가
                rotAxis.y += 0.25f;
                rotAxis.Normalize();
            }
            float angularSpeed = Random.Range(minAngularSpeed, maxAngularSpeed); // 도/초

            float t = 0f;
            while (t < duration)
            {
                if (go == null) yield break;
                t += Time.deltaTime;
                float k = Mathf.Clamp01(t / duration);
                // 부드러운 Ease-Out 효과를 위해 SmoothStep 사용
                float ease = Mathf.SmoothStep(0f, 1f, k);
                go.transform.position = Vector3.Lerp(start, target, ease);

                // 회전: 월드 공간 기준으로 축(rotAxis) 만큼 각도 적용
                // Rotate는 축과 각도(도 단위)를 사용
                try
                {
                    go.transform.Rotate(rotAxis, angularSpeed * Time.deltaTime, Space.World);
                }
                catch
                {
                    // 안전: Transform이 사라지거나 예외 발생 시 루프 탈출
                    yield break;
                }

                yield return null;
            }

            // 최종 위치 보정 후 제거
            if (go != null)
            {
                go.transform.position = target;
                Destroy(go);
            }
        }

        // 에디터에서 우클릭으로 테스트 호출 가능
        [ContextMenu("Spawn Default Planks And Spread")]
        private void ContextSpawn()
        {
            SpawnAndSpread(transform.position);
        }
    }
}