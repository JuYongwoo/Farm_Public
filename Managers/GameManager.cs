using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private const int targetAspectWidth = 16;
    private const int targetAspectHeight = 9;

    private int lastWidth;
    private int lastHeight;

    private float resizeDetectTimer;
    private const float resizeApplyDelay = 0.1f; // 핵심: 리사이즈 종료 대기 시간

    private enum ResizeAxis { None, Width, Height }
    private ResizeAxis resizeAxis = ResizeAxis.None;

    protected override void Awake()
    {
        base.Awake();

        lastWidth = Screen.width;
        lastHeight = Screen.height;
    }

    void Update()
    {
        bool widthChanged = Screen.width != lastWidth;
        bool heightChanged = Screen.height != lastHeight;

        // 해상도 변경 감지
        if (widthChanged || heightChanged)
        {
            // 최초 변경 시 어떤 축이 변경되었는지 기록
            if (resizeAxis == ResizeAxis.None)
            {
                if (widthChanged && !heightChanged) resizeAxis = ResizeAxis.Width;
                else if (!widthChanged && heightChanged) resizeAxis = ResizeAxis.Height;
                else resizeAxis = ResizeAxis.None; // 양축 동시 변경이면 축 고정 없이 일반 로직
            }

            // 변경이 발생하면 타이머 리셋
            resizeDetectTimer = resizeApplyDelay;

            lastWidth = Screen.width;
            lastHeight = Screen.height;
            return;
        }

        // 변경이 멈춘 뒤에만 보정
        if (resizeDetectTimer > 0f)
        {
            resizeDetectTimer -= Time.unscaledDeltaTime;

            if (resizeDetectTimer <= 0f)
            {
                ApplyAspectRatioOnce();
                // 적용 후 축 기록 초기화
                resizeAxis = ResizeAxis.None;
            }
        }
    }

    private void ApplyAspectRatioOnce()
    {
        int currentWidth = Screen.width;
        int currentHeight = Screen.height;

        float targetAspect = (float)targetAspectWidth / targetAspectHeight;

        int newWidth = currentWidth;
        int newHeight = currentHeight;

        // 한 축만 변경된 경우: 변경된 축은 유지하고 반대 축을 계산하여 16:9 맞춤
        if (resizeAxis == ResizeAxis.Width)
        {
            newWidth = currentWidth;
            newHeight = Mathf.RoundToInt(currentWidth / targetAspect);
        }
        else if (resizeAxis == ResizeAxis.Height)
        {
            newHeight = currentHeight;
            newWidth = Mathf.RoundToInt(currentHeight * targetAspect);
        }
        else
        {
            // 양축 동시 변경 또는 축을 알 수 없을 때: 기존 방식으로 가장 가까운 16:9로 보정
            float windowAspect = (float)currentWidth / currentHeight;
            if (windowAspect > targetAspect)
            {
                // 가로가 더 긴 상태 → 세로 기준으로 가로 계산
                newHeight = currentHeight;
                newWidth = Mathf.RoundToInt(newHeight * targetAspect);
            }
            else
            {
                // 세로가 더 긴 상태 → 가로 기준으로 세로 계산
                newWidth = currentWidth;
                newHeight = Mathf.RoundToInt(newWidth / targetAspect);
            }
        }

        // 이미 맞아 있으면 아무것도 안 함
        if (currentWidth == newWidth && currentHeight == newHeight)
            return;

        Screen.SetResolution(newWidth, newHeight, false);

        lastWidth = newWidth;
        lastHeight = newHeight;
    }
}