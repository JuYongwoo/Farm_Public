using JYW.Game.EventPlay;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JYW.Game.Players
{
    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField] private GameObject inventoryUIPrefab;
        [SerializeField] private GameObject itemUIPrefab;
        private List<GameObject> collectedObjects = new List<GameObject>();

        // 인스턴스 참조 추가
        private GameObject inventoryUIInstance;


        public bool CheckExist(string name, int count)
        {
            int foundCount = 0;
            foreach (var obj in collectedObjects)
            {
                if (obj.name == name) foundCount++;
                if (foundCount >= count) return true;
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
                if (inventoryUIPrefab == null)
                {
                    Debug.LogWarning("[PlayerInventory] inventoryUIPrefab이 할당되어 있지 않습니다.");
                    return;
                }

                // 인스턴스가 없으면 생성
                if (inventoryUIInstance == null)
                {
                    inventoryUIInstance = Instantiate(inventoryUIPrefab);
                    inventoryUIInstance.name = inventoryUIPrefab.name;
                }
                inventoryUIInstance.SetActive(true);

                RefreshInventoryContents();

            }
        }

        private void RefreshInventoryContents()
        {
            if (inventoryUIInstance == null)
            {
                Debug.LogWarning("[PlayerInventory] inventoryUIInstance가 없습니다. 먼저 열어주세요.");
                return;
            }

            // GridLayout 찾기 (인스턴스에서 검색)
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
                if (t != null)
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

                // UI가 열려있으면 즉시 갱신
                if (inventoryUIInstance != null && inventoryUIInstance.activeSelf)
                    RefreshInventoryContents();
            }
        }
    }
}
