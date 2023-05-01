using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlessingUI : MonoBehaviour
{
    public BlessingSO[] BlessingSOArray;
    BlessingSO[] BlessingDisplay = new BlessingSO[3];
    public GameObject BlessingDisplayPrefab1;
    public GameObject BlessingDisplayPrefab2;
    public GameObject BlessingDisplayPrefab3;
    private void OnEnable()
    {
        for(int i = 0; i < BlessingDisplay.Length; i++)
        {
            int blessingIndex = Mathf.FloorToInt(Random.Range(0f, BlessingSOArray.Length));
            BlessingDisplay[i] = BlessingSOArray[blessingIndex];
        }
        BlessingDisplayPrefab1.GetComponent<BlessingMiniUI>().UpdateBlessingDisplay(BlessingDisplay[0]);
        BlessingDisplayPrefab2.GetComponent<BlessingMiniUI>().UpdateBlessingDisplay(BlessingDisplay[1]);
        BlessingDisplayPrefab3.GetComponent<BlessingMiniUI>().UpdateBlessingDisplay(BlessingDisplay[2]);
    }
}
