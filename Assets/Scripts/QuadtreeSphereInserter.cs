using UnityEngine;

public class QuadtreeSphereInserter : MonoBehaviour {
    [SerializeField] private QuadtreeComponent quadtree;
    [SerializeField] private float radius;

    private void Update() {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButton(0)) {
            quadtree.Quadtree.InsertCircle(ray.origin, radius, false);
        }
        else if (Input.GetMouseButton(1)) {
            quadtree.Quadtree.InsertCircle(ray.origin, radius, true);
        }
    }
}
