using JYW.Game.EventPlay;
using UnityEngine;

namespace JYW.Game.Players
{

    public class PlayerRotate : MonoBehaviour
    {
        public Vector2 camerarot;
        private Camera cameraObject;
        private const float rotSpeed = 70f;

        private void Awake()
        {
            cameraObject = Camera.main;

            // 씬에 설정된 초기값을 덮어쓰지 않기 위해 현재 Transform에서 초기 camerarot 동기화
            if (cameraObject != null)
            {
                float pitch = cameraObject.transform.localEulerAngles.x;
                if (pitch > 180f) pitch -= 360f;
                camerarot.x = pitch;
            }

            float yaw = transform.localEulerAngles.y;
            if (yaw > 180f) yaw -= 360f;
            camerarot.y = yaw;
        }

        private void Update()
        {
            if (cameraObject == null) return;
            if (EventPlayManager.Instance != null && EventPlayManager.Instance.isLockCamera) return;

            float mx = Input.GetAxis("Mouse X");
            float my = Input.GetAxis("Mouse Y");

            float yawDelta = mx * rotSpeed * Time.deltaTime;
            float pitchDelta = my * rotSpeed * Time.deltaTime;

            // 마우스의 Yaw는 플레이어(루트)에 적용 -> 프리팹에서 설정한 Y(예: 90도) 유지됨
            transform.Rotate(0f, yawDelta, 0f, Space.Self);

            // Pitch는 카메라의 로컬 회전으로 처리하여 카메라가 위아래로만 움직이게 함
            float pitch = cameraObject.transform.localEulerAngles.x;
            if (pitch > 180f) pitch -= 360f;
            pitch -= pitchDelta;
            pitch = Mathf.Clamp(pitch, -80f, 80f);
            cameraObject.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);

            // public 필드 동기화 (외부에서 참조할 경우)
            camerarot.x = pitch;
            float yaw = transform.localEulerAngles.y;
            if (yaw > 180f) yaw -= 360f;
            camerarot.y = yaw;
        }
    }
}