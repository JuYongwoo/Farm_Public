using JYW.Game.Commons;
using JYW.Game.SOs;
using JYW.Game.Managers.TriggerSystem;
using System;
using UnityEngine;

namespace JYW.Game.Items
{
    public class ItemInteractable : MonoBehaviour
    {
        public EventSO thisEventSO;
        public KeyCode interactionKey; //상호작용 키
        public string isAimText = ""; //조준시 나오는 텍스트
        public string requiredItem = ""; //필요 아이템
        public bool isUseOneTime = false;

        public Func<AimingIntractionPrompt, object> beforeInteractAction;
        public Action<object> currentInteractAction;

        public void Awake()
        {
            beforeInteractAction = BeforeInteractAction;
            currentInteractAction = CurrentInteractAction;
        }

        public AimingIntractionPrompt BeforeInteractAction(object obj)
        {
            AimingIntractionPrompt aimingIntractionPrompt = new AimingIntractionPrompt();

            aimingIntractionPrompt.promptText = isAimText;
            aimingIntractionPrompt.interactionKey = interactionKey;
            aimingIntractionPrompt.requiredItem = requiredItem;

            return aimingIntractionPrompt;

        }

        public void CurrentInteractAction(object obj) //Before에서 받은 키를 누르면 실행하는 이벤트
        {
            EventPlayManager.Instance.PlayEvent(thisEventSO);
            if(isUseOneTime) GetComponent<Collider>().enabled = false; //다시 조준되지 않도록 콜라이더 삭제
        }

    }
}