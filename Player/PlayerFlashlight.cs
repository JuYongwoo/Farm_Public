using JYW.Game.EventPlay;
using UnityEngine;

public class PlayerFlashlight : MonoBehaviour
{
    [Header("Light Reference (Spot)")]
    [SerializeField] private Light spotLight; // Spot 모드 Light

    [Header("Maximum Values")]
    [SerializeField] private float maximumAngle = 90f;
    [SerializeField] private float maximumRange = 100f;
    [SerializeField] private float maximumIntensity = 1f;

    [Header("Minimum Values")]
    [SerializeField] private float minimumAngle = 60f;
    [SerializeField] private float minimumRange = 75f;
    [SerializeField] private float minimumIntensity = 0.75f;

    [Header("Drain Settings")]
    [SerializeField] private float startCharge = 150f; // 
    [SerializeField] private float drainPerSecond = 1f; // 초당 1씩 감소

    private float charge;            // 현재 게이지 (0 ~ startCharge)
    private bool initialized = false;

    void Awake()
    {
        // Spot 라이트 검증
        if (spotLight == null)
            spotLight = GetComponentInChildren<Light>();

        if (spotLight != null && spotLight.type != LightType.Spot)
        {
            Debug.LogWarning("[PlayerFlashlight] 지정된 Light가 Spot 타입이 아닙니다. Spot 타입으로 변경합니다.");
            spotLight.type = LightType.Spot;
        }
    }

    void Start()
    {
        charge = Mathf.Max(0f, startCharge); // 200에서 시작
        ApplyLightByCharge();                 // 시작 시 최대치 적용
        initialized = true;
        EventPlayManager.Instance.AddAction(gameObject, Refill);
    }

    void Update()
    {
        if (!initialized || spotLight == null) return;

        // 초당 1 감소 (Time.deltaTime 기반)
        charge -= drainPerSecond * Time.deltaTime;
        if (charge <= 0f)
            charge = 0f; // 바닥

        ApplyLightByCharge();
    }

    // 게이지에 따라 라이트 값을 선형 보간하여 적용
    private void ApplyLightByCharge()
    {
        if (spotLight == null) return;

        // t: 0이면 Maximum, 1이면 Minimum에 해당
        float t = 1f - Mathf.Clamp01(charge / Mathf.Max(0.0001f, startCharge));

        // 선형 보간으로 점점 Minimum에 가까워짐
        spotLight.spotAngle = Mathf.Lerp(maximumAngle, minimumAngle, t);
        spotLight.range = Mathf.Lerp(maximumRange, minimumRange, t);
        spotLight.intensity = Mathf.Lerp(maximumIntensity, minimumIntensity, t);

        // 완전히 바닥이면 정확히 Minimum으로 스냅
        if (charge <= 0f)
        {
            spotLight.spotAngle = minimumAngle;
            spotLight.range = minimumRange;
            spotLight.intensity = minimumIntensity;
        }
    }

    // 외부에서 호출하면 즉시 최대치로 복구
    public void Refill()
    {
        charge = Mathf.Max(0f, startCharge);

        if (spotLight == null) return;

        spotLight.spotAngle = maximumAngle;
        spotLight.range = maximumRange;
        spotLight.intensity = maximumIntensity;
    }

    // 디버그/외부 확인용 게이지 값 반환
    public float GetCharge() => charge;

    // 에디터에서 값 변경 시 즉시 Light에 반영되도록
    void OnValidate()
    {
        // startCharge는 요구사항상 200 유지가 권장되지만, 안전하게 최소값 보정
        if (startCharge < 1f) startCharge = 200f;
        if (drainPerSecond <= 0f) drainPerSecond = 1f;

        if (spotLight != null && spotLight.type != LightType.Spot)
            spotLight.type = LightType.Spot;

        // 에디터에서 미리보기 반영
        if (Application.isPlaying) ApplyLightByCharge();
        else
        {
            // 플레이 중이 아니면 최대치 기준으로 미리보기
            if (spotLight != null)
            {
                spotLight.spotAngle = maximumAngle;
                spotLight.range = maximumRange;
                spotLight.intensity = maximumIntensity;
            }
        }
    }
}