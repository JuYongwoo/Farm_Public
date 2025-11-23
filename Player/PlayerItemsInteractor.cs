using JYW.Game.Commons;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace JYW.Game.Players
{
    public class PlayerItemsInteractor : MonoBehaviour
    {
        [HideInInspector]
        public PlayerInventory playerInventory;

        // 공용 힌트/조작 UI
        private GameObject hoverUI;
        private Text hoverText;

        // 현재 바라보는 대상
        private GameObject currentTarget;
        private AimingIntractionPrompt currentPrompt;

        // 선택된 컴포넌트와 단일 델리게이트(통합)
        private MonoBehaviour selectedComponent;
        private Func<object, AimingIntractionPrompt> beforeFunc;   // 프롬프트 획득용 (있다면)
        private Action<object> currentAction;                // 실행용 통합 델리게이트

        private void Start()
        {
            playerInventory = GetComponent<PlayerInventory>();
            if (playerInventory == null)
            {
                Debug.LogWarning("[PlayerItemsInteractor] PlayerInventory 컴포넌트를 찾을 수 없습니다.");
            }
        }

        private void Update()
        {
            Camera cam = Camera.main;
            if (cam == null) return;

            Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1.5f))
            {
                GameObject hitGO = hit.collider.gameObject;

                // 대상이 바뀌었으면 새로 탐색
                if (currentTarget != hitGO)
                {
                    ResetSelection();
                    ClearHoverAndState(); //일단 대상이 바뀌었기 때문에 Hover UI 리셋

                    currentTarget = hitGO;

                    var comps = hitGO.GetComponents<MonoBehaviour>();
                    foreach (var comp in comps) //씬에 존재하는 게임오브젝트들은 평균 0~1개의 Monobehaviour컴포넌트를 가지기 때문에 성능상 큰 문제는 없음
                    {
                        if (comp == null) continue;
                        var t = comp.GetType();

                        var beforeField = t.GetField("beforeInteractAction", BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                        var curField = t.GetField("currentInteractAction", BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

                        if (beforeField != null)
                        {
                            var beforeValue = beforeField.GetValue(comp);

                            if (beforeValue is Delegate beforeDel)
                            {
                                selectedComponent = comp;

                                beforeFunc = (ctx) =>
                                {
                                    try
                                    {
                                        var res = beforeDel.DynamicInvoke(new object[] { ctx });
                                        return res as AimingIntractionPrompt;
                                    }
                                    catch
                                    {
                                        return null;
                                    }
                                };
                            }
                        }
                        if (curField != null)
                        {
                            var curValue = curField.GetValue(comp);

                            if (curValue is Delegate currentDel)
                            {
                                currentAction = (ctx) =>
                                {
                                    try
                                    {
                                        currentDel.DynamicInvoke(new object[] { ctx });
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.LogWarning(
                                            $"[PlayerItemsInteractor] currentInteractAction 예외: {ex.Message}");
                                    }
                                };
                            }

                            // current를 찾았으면 더 깊게 돌 필요 없으니 break
                            break;
                        }


                    } // foreach

                    // 프롬프트 결정
                    if (currentPrompt == null)
                    {
                        currentPrompt = ResolvePromptFromSelected();
                    }

                    EnsureHoverUIExists();
                    UpdateHoverText(currentPrompt);
                } // target changed

                // 입력 처리
                if (selectedComponent != null && currentPrompt != null && Input.GetKeyDown(currentPrompt.interactionKey))
                {
                    string requiredName = currentPrompt.requiredItem;
                    bool hasKey = playerInventory != null && (string.IsNullOrEmpty(requiredName) || playerInventory.CheckExist(requiredName));

                    if (!hasKey)
                    {
                        if (hoverText != null) hoverText.text = "필요한 아이템이 없습니다";
                    }
                    else
                    {
                        // 단일 델리게이트 호출
                        if (currentAction != null)
                        {
                            try
                            {
                                currentAction.Invoke(this);
                            }
                            catch (Exception ex)
                            {
                                Debug.LogWarning($"[PlayerItemsInteractor] interactionDelegate 예외: {ex.Message}");
                            }
                        }
                        else
                        {
                            Debug.LogWarning("[PlayerItemsInteractor] 실행할 델리게이트나 메서드를 찾지 못했습니다.");
                        }

                        // 아이템 소모 및 상태 초기화
                        if (!string.IsNullOrEmpty(requiredName) && playerInventory != null)
                        {
                            playerInventory.RemoveObject(requiredName);
                        }

                        ClearHoverAndState(); //상호작용 후 초기화
                    }
                }//current 액션 실행

                return;
            } // ray hit
            ClearHoverAndState(); //레이캐스트가 맞지 않을 때 초기화
        }

        private AimingIntractionPrompt ResolvePromptFromSelected()
        {
            if (selectedComponent == null) return null;

            // beforeFunc 우선
            if (beforeFunc != null)
            {
                try
                {
                    var res = beforeFunc.Invoke(null);
                    if (res != null) return res;
                }
                catch { }
            }

            // 메서드 폴백으로 BeforeInteractAction 직접 호출
            var t = selectedComponent.GetType();
            var beforeMethod = t.GetMethod("BeforeInteractAction", BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            if (beforeMethod != null)
            {
                try
                {
                    var res = beforeMethod.Invoke(selectedComponent, new object[] { null });
                    if (res is AimingIntractionPrompt p) return p;
                }
                catch { }
            }

            // 필드 폴백
            try
            {
                var prompt = new AimingIntractionPrompt();
                var aimField = t.GetField("isAimText", BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                if (aimField != null) prompt.promptText = aimField.GetValue(selectedComponent) as string;

                var keyField = t.GetField("interactionKey", BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                if (keyField != null) prompt.interactionKey = (KeyCode)(keyField.GetValue(selectedComponent) ?? KeyCode.E);

                var reqField = t.GetField("requiredItem", BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                if (reqField != null) prompt.requiredItem = reqField.GetValue(selectedComponent) as string;

                return prompt;
            }
            catch
            {
                return new AimingIntractionPrompt { promptText = "", interactionKey = KeyCode.E, requiredItem = "" };
            }
        }

        private void EnsureHoverUIExists() //단순 글자를 띄우는 것이기 때문에 별도 프리팹 없이 코드로 생성
        {
            if (hoverUI != null) return;

            hoverUI = new GameObject("ItemsInteractHoverUI");
            Canvas canvas = hoverUI.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            hoverUI.AddComponent<CanvasScaler>();
            hoverUI.AddComponent<GraphicRaycaster>();
            DontDestroyOnLoad(hoverUI);

            GameObject textGO = new GameObject("InteractText");
            textGO.transform.SetParent(hoverUI.transform, false);
            hoverText = textGO.AddComponent<Text>();

            Font runtimeFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (runtimeFont == null)
            {
                runtimeFont = Font.CreateDynamicFontFromOSFont("Arial", 36);
                Debug.LogWarning("[PlayerItemsInteractor] LegacyRuntime.ttf not found. Using dynamic OS Arial fallback.");
            }

            hoverText.font = runtimeFont;
            hoverText.fontSize = 36;
            hoverText.alignment = TextAnchor.MiddleCenter;
            hoverText.color = Color.white;
            hoverText.horizontalOverflow = HorizontalWrapMode.Wrap;
            hoverText.verticalOverflow = VerticalWrapMode.Overflow;
            hoverText.raycastTarget = false;

            RectTransform rt = hoverText.rectTransform;
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(0f, -Screen.height * 0.18f);
            rt.sizeDelta = new Vector2(600f, 100f);
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
            if (hoverUI != null)
            {
                Destroy(hoverUI);
                hoverUI = null;
                hoverText = null;
            }

            currentTarget = null;
            currentPrompt = null;
            selectedComponent = null;
            beforeFunc = null;
            currentAction = null;
        }

        private void ResetSelection()
        {
            selectedComponent = null;
            beforeFunc = null;
            currentAction = null;
        }
    }
}
