using JYW.Game.EventPlay;
using UnityEngine;

public class Battery : MonoBehaviour
{

    private void Start()
    {
        EventPlayManager.Instance.AddAction(gameObject, GetBattery);
    }

    private void GetBattery()
    {
        EventPlayManager.Instance.PlayAction("FlashLight");
    }

}
