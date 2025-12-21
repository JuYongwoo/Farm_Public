using JYW.Game.EventPlay;
using System;
using UnityEngine;
using UnityEngine.UI;
// using TMPro; // TextMeshProUGUI 사용 시 주석 해제

public class PasswordCanvas : MonoBehaviour
{
    [SerializeField] EventSO completeSO;
    // 숫자 범위 (순환)
    [SerializeField] private int minValue = 0;
    [SerializeField] private int maxValue = 9;

    [SerializeField] private Button upBtn1;
    [SerializeField] private Button downBtn1;
    [SerializeField] private Button upBtn2;
    [SerializeField] private Button downBtn2;
    [SerializeField] private Button upBtn3;
    [SerializeField] private Button downBtn3;
    [SerializeField] private Button upBtn4;
    [SerializeField] private Button downBtn4;
    [SerializeField] private Button confirmBtn;
    [SerializeField] private Button cancelBtn;

    [SerializeField] private int[] password = new int[4];

    private void Awake()
    {
        // 씬에 이미 다른 PasswordCanvas가 있으면(비활성 포함) 새로 생성된 이 인스턴스를 파괴
        var others = FindObjectsOfType<PasswordCanvas>(true);
        for (int i = 0; i < others.Length; i++)
        {
            var o = others[i];
            if (o != null && o != this)
            {
                Destroy(o.gameObject); //기존거 지워버리고 시작
            }
        }

        upBtn1.onClick.AddListener(() => ChangeNum("Image/1/1_NUM", +1));
        downBtn1.onClick.AddListener(() => ChangeNum("Image/1/1_NUM", -1));
        upBtn2.onClick.AddListener(() => ChangeNum("Image/2/2_NUM", +1));
        downBtn2.onClick.AddListener(() => ChangeNum("Image/2/2_NUM", -1));
        upBtn3.onClick.AddListener(() => ChangeNum("Image/3/3_NUM", +1));
        downBtn3.onClick.AddListener(() => ChangeNum("Image/3/3_NUM", -1));
        upBtn4.onClick.AddListener(() => ChangeNum("Image/4/4_NUM", +1));
        downBtn4.onClick.AddListener(() => ChangeNum("Image/4/4_NUM", -1));
        confirmBtn.onClick.AddListener(() => Confirm());
        cancelBtn.onClick.AddListener(() => Cancel());
    }

    private void OnEnable()
    {
        Time.timeScale = 0f; // 게임 일시정지
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OnDisable()
    {
        Time.timeScale = 1f; // 게임 재개
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void BindButton(string path, Action onClick)
    {
        var t = transform.Find(path);
        if (t == null)
        {
            Debug.LogWarning($"[PasswordCanvas] 경로를 찾을 수 없습니다: {path}");
            return;
        }
        var btn = t.GetComponent<Button>();
        if (btn == null)
        {
            Debug.LogWarning($"[PasswordCanvas] Button 컴포넌트가 없습니다: {path}");
            return;
        }
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => onClick?.Invoke());
    }

    private void ChangeNum(string numPath, int delta)
    {
        var t = transform.Find(numPath);
        if (t == null)
        {
            Debug.LogWarning($"[PasswordCanvas] NUM 경로를 찾을 수 없습니다: {numPath}");
            return;
        }

        // UnityEngine.UI.Text 사용 시
        var uiText = t.GetComponent<Text>();
        if (uiText != null)
        {
            int cur = ParseDigit(uiText.text);
            int next = Wrap(cur + delta, minValue, maxValue);
            uiText.text = next.ToString();
            return;
        }

        // TextMeshProUGUI 사용 시
        // var tmp = t.GetComponent<TextMeshProUGUI>();
        // if (tmp != null)
        // {
        //     int cur = ParseDigit(tmp.text);
        //     int next = Wrap(cur + delta, minValue, maxValue);
        //     tmp.text = next.ToString();
        //     return;
        // }

        Debug.LogWarning($"[PasswordCanvas] 텍스트 컴포넌트를 찾을 수 없습니다: {numPath}");
    }

    private int ParseDigit(string s)
    {
        if (int.TryParse(s, out var v)) return v;
        return 0;
    }

    private int Wrap(int value, int min, int max)
    {
        int range = (max - min + 1);
        int v = (value - min) % range;
        if (v < 0) v += range;
        return min + v;
    }

    private void Confirm()
    {
        if (transform.Find("Image/1/1_NUM").GetComponent<Text>().text == password[0].ToString()
            && transform.Find("Image/2/2_NUM").GetComponent<Text>().text == password[1].ToString()
            && transform.Find("Image/3/3_NUM").GetComponent<Text>().text == password[2].ToString()
            && transform.Find("Image/4/4_NUM").GetComponent<Text>().text == password[3].ToString())
        {
            EventPlayManager.Instance.PlayEvent(completeSO, gameObject);
            EventPlayManager.Instance.GetSpawnCaller(gameObject).GetComponent<Collider>().enabled = false; //해당 문 더이상 못누르게
        }
        Destroy(gameObject);
    }

    private void Cancel()
    {
        Destroy(gameObject);
    }
}