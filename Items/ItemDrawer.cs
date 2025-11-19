using JYW.Game.Commons;
using JYW.Game.SOs;
using JYW.Game.Managers.TriggerSystem;
using System;
using UnityEngine;

namespace JYW.Game.Items
{
    public class ItemDrawer : MonoBehaviour
    {
        public EventSO thisEventSO;
        public KeyCode interactionKey; //상호작용 키
        public string isAimText = ""; //조준시 나오는 텍스트
        public string requiredItem = ""; //필요 아이템
        public bool isOneTimeUse = false;
        private bool isOpened = false;

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
            if (!isOpened) gameObject.transform.Translate(0, 0, 0.6f);
            else gameObject.transform.Translate(0, 0, -0.6f);
            isOpened = !isOpened;

            EventPlayManager.Instance.PlayEvent(thisEventSO);
            requiredItem = ""; //문을 한번 열었기 때문에 언제든 열 수 있도록 필요 아이템 삭제

            if(isOneTimeUse) GetComponent<Collider>().enabled = false; //한번 열고 고정일 경우 콜라이더 삭제
        }
    }
}