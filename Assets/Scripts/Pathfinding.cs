using UnityEngine;
// using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Grid))]



public class Pathfinding : MonoBehaviour
{
    private Vector3 nextPos;
    Grid grid;

    void Awake()
    {
        grid = GetComponent<Grid>();
    }

    public Vector3 FindPath_Update(Vector3 aseeker, Vector3 atarget)
    {
        FindPath(aseeker, atarget);
        return nextPos;
    }
    public bool decteBetween(Vector3 start, Vector3 target)
    {
        Vector3 pp = start;
        float xx = target.x - start.x;
        float zz = target.z - start.z;

        pp.x += xx * 0.20f;
        pp.z += zz * 0.20f;
        Node now = grid.NodeFromWorldPoint(pp);
        if (!grid.walkable(now)) return false;
        pp.x += xx * 0.20f;
        pp.z += zz * 0.20f;
        now = grid.NodeFromWorldPoint(pp);
        if (!grid.walkable(now)) return false;
        pp.x += xx * 0.20f;
        pp.z += zz * 0.20f;
        now = grid.NodeFromWorldPoint(pp);
        if (!grid.walkable(now)) return false;
        pp.x += xx * 0.20f;
        pp.z += zz * 0.20f;
        now = grid.NodeFromWorldPoint(pp);
        if (!grid.walkable(now)) return false;

        return true;

    }
    //找出第一個轉彎點
    Vector3 redo(Node nowNode, Node lastNode, float angel)
    {
        foreach (Node neighbour in grid.GetNeighbours(nowNode))
        {
            if (grid.path.Contains(neighbour))
            {
                Vector2 nowVec = new Vector2(nowNode.gridX, nowNode.gridY) - new Vector2(neighbour.gridX, neighbour.gridY);
                float nowAngel = Mathf.Atan2(nowVec.y, nowVec.x);
                //如果方向一致，遞迴函數找下一個
                if (nowAngel == angel)
                {
                    return redo(neighbour, nowNode, nowAngel);
                }
                else if
                //如果現在這個節點就是上一個節點
                 (neighbour == lastNode)
                {
                    continue;
                }
                else
                //如果方向不同，結束遞迴狀態
                {
                    return nowNode.worldPosition;

                }
            }
        }
        return new Vector3(0, 0, 0);

    }
    void FindPath(Vector3 startPos, Vector3 targetPos)
    {

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node node = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost)
                {
                    if (openSet[i].hCost < node.hCost)
                        node = openSet[i];
                }
            }

            openSet.Remove(node);
            closedSet.Add(node);

            if (node == targetNode)
            {
                RetracePath(startNode, targetNode);
                //取得最近可以走得node
                foreach (Node neighbour in grid.GetNeighbours(startNode))
                {
                    if (grid.path.Contains(neighbour))
                    {
                        Vector2 nowVec = new Vector2(startNode.gridX, startNode.gridY) - new Vector2(neighbour.gridX, neighbour.gridY);
                        float nowAngel = Mathf.Atan2(nowVec.y, nowVec.x);
                        nextPos = redo(startNode, startNode, nowAngel);
                        if (nextPos == new Vector3(0, 0, 0))
                        {
                            nextPos = targetPos;
                        }
                        break;
                    }

                }

                return;

            }


            foreach (Node neighbour in grid.GetNeighbours(node))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = node;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }

        }
        nextPos = new Vector3(-999, -999, -999);
        return;
    }

    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        grid.path = path;

    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}
