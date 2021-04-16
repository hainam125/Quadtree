using System;
using System.Collections.Generic;
using UnityEngine;

public enum QuadtreeIndex {
    TopLeft = 0,        //00
    TopRight = 1,       //01
    BottomLeft = 2,     //10
    BottomRight = 3,    //11
}

public class Quadtree<TType> {
    public QuadtreeNode<TType>[] Nodes { get; private set; }
    public List<QuadtreeNode<TType>> LeafNodes { get; private set; }
    public int Depth { get; private set; }

    public event EventHandler QuadtreeUpdated;

    public Quadtree(Vector2 position, float size, int depth) {
        Depth = depth;
        Nodes = BuildQuadtree(position, size);
        LeafNodes = GetLeafNodes();
    }

    public void InsertCircle(Vector2 position, float radius, TType value) {
        var list = new List<QuadtreeNode<TType>>();
        CircleSearch(list, position, radius, 0);
        foreach (var node in list) node.Data = value;
        NotifyQuadtreeUpdate();
    }

    public void CircleSearch(IList<QuadtreeNode<TType>> selectedNodes, Vector2 targetPosition, float radius, int index) {
        if (Nodes[index].Depth >= Depth) {
            selectedNodes.Add(Nodes[index]);
            return;
        }
        int nextNode = 4 * index;
        for (int i = 1; i <= 4; i++) {
            if (ContainedInCircle(targetPosition, radius, Nodes[nextNode + i])) {
                CircleSearch(selectedNodes, targetPosition, radius, nextNode + i);
            }
        }
    }

    public void Insert(Vector2 position, TType value) {
        var leafNode = Search(position, 0);
        leafNode.Data = value;
        NotifyQuadtreeUpdate();
    }

    public QuadtreeNode<TType> Search(Vector2 targetPosition, int index) {
        if (Nodes[index].Depth >= Depth) {
            return Nodes[index];
        }
        var nextNode = 4 * index + GetIndexOfPosition(targetPosition, Nodes[index].Position) + 1;
        return Search(targetPosition, nextNode);
    }

    public int GetIndexOfPosition(Vector2 lookupPosition, Vector2 nodePosition) {
        int index = 0;
        index |= lookupPosition.y < nodePosition.y ? 2 : 0;
        index |= lookupPosition.x > nodePosition.x ? 1 : 0;
        return index;
    }

    private List<QuadtreeNode<TType>> GetLeafNodes() {
        int leafNodeLength = Mathf.RoundToInt(Mathf.Pow(4, Depth));
        var leafNodes = new List<QuadtreeNode<TType>>(leafNodeLength);
        for (int i = Nodes.Length - leafNodeLength; i < Nodes.Length; i++) {
            leafNodes.Add(Nodes[i]);
        }
        leafNodes.Sort((n1, n2) => {
            int xCompare = n1.Position.x.CompareTo(n2.Position.x);
            if (xCompare == 0) return n1.Position.y.CompareTo(n2.Position.y);
            return xCompare;
        });
        return leafNodes;
    }

    private QuadtreeNode<TType>[] BuildQuadtree(Vector2 position, float size) {
        int length = 0;
        for (int i = 0; i <= Depth; i++) {
            length += (int)Mathf.Pow(4, i);
        }
        var tree = new QuadtreeNode<TType>[length];
        tree[0] = new QuadtreeNode<TType>(position, size, 0);
        BuildQuadtreeRecursive(tree, 0);
        return tree;
    }

    private void BuildQuadtreeRecursive(QuadtreeNode<TType>[] tree, int index) {
        var node = tree[index];
        if (node.Depth >= Depth) {
            return;
        }
        int nextNode = 4 * index;
        var deltaX = new Vector2(node.Size / 4, 0);
        var deltaY = new Vector2(0, node.Size / 4);
        tree[nextNode + 1] = new QuadtreeNode<TType>(node.Position - deltaX + deltaY, node.Size / 2, node.Depth + 1);
        tree[nextNode + 2] = new QuadtreeNode<TType>(node.Position + deltaX + deltaY, node.Size / 2, node.Depth + 1);
        tree[nextNode + 3] = new QuadtreeNode<TType>(node.Position - deltaX - deltaY, node.Size / 2, node.Depth + 1);
        tree[nextNode + 4] = new QuadtreeNode<TType>(node.Position + deltaX - deltaY, node.Size / 2, node.Depth + 1);
        BuildQuadtreeRecursive(tree, nextNode + 1);
        BuildQuadtreeRecursive(tree, nextNode + 2);
        BuildQuadtreeRecursive(tree, nextNode + 3);
        BuildQuadtreeRecursive(tree, nextNode + 4);
    }

    private bool ContainedInCircle(Vector2 position, float radius, QuadtreeNode<TType> node) {
        Vector2 difference = node.Position - position;
        difference.x = Mathf.Max(0, Mathf.Abs(difference.x) - node.Size / 2);
        difference.y = Mathf.Max(0, Mathf.Abs(difference.y) - node.Size / 2);
        return difference.magnitude < radius;
    }

    private void NotifyQuadtreeUpdate() {
        QuadtreeUpdated?.Invoke(this, new EventArgs());
    }
}

public class QuadtreeNode<TType> {
    private Vector2 position;
    private float size;
    private TType data;
    private int depth;

    public QuadtreeNode(Vector2 position, float size, int depth, TType data = default(TType)) {
        this.position = position;
        this.size = size;
        this.data = data;
        this.depth = depth;
    }

    public Vector2 Position => position;

    public float Size => size;

    public int Depth => depth;

    public TType Data {
        get => data;
        internal set => data = value;
    }
}
