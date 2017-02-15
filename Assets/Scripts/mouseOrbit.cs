using myMath;
using UnityEngine;

//http://wiki.unity3d.com/index.php?title=MouseOrbitImproved
[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class mouseOrbit : MonoBehaviour
{


    public float distance = 10.0f;
    public float xSpeed = 5.0f;
    public float ySpeed = 5.0f;
    public Vector3 targetMove;

    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;

    public float distanceMin = 3f;
    public float distanceMax = 15f;
    bool orbitCamera = false;
    private Rigidbody rigidbody;

    private gameCS maingameCS;
    float x = 0.0f, startX = 0.0f, finalX = 0.0f, y = 0.0f;

    float orbitCameratime = 0.0f, traceTime = 0.0f;

    // Use this for initialization
    void Start()
    {
        maingameCS = GameObject.Find("mainGame").GetComponent<gameCS>();
        var angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
        finalX = x;

        // Make the rigid body not change rotation
        if (GetComponent<Rigidbody>())
        {
            GetComponent<Rigidbody>().freezeRotation = true;
        }
        mouseOrbitSet();
        x = angles.y;
        y = angles.x;
        finalX = x;
    }

    void LateUpdate()
    {

        if (maingameCS.touchScreen &&
            maingameCS.hitUIObjectName == "")
        {
            mouseOrbitSet();
        }
        if (doLeftCount > 0)
        {
            _left();
            doLeftCount--;
        }

        if (doRightCount > 0)
        {
            _right();
            doRightCount--;
        }

        if (orbitCamera)
        {
            mouseOrbitSet();
        }
    }
    int doRightCount = 0;
    int doLeftCount = 0;
    public void _right()
    {
        if (!orbitCamera)
        {
            orbitCameratime = Time.time;
            finalX = 45;
            startX = x;
            orbitCamera = true;
        }
        else
        {
            doRightCount += 1;
            doLeftCount = 0;
            doRightCount = Mathf.Clamp(doRightCount, 0, 2);
        }
    }

    public void _left()
    {
        if (!orbitCamera)
        {
            orbitCameratime = Time.time;
            finalX = -45;
            startX = x;
            orbitCamera = true;
        }
        else
        {
            doLeftCount += 1;
            doRightCount = 0;
            doLeftCount = Mathf.Clamp(doLeftCount, 0, 2);
        }
    }


    void mouseOrbitSet()
    {
        // x += Input.GetAxis("Mouse X") * xSpeed;
        // y -= Input.GetAxis("Mouse Y") * ySpeed;

        if (orbitCamera)
            x = MathS.easeInOutExpo(Time.time - orbitCameratime, startX, finalX, 0.25f);
        y = ClampAngle(y, yMinLimit, yMaxLimit);

        if (Time.time - orbitCameratime >= 0.5)
        {
            orbitCamera = false;
        }

        Quaternion rotation = Quaternion.Euler(y, x, 0);

        distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);


        Vector3 position = rotation * new Vector3(0.0f, 0.0f, -distance) + maingameCS.getPlayerPos() + targetMove;

        transform.rotation = rotation;
        transform.position = position;

        //更新攝影機與目標的相對位置
        maingameCS.cameraRELtarget = maingameCS.mainCamera.transform.position - maingameCS.getPlayerPos();

    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
}
