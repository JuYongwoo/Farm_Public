using JYW.Game.SOs;
using JYW.Game.Managers.TriggerSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JYW.Game.Players
{
    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField] private EventSO inventoryOpenSO;
        [SerializeField] private GameObject itemUIPrefab;
        private List<GameObject> collectedObjects = new List<GameObject>();

        public bool CheckExist(string name)
        {
            foreach (var obj in collectedObjects)
            {
                if (obj.name == name) return true;
            }
            return false;
        }

        public void RemoveObject(string name)
        {
            collectedObjects.RemoveAll(obj => obj.name == name);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                EventPlayManager.Instance.PlayEvent(inventoryOpenSO);
                RefreshInventoryContents();
            }
        }

        private void RefreshInventoryContents()
        {
            // spawnedObjects 안에서 실제 인스턴스화된 인벤토리 UI 객체 찾기
            GameObject inventoryUIInstance = null;

            foreach (GameObject obj in EventPlayManager.Instance.spawnedObjects)
            {
                if (obj != null && obj.name == inventoryOpenSO.UIObject.name)
                {
                    inventoryUIInstance = obj;
                    break;
                }
            }

            if (inventoryUIInstance == null)
                return; // UI가 아직 생성되기 전이라면 그냥 무시

            // GridLayout 찾기
            GridLayoutGroup grid = inventoryUIInstance.GetComponentInChildren<GridLayoutGroup>(true);

            if (grid == null)
            {
                Debug.LogWarning("GridLayoutGroup을 찾지 못했습니다.");
                return;
            }

            Transform contentsfolder = grid.transform;

            for (int i = contentsfolder.childCount - 1; i >= 0; i--)
            {
                Destroy(contentsfolder.GetChild(i).gameObject);
            }

            foreach (GameObject itemObj in collectedObjects)
            {
                GameObject itemUI = Instantiate(itemUIPrefab, contentsfolder, false);
                itemUI.name = itemObj.name;

                Text t = itemUI.GetComponentInChildren<Text>();
                t.text = itemObj.name;
            }
        }

        public List<GameObject> GetCollectedObjects()
        {
            return new List<GameObject>(collectedObjects);
        }

        public void AddInventory(GameObject obj)
        {
            if (obj != null)
            {
                collectedObjects.Add(obj);
            }
        }
    }
}
