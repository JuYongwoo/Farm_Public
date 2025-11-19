using JYW.Game.Managers.TriggerSystem;
using UnityEngine;

namespace JYW.Game.Players
{

    public class PlayerCamera : MonoBehaviour
    {
        public Vector2 camerarot;
        private Camera cameraObject;
        private const float rotSpeed = 70f;


        private void Awake()
        {
            cameraObject = GetComponentInChildren<Camera>();
        }

        private void Update()
        {

            if (EventPlayManager.Instance.isLockCamera) return;

            float mx = Input.GetAxis("Mouse X");
            float my = Input.GetAxis("Mouse Y");

            camerarot.y += mx * rotSpeed * Time.deltaTime;
            camerarot.x -= my * rotSpeed * Time.deltaTime;
            camerarot.x = Mathf.Clamp(camerarot.x, -80f, 80f);

            cameraObject.transform.rotation = Quaternion.Euler(camerarot.x, camerarot.y, 0f);


        }
    }
}