using UnityEngine;

public class QuadMeshCreator : MonoBehaviour
{
    [SerializeField] private bool generate;
    [SerializeField] private QuadtreeComponent quadtree;

    private void Update() {
        if (generate) {
            GenerateMesh();
            generate = false;
        }
    }

    private void GenerateMesh() {
        foreach (var leaf in quadtree.Quadtree.GetLeafNodes()) {
            var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
            go.transform.parent = quadtree.transform;
            go.transform.localPosition = leaf.Position;
            go.transform.localScale = leaf.Size * Vector3.one;
        }
    }
}
