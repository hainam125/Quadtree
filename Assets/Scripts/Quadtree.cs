﻿using System;
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

    public event EventHandler QuadtreeUpdated;

    public Quadtree(Vector2 position, float size, int depth) {
        this.node = new QuadtreeNode<TType>(position, size, depth);
        this.depth = depth;
    }

    public void Insert(Vector2 position, TType value) {
        var leafNode = node.Subdivide(position, value, depth);
        leafNode.Data = value;
        NotifyQuadtreeUpdate();
    }

    public void InsertCircle(Vector2 position, float radius, TType value) {
        var list = new List<QuadtreeNode<TType>>();
        node.CircleSubdivide(list, position, radius, value, depth);
        NotifyQuadtreeUpdate();
    }

    public QuadtreeNode<TType> GetRoot() => node;

    public IEnumerable<QuadtreeNode<TType>> GetLeafNodes() {
        return node.GetLeafNodes();
    }

    private void NotifyQuadtreeUpdate() {
        QuadtreeUpdated?.Invoke(this, new EventArgs());
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
    private TType data;
    private int depth;

    public QuadtreeNode(Vector2 position, float size, int depth, TType data = default(TType)) {
        this.position = position;
        this.size = size;
        this.data = data;
        this.depth = depth;
    }

    public IEnumerable<QuadtreeNode<TType>> Nodes => subnodes;

    public Vector2 Position => position;

    public float Size => size;

    public TType Data {
        get => data;
        internal set => data = value;
    }

    public bool IsLeaf() => Nodes == null;

    public IEnumerable<QuadtreeNode<TType>> GetLeafNodes() {
        if (IsLeaf()) {
            yield return this;
        }
        else if (Nodes != null) {
            foreach (var node in Nodes) {
                foreach (var leaf in node.GetLeafNodes()) {
                    yield return leaf;
                }
            }
        }
    }

    public QuadtreeNode<TType> Subdivide(Vector2 targetPosition, TType value, int depth = 0) {
        if (depth == 0) {
            return this;
        }
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
                subnodes[i] = new QuadtreeNode<TType>(newPos, size * 0.5f, depth - 1);
            }
        }
        return subnodes[subIndex].Subdivide(targetPosition, value, depth - 1);
    }

    public void CircleSubdivide(IList<QuadtreeNode<TType>> selectedNodes, Vector2 targetPosition, float radius, TType value, int depth = 0) {
        if (depth == 0) {
            data = value;
            selectedNodes.Add(this);
            return;
        }
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
                subnodes[i] = new QuadtreeNode<TType>(newPos, size * 0.5f, depth - 1, Data);
            }
        }
        for (int i = 0; i < subnodes.Length; i++) {
            if (subnodes[i].ContainedInCircle(targetPosition, radius)) {
                subnodes[i].CircleSubdivide(selectedNodes, targetPosition, radius, value, depth - 1);
            }
        }

        var shouldReduce = true;
        var initialValue = subnodes[0].Data;
        for(int i = 0; i < subnodes.Length; i++) {
            shouldReduce &= initialValue.Equals(subnodes[i].Data);
            shouldReduce &= subnodes[i].IsLeaf();
        }
        if (shouldReduce) {
            Data = initialValue;
            subnodes = null;
        }
    }

    private bool ContainedInCircle(Vector2 position, float radius) {
        Vector2 difference = Position - position;
        difference.x = Mathf.Max(0, Mathf.Abs(difference.x) - size / 2);
        difference.y = Mathf.Max(0, Mathf.Abs(difference.y) - size / 2);
        return difference.magnitude < radius;
    }
}
