using UnityEngine;

public class CameraFilter : MonoBehaviour
{
    [SerializeField]
    public Material mat;                          // Pixelate_Mat 연결

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, mat);            // 셰이더 적용해서 화면 그리기
    }
}
