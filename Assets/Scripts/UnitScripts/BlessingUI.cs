using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlessingUI : MonoBehaviour
{
    public List<BlessingSO> BlessingSOArray;
    BlessingSO[] BlessingDisplay = new BlessingSO[3];
    public GameObject BlessingDisplayPrefab1;
    public GameObject BlessingDisplayPrefab2;
    public GameObject BlessingDisplayPrefab3;
    private void OnEnable()
    {
        for(int i = 0; i < BlessingDisplay.Length; i++)
        {
            bool hasSelected = false;
            while (!hasSelected)
            {
                int blessingIndex = Mathf.FloorToInt(Random.Range(0f, BlessingSOArray.Count));
                float randomNumber = Random.Range(0f, 1f);

                switch (BlessingSOArray[blessingIndex].rarity.name)
                {
                    case "Common":
                        if(randomNumber <= 0.8f)
                        {
                            hasSelected = true;
                        }
                        break;
                    case "Rare":
                        if (randomNumber >= 0.5f)
                        {
                            hasSelected = true;

                        }
                        break;
                    case "Epic":
                        if (randomNumber <= 0.3f)
                        {
                            hasSelected = true;

                        }
                        break;
                    case "Legendary":
                        if (randomNumber <= 0.1f)
                        {
                            hasSelected = true;

                        }
                        break;
                }
                BlessingDisplay[i] = BlessingSOArray[blessingIndex];
            }
        }
        BlessingDisplayPrefab1.GetComponent<BlessingMiniUI>().UpdateBlessingDisplay(BlessingDisplay[0]);
        BlessingDisplayPrefab2.GetComponent<BlessingMiniUI>().UpdateBlessingDisplay(BlessingDisplay[1]);
        BlessingDisplayPrefab3.GetComponent<BlessingMiniUI>().UpdateBlessingDisplay(BlessingDisplay[2]);
    }
}
