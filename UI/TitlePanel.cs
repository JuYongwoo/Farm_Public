using JYW.Game.EventPlay;
using JYW.Game.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace JYW.Game.UIs
{
    public class TitlePanel : MonoBehaviour
    {
        private enum TitlePanelObjsEnum
        {
            StartButton,
            SettingsButton,
            GameExitButton,
            DeleteButton
        }

        private Dictionary<TitlePanelObjsEnum, GameObject> titleUIObjsMap;


        private void Start()
        {
            titleUIObjsMap = Util.MapEnumChildObjects<TitlePanelObjsEnum, GameObject>(this.gameObject);
            titleUIObjsMap[TitlePanelObjsEnum.StartButton].GetComponent<Button>().onClick.AddListener(() =>
            {
                SceneManager.LoadScene("Chapter1");
            });
            titleUIObjsMap[TitlePanelObjsEnum.SettingsButton].GetComponent<Button>().onClick.AddListener(() =>
            {
                EventPlayManager.Instance.PlayAction("SettingsPanel");
            });
            titleUIObjsMap[TitlePanelObjsEnum.GameExitButton].GetComponent<Button>().onClick.AddListener(() =>
            {
                Application.Quit();
            });
            titleUIObjsMap[TitlePanelObjsEnum.DeleteButton].GetComponent<Button>().onClick.AddListener(() =>
            {
                PlayerPrefs.DeleteAll();
            });

            foreach (var trs in titleUIObjsMap)
            {
                AttachHoverEvents(trs.Value.transform);
            }

        }

        private void AttachHoverEvents(Transform trs)
        {
            var trigger = Util.AddOrGetComponent<EventTrigger>(trs.gameObject);
            trigger.triggers ??= new List<EventTrigger.Entry>();

            AddTrigger(trigger, EventTriggerType.PointerEnter, () =>
            {
                var text = trs.GetComponentInChildren<Text>();
                if (text) text.color = Color.red;
            });

            AddTrigger(trigger, EventTriggerType.PointerExit, () =>
            {
                var text = trs.GetComponentInChildren<Text>();
                if (text) text.color = Color.white;
            });
        }

        private void AddTrigger(EventTrigger trigger, EventTriggerType type, UnityAction action)
        {
            var entry = new EventTrigger.Entry { eventID = type };
            entry.callback.AddListener(_ => action());
            trigger.triggers.Add(entry);
        }

    }
}