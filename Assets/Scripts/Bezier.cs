using UnityEngine;
using System.Collections.Generic;
[RequireComponent(typeof(LineRenderer))]
public class Bezier : MonoBehaviour
{
    public Transform[] controlPoints;
    public LineRenderer lineRenderer;
    public bool drawIt = false;

    private int curveCount = 0;
    private int layerOrder = 0;
    private int SEGMENT_COUNT = 50;
    public float linepapa;

    public float a, b, c, l, startTime, thistime;


    void Awake()
    {
        // Debug.Log("Awake");
    }

    void Start()
    {
        linepapa = 0;
        a = 10;
        b = 6;
        c = 1;
        l = 5f;
        if (!lineRenderer)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
        lineRenderer.sortingLayerID = layerOrder;
        curveCount = (int)controlPoints.Length / 3;
        startTime = Time.time;
    }

    void Update()
    {
        // print(Time.deltaTime);
        if (drawIt)
        {
            DrawCurve();
        }

    }

    void DrawCurve()
    {
        thistime = Time.time;
        for (int j = 0; j < curveCount; j++)
        {
            for (int i = 1; i <= SEGMENT_COUNT; i++)
            {
                float t = i / (float)SEGMENT_COUNT;

                if (linepapa <= 1)
                {
                    // y=ax^b+cx
                    Debug.Log("linepapa");
                    float x = (Time.time - startTime) / l;
                    float y = Mathf.Pow(a * x, b) + c * x;
                    linepapa = y / (a + b);
                    // if (linepapa > 1)
                    // {
                    //     linepapa = 0.01f;
                    // }
                    if (t > linepapa)
                    {
                        t = linepapa;
                    }
                }
                int nodeIndex = j * 3;
                Vector3 pixel = CalculateCubicBezierPoint(t, controlPoints[nodeIndex].position, controlPoints[nodeIndex + 1].position, controlPoints[nodeIndex + 2].position, controlPoints[nodeIndex + 3].position);
                lineRenderer.SetVertexCount(((j * SEGMENT_COUNT) + i));
                lineRenderer.SetPosition((j * SEGMENT_COUNT) + (i - 1), pixel);

            }

        }
    }

    Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uuu * p0;
        p += 3 * uu * t * p1;
        p += 3 * u * tt * p2;
        p += ttt * p3;

        return p;
    }
}
