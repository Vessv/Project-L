using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Utils
{
    public static TextMeshPro CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3), int fontSize = 40, Color color = default(Color), TextAlignmentOptions textAlignment = default(TextAlignmentOptions), int sortingOrder = 5000)
    {
        if (color == null) { color = Color.white; }
        return CreateWorldText(parent, text, localPosition, fontSize, (Color)color, textAlignment, sortingOrder);
    }

    public static TextMeshPro CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color,TextAlignmentOptions textAlignment, int sortingOrder)
    {
        GameObject gameObject = new GameObject("Wolrd_Text", typeof(TextMeshPro));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        TextMeshPro textMesh = gameObject.GetComponent<TextMeshPro>();
        textMesh.SetText(text);
        textMesh.color = color;
        textMesh.alignment = textAlignment;
        textMesh.fontSize = fontSize;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        return textMesh;

    }

    //Get mouse position in wold with z = 0f
    public static Vector3 GetMouseWorldPosition()
    {
        Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
        vec.z = 0f;
        return vec;
    }

    public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition)
    {
        return GetMouseWorldPositionWithZ(screenPosition, Camera.main);
    }
    public static Vector3 GetMouseWorldPositionWithZ( Camera worldCamera)
    {
        return GetMouseWorldPositionWithZ(Input.mousePosition, worldCamera);
    }
    public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera)
    {
        Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
        return worldPosition;
    }
}
