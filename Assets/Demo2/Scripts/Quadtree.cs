using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Demo2 {
    public class Quadtree {
        public Rect boundary;
        private int capacity;
        private List<Vector2> points;

        private Quadtree northwest;
        private Quadtree northeast;
        private Quadtree southwest;
        private Quadtree southeast;

        private bool divided;

        public Quadtree(Rect boundary, int capacity) {
            this.boundary = boundary;
            this.capacity = capacity;
            this.points = new List<Vector2>();
        }

        public void DrawGizmos() {
            Gizmos.DrawLine(new Vector3(boundary.xMin, boundary.yMax), new Vector3(boundary.xMax, boundary.yMax));
            Gizmos.DrawLine(new Vector3(boundary.xMax, boundary.yMax), new Vector3(boundary.xMax, boundary.yMin));
            Gizmos.DrawLine(new Vector3(boundary.xMax, boundary.yMin), new Vector3(boundary.xMin, boundary.yMin));
            Gizmos.DrawLine(new Vector3(boundary.xMin, boundary.yMin), new Vector3(boundary.xMin, boundary.yMax));
            if (divided) {
                northeast.DrawGizmos();
                northeast.DrawGizmos();
                southwest.DrawGizmos();
                southeast.DrawGizmos();
            }
        }

        public void Insert(Vector2 point) {
            if (!boundary.Contains(point)) {
                return;
            }
            if (points.Count < capacity) {
                points.Add(point);
            }
            else {
                if (!divided) {
                    Subdivide();
                }
                northeast.Insert(point);
                northwest.Insert(point);
                southeast.Insert(point);
                southwest.Insert(point);
            }
        }

        public List<Vector2> Query(Rect rect) {
            var found = new List<Vector2>();
            if (!rect.Overlaps(boundary)) {
                return found;
            }
            foreach(var p in points) {
                if (rect.Contains(p)) {
                    found.Add(p);
                }
            }
            if (divided) {
                found.AddRange(northeast.Query(rect));
                found.AddRange(northwest.Query(rect));
                found.AddRange(southeast.Query(rect));
                found.AddRange(southwest.Query(rect));
            }
            return found;
        }

        private void Subdivide() {
            var ne = new Rect(boundary.x + boundary.width / 2, boundary.y + boundary.height / 2, boundary.width / 2, boundary.height / 2);
            northeast = new Quadtree(ne, capacity);
            var nw = new Rect(boundary.x, boundary.y + boundary.height / 2, boundary.width / 2, boundary.height / 2);
            northwest = new Quadtree(nw, capacity);
            var se = new Rect(boundary.x + boundary.width / 2, boundary.y, boundary.width / 2, boundary.height / 2);
            southeast = new Quadtree(se, capacity);
            var sw = new Rect(boundary.x, boundary.y, boundary.width / 2, boundary.height / 2);
            southwest = new Quadtree(sw, capacity);
            divided = true;
        }
    }
}