using UnityEngine;

public class QuadtreeSphereInserter : MonoBehaviour {
    [SerializeField] private QuadtreeComponent quadtree;
    [SerializeField] private float radius;
    [SerializeField] private int value;

    private void Update() {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButton(0)) {
            quadtree.Quadtree.InsertCircle(ray.origin, radius, 0);
        }
        else if (Input.GetMouseButton(1)) {
            quadtree.Quadtree.InsertCircle(ray.origin, radius, value);
        }
    }
}
