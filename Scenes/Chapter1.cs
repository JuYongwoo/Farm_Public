using JYW.Game.EventPlay;
using UnityEngine;

namespace JYW.Game.Scenes
{
    public class Chapter1 : MonoBehaviour
    {
        [SerializeField]
        private EventSO startEventSO;

        [SerializeField]
        private Canvas escCanvas;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            EventPlayManager.Instance.PlayEvent(startEventSO, gameObject);
            escCanvas.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                escCanvas.gameObject.SetActive(true);
            }
        }
    }
}