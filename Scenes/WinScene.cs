using JYW.Game.EventPlay;
using UnityEngine;

public class WinScene : MonoBehaviour
{
    [SerializeField] private EventSO eventSO;

    void Start()
    {
        if (eventSO != null)
        {
            EventPlayManager.Instance.PlayEvent(eventSO, gameObject);
        }
    }

    void Update()
    {
        
    }
}
