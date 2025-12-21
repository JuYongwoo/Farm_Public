using JYW.Game.EventPlay;
using UnityEngine;
using UnityEngine.UI;

namespace JYW.Game.UIs
{
    public class SettingsPanel : MonoBehaviour
    {

        [SerializeField]
        private Button exitButton;




        private void Start()
        {
            EventPlayManager.Instance.AddAction(gameObject, SettingOn);

            exitButton.onClick.AddListener(() =>
            {
                //EventHub.Instance.masterVolume = masterVolumeSlider.value;
                gameObject.SetActive(false);
            });

            gameObject.SetActive(false);

        }


        private void SettingOn()
        {
            gameObject.SetActive(true);
        }
            }
}