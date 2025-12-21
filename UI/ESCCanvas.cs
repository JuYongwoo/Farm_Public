using UnityEngine;
using UnityEngine.UI;

public class ESCCanvas : MonoBehaviour {
    [SerializeField] Button backBtn;
    [SerializeField] Button titleBtn;

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

    private void Awake()
    {
        backBtn.gameObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });
        titleBtn.gameObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
        });
    }

}
