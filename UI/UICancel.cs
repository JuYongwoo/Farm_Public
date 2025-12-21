using UnityEngine;

public class UICancel : MonoBehaviour
{
    public KeyCode cancelKey;
    private void Update()
    {
        if (Input.GetKeyDown(cancelKey))
        {
            gameObject.SetActive(false);
        }
    }
}
