using JYW.Game.EventPlay;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverVideoCanavas : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private Button skipButton;
    [SerializeField] private string changeSceneName;
    private void Awake()
    {
        skipButton.onClick.AddListener(ToTitle);
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void ToTitle()
    {
        SceneManager.LoadScene(changeSceneName);
    }
}
