using UnityEngine;
using UnityEditor;

public class GridSpawnerWindow : EditorWindow
{
    private GameObject prefab;
    private int count = 100;
    private float size = 1f;
    private string parentName = "SpawnedGrid";

    [MenuItem("Tools/Grid Spawner")]
    private static void ShowWindow()
    {
        GetWindow<GridSpawnerWindow>("Grid Spawner");
    }

    private void OnGUI()
    {
        GUILayout.Label("Grid Spawner", EditorStyles.boldLabel);

        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);
        count = EditorGUILayout.IntField("Count", count);
        size = EditorGUILayout.FloatField("Size", size);
        parentName = EditorGUILayout.TextField("Parent Name", parentName);

        if (GUILayout.Button("Spawn Grid"))
        {
            if (prefab == null)
            {
                Debug.LogWarning("GridSpawner: 프리팹이 지정되지 않았습니다.");
                return;
            }

            SpawnGrid();
        }

        if (GUILayout.Button("Clear Spawned"))
        {
            var parent = GameObject.Find(parentName);
            if (parent != null)
                DestroyImmediate(parent);
        }
    }

    private void SpawnGrid()
    {
        int gridSize = Mathf.RoundToInt(Mathf.Sqrt(count));
        if (gridSize * gridSize != count)
        {
            Debug.LogWarning($"GridSpawner: {count}개는 완전한 정사각형 배열이 아닙니다. {gridSize * gridSize}개로 조정됩니다.");
            count = gridSize * gridSize;
        }

        GameObject parent = new GameObject(parentName);

        float totalWidth = (gridSize - 1) * size;
        float offset = totalWidth / 2f;

        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                int index = y * gridSize + x;
                if (index >= count) break;

                Vector3 pos = new Vector3(x * size - offset, 0, y * size - offset);
                GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                obj.transform.position = pos;
                obj.transform.SetParent(parent.transform);
            }
        }

        Debug.Log($"GridSpawner: {count}개 프리팹을 {gridSize}x{gridSize} 중앙정렬로 배치했습니다.");
    }
}
