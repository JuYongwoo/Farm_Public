using JYW.Game.EventPlay;
using UnityEngine;

namespace JYW.Game.Players
{
    public class PlayerMove : MonoBehaviour
    {
        [SerializeField]
        public float Playerspeed = 2.5f;
        [SerializeField]
        private AudioClip footstepSound;
        private AudioSource audioSource;
        private CharacterController charCtrl;

        // 추가: 한 걸음 사이 간격(초), 볼륨, 피치 변동
        [SerializeField] private float stepInterval = 0.45f;
        [SerializeField] private float stepVolume = 1f;
        [SerializeField] private float pitchVariance = 0.05f;
        private float stepTimer = 0f;

        private void Awake()
        {
            charCtrl = GetComponent<CharacterController>();
            audioSource = GetComponent<AudioSource>();
        }

        private void FixedUpdate()
        {
            if (Camera.main == null) return;
            transform.eulerAngles = new Vector3(0, Camera.main.transform.eulerAngles.y, 0);
            if (!EventPlayManager.Instance.isLockMove && charCtrl != null && charCtrl.enabled) charCtrl.SimpleMove(GetDirection() * Playerspeed);

            HandleFootsteps();
        }

        private Vector3 GetDirection()
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            Vector3 dir = new Vector3(h, 0, v);

            if (dir.sqrMagnitude > 1f)
                dir.Normalize();

            return transform.TransformDirection(dir);
        }

        private void HandleFootsteps()
        {
            // 조건: 이동 중이고 바닥에 닿아있고 사운드가 설정되어 있어야 함
            if (footstepSound == null || audioSource == null || charCtrl == null)
            {
                return;
            }

            // 이동 여부 판단 (로컬 기준)
            Vector3 horizontalVel = new Vector3(charCtrl.velocity.x, 0f, charCtrl.velocity.z);
            bool isMoving = horizontalVel.sqrMagnitude > 0.01f;

            // 점유된 이동 잠금 시 소리 금지
            if (EventPlayManager.Instance != null && EventPlayManager.Instance.isLockMove)
            {
                isMoving = false;
            }

            // FixedUpdate에서 타이머 감소 (물리 업데이트)
            if (stepTimer > 0f)
                stepTimer -= Time.fixedDeltaTime;

            if (isMoving && charCtrl.isGrounded)
            {
                if (stepTimer <= 0f)
                {
                    // 피치에 약간 랜덤 변동을 주면 자연스러움
                    float origPitch = audioSource.pitch;
                    if (pitchVariance > 0f)
                        audioSource.pitch = 1f + Random.Range(-pitchVariance, pitchVariance);

                    audioSource.PlayOneShot(footstepSound, stepVolume);

                    // 피치 복구
                    audioSource.pitch = origPitch;

                    stepTimer = stepInterval;
                }
            }
            else
            {
                // 멈출 때 타이머을 초기화하면 다음 시작 시 즉시 소리 재생 가능
                // stepTimer = 0f;
            }
        }
    }
}