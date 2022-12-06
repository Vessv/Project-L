using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapVisual : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    Transform _mapVisualHolder;
    [SerializeField]
    Transform _gridVisual;

    public Transform[,] GridVisualArray
    {
        get; private set;
    }
    void Start()
    {
        int width = (int)GameHandler.Instance.GetWidthAndHeight().x;
        int height = (int)GameHandler.Instance.GetWidthAndHeight().y;
        GridVisualArray = new Transform[width, height];
        for(int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Transform gridVisual = Instantiate(_gridVisual, GameHandler.Instance.GetGrid().GetWorldPosition(i, j) + new Vector3(0.5f, 0.5f), Quaternion.identity);
                gridVisual.transform.SetParent(_mapVisualHolder, true);
                gridVisual.gameObject.name = "GridVisual " + i +":" + j;
                gridVisual.gameObject.SetActive(false);
                GridVisualArray[i,j] = gridVisual;
            }
        }
        GameHandler.Instance.InitializeMapVisualArray();
    }

    public void HideAll()
    {
        foreach (Transform child in _mapVisualHolder.transform)
        {
            // Set the child gameobject's active property to false
            child.gameObject.SetActive(false);
        }
    }
}
