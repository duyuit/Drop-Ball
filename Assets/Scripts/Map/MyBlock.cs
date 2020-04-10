using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using int64 = System.Int64;
using Vector2i = ClipperLib.IntPoint;
using Vector2f = UnityEngine.Vector2;
using System;
using DG.Tweening;

public class MyBlock : MonoBehaviour
{
    public Vector2Int index;
    public bool collider;
    public float depth = 1f;
    private List<List<Vector2i>> polygons;

    private MyBlock damagePart;
    //private List<List<Vector2f>> edgesList;

    private List<EdgeCollider2D> colliders = new List<EdgeCollider2D>();

    public List<List<Vector2i>> Polygons { get { return polygons; } }

    public void SetDepth(float v)
    {
        depth = v;
        Initialize();
    }

    private Mesh mesh;

    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;
    private MeshFilter meshFilter;

    // Start is called before the first frame update
    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = meshFilter.mesh;
        mesh.MarkDynamic();
        //meshFilter.mesh = mesh;

        meshRenderer = GetComponent<MeshRenderer>();

        meshCollider = GetComponent<MeshCollider>();
    }

    public void SetMaterial(Material material)
    {
        meshRenderer.material = material;
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }

    public void Initialize()
    {
        List<List<Vector2i>> Inpolygons = new List<List<Vector2i>>();
        List<Vector2i> vertices = new List<Vector2i>();

        int length = (int)VectorEx.float2int64;
        //foreach (var ver in meshFilter.sharedMesh.vertices)
        //{
        //    Debug.Log(ver.x + " " + ver.y);
        //    vertices.Add(new Vector2i { x = (int)(ver.x * VectorEx.float2int64), y = (int)(ver.y * VectorEx.float2int64) });
        //}
        vertices.Add(new Vector2i { x = 0, y = length });
        vertices.Add(new Vector2i { x = 0, y = 0 });
        vertices.Add(new Vector2i { x = length, y = 0 });
        vertices.Add(new Vector2i { x = length, y = length });

        Inpolygons.Add(vertices);
        UpdateGeometryWithMoreVertices(Inpolygons, 1, 1, depth);

    }

    void Start()
    {
        Initialize();
    }

    //public MyBlock CalculateDamage()
    //{
    //    damagePart = Instantiate(PrefabController.Instance.myBlock).GetComponent<MyBlock>();
    //    damagePart.gameObject.tag = "Recover Grass";
    //    var pos = transform.position;
    //    pos.y -= 0.001f + depth;
    //    damagePart.transform.position = pos;
    //    //damagePart.transform.rotation = Quaternion.Euler()

    //    //List<List<Vector2i>> solutions = new List<List<Vector2i>>();

    //    //DelayFunctionHelper delay = gameObject.AddComponent<DelayFunctionHelper>();
    //    //delay.delayFunction(() =>
    //    //{
    //    //ClipperLib.Clipper clipper = new ClipperLib.Clipper();
    //    //clipper.AddPolygons(damagePart.polygons, ClipperLib.PolyType.ptSubject);
    //    //clipper.AddPolygons(polygons, ClipperLib.PolyType.ptClip);
    //    //clipper.Execute(ClipperLib.ClipType.ctDifference, solutions,
    //    //    ClipperLib.PolyFillType.pftNonZero, ClipperLib.PolyFillType.pftNonZero);

    //    //damagePart.UpdateGeometryWithMoreVertices(solutions, 1, 1, depth);
    //    damagePart.transform.localScale = Vector3.one * 0.01f;

    //    //}, 0.1f);

    //    return damagePart;
    //}

    public void UpdateGeometryWithMoreVertices(List<List<Vector2i>> inPolygons, float width, float height, float depth)
    {
        if (polygons != null)
            polygons.Clear();
        else
            polygons = new List<List<Vector2i>>();

        List<List<Vector2>> edgesList = new List<List<Vector2f>>();

        int totalVertexCount = 0;
        int edgeTriangleIndexCount = 0;

        for (int i = 0; i < inPolygons.Count; i++)
        {
            Vector2i[] simplifiedPolygon = BlockSimplification.Execute(inPolygons[i], edgesList);
            if (simplifiedPolygon != null)
            {
                polygons.Add(new List<Vector2i>(simplifiedPolygon));

                totalVertexCount += simplifiedPolygon.Length;
            }
        }

        for (int i = 0; i < edgesList.Count; i++)
        {
            int vertexCount = edgesList[i].Count;
            totalVertexCount += (vertexCount - 1) * 4;
            edgeTriangleIndexCount += (vertexCount - 1) * 6;
        }

        Vector3[] vertices = new Vector3[totalVertexCount];
        Vector3[] normals = new Vector3[totalVertexCount];
        Vector2f[] texCoords = new Vector2f[totalVertexCount];

        List<int> triangles = new List<int>();
        int[] edgeTriangles = new int[edgeTriangleIndexCount];

        int vertexIndex = 0;
        int vertexOffset = 0;

        for (int i = 0; i < polygons.Count; i++)
        {
            List<Vector2i> polygon = polygons[i];
            int vertexCount = polygon.Count;

            for (int j = vertexCount - 1; j >= 0; j--)
            {
                Vector3 point = polygon[j].ToVector3f();
                vertices[vertexIndex] = point;
                normals[vertexIndex] = new Vector3(0, 0, -1);
                texCoords[vertexIndex] = new Vector2f(point.x / width, point.y / height);
                vertexIndex++;
            }

            Triangulate.Execute(vertices, vertexOffset, vertexOffset + vertexCount, triangles);
            vertexOffset += vertexCount;
        }

        int edgeTriangleIndex = 0;
        int vertexOnEdgeIndex = vertexIndex;
        for (int i = 0; i < edgesList.Count; i++)
        {
            List<Vector2f> edgePoints = edgesList[i];
            int vertexCount = edgePoints.Count;
            Vector3 point1;
            Vector3 point2;
            for (int j = 0; j < vertexCount - 1; j++)
            {
                point1 = edgePoints[j].ToVector3f();
                point2 = edgePoints[j + 1].ToVector3f();
                vertices[vertexIndex + 0] = point1;
                vertices[vertexIndex + 2] = point2;

                point1.z += depth;
                point2.z += depth;

                vertices[vertexIndex + 1] = point1;
                vertices[vertexIndex + 3] = point2;

                Vector3 normal = (point2 - point1).normalized;
                normal = new Vector3(normal.y, -normal.x);
                normals[vertexIndex + 0] = normal;
                normals[vertexIndex + 2] = normal;
                normals[vertexIndex + 1] = normal;
                normals[vertexIndex + 3] = normal;


                edgeTriangles[edgeTriangleIndex + 0] = vertexIndex;
                edgeTriangles[edgeTriangleIndex + 1] = vertexIndex + 2;
                edgeTriangles[edgeTriangleIndex + 2] = vertexIndex + 1;

                edgeTriangles[edgeTriangleIndex + 3] = vertexIndex + 2;
                edgeTriangles[edgeTriangleIndex + 4] = vertexIndex + 3;
                edgeTriangles[edgeTriangleIndex + 5] = vertexIndex + 1;

                vertexIndex += 4;
                edgeTriangleIndex += 6;
            }
        }

        triangles.AddRange(edgeTriangles);

        mesh.Clear();
        mesh.vertices = vertices;
        //mesh.normals = normals;
        mesh.uv = texCoords;
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        //mesh.MarkModified();

        UpdateColliders(edgesList);
    }

    public void ExecuteClip(IClip awakeCircleClipper)
    {
        BlockSimplification.epsilon = (int64)(3.13f / 100f * 1 * VectorEx.float2int64);

        List<List<Vector2i>> solutions = new List<List<Vector2i>>();

        ClipperLib.Clipper clipper = new ClipperLib.Clipper();
        clipper.AddPolygons(polygons, ClipperLib.PolyType.ptSubject);
        clipper.AddPolygon(awakeCircleClipper.GetVertices(), ClipperLib.PolyType.ptClip);
        clipper.Execute(ClipperLib.ClipType.ctDifference, solutions,
            ClipperLib.PolyFillType.pftNonZero, ClipperLib.PolyFillType.pftNonZero);

        UpdateGeometryWithMoreVertices(solutions, 1, 1, depth);
    }

    private void UpdateColliders(List<List<Vector2>> edgesList)
    {
        if (collider)
            meshCollider.sharedMesh = mesh;
    }
}
