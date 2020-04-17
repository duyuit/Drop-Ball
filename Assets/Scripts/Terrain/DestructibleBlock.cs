using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using int64 = System.Int64;
using Vector2i = ClipperLib.IntPoint;
using Vector2f = UnityEngine.Vector2;

public class DestructibleBlock
{

    public DestructibleBlock()
    {
        polygons = new List<List<Vector2i>>();

        subVertices = new Vector3[0];
        subTexCoords = new Vector2[0]; ;
        subTriangles = new int[0];
        subNormals = new Vector3[0]; 
    }

    public List<List<Vector2i>> polygons;

    public Vector3[] subVertices;

    public Vector2[] subTexCoords;

    public Vector3[] subNormals;

    public int[] subTriangles;

    public void UpdateSubEdgeMesh(List<List<Vector2i>> inPolygons, float depth)
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
                texCoords[vertexIndex] = new Vector2f(point.x, point.y);
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

        subVertices = vertices;
        subTexCoords = texCoords;
        subTriangles = triangles.ToArray();
        subNormals = normals;
    }
}
