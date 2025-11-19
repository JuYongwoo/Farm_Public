using JYW.Game.SOs;
using JYW.Game.Commons;
using JYW.Game.Managers.TriggerSystem;
using JYW.Game.Players;
using System;
using UnityEngine;

namespace JYW.Game.Items
{
    public class ItemCollectable : MonoBehaviour
    {
        public EventSO thisEventSO;
        public KeyCode interactionKey; //상호작용 키
        public string isAimText = ""; //조준시 나오는 텍스트
        public string requiredItem = ""; //필요 아이템

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

            // obj를 PlayerItemsInteractor로 캐스팅하여 인벤토리에 추가
            var interactor = obj as PlayerItemsInteractor;
            if (interactor != null)
            {
                var inventory = interactor.playerInventory;
                if (inventory != null)
                {
                    inventory.AddInventory(gameObject);
                }
                else
                {
                    Debug.LogWarning("[ItemObjectCollectable] PlayerInventory를 찾을 수 없습니다. 아이템을 인벤토리에 추가하지 못했습니다.");
                }

            }
            gameObject.SetActive(false);
        }

    }
}