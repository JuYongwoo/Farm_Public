// Assets/Editor/ChangeAllMaterialsShader.cs
using UnityEngine;                     // Unity 기본 네임스페이스
using UnityEditor;                     // 에디터 전용 API

public class ChangeAllMaterialsShader : EditorWindow
{
    Shader targetShader;               // 변경할 대상 Shader

    [MenuItem("Tools/Change All Materials Shader")]
    static void Open()                  // 메뉴에서 창 열기
    {
        GetWindow<ChangeAllMaterialsShader>("Change Materials Shader");
    }

    void OnGUI()                        // 에디터 UI
    {
        targetShader = (Shader)EditorGUILayout.ObjectField(
            "Target Shader",            // 라벨
            targetShader,               // 현재 값
            typeof(Shader),             // 타입
            false                       // 씬 오브젝트 아님
        );

        if (GUILayout.Button("Apply To All Scene Objects"))
        {
            if (targetShader == null) return;

            ApplyShaderToAll();         // 실제 적용
        }
    }

    void ApplyShaderToAll()
    {
        Renderer[] renderers = FindObjectsOfType<Renderer>(true); // 씬 내 모든 Renderer 검색

        Undo.RecordObjects(renderers, "Change All Materials Shader"); // Undo 지원

        foreach (Renderer renderer in renderers)                     // 각 Renderer 순회
        {
            Material[] materials = renderer.sharedMaterials;         // 공유 머티리얼 배열

            for (int i = 0; i < materials.Length; i++)               // 모든 머티리얼 순회
            {
                if (materials[i] == null) continue;                  // null 방지
                materials[i].shader = targetShader;                  // 쉐이더 변경
            }

            renderer.sharedMaterials = materials;                     // 변경 반영
            EditorUtility.SetDirty(renderer);                         // 변경 표시
        }
    }
}
