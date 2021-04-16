using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Demo2 {
    public class QuadtreeDemo2 : MonoBehaviour {
        private Quadtree quadtree;
        private List<Vector2> points;
        private Rect identifiedRect;

        private void Start() {
            var size = 8;
            identifiedRect = new Rect(-1, -1, 2, 2);
            var rect = new Rect(-size * 0.5f, -size * 0.5f, size, size);
            quadtree = new Quadtree(rect, 4);
            points = new List<Vector2>();
            for (int i = 0; i < 50; i++) {
                var point = new Vector2(Random.Range(rect.xMin, rect.xMax), Random.Range(rect.yMin, rect.yMax));
                quadtree.Insert(point);
                points.Add(point);
            }
            var p = quadtree.Query(identifiedRect);
            Debug.Log(p.Count);
        }

        private void Update() {
            if (Input.GetMouseButtonDown(0)) {
                var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                quadtree.Insert(pos);
                points.Add(pos);
            }
        }

        private void OnDrawGizmos() {
            if (quadtree != null) {
                Gizmos.color = Color.white;
                quadtree.DrawGizmos();
                Gizmos.color = Color.yellow;
                foreach(var p in points) {
                    Gizmos.DrawSphere(p, 0.05f);
                }

                Gizmos.color = Color.blue;
                Gizmos.DrawLine(new Vector3(identifiedRect.xMin, identifiedRect.yMax), new Vector3(identifiedRect.xMax, identifiedRect.yMax));
                Gizmos.DrawLine(new Vector3(identifiedRect.xMax, identifiedRect.yMax), new Vector3(identifiedRect.xMax, identifiedRect.yMin));
                Gizmos.DrawLine(new Vector3(identifiedRect.xMax, identifiedRect.yMin), new Vector3(identifiedRect.xMin, identifiedRect.yMin));
                Gizmos.DrawLine(new Vector3(identifiedRect.xMin, identifiedRect.yMin), new Vector3(identifiedRect.xMin, identifiedRect.yMax));
            }
        }
    }
}