using UnityEngine;

namespace JYW.Game.Scenes
{
    public class TitleScene : MonoBehaviour
    {
        private void Awake()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

        }
    }
}