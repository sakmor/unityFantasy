using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Grid))]



public class Pathfinding : MonoBehaviour
{
    private Vector3 nextPos;
    Grid grid;

    void Awake()
    {
        grid = GetComponent<Grid>();
        Debug.Log("wake up");
    }

    public Vector3 FindPath_Update(Transform aseeker, Transform atarget)
    {

        Debug.Log("尋路中");
        grid.CreateGrid();
        transform.position = new Vector3(Mathf.Floor(aseeker.position.x), 0, Mathf.Floor(aseeker.position.z));
        grid.gridWorldSizeShift = new Vector2(Mathf.Floor(aseeker.position.x), Mathf.Floor(aseeker.position.z));
        grid.CreateGrid();
        FindPath(aseeker.position, atarget.position);
        return nextPos;
    }

    Vector3 redo(Node nowNode, float angel, Grid grid)
    {
        foreach (Node neighbour in grid.GetNeighbours(nowNode))
        {
            if (grid.path.Contains(neighbour) &&
                neighbour != nowNode)
            {
                Vector2 nowVec = new Vector2(nowNode.gridX, nowNode.gridY) - new Vector2(neighbour.gridX, neighbour.gridY);
                float nowAngel = Mathf.Atan2(nowVec.y, nowVec.x) * Mathf.Rad2Deg;
                //如果方向一致，遞迴函數找下一個
                if (nowAngel - angel == 0)
                {
                    return redo(neighbour, nowAngel, grid);
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
                        float nowAngel = Mathf.Atan2(nowVec.y, nowVec.x) * Mathf.Rad2Deg;
                        nextPos = redo(startNode, nowAngel, grid);
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
