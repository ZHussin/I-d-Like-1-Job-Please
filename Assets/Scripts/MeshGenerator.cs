using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;
    float hexagonRatio = 0.8660254f;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateHexagon(0f, 0f, 0f);
        UpdateMesh();
    }

    void CreateHexagon(Vector3 offsetVector)
    {
        CreateHexagon(offsetVector.x, offsetVector.y, offsetVector.z);
    }

    void CreateHexagon(float x, float y, float z)
    {
        // Vertices are listed in clockwise order beginning with the vertex on the positive z axis
        vertices = new Vector3[]
        {
            // Top face
            new Vector3(0,0.1f,0.5f),
            new Vector3(hexagonRatio/2,0.1f,0.25f),
            new Vector3(hexagonRatio/2,0.1f,-0.25f),
            new Vector3(0,0.1f,-0.5f),
            new Vector3(-hexagonRatio/2,0.1f,-0.25f),
            new Vector3(-hexagonRatio/2,0.1f,0.25f),
            // Bottom face
            new Vector3(0,0,0.5f),
            new Vector3(hexagonRatio/2,0,0.25f),
            new Vector3(hexagonRatio/2,0,-0.25f),
            new Vector3(0,0,-0.5f),
            new Vector3(-hexagonRatio/2,0,-0.25f),
            new Vector3(-hexagonRatio/2,0,0.25f),

            // Repeating all vertices to use for the sides, to get proper normal calculations for lighting
            // Repeated again b/c we can't use the same vertices for adjacent sides. Frick normals
            // Top face
            new Vector3(0,0.1f,0.5f),
            new Vector3(hexagonRatio/2,0.1f,0.25f),
            new Vector3(hexagonRatio/2,0.1f,-0.25f),
            new Vector3(0,0.1f,-0.5f),
            new Vector3(-hexagonRatio/2,0.1f,-0.25f),
            new Vector3(-hexagonRatio/2,0.1f,0.25f),
            // Bottom face
            new Vector3(0,0,0.5f),
            new Vector3(hexagonRatio/2,0,0.25f),
            new Vector3(hexagonRatio/2,0,-0.25f),
            new Vector3(0,0,-0.5f),
            new Vector3(-hexagonRatio/2,0,-0.25f),
            new Vector3(-hexagonRatio/2,0,0.25f),

            // Repeating all vertices to use for the sides, to get proper normal calculations for lighting
            // Top face
            new Vector3(0,0.1f,0.5f),
            new Vector3(hexagonRatio/2,0.1f,0.25f),
            new Vector3(hexagonRatio/2,0.1f,-0.25f),
            new Vector3(0,0.1f,-0.5f),
            new Vector3(-hexagonRatio/2,0.1f,-0.25f),
            new Vector3(-hexagonRatio/2,0.1f,0.25f),
            // Bottom face
            new Vector3(0,0,0.5f),
            new Vector3(hexagonRatio/2,0,0.25f),
            new Vector3(hexagonRatio/2,0,-0.25f),
            new Vector3(0,0,-0.5f),
            new Vector3(-hexagonRatio/2,0,-0.25f),
            new Vector3(-hexagonRatio/2,0,0.25f)
        };

        triangles = new int[]
        {
            // Top face
            0,1,2,
            2,3,4,
            4,5,0,
            0,2,4,
            // Bottom face
            8,7,6,
            10,9,8,
            6,11,10,
            10,8,6,
        };
        System.Array.Resize(ref triangles, 60);
            // this generates the 6 sides in clockwise order
        for (int i = 0; i < 6; i+=2)
        {
            triangles[24 + i*6] = 12 + (0 + i);
            triangles[25 + i*6] = 12 + (6 + i);
            triangles[26 + i*6] = 12 + (1 + i) % 6;
            triangles[27 + i*6] = 12 + (6 + i);
            triangles[28 + i*6] = 12 + (1 + i) % 6 + 6;
            triangles[29 + i*6] = 12 + (1 + i) % 6;
        }

        for (int i = 1; i < 6; i += 2)
        {
            triangles[24 + i * 6] = 24 + (0 + i);
            triangles[25 + i * 6] = 24 + (6 + i);
            triangles[26 + i * 6] = 24 + (1 + i) % 6;
            triangles[27 + i * 6] = 24 + (6 + i);
            triangles[28 + i * 6] = 24 + (1 + i) % 6 + 6;
            triangles[29 + i * 6] = 24 + (1 + i) % 6;
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    public KeyCode saveKey = KeyCode.Space;
    public string saveName = "SavedMesh";

    void SaveAsset()
    {
        string savePath = "Assets/" + saveName + ".asset";
        Debug.Log("Saved Mesh to:" + savePath);
        AssetDatabase.CreateAsset(mesh, savePath);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(saveKey))
        {
            SaveAsset();
        }
    }
}
