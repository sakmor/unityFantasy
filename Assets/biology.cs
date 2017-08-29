using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uFantasy;
public class biology : MonoBehaviour
{
    public int HP;
    public int MP;
    public float moveSpeed = 0.001f;

    public Vector3 goalPos;
    Transform _transform;


    // Use this for initialization
    void Start()
    {
        _transform = transform;

    }

    // Update is called once per frame
    void Update()
    {
        movetoGoalPos();

    }
    public void SetGoalPos(Vector3 p)
    {
        goalPos = p;
    }
    bool isDistUnderMini()
    {
        return (Vector3.Distance(_transform.position, goalPos) < 0.05f);
    }
    void movetoGoalPos()
    {
        if (isDistUnderMini()) return;
        _transform.position = Vector3.MoveTowards(_transform.position, goalPos, moveSpeed);
    }
}
