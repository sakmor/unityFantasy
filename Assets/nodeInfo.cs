using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using myMath;

public class nodeInfo : MonoBehaviour
{

    // Use this for initialization
    public bool walkable;
    public float playerDist;
    public Vector3 worldPosition;

    public Vector3 mPosition;
    public int gridX;
    public int gridY;
    public int gCost;
    public int hCost;
    public Node parent;
    public Grid grid;
    public float dregg;
    public float right;
    public float forward;
    public Transform target;
    private float rr;
    void Start()
    {
        target = GameObject.Find("Cha_Knight").transform;
        grid = GameObject.Find("aStart").GetComponent<Grid>();
        rr = Vector3.Distance(this.transform.position, target.position);
    }

    // Update is called once per frame
    void Update()
    {

        this.transform.position = MathS.getCirclePath(this.transform.position, target.position, right, rr + forward);

        playerDist = Vector3.Distance(GameObject.Find("Cha_Knight").transform.position, this.transform.position);
        gridX = grid.NodeFromWorldPoint(this.transform.position).gridX;
        gridY = grid.NodeFromWorldPoint(this.transform.position).gridY;
        walkable = grid.NodeFromWorldPoint(this.transform.position).walkable;
        worldPosition = grid.NodeFromWorldPoint(this.transform.position).worldPosition;
        mPosition = GameObject.Find("mainGame").GetComponent<gameCS>().normalized(worldPosition);

    }
}
