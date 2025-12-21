using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class PrefabScatterWindow : EditorWindow
{
    private GameObject prefab;
    private Vector3 origin;
    private bool hasOrigin;
    private bool pickingOrigin;

    private float areaWidth = 100f;
    private float areaDepth = 100f;
    private int count = 200;

    private bool useTerrainHeight = true;
    private bool randomRotationY = true;
    private float minScale = 1f;
    private float maxScale = 1f;

    private bool alignToNormal = false; // Terrain 법선에 맞춰 회전 (기본 off)

    [MenuItem("Tools/Terrain/Prefab Scatter Window")]
    private static void Open()
    {
        GetWindow<PrefabScatterWindow>("Prefab Scatter");
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("프리팹 랜덤 배치", EditorStyles.boldLabel);

        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("원점 설정", EditorStyles.boldLabel);
        using (new EditorGUILayout.HorizontalScope())
        {
            GUI.enabled = !pickingOrigin;
            if (GUILayout.Button("원점 선택"))
            {
                pickingOrigin = true;
                hasOrigin = false;
                Repaint();
            }
            GUI.enabled = true;
            if (hasOrigin)
            {
                EditorGUILayout.Vector3Field("", origin);
            }
            else
            {
                EditorGUILayout.LabelField("미지정");
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("배치 설정", EditorStyles.boldLabel);
        areaWidth = EditorGUILayout.FloatField("Area Width (X)", areaWidth);
        areaDepth = EditorGUILayout.FloatField("Area Depth (Z)", areaDepth);
        count = EditorGUILayout.IntField("Instance Count", count);
        useTerrainHeight = EditorGUILayout.Toggle("Terrain 높이 적용", useTerrainHeight);
        alignToNormal = EditorGUILayout.Toggle("Terrain 법선 정렬", alignToNormal);
        randomRotationY = EditorGUILayout.Toggle("랜덤 Y 회전", randomRotationY);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("랜덤 스케일", EditorStyles.boldLabel);
        minScale = EditorGUILayout.FloatField("Min Scale", minScale);
        maxScale = EditorGUILayout.FloatField("Max Scale", maxScale);
        if (maxScale < minScale) maxScale = minScale;

        EditorGUILayout.Space();
        GUI.enabled = prefab != null && hasOrigin && count > 0 && areaWidth > 0 && areaDepth > 0;
        if (GUILayout.Button("Scatter Prefabs"))
        {
            Scatter();
        }
        GUI.enabled = true;

        EditorGUILayout.HelpBox("원점은 영역의 중심으로 사용됩니다.\nScene 뷰에서 '원점 선택' 후 좌클릭으로 지정하세요.", MessageType.Info);
    }

    private void OnSceneGUI(SceneView sv)
    {
        if (!pickingOrigin)
        {
            // 원점 핸들 표시
            if (hasOrigin)
            {
                Handles.color = Color.yellow;
                Handles.DrawWireDisc(origin, Vector3.up, 1f);
                Handles.color = new Color(1f, 0.6f, 0f, 0.25f);
                DrawAreaPreview();
            }
            return;
        }

        Event e = Event.current;
        if (e.type == EventType.MouseDown && e.button == 0 && !e.alt)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (Physics.Raycast(ray, out var hit, 10000f))
            {
                origin = hit.point;
            }
            else
            {
                // Terrain 높이 샘플 시도
                float y = 0f;
                var activeTerr = Terrain.activeTerrain;
                if (activeTerr != null)
                {
                    Vector3 planePoint = ray.origin + ray.direction * (-(ray.origin.y - activeTerr.transform.position.y) / ray.direction.y);
                    y = activeTerr.SampleHeight(planePoint) + activeTerr.transform.position.y;
                    origin = new Vector3(planePoint.x, y, planePoint.z);
                }
                else
                {
                    origin = ray.origin + ray.direction * 50f;
                }
            }

            hasOrigin = true;
            pickingOrigin = false;
            e.Use();
            Repaint();
        }
        else if (e.type == EventType.Layout)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        }

        // 프리뷰
        if (hasOrigin)
        {
            Handles.color = Color.green;
            Handles.DrawWireDisc(origin, Vector3.up, 1f);
            Handles.color = new Color(0f, 0.8f, 0.2f, 0.25f);
            DrawAreaPreview();
        }

        sv.Repaint();
    }

    private void DrawAreaPreview()
    {
        Vector3 c = origin;
        Vector3 p1 = c + new Vector3(-areaWidth / 2f, 0, -areaDepth / 2f);
        Vector3 p2 = c + new Vector3(areaWidth / 2f, 0, -areaDepth / 2f);
        Vector3 p3 = c + new Vector3(areaWidth / 2f, 0, areaDepth / 2f);
        Vector3 p4 = c + new Vector3(-areaWidth / 2f, 0, areaDepth / 2f);
        Handles.DrawLine(p1, p2);
        Handles.DrawLine(p2, p3);
        Handles.DrawLine(p3, p4);
        Handles.DrawLine(p4, p1);
    }

    private void Scatter()
    {
        if (prefab == null || !hasOrigin)
        {
            Debug.LogWarning("[PrefabScatter] 프리팹 또는 원점 미지정");
            return;
        }

        var root = new GameObject($"ScatterRoot_{prefab.name}_{System.DateTime.Now:HHmmss}");
        Undo.RegisterCreatedObjectUndo(root, "Scatter Prefabs");

        Terrain activeTerrain = Terrain.activeTerrain;

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = origin;
            pos.x += Random.Range(-areaWidth / 2f, areaWidth / 2f);
            pos.z += Random.Range(-areaDepth / 2f, areaDepth / 2f);

            if (useTerrainHeight && activeTerrain != null)
            {
                float terrainY = activeTerrain.SampleHeight(pos) + activeTerrain.transform.position.y;
                pos.y = terrainY;
            }

            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            Undo.RegisterCreatedObjectUndo(instance, "Scatter Prefab");
            instance.transform.SetParent(root.transform, true);
            instance.transform.position = pos;

            // Scale
            float sc = Random.Range(minScale, maxScale);
            instance.transform.localScale = Vector3.one * sc;

            // Rotation
            if (randomRotationY)
            {
                float yRot = Random.Range(0f, 360f);
                if (alignToNormal && activeTerrain != null)
                {
                    // Terrain 법선 계산
                    Vector3 normal = SampleTerrainNormal(activeTerrain, pos);
                    Quaternion faceUp = Quaternion.FromToRotation(Vector3.up, normal);
                    instance.transform.rotation = faceUp * Quaternion.Euler(0f, yRot, 0f);
                }
                else
                {
                    instance.transform.rotation = Quaternion.Euler(0f, yRot, 0f);
                }
            }
            else if (alignToNormal && activeTerrain != null)
            {
                Vector3 normal = SampleTerrainNormal(activeTerrain, pos);
                instance.transform.rotation = Quaternion.FromToRotation(Vector3.up, normal);
            }
        }

        EditorSceneManager.MarkSceneDirty(root.scene);
        Debug.Log($"[PrefabScatter] 배치 완료: {count}개, 영역 {areaWidth}x{areaDepth}, 원점 {origin}");
        Selection.activeGameObject = root;
    }

    private Vector3 SampleTerrainNormal(Terrain terrain, Vector3 worldPos)
    {
        var td = terrain.terrainData;
        Vector3 localPos = worldPos - terrain.transform.position;
        float u = Mathf.Clamp01(localPos.x / td.size.x);
        float v = Mathf.Clamp01(localPos.z / td.size.z);
        // TerrainData.GetInterpolatedNormal(u,v)는 0..1 UV 기반
        return td.GetInterpolatedNormal(u, v).normalized;
    }
}