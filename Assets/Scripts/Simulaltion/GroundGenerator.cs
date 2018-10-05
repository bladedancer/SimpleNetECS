using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GroundGenerator : MonoBehaviour {
    public int width;
    public int length;
    public int height = 2;

    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    int groundVertCount;

    void Awake()
    {
        generate();
    }

    // Use this for initialization
    void generate() {
        createGround();
        createWall();
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        createColliders(mesh.normals);
    }

    void createGround()
    {
        // Verts
        int offset = vertices.Count;
        vertices.Add(transform.position + new Vector3(width / 2, 0, length / 2));
        vertices.Add(transform.position + new Vector3(width / 2, 0, -length / 2));
        vertices.Add(transform.position + new Vector3(-width / 2, 0, -length / 2));
        vertices.Add(transform.position + new Vector3(-width / 2, 0, length / 2));
        groundVertCount = vertices.Count;

        // Triangles
        triangles.Add(offset + 0);
        triangles.Add(offset + 1);
        triangles.Add(offset + 3);
        triangles.Add(offset + 1);
        triangles.Add(offset + 2);
        triangles.Add(offset + 3);
    }

    void createWall()
    {
        for (int i = 0; i < groundVertCount; ++i)
        {
            int offset = vertices.Count;
            vertices.Add(vertices[i + 0]);
            vertices.Add(vertices[i + 0] + (Vector3.up * height));
            vertices.Add(vertices[i + 1] + (Vector3.up * height));
            vertices.Add(vertices[i + 1]);

            triangles.Add(offset + 0);
            triangles.Add(offset + 1);
            triangles.Add(offset + 3);
            triangles.Add(offset + 1);
            triangles.Add(offset + 2);
            triangles.Add(offset + 3);
        }
    }

    void createColliders(Vector3[] normals)
    {
        int depth = 2;
        // Ground
        BoxCollider ground = gameObject.AddComponent<BoxCollider>();
        ground.size = new Vector3(width, depth, length);
        ground.center = transform.position - new Vector3(0, ground.size.y / 2, 0);

        // Walls
        for (int i = groundVertCount; i < vertices.Count; i += 4)
        {
            float3 size = normals[i] * depth + (vertices[i] - vertices[i + 1]) + (vertices[i] - vertices[i + 3]);
            size.x = Mathf.Abs(size.x);
            size.y = Mathf.Abs(size.y);
            size.z = Mathf.Abs(size.z);
            Vector3 center = (vertices[i] + vertices[i + 1] + vertices[i + 2] + vertices[i + 3]) / 4;
            center = center - (normals[i] * depth / 2);
            BoxCollider wall = gameObject.AddComponent<BoxCollider>();
            wall.center = center;
            wall.size = size;
        }
        /*
        // Collider
        float3 size = vertices[i + 1] - vertices[i] + (Vector3.up * height);
        size.x = Mathf.Abs(size.x);
        size.z = Mathf.Abs(size.z);
        wall.size = size;
        wall.center = vertices[i + 1] + ((vertices[i] - vertices[i + 1]) / 2);
        */

    }
    void OnDrawGizmos()
    {
        // Gizmos.color = Color.green;
        // Gizmos.DrawCube(transform.position, new Vector3(width, 1, length));
    }
}
