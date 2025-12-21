using JYW.Game.EventPlay;
using UnityEngine;
using UnityEngine.Video;

public class MiniTV : MonoBehaviour
{

    private bool isOn = false;

    [SerializeField]private Material tvOn;
    [SerializeField]private Material tvOff;

    private void Start()
    {
        EventPlayManager.Instance.AddAction(gameObject, TVOnOff);
    }

    private void TVOnOff()
    {
        if (isOn)
        {
            GetComponent<Renderer>().material = tvOff;
            isOn = false;
            EventPlayManager.Instance.SetGlobals(gameObject, 0);
        }
        else
        {
            GetComponent<Renderer>().material = tvOn;
            isOn = true;
            EventPlayManager.Instance.SetGlobals(gameObject, 1);
        }

    }

}
