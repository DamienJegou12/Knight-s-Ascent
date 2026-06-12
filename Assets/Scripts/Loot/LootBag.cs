using UnityEngine;
using System.Collections.Generic;

public class LootBag : MonoBehaviour
{
    public List<Loot> LootList = new List<Loot>();
    

    Loot GetDroppedItem()
    {
        int randomNumber = Random.Range(1, 101);
        List<Loot> possibleLoot = new List<Loot>();
        foreach (Loot loot in LootList)
        {
            if (randomNumber <= loot.dropChance)
            {
                possibleLoot.Add(loot);
            }
        }
        if (possibleLoot.Count > 0)
        {
            Loot droppedLoot = possibleLoot[0];
            foreach (Loot loot in possibleLoot)
            {
                if (loot.dropChance < droppedLoot.dropChance)
                {
                    droppedLoot = loot;
                }
            }
            return droppedLoot;
        }
        Debug.Log("No Loot dropped");
        return null;
    }

    public void InstantiateLoot(Vector3 spawnPosition)
    {
        Loot droppedLoot = GetDroppedItem();
        if (droppedLoot != null)
        {
            Instantiate(droppedLoot.loot, spawnPosition, Quaternion.identity);
        }
    }
    
}
