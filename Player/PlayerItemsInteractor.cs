using JYW.Game.Commons;
using JYW.Game.EventPlay;
using UnityEngine;
using UnityEngine.UI;

namespace JYW.Game.Players
{
    public class PlayerItemsInteractor : MonoBehaviour
    {

        [SerializeField]
        private Text hoverText;

        // 현재 바라보는 대상
        private GameObject currentTarget;
        private AimingIntractionPrompt currentPrompt;

        IInteractable interact;

        private void Start()
        {
            EventPlayManager.Instance.AddAction(gameObject, GameOver);
        }

        private void GameOver()
        {
            GetComponentInChildren<Collider>().enabled = false;
            gameObject.tag = "GameOver";
            this.enabled = false;
        }

        private void Update()
        {
            Camera cam = Camera.main;
            if (cam == null) return;

            Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 2f))
            {
                GameObject hitGO = hit.collider.gameObject;

                if (currentTarget != hitGO)// 대상이 바뀌었을때 1회 실행
                {
                    ClearHoverAndState();

                    currentTarget = hitGO;
                    interact = hitGO.GetComponent<IInteractable>();


                    if (interact != null)
                    {
                        currentPrompt = interact.AimingIntractionPrompt;

                        //EnsureHoverUIExists();
                        UpdateHoverText(currentPrompt);
                    }
                }

                if (interact != null) //ItemCommon이 null이 아닌 이상 계속실행(키입력 체크 등)
                {
                    // 입력 처리
                    if (Input.GetKeyDown(currentPrompt.interactionKey))
                    {
                        interact.Interact();
                    }//current 액션 실행
                }

                return;
            } // ray hit

            ClearHoverAndState(); //레이캐스트가 맞지 않을 때 초기화
        }


        private void UpdateHoverText(AimingIntractionPrompt prompt)
        {
            if (hoverText == null || prompt == null) return;
            var keyName = prompt.interactionKey.ToString();
            hoverText.text = $"{prompt.promptText}\n[{keyName}]";
            hoverText.enabled = true;
        }

        private void ClearHoverAndState()
        {
            if (hoverText != null)
            {
                hoverText.text = "";
            }

            currentTarget = null;
            currentPrompt = null;
        }

    }
}
