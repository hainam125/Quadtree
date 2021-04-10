using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarchingSquareMeshCreator : MonoBehaviour {
    private readonly Color[] voxelColorCodes = new Color[] {
        Color.clear,
        Color.red,
        Color.green,
        Color.blue,
    };

    [SerializeField] private bool generate;
    [SerializeField] private QuadtreeComponent quadtree;
    [SerializeField] private Material voxelMaterial;

    private GameObject prevMesh;
    private bool initialized;

    private void Update() {
        if (quadtree.Quadtree != null && !initialized) {
            initialized = false;
            quadtree.Quadtree.QuadtreeUpdated += (obj, args) => generate = true;
        }

        if (generate) {
            generate = false;
            var mesh = GenerateMesh();
            if (prevMesh != null) {
                Destroy(prevMesh);
            }
            prevMesh = mesh;
        }
    }

    private GameObject GenerateMesh() {
        GameObject chunk = new GameObject("Voxel Chunk");
        chunk.transform.parent = transform;
        chunk.transform.localPosition = Vector3.zero;

        var mesh = new Mesh();
        var vertices = new List<Vector3>();
        var triangles = new List<int>();
        var uvs = new List<Vector2>();
        var normals = new List<Vector3>();
        var colors = new List<Color>();

        foreach (var leaf in quadtree.Quadtree.GetLeafNodes()) {
            if (leaf.Data == 0) continue;
            NewMethod(leaf, vertices, triangles, uvs, normals, colors);
        }

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetUVs(0, uvs);
        mesh.SetNormals(normals);
        mesh.SetColors(colors);


        var meshFilter = chunk.AddComponent<MeshFilter>();
        var meshRenderer = chunk.AddComponent<MeshRenderer>();
        meshRenderer.material = voxelMaterial;

        meshFilter.mesh = mesh;
        return chunk;
    }

    private void NewMethod(QuadtreeNode<int> leaf, List<Vector3> vertices, List<int> triangles, List<Vector2> uvs, List<Vector3> normals, List<Color> colors) {
        var upperLeft = new Vector3(leaf.Position.x - leaf.Size * 0.5f, leaf.Position.y + leaf.Size * 0.5f, 0);
        var initialIndex = vertices.Count;

        vertices.Add(upperLeft);
        vertices.Add(upperLeft + Vector3.right * leaf.Size);
        vertices.Add(upperLeft + Vector3.down * leaf.Size);
        vertices.Add(upperLeft + Vector3.down * leaf.Size + Vector3.right * leaf.Size);

        uvs.Add(upperLeft);
        uvs.Add(upperLeft + Vector3.right * leaf.Size);
        uvs.Add(upperLeft + Vector3.down * leaf.Size);
        uvs.Add(upperLeft + Vector3.down * leaf.Size + Vector3.right * leaf.Size);

        normals.Add(Vector3.back);
        normals.Add(Vector3.back);
        normals.Add(Vector3.back);
        normals.Add(Vector3.back);

        triangles.Add(initialIndex);
        triangles.Add(initialIndex + 1);
        triangles.Add(initialIndex + 2);

        triangles.Add(initialIndex + 3);
        triangles.Add(initialIndex + 2);
        triangles.Add(initialIndex + 1);

        colors.Add(voxelColorCodes[leaf.Data]);
        colors.Add(voxelColorCodes[leaf.Data]);
        colors.Add(voxelColorCodes[leaf.Data]);
        colors.Add(voxelColorCodes[leaf.Data]);
    }
}
