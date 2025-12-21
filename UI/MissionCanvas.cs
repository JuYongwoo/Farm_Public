using JYW.Game.EventPlay;
using UnityEngine;
using UnityEngine.UI;

public class MissionCanvas : MonoBehaviour
{
    private Text text;

    private void Awake()
    {
        if (text == null)
            text = GetComponentInChildren<Text>();

            DisableText();
    }

    private void Start()
    {
        EventPlayManager.Instance.AddAction(gameObject, UpdateText);
    }

    private void UpdateText()
    {
        if(!text.gameObject.activeSelf) text.gameObject.SetActive(true);

        if ((int)EventPlayManager.Instance.GetGlobals("기름통") == 1)
        {
            text.text = $"차로 돌아가라";

        }
        else
        {

            text.text = $"기름을 찾아라";
        }


    }

    private void DisableText()
    {
        text.gameObject.SetActive(false);
    }



}
