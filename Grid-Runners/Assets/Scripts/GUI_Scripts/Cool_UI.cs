using UnityEngine;
using UnityEngine.UI;

public class Cool_UI : MonoBehaviour
{
    public MeshFilter meshFilter;
    public Canvas canvas;

    void Start()
    {
        Mesh mesh = meshFilter.mesh;

        RectTransform rectTransform = canvas.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(mesh.bounds.size.x, mesh.bounds.size.y);

        canvas.transform.position = meshFilter.transform.position;
    }
}