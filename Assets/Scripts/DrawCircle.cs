using UnityEngine;
using myMath;

[RequireComponent(typeof(LineRenderer))]

public class DrawCircle : MonoBehaviour
{
    public enum Axis { X, Y, Z };

    [SerializeField]
    [Tooltip("The number of lines that will be used to draw the circle. The more lines, the more the circle will be \"flexible\".")]
    [Range(0, 1000)]
    private int _segments = 30;

    [SerializeField]
    [Tooltip("The radius of the horizontal axis.")]
    private float _horizRadius = 30;

    [SerializeField]
    [Tooltip("The radius of the vertical axis.")]
    private float _vertRadius = 30;

    [SerializeField]
    [Tooltip("The offset will be applied in the direction of the axis.")]
    private float _offset = 0.75f;

    [SerializeField]
    [Tooltip("The axis about which the circle is drawn.")]
    private Axis _axis = Axis.Y;

    [SerializeField]
    [Tooltip("If checked, the circle will be rendered again each time one of the parameters change.")]
    private bool _checkValuesChanged = true;

    private int _previousSegmentsValue;
    private float _previousHorizRadiusValue;
    private float _previousVertRadiusValue;
    private float _previousOffsetValue;

    private float blinkDirect;
    private Axis _previousAxisValue;

    private float linePrecent = 0;
    private float _linePrecent = 0;
    private LineRenderer _line;
    private Renderer rend;
    private float lastBlinkTime;

    void Start()
    {
        rend = GetComponent<Renderer>();
        _line = gameObject.GetComponent<LineRenderer>();
        _line.SetVertexCount(_segments + 1);
        _line.useWorldSpace = false;
        _line.startWidth = 0.3f;
        _line.endWidth = 0.3f;
        _line.material = Resources.Load("item/model/Materials/LineBeam", typeof(Material)) as Material;
        UpdateValuesChanged();
        CreatePoints();
    }

    public void setLinePrecent(float n)
    {
        linePrecent = Mathf.Clamp(n, 0, 1);

    }
    public void setScale(float nA)
    {

        nA *= 0.05f;
        _horizRadius *= nA;
        _vertRadius *= nA;

    }
    public void blink()
    {
        Color tempColor = rend.material.GetColor("_Color");

        if (tempColor.a >= 1f)
            blinkDirect = -0.025f;
        if (tempColor.a <= 0f)
            blinkDirect = 0.025f;

        tempColor += new Color(0, 0, 0, blinkDirect);
        rend.material.SetColor("_Color", tempColor);

    }

    public void setAlpha()
    {
        Color tempColor = rend.material.GetColor("_Color");
        rend.material.SetColor("_Color", new Color(tempColor.r, tempColor.g, tempColor.b, 1));
    }



    void Update()
    {
        if (_checkValuesChanged)
        {
            if (_previousSegmentsValue != _segments ||
                _previousHorizRadiusValue != _horizRadius ||
                _previousVertRadiusValue != _vertRadius ||
                _previousOffsetValue != _offset ||
                _previousAxisValue != _axis ||
                _linePrecent != linePrecent)
            {
                CreatePoints();
            }

            UpdateValuesChanged();
        }
    }

    void UpdateValuesChanged()
    {
        _previousSegmentsValue = _segments;
        _previousHorizRadiusValue = _horizRadius;
        _previousVertRadiusValue = _vertRadius;
        _previousOffsetValue = _offset;
        _previousAxisValue = _axis;
        _linePrecent = linePrecent;
    }

    void CreatePoints()
    {

        if (_previousSegmentsValue != _segments)
        {
            _line.SetVertexCount(_segments + 1);
        }

        float x;
        float y;
        //除localScale，目的是讓高度不隨模型放大縮小改變
        float z = _offset / transform.localScale.y;

        float angle = 0f;

        for (int i = 0; i < (_segments + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * _horizRadius;
            y = Mathf.Cos(Mathf.Deg2Rad * angle) * _vertRadius;

            switch (_axis)
            {
                case Axis.X:
                    _line.SetPosition(i, new Vector3(z, y, x));
                    break;
                case Axis.Y:
                    _line.SetPosition(i, new Vector3(y, z, x));
                    break;
                case Axis.Z:
                    _line.SetPosition(i, new Vector3(x, y, z));
                    break;
                default:
                    break;
            }

            angle += (360f / _segments * linePrecent);
        }
    }
}