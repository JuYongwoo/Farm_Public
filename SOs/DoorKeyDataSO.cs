using UnityEngine;

namespace JYW.Game.SOs
{
    [CreateAssetMenu(menuName = "Game/DoorKey Data")]
    public class DoorKeyDataSO : ScriptableObject
    {
        public DoorKeyData[] doorKeyDatas;

        public DoorKeyData GetTriggerDataById(string keyDoorID)
        {
            foreach (var data in doorKeyDatas)
            {
                if (data.KeyDoorID == keyDoorID)
                    return data;
            }
            return null;
        }

    }

    [System.Serializable]
    public class DoorKeyData
    {
        [SerializeField]
        private string keyDoorID;

        [SerializeField]
        private string activeChapter;

        [SerializeField]
        private string doorName = "";

        [SerializeField]
        private string keyName = "";

        public string KeyDoorID => keyDoorID;
        public string ActiveChapter => activeChapter;
        public string DoorName => doorName;
        public string KeyName => keyName;
    }
}