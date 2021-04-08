using UnityEngine;

public class QuadtreeComponent : MonoBehaviour {
    public float size;
    public int depth;

    public Quadtree<bool> Quadtree { get; private set; }

    private void Awake() {
        Quadtree = new Quadtree<bool>(transform.position, size, depth);
    }

    private void OnDrawGizmos() {
        if (Quadtree != null) DrawNode(Quadtree.GetRoot());
    }

    private Color minColor = new Color(1, 1, 1, 1);
    private Color maxColor = new Color(0, 0.5f, 1, 0.25f);

    private void DrawNode(QuadtreeNode<bool> node, int nodeDepth = 0) {
        if (!node.IsLeaf()) {
            foreach (var subNode in node.Nodes) {
                if (subNode != null) DrawNode(subNode, nodeDepth + 1);
            }
        }
        Gizmos.color = Color.Lerp(minColor, maxColor, nodeDepth / (float)depth);
        Gizmos.DrawWireCube(node.Position, new Vector3(1, 1, 0.1f) * node.Size);
    }
}
