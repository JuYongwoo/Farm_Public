using JYW.Game.EventPlay;
using UnityEngine;

namespace JYW.Game.Scenes
{
    public class GameOverScene : MonoBehaviour
    {
        [SerializeField] private EventSO eventSO;

        private void Start()
        {
            EventPlayManager.Instance.PlayEvent(eventSO, gameObject);
        }
    }
}