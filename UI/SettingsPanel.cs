using JYW.Game.Managers;
using JYW.Game.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JYW.Game.UIs
{
    public class SettingsPanel : MonoBehaviour
    {
        private bool isFullScreen = false;

        private readonly Vector2Int[] resolutions = new Vector2Int[]
    {
        new Vector2Int(1920, 1080),
        new Vector2Int(1600, 900)
    };

        public enum SettingsPanelObjs
        {
            SettingsPanel,
            VideoSettings,
            SoundSettings,
            InfoSettings,
            TotalSound,
            VideoDropdown,
            Toggle,
            SettingExitButton,
            VideoButton,
            SoundButton,
            InfomationButton,
            TotalSoundSlider
        }

        private Dictionary<SettingsPanelObjs, GameObject> settingsPanelObjsMap;

        private void Awake()
        {
            settingsPanelObjsMap = Util.MapEnumChildObjects<SettingsPanelObjs, GameObject>(this.gameObject);
            EventHub.Instance.SettingsPanelOnEvent -= SettingOn;
            EventHub.Instance.SettingsPanelOnEvent += SettingOn;

            settingsPanelObjsMap[SettingsPanelObjs.SettingExitButton].GetComponent<Button>().onClick.AddListener(() =>
            {
                settingsPanelObjsMap[SettingsPanelObjs.SettingsPanel].gameObject.SetActive(false);
            });

            settingsPanelObjsMap[SettingsPanelObjs.VideoButton].GetComponent<Button>().onClick.AddListener(VideoButtonClick);
            settingsPanelObjsMap[SettingsPanelObjs.SoundButton].GetComponent<Button>().onClick.AddListener(SoundButtonClick);
            settingsPanelObjsMap[SettingsPanelObjs.InfomationButton].GetComponent<Button>().onClick.AddListener(InformationButtonClick);

            settingsPanelObjsMap[SettingsPanelObjs.VideoDropdown].GetComponent<Dropdown>().ClearOptions();
            settingsPanelObjsMap[SettingsPanelObjs.VideoDropdown].GetComponent<Dropdown>().AddOptions(new List<string> { "1920 x 1080", "1600 x 900" });
            settingsPanelObjsMap[SettingsPanelObjs.SettingsPanel].gameObject.SetActive(false);
            settingsPanelObjsMap[SettingsPanelObjs.TotalSoundSlider].GetComponent<Slider>().onValueChanged.AddListener((value) => { SliderChangeEvent(value); });
        }

        private void Start()
        {
            VideoButtonClick();
            InitVideoSettings();
        }

        private void OnDestroy()
        {
            EventHub.Instance.SettingsPanelOnEvent -= SettingOn;

        }

        private void SettingOn()
        {
            settingsPanelObjsMap[SettingsPanelObjs.SettingsPanel].SetActive(true);
        }
        private void InitVideoSettings()
        {


            //해상도

            //초기화 //마지막으로 변경한 Resolution과 Fullscreen 여부를 파일에 저장하여 처리하도록 확장 가능
            settingsPanelObjsMap[SettingsPanelObjs.VideoDropdown].GetComponent<Dropdown>().value = 0;
            settingsPanelObjsMap[SettingsPanelObjs.VideoDropdown].GetComponent<Dropdown>().RefreshShownValue();
            ApplyResolution(settingsPanelObjsMap[SettingsPanelObjs.VideoDropdown].GetComponent<Dropdown>().value);

            // 해상도 선택 시
            settingsPanelObjsMap[SettingsPanelObjs.VideoDropdown].GetComponent<Dropdown>().onValueChanged.AddListener(index =>
            {
                ApplyResolution(index);
            });


            //전체화면 토글

            //초기화
            settingsPanelObjsMap[SettingsPanelObjs.Toggle].GetComponent<Toggle>().isOn = false;
            ToggleFullscreen(settingsPanelObjsMap[SettingsPanelObjs.Toggle].GetComponent<Toggle>().isOn);
            // 전체화면 토글 시
            settingsPanelObjsMap[SettingsPanelObjs.Toggle].GetComponent<Toggle>().onValueChanged.AddListener(isFullscreen =>
            {
                ToggleFullscreen(isFullscreen);
            });

        }

        private void ToggleFullscreen(bool fullscreen)
        {
            isFullScreen = fullscreen;
            Screen.fullScreen = isFullScreen;
            Screen.fullScreenMode = isFullScreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        }

        // 인덱스 기반 해상도 적용 (Dropdown 연동 시)
        private void ApplyResolution(int index)
        {
            if (index < 0 || index >= resolutions.Length) return;

            Vector2Int res = resolutions[index];
            Screen.SetResolution(res.x, res.y, isFullScreen);
        }



        private void VideoButtonClick()
        {
            settingsPanelObjsMap[SettingsPanelObjs.SoundSettings].gameObject.SetActive(false);
            settingsPanelObjsMap[SettingsPanelObjs.InfoSettings].gameObject.SetActive(false);
            settingsPanelObjsMap[SettingsPanelObjs.VideoSettings].gameObject.SetActive(true);
        }

        private void SoundButtonClick()
        {
            settingsPanelObjsMap[SettingsPanelObjs.VideoSettings].gameObject.SetActive(false);
            settingsPanelObjsMap[SettingsPanelObjs.InfoSettings].gameObject.SetActive(false);
            settingsPanelObjsMap[SettingsPanelObjs.SoundSettings].gameObject.SetActive(true);
        }

        private void InformationButtonClick()
        {
            settingsPanelObjsMap[SettingsPanelObjs.VideoSettings].gameObject.SetActive(false);
            settingsPanelObjsMap[SettingsPanelObjs.SoundSettings].gameObject.SetActive(false);
            settingsPanelObjsMap[SettingsPanelObjs.InfoSettings].gameObject.SetActive(true);
        }

        private void SliderChangeEvent(float value)
        {
            //GamePropertyManager.Instance.SetMasterVolume(value);

        }
    }
}