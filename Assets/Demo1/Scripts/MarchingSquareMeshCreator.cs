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

        var leafs = quadtree.Quadtree.LeafNodes;
        var size = (int)Mathf.Pow(2, quadtree.depth) - 1;
        for (int x = 0; x < size; x++) {
            for (int y = 0; y < size; y++) {
                int top = x + y * (size + 1);
                int bottom = x + (y + 1) * (size + 1);
                var topLeft = leafs[top];
                var topRight = leafs[top + 1];
                var bottomLeft = leafs[bottom];
                var bottomRight = leafs[bottom + 1];
                NewMethod(new QuadtreeNode<int>[] { topLeft, topRight, bottomLeft, bottomRight }, vertices, triangles, uvs, normals, colors);
            }
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

    private void NewMethod(QuadtreeNode<int>[] points, List<Vector3> vertices, List<int> triangles, List<Vector2> uvs, List<Vector3> normals, List<Color> colors) {
        var leaf = points[0];
        var upperLeft = new Vector3(leaf.Position.x - leaf.Size * 0.5f, leaf.Position.y + leaf.Size * 0.5f, 0);
        var initialIndex = vertices.Count;

        vertices.Add(points[0].Position);
        vertices.Add(points[1].Position);
        vertices.Add(points[2].Position);
        vertices.Add(points[3].Position);

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

        colors.Add(voxelColorCodes[points[0].Data]);
        colors.Add(voxelColorCodes[points[1].Data]);
        colors.Add(voxelColorCodes[points[2].Data]);
        colors.Add(voxelColorCodes[points[3].Data]);
    }
}
