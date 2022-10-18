using UnityEngine;

public class DebugMesh : MonoBehaviour
{
    public bool drawMech;
    public bool drawBoundary;

    Mesh mesh;

    void DrawMech()
    {
        mesh = GetComponent<MeshFilter>().sharedMesh;
        var vertices = mesh.vertices;
        var triangles = mesh.triangles;
        Gizmos.color = Color.green;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            var t = triangles[i];
            var t1 = triangles[i + 1];
            var t2 = triangles[i + 2];

            var v1 = vertices[t];
            var v2 = vertices[t1];
            var v3 = vertices[t2];

            Gizmos.DrawLine(v1, v2);
            Gizmos.DrawLine(v2, v3);
            Gizmos.DrawLine(v3, v1);
        }
    }

    void DrawBoundary()
    {
        mesh = GetComponent<MeshFilter>().sharedMesh;
        var vertices = mesh.vertices;
        var triangles = mesh.triangles;
        var boundaryPath = EdgeHelpers.FindBoundary(EdgeHelpers.GetEdges(triangles));

        Gizmos.color = Color.red;
        for (int i = 0; i < boundaryPath.Count; i++)
        {
            var t1 = boundaryPath[i].v1;
            var t2 = boundaryPath[i].v2;

            var v1 = vertices[t1];
            var v2 = vertices[t2];

            Gizmos.DrawLine(v1, v2);
        }
    }

    private void OnDrawGizmos()
    {
        if (drawMech) DrawMech();
        if (drawBoundary) DrawBoundary();

    }
}
