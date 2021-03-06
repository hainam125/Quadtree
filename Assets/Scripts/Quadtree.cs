using System.Collections.Generic;
using UnityEngine;

public enum QuadtreeIndex {
    TopLeft = 0,        //00
    TopRight = 1,       //01
    BottomLeft = 2,     //10
    BottomRight = 3,    //11
}

public class Quadtree<TType> : MonoBehaviour {
    private QuadtreeNode<TType> node;
    private int depth;

    public Quadtree(Vector2 position, float size, int depth) {
        node = new QuadtreeNode<TType>(position, size);
        node.Subdivide(depth);
    }

    public QuadtreeNode<TType> GetRoot() => node;

    private int GetIndexOfPosition(Vector2 lookupPosition, Vector2 nodePosition) {
        int index = 0;
        index |= lookupPosition.y > nodePosition.y ? 2 : 0;
        index |= lookupPosition.x > nodePosition.x ? 1 : 0;
        return index;
    }
}

public class QuadtreeNode<TType> {
    private Vector2 position;
    private float size;
    private QuadtreeNode<TType>[] subnodes;
    private IList<TType> value;

    public QuadtreeNode(Vector2 position, float size) {
        this.position = position;
        this.size = size;
    }

    public IEnumerable<QuadtreeNode<TType>> Nodes => subnodes;

    public Vector3 Position => position;

    public float Size => size;

    public bool IsLeaf() => subnodes == null;

    public void Subdivide(int depth = 0) {
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
            if (depth > 0) {
                subnodes[i].Subdivide(depth - 1);
            }
        }
    }
}
