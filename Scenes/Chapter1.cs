using JYW.Game.SOs;
using JYW.Game.Managers.TriggerSystem;
using UnityEngine;

namespace JYW.Game.Managers
{
    public class Chapter1 : MonoBehaviour
    {
        [SerializeField]
        private EventSO eventSO;

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        private void Start()
        {
            EventPlayManager.Instance.PlayEvent(eventSO);
        }
    }
}