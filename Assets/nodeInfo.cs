using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nodeInfo : MonoBehaviour
{

    // Use this for initialization
    public bool walkable;
    public float playerDist;
    public Vector3 worldPosition;

    public Vector3 mPosition;
    public int gridX;
    public int gridY;
    public float r;
    public int gCost;
    public int hCost;
    public Node parent;
    public Grid grid;
    public float dregg;
    public float bais;
    public Transform target;
    void Start()
    {
        target = GameObject.Find("Cha_Knight").transform;
        grid = GameObject.Find("aStart").GetComponent<Grid>();

    }

    // Update is called once per frame
    void Update()
    {
        r = Vector3.Distance(target.position, GameObject.Find("nodeINFO (1)").transform.position);
        // dregg = Vector3.Angle(new Vector3(1, 0, 0), GameObject.Find("nodeINFO (1)").transform.position);
        Vector3 cross = Vector3.Cross(new Vector3(1, 0, 0), GameObject.Find("nodeINFO (1)").transform.position);
        // if (cross.z > 0)
        // {
        //     dregg = 360 - dregg;
        // }

        //circle arow around targt
        dregg = AngleBetweenVector3(target.position, GameObject.Find("nodeINFO (1)").transform.position) + bais;
        this.transform.position = target.position + r * (new Vector3(Mathf.Cos(dregg * Mathf.Deg2Rad), 0, Mathf.Sin(dregg * Mathf.Deg2Rad)));

        playerDist = Vector3.Distance(GameObject.Find("Cha_Knight").transform.position, this.transform.position);
        gridX = grid.NodeFromWorldPoint(this.transform.position).gridX;
        gridY = grid.NodeFromWorldPoint(this.transform.position).gridY;
        walkable = grid.NodeFromWorldPoint(this.transform.position).walkable;
        worldPosition = grid.NodeFromWorldPoint(this.transform.position).worldPosition;
        mPosition = GameObject.Find("mainGame").GetComponent<gameCS>().normalized(worldPosition);

    }

    private float AngleBetweenVector3(Vector3 vec1, Vector3 vec2)
    {
        Vector3 diference = vec2 - vec1;
        float sign = (vec2.z < vec1.z) ? -1.0f : 1.0f;
        return Vector3.Angle(Vector2.right, diference) * sign;
    }
}
