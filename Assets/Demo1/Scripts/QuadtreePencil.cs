using UnityEngine;

public class QuadtreePencil : MonoBehaviour {
    [SerializeField] private QuadtreeComponent quadtree;
    [SerializeField] private int value;

    private void Update() {
        var ray =  Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButton(0)) {
            quadtree.Quadtree.Insert(ray.origin, 0);
        }
        else if (Input.GetMouseButton(1)) {
            quadtree.Quadtree.Insert(ray.origin, value);
        }
    }
}
