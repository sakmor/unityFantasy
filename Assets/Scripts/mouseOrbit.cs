using myMath;
using UnityEngine;

//http://wiki.unity3d.com/index.php?title=MouseOrbitImproved
[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class mouseOrbit : MonoBehaviour
{

    public Transform target;
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
    float x = 0.0f, startX = 0.0f, finalX = 0.0f;
    float y = 0.0f;

    float stime = 0.0f;

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

        if (orbitCamera)
        {
            mouseOrbitSet();
        }
    }
    public void right()
    {
        stime = Time.time;
        finalX = 90;
        startX = x;
        orbitCamera = true;
    }

    public void left()
    {
        stime = Time.time;
        finalX = -90;
        startX = x;
        orbitCamera = true;
    }


    void mouseOrbitSet()
    {
        // x += Input.GetAxis("Mouse X") * xSpeed;
        // y -= Input.GetAxis("Mouse Y") * ySpeed;

        if (orbitCamera)
            x = MathS.easeInOutExpo(Time.time - stime, startX, finalX, 0.5f);
        y = ClampAngle(y, yMinLimit, yMaxLimit);

        if (Time.time - stime >= 0.5)
        {
            orbitCamera = false;
        }

        Quaternion rotation = Quaternion.Euler(y, x, 0);

        distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);


        Vector3 position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position + targetMove;

        transform.rotation = rotation;
        transform.position = position;

        //更新攝影機與目標的相對位置
        maingameCS.cameraRELtarget = maingameCS.mainCamera.transform.position - maingameCS.Player.transform.position;

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
