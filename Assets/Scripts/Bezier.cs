using UnityEngine;
using myMath;

[RequireComponent(typeof(LineRenderer))]
public class Bezier : MonoBehaviour
{
    public Transform[] controlPoints;
    public LineRenderer lineRenderer;
    public bool drawIt = false;

    private int curveCount = 0;
    private int layerOrder = 0;
    private int SEGMENT_COUNT = 50;
    private float startTime;
    public float duration, linepapa, fadeOut;

    Renderer rend;
    void Awake()
    {
        // Debug.Log("Awake");
    }
    public void line2target(Transform target)
    {
        if (this.controlPoints[2] == this.transform.parent.transform)
        {
            this.controlPoints[2] = target;
            this.controlPoints[3] = target;
            drawIt = true;
            linepapa = 0;
            startTime = Time.time;
            rend.material.SetColor("_Color", new Color(1, 1, 1, 1));
        }
    }
    public void closeLine()
    {
        this.controlPoints[2] = this.transform.parent.transform;
        this.controlPoints[3] = this.transform.parent.transform;
        drawIt = false;

    }
    void Start()
    {
        rend = GetComponent<Renderer>();
        rend.material = new Material(Shader.Find("Custom/UnlitAlphaWithFade"));
        rend.material.SetColor("_Color", new Color(1, 1, 1, 1f));
        rend.material.mainTexture = Resources.Load("texture/linebeam") as Texture;

        linepapa = 0;
        duration = 1f;
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

        for (int j = 0; j < curveCount; j++)
        {
            for (int i = 1; i <= SEGMENT_COUNT; i++)
            {
                float t = i / (float)SEGMENT_COUNT;

                if (linepapa <= 0.9)
                {
                    // y=ax^b+cx
                    linepapa = MathS.easeInQuad(Time.time - startTime, 0, 1, duration);
                    // if (linepapa > 1)
                    // {
                    //     linepapa = 0.01f;
                    // }
                    if (t > linepapa)
                    {
                        t = linepapa;
                    }
                    fadeOut = 1;
                }
                else
                {
                    fadeOut -= Time.deltaTime * 0.03f;
                    if (fadeOut > 0)
                    {
                        rend.material.SetColor("_Color", new Color(1, 1, 1, fadeOut));
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
