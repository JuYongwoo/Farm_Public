using UnityEngine;
using UnityEngine.UI;

public class PlayerAimUI : MonoBehaviour
{
    private GameObject _aimRoot;
    private Text _aimText;

    private void Start()
    {
        EnsureAimUIExists();
    }


    private void EnsureAimUIExists() //간단한 조준 UI이기 때문에 별도 프리팹 없이 코드로 생성
    {

        var canvasGO = new GameObject("PlayerAim_Canvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasGO.AddComponent<GraphicRaycaster>();
        canvas.sortingOrder = 1000; // 항상 위에 보이도록

        // 루트 오브젝트
        _aimRoot = new GameObject("PlayerAimUI");
        _aimRoot.transform.SetParent(canvas.transform, false);

        var rt = _aimRoot.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = new Vector2(50f, 50f);

        // 텍스트
        _aimText = _aimRoot.AddComponent<Text>();
        _aimText.text = "+";
        // 변경: Unity 2021+에서 Arial.ttf 사용 시 ArgumentException 발생 -> LegacyRuntime.ttf 사용
        _aimText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        _aimText.fontSize = 24;
        _aimText.alignment = TextAnchor.MiddleCenter;
        _aimText.color = Color.white;
        _aimText.raycastTarget = false;

        // 항상 보이게 활성화 상태로 둠
        _aimRoot.SetActive(true);
    }

    // 외부에서 켜고 끌 수 있는 공개 함수
    public void SetAimVisible(bool visible)
    {
        if (_aimRoot == null) EnsureAimUIExists();
        _aimRoot.SetActive(visible);
    }

    // 토글 편의 메서드
    public void ToggleAim()
    {
        if (_aimRoot == null) EnsureAimUIExists();
        _aimRoot.SetActive(!_aimRoot.activeSelf);
    }
}
