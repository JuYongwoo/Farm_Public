using JYW.Game.EventPlay;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private float openDuration = 0.2f; // 열고 닫는 데 걸리는 시간
    [SerializeField] private float openRotation = 90f;  // 회전 각도
    private bool isOpen = false;                        // 문이 열려 있는 상태인지 여부
    private Quaternion closedRot;                       // 닫힌 상태의 회전값
    private Quaternion openedRot;                       // 열린 상태의 회전값
    private Coroutine routine;                          // 중복 실행 방지용 코루틴 참조

    private void Awake()
    {
        closedRot = transform.localRotation;                                      // 현재 회전을 닫힌 회전으로 저장
        openedRot = closedRot * Quaternion.Euler(0f, openRotation, 0f);           // 열린 회전값 계산
    }

    public void Start()
    {
        EventPlayManager.Instance.AddAction(gameObject, OpenOrClose);
    }

    public void DisableCollider()
    {
        GetComponentInChildren<Collider>().enabled = false;
    }

    public void OpenOrClose()
    {
        if (routine != null) StopCoroutine(routine);                              // 기존 회전 코루틴 중지
        routine = StartCoroutine(RotateDoor());                                   // 새 코루틴 시작
    }

    private System.Collections.IEnumerator RotateDoor()
    {
        float elapsed = 0f;
        Quaternion start = transform.localRotation;                               // 시작 회전값
        Quaternion end = isOpen ? closedRot : openedRot;                          // 목표 회전값

        while (elapsed < openDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / openDuration);
            transform.localRotation = Quaternion.Slerp(start, end, t);            // 보간 회전
            yield return null;
        }

        transform.localRotation = end;                                            // 정확한 최종값 적용
        isOpen = !isOpen;                                                         // 상태 반전
        routine = null;                                                           // 코루틴 해제
    }
}
