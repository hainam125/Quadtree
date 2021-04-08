using System.Collections.Generic;
using UnityEngine;

public enum QuadtreeIndex {
    TopLeft = 0,        //00
    TopRight = 1,       //01
    BottomLeft = 2,     //10
    BottomRight = 3,    //11
}

public class Quadtree<TType> {
    private QuadtreeNode<TType> node;
    private int depth;

    public Quadtree(Vector2 position, float size, int depth) {
        this.node = new QuadtreeNode<TType>(position, size);
        this.depth = depth;
    }

    public void Insert(Vector2 position, TType value) {
        node.Subdivide(position, value, depth - 1);
    }

    public QuadtreeNode<TType> GetRoot() => node;

    public IEnumerable<QuadtreeNode<TType>> GetLeafNodes() {
        return node.GetLeafNodes();
    }

    public static int GetIndexOfPosition(Vector2 lookupPosition, Vector2 nodePosition) {
        int index = 0;
        index |= lookupPosition.y < nodePosition.y ? 2 : 0;
        index |= lookupPosition.x > nodePosition.x ? 1 : 0;
        return index;
    }
}

public class QuadtreeNode<TType> {
    private Vector2 position;
    private float size;
    private QuadtreeNode<TType>[] subnodes;
    private TType value;

    public QuadtreeNode(Vector2 position, float size, TType value = default(TType)) {
        this.position = position;
        this.size = size;
        this.value = value;
    }

    public IEnumerable<QuadtreeNode<TType>> Nodes => subnodes;

    public Vector3 Position => position;

    public float Size => size;

    public TType Value => value;

    public bool IsLeaf() => subnodes == null;

    public IEnumerable<QuadtreeNode<TType>> GetLeafNodes() {
        if (IsLeaf()) {
            yield return this;
        }
        else {
            foreach (var node in Nodes) {
                foreach(var leaf in node.GetLeafNodes()) {
                    yield return leaf;
                }
            }
        }
    }

    public void Subdivide(Vector2 targetPosition, TType value, int depth = 0) {
        var subIndex = Quadtree<TType>.GetIndexOfPosition(targetPosition, position);
        if (subnodes == null) {
            subnodes = new QuadtreeNode<TType>[4];
            for (int i = 0; i < subnodes.Length; i++) {
                Vector2 newPos = position;
                if ((i & 2) == 2) {
                    newPos.y -= size * 0.25f;
                }
                else {
                    newPos.y += size * 0.25f;
                }
                if ((i & 1) == 1) {
                    newPos.x += size * 0.25f;
                }
                else {
                    newPos.x -= size * 0.25f;
                }
                subnodes[i] = new QuadtreeNode<TType>(newPos, size * 0.5f);
            }
        }
        if (depth > 0) {
            subnodes[subIndex].Subdivide(targetPosition, value, depth - 1);
        }
    }
}
