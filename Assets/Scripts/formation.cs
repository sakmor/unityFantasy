using UnityEngine;


public class formation : MonoBehaviour
{

    public Grid grid;
    public int n = 0;

    Vector3 newPos;

    Transform leader;
    void Start()
    {
        grid = GameObject.Find("aStart").GetComponent<Grid>();
        newPos = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(this.transform.parent.transform);

        //如果原始位置可以站立
        if (grid.NodeFromWorldPoint(this.transform.position).walkable)
        {
            newPos = this.transform.position;
        }
        else
        {
            while (grid.NodeFromWorldPoint(newPos).walkable == false)
            {
                newPos += transform.forward;

            }

        }


    }

}
