using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryUIScroll : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;          // 연결되면 verticalNormalizedPosition 사용
    [SerializeField] private Scrollbar scrollbar;            // ScrollRect 없을 때 직접 제어
    [SerializeField] private float wheelStep = 0.12f;         // 한 번의 휠 델타당 이동 비율
    [SerializeField] private bool invert = false;             // 방향 반전 필요 시
    [SerializeField] private bool onlyWhenPointerOver = true; // 마우스가 Viewport 위에 있을 때만

    private RectTransform viewportRT;

    private void Awake()
    {
        if (scrollRect == null)
            scrollRect = GetComponentInChildren<ScrollRect>(true);

        if (scrollbar == null)
        {
            if (scrollRect != null)
                scrollbar = scrollRect.verticalScrollbar;
            if (scrollbar == null)
                scrollbar = GetComponentInChildren<Scrollbar>(true);
        }

        if (scrollRect != null)
            viewportRT = scrollRect.viewport;
    }

    private void Update()
    {
        float wheel = Input.mouseScrollDelta.y;
        if (Mathf.Abs(wheel) < 0.0001f) return;

        // 영역 위 조건
        if (onlyWhenPointerOver && viewportRT != null)
        {
            if (!RectTransformUtility.RectangleContainsScreenPoint(
                    viewportRT, Input.mousePosition, null))
                return;
        }

        float dir = invert ? -wheel : wheel; // Unity: 위로 휠 = +값
        float delta = dir * wheelStep;

        if (scrollRect != null)
        {
            // 위로 스크롤 시 콘텐츠 위로 → normalizedPosition 증가 (UI 구조에 따라 반전 필요)
            scrollRect.verticalNormalizedPosition = Mathf.Clamp01(
                scrollRect.verticalNormalizedPosition + delta
            );
            // ScrollRect가 Scrollbar를 자동 갱신
        }
        else if (scrollbar != null)
        {
            scrollbar.value = Mathf.Clamp01(scrollbar.value + delta);
        }
    }

    // 외부에서 강제 새로고침(예: 아이템 추가 후)
    public void ScrollToTop()
    {
        if (scrollRect != null) scrollRect.verticalNormalizedPosition = 1f;
        else if (scrollbar != null) scrollbar.value = 1f;
    }

    public void ScrollToBottom()
    {
        if (scrollRect != null) scrollRect.verticalNormalizedPosition = 0f;
        else if (scrollbar != null) scrollbar.value = 0f;
    }
}