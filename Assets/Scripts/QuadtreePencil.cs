using UnityEngine;

public class QuadtreePencil : MonoBehaviour {
    [SerializeField] private QuadtreeComponent quadtree;

    private void Update() {
        var ray =  Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButton(0)) {
            quadtree.Quadtree.Insert(ray.origin, false);
        }
        else if (Input.GetMouseButton(1)) {
            quadtree.Quadtree.Insert(ray.origin, true);
        }
    }
}
