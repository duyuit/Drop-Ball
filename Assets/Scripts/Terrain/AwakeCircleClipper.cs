using System.Collections;
using System.Collections.Generic;
using ClipperLib;
using UnityEngine;

using Vector2i = ClipperLib.IntPoint;

public class AwakeCircleClipper : MonoBehaviour, IClip
{
    public DestructibleTrunk terrain;

    public float diameter = 1.2f;

    private float radius = 1.2f;

    public int segmentCount = 10;

    private Vector2i[] vertices;
    private Vector2 clipPosition;

    private Mesh mesh;
    public bool CheckBlockOverlapping(Vector2 p, float size)
    {
        float dx = Mathf.Abs(clipPosition.x - p.x) - radius - size / 2;
        float dy = Mathf.Abs(clipPosition.y - p.y) - radius - size / 2;

        return dx < 0f && dy < 0f;
    }

    public ClipBounds GetBounds()
    {
        return new ClipBounds
        {
            lowerPoint = new Vector2(clipPosition.x - radius, clipPosition.y - radius),
            upperPoint = new Vector2(clipPosition.x + radius, clipPosition.y + radius)
        };
    }

    void Build(Vector2 center)
    {
        Vector3[] meshVertices = new Vector3[segmentCount];
        Vector3[] meshNormals = new Vector3[segmentCount];
        vertices = new Vector2i[segmentCount];

        for (int i = 0; i < segmentCount; i++)
        {
            float angle = Mathf.Deg2Rad * (-90f - 360f / segmentCount * i);

            Vector2 point = new Vector2(center.x + radius * Mathf.Cos(angle), center.y + radius * Mathf.Sin(angle));
            vertices[i] = point.ToVector2i();

            meshVertices[i] = point.ToVector3f();
            meshNormals[i] = (meshVertices[i] - center.ToVector3f()) / radius;
        }

        mesh.Clear();
        mesh.vertices = meshVertices;
        mesh.normals = meshNormals;
        mesh.triangles = Triangulate.Execute(meshVertices).ToArray();
    }

    public List<Vector2i> GetVertices()
    {
        List<Vector2i> vertices = new List<Vector2i>();
        for (int i = 0; i < segmentCount; i++)
        {
            float angle = Mathf.Deg2Rad * (-90f - 360f / segmentCount * i);

            Vector2 point = new Vector2(clipPosition.x + radius * Mathf.Cos(angle), clipPosition.y + radius * Mathf.Sin(angle));
            Vector2i point_i64 = point.ToVector2i();
            vertices.Add(point_i64);
        }
        return vertices;
    }

    void Awake()
    {
        radius = diameter / 2f;
    }

    public void Cut()
    {
        Vector3 positionWorldSpace = transform.position;
        var temp = positionWorldSpace - terrain.transform.position;
        clipPosition.x = temp.x + terrain.transform.position.x;
        clipPosition.y = temp.z;
        Debug.Log(clipPosition);

        Build(clipPosition);
        terrain.ExecuteClip(this);
    }

    public void CutInPosition(Vector2 clipPos)
    {
        this.clipPosition = clipPos;
        Build(clipPosition);
        terrain.ExecuteClip(this);
    }

    public Vector2 GetClipPos()
    {
        return clipPosition;
    }

    void Start()
    {
        mesh = new Mesh();
        mesh.MarkDynamic();
        //Cut();
    }

    public Mesh GetMesh()
    {
        return mesh;
    }

    Vector2i[] IClip.GetVertices()
    {
        return GetVertices().ToArray();
    }
}
