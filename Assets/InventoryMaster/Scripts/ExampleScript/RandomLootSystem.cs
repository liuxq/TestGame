using UnityEngine;
using System.Collections;

public class RandomLootSystem : MonoBehaviour
{

    public int amountOfLoot = 10;
    static ItemDataBaseList inventoryItemList;

    int counter = 0;

    // Use this for initialization
    void Start()
    {

        inventoryItemList = (ItemDataBaseList)Resources.Load("ItemDatabase");

        while (counter < amountOfLoot)
        {
            counter++;

            int randomNumber = Random.Range(1, inventoryItemList.itemList.Count - 1);

            Terrain terrain = Terrain.activeTerrain;

            float x = Random.Range(5, terrain.terrainData.size.x - 5);
            float z = Random.Range(5, terrain.terrainData.size.z - 5);


            if (inventoryItemList.itemList[randomNumber].itemModel == null)
                counter--;
            else
            {
                GameObject randomLootItem = (GameObject)Instantiate(inventoryItemList.itemList[randomNumber].itemModel);
                PickUpItem item = randomLootItem.AddComponent<PickUpItem>();
                item.item = inventoryItemList.itemList[randomNumber];

                randomLootItem.transform.localPosition = new Vector3(x, 0, z);
            }
        }

    }

}
