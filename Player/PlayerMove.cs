using JYW.Game.Managers.TriggerSystem;
using UnityEngine;

namespace JYW.Game.Players
{
    public class PlayerMove : MonoBehaviour
    {
        [SerializeField]
        private float Playerspeed = 2.5f;
        private CharacterController charCtrl;

        private void Awake()
        {
            charCtrl = GetComponent<CharacterController>();
        }

        private void FixedUpdate()
        {
            transform.eulerAngles = new Vector3(0, Camera.main.transform.eulerAngles.y, 0);
            if (!EventPlayManager.Instance.isLockMove) charCtrl.SimpleMove(GetDirection() * Playerspeed);
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



    }
}