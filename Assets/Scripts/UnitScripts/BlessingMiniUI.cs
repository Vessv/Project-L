using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BlessingMiniUI : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;
    public Image image;

    public BlessingSO BlessingSO;

    public void UpdateBlessingDisplay(BlessingSO blessingSO_)
    {
        BlessingSO = blessingSO_;
        title.text = blessingSO_.title;
        description.text = blessingSO_.description;
        image.sprite = blessingSO_.sprite;
    }

    public void SelectedBlessing()
    {
        BlessingSO.Get();
    }
}
