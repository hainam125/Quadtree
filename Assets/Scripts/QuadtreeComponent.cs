using UnityEngine;

public class QuadtreeComponent : MonoBehaviour {
    public float size;
    public int depth;

    public Quadtree<int> Quadtree { get; private set; }

    private void Awake() {
        Quadtree = new Quadtree<int>(transform.position, size, depth);
    }

    private void OnDrawGizmos() {
        if (Quadtree != null) {
            foreach (var node in Quadtree.Nodes) DrawNode(node);
        }
    }

    private Color minColor = new Color(1, 1, 1, 1);
    private Color maxColor = new Color(0, 0.5f, 1, 0.25f);

    private void DrawNode(QuadtreeNode<int> node) {
        Gizmos.color = Color.Lerp(minColor, maxColor, node.Depth / (float)depth);
        Gizmos.DrawWireCube(node.Position, new Vector3(1, 1, 0.1f) * node.Size);
    }
}
