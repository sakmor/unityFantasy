using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameCS : MonoBehaviour
{
    //GameObject
    GameObject Player, Cube, mainCamera, mainCamera2, hitUIObject, movePlateMouse, movePlate, cammeraPlate, logText;

    //Dictionary、Array----------------------------
    int[] cubeArrayTxt;

    Dictionary<Vector3, Vector2> cubesPosDictionary =
               new Dictionary<Vector3, Vector2>();

    //boolean----------------------------
    bool cammeraPlatein2out, movePlatein2out, clickStart, touchScreen;

    //String----------------------------
    string hitUIObjectName = "";

    //Vector2、Vector3----------------------------
    Vector2 myIputPostion;
    Vector3 mouseStartPOS, mouseDragVector, cameraRELtarget;

    //Script 自定義----------------------------

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
