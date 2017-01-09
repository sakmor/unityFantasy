using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nodeInfo : MonoBehaviour
{

    // Use this for initialization
    public bool walkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;
    public Node parent;
    public Grid grid;
    void Start()
    {
        grid = GameObject.Find("aStart").GetComponent<Grid>();
    }

    // Update is called once per frame
    void Update()
    {
        gridX = grid.NodeFromWorldPoint(this.transform.position).gridX;
        gridY = grid.NodeFromWorldPoint(this.transform.position).gridY;
        walkable = grid.NodeFromWorldPoint(this.transform.position).walkable;
        worldPosition = grid.NodeFromWorldPoint(this.transform.position).worldPosition;

    }
}
