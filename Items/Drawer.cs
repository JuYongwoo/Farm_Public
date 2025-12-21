using JYW.Game.EventPlay;
using UnityEngine;

public class Drawer : MonoBehaviour
{
    [SerializeField] private Vector3 moveDistance;  // 서랍 이동 거리(로컬 좌표 기준)
    [SerializeField] private float moveDuration = 0.2f;  // 이동 시간
    private bool isOpen = false;                         // 열린 상태 여부
    private Vector3 closedPos;                           // 닫힌 위치
    private Vector3 openedPos;                           // 열린 위치
    private Coroutine routine;                           // 중복 실행 방지용 코루틴 참조

    private void Awake()
    {
        closedPos = transform.localPosition;                         // 현재 위치 저장
        openedPos = closedPos + moveDistance;     // 앞으로 이동할 위치 계산
    }

    private void Start()
    {
        EventPlayManager.Instance.AddAction(gameObject, OpenOrClose); // 이벤트 등록
    }

    public void OpenOrClose()
    {
        if (routine != null) StopCoroutine(routine);                  // 기존 이동 코루틴 중지
        routine = StartCoroutine(MoveDrawer());                       // 새 코루틴 시작
    }

    private System.Collections.IEnumerator MoveDrawer()
    {
        float elapsed = 0f;
        Vector3 start = transform.localPosition;                      // 시작 위치
        Vector3 end = isOpen ? closedPos : openedPos;                  // 목표 위치

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);
            transform.localPosition = Vector3.Lerp(start, end, t);     // 보간 이동
            yield return null;
        }

        transform.localPosition = end;                                // 정확한 최종 위치 적용
        isOpen = !isOpen;                                             // 상태 반전
        routine = null;                                               // 코루틴 해제
    }
}
