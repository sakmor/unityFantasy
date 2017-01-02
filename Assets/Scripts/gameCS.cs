using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameCS : MonoBehaviour
{
    //GameObject
    GameObject cammeraPlateMouse, Player, Cube, mainCamera, mainCamera2, hitUIObject, movePlateMouse, movePlate, cammeraPlate, logText;

    //Dictionary、Array----------------------------
    int[] cubeArrayTxt;
    GameObject[] allBiologys;
    Dictionary<Vector3, Vector2> cubesPosDictionary =
               new Dictionary<Vector3, Vector2>();

    //boolean----------------------------
    bool cammeraPlatein2out, movePlatein2out, clickStart, touchScreen;

    //String----------------------------
    string hitUIObjectName = "";

    //Vector2、Vector3----------------------------
    Vector2 myIputPostion;
    Vector3 lastCameraPos, mouseStartPOS, mouseDragVector, cameraRELtarget;

    //Script 自定義----------------------------
    Pathfinding PathfindingCS;
    biologyCS playerBioCS;
    TextAsset biologyList;

    //UnityEngine ----------------------------
    Camera camera1, camera2;
    // Use this for initialization
    void Start()
    {
        //將所有生物套上biologyCS;
        allBiologys = GameObject.FindGameObjectsWithTag("biology");
        foreach (GameObject thisBiology in allBiologys)
        {
            thisBiology.AddComponent<biologyCS>();
        }

        biologyList = Resources.Load("db/biologyList", typeof(TextAsset)) as TextAsset;
        logText = GameObject.Find("logText");
        // logg('This Device is:' + SystemInfo.deviceType);

        cammeraPlateMouse = GameObject.Find("cammeraPlateMouse");
        movePlateMouse = GameObject.Find("movePlateMouse");
        movePlate = GameObject.Find("movePlate");
        Cube = GameObject.Find("Cube");
        Player = GameObject.Find("Cha_Knight");
        mainCamera = GameObject.Find("mainCamera");
        mainCamera2 = GameObject.Find("mainCamera2");
        playerBioCS = Player.GetComponent<biologyCS>();
        cammeraPlate = GameObject.Find("cammeraPlate");
    }

    // Update is called once per frame
    void Update()
    {

    }


}
