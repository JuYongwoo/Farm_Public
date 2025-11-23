using UnityEngine;

public class UICameraBind : MonoBehaviour
{

    private void Awake()
    {
        Canvas canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
        canvas.planeDistance = 1;
    }
}
