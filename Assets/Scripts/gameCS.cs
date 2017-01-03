using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MiniJSON;

public class gameCS : MonoBehaviour
{
    //GameObject
    GameObject cammeraPlateMouse, Player, Cube, mainCamera, mainCamera2, hitUIObject, movePlateMouse, movePlate, cammeraPlate, logText;

    //Dictionary、Array----------------------------
    List<int> cubeArrayTxt;
    GameObject[]  allBiologys;
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
        logg("This Device is:" + SystemInfo.deviceType);

        cammeraPlateMouse = GameObject.Find("cammeraPlateMouse");
        movePlateMouse = GameObject.Find("movePlateMouse");
        movePlate = GameObject.Find("movePlate");
        Cube = GameObject.Find("Cube");
        Player = GameObject.Find("Cha_Knight");
        mainCamera = GameObject.Find("mainCamera");
        mainCamera2 = GameObject.Find("mainCamera2");
        playerBioCS = Player.GetComponent<biologyCS>();
        cammeraPlate = GameObject.Find("cammeraPlate");

        cameraRELtarget = mainCamera.transform.position - Player.transform.position;
        camera1 = mainCamera.GetComponent<Camera>();
        camera2 = mainCamera2.GetComponent<Camera>();
        camera1.enabled = true;
        camera2.enabled = false;

        GameObject.Find("aStart").AddComponent<Pathfinding>();
        PathfindingCS = GameObject.Find("aStart").GetComponent<Pathfinding>();
        loadResources();
        loadGame();
        // mouseOrbitSet();
    }

    // Update is called once per frame
    void Update()
    {

    }
    int loggLine = 0;
    int loggLineMax = 10;

    void logg(string n)
    {
        if (loggLine == loggLineMax)
        {
            logText.GetComponent<Text>().text += '\n';
            logText.GetComponent<Text>().text += n;
            int firstLine = 0;
            string tempString = logText.GetComponent<Text>().text;
            logText.GetComponent<Text>().text = "";
            for (var i = 0; i < tempString.Length; i++)
            {
                if (tempString[i] == '\n')
                {
                    firstLine = i + 1;
                    break;
                }
            }
            for (; firstLine < tempString.Length; firstLine++)
            {
                logText.GetComponent<Text>().text += tempString[firstLine];
            }

        }
        else
        {
            logText.GetComponent<Text>().text += '\n';
            logText.GetComponent<Text>().text += n;
            loggLine++;
        }

    }
    void loadResources()
    {
        //將特定資料夾內的fbx檔名存入陣列之中
        Mesh tempMesh = new Mesh();

        //todo：之後要讀設定檔
        cubeArrayTxt.Add(10001);
        cubeArrayTxt.Add(10002);
        cubeArrayTxt.Add(10003);
        cubeArrayTxt.Add(10004);
        cubeArrayTxt.Add(10005);
        cubeArrayTxt.Add(10017);
        cubeArrayTxt.Add(10020);
        cubeArrayTxt.Add(10045);
        cubeArrayTxt.Add(10098);

    }

    void loadGame()
    {
        //     TextAsset arrayText = Resources.Load("scene/s999") as TextAsset;
        //     Dictionary<string, System.Object> array3dLoadJson = Json.Deserialize(arrayText.text) as Dictionary<string, System.Object>;
        //     GameObject Cube = GameObject.Find("Cube");
        //     int tempi = array3dLoadJson["length"];
        //     for (var i = 1; i < tempi; i++)
        //     {
        //         var tempVector3: Vector3;
        //         var tempVector2: Vector2;
        //         tempVector3.x = ((array3dLoadJson[i.ToString()]) as List. < System.Object >)[0];
        //         tempVector3.y = ((array3dLoadJson[i.ToString()]) as List. < System.Object >)[1];
        //         tempVector3.z = ((array3dLoadJson[i.ToString()]) as List. < System.Object >)[2];
        //         tempVector2.x = ((array3dLoadJson[i.ToString()]) as List. < System.Object >)[3];
        //         tempVector2.y = ((array3dLoadJson[i.ToString()]) as List. < System.Object >)[4];
        //         //建立目錄cubesPosDictionary
        //         cubesPosDictionary[Vector3(tempVector3.x, tempVector3.y, tempVector3.z)] = tempVector2;
        //         GameObject.Find("aStart").GetComponent. < Grid > ().cubesPosDictionary[Vector3(tempVector3.x, tempVector3.y, tempVector3.z)] = tempVector2;

        //         //重建CUBE
        //         if (GameObject.Find("(" + tempVector3.x.ToString("F0") + ", " + tempVector3.y.ToString("F0") + ", " + tempVector3.z.ToString("F0") + ")") == null)
        //         {

        //             var temp = Instantiate(Cube);
        //             switch (tempVector2.y)
        //             {
        //                 case 0:
        //                     temp.tag = "Cube";
        //                     break;
        //                 case 1:
        //                     temp.tag = "Cube_WalkSMP";
        //                     break;
        //             }
        //             temp.GetComponent. < MeshRenderer > ().receiveShadows = true;
        //             temp.GetComponent. < Renderer > ().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        //             temp.GetComponent. < Renderer > ().enabled = true;
        //             //        temp.GetComponent. < MeshFilter > ().mesh = Resources.Load('item/model/CUBE/' + Path.GetFileNameWithoutExtension(cubeArrayTxt[array3dLoadJson[i][3]]), Mesh);
        //             temp.GetComponent. < MeshFilter > ().mesh = Resources.Load('item/model/CUBE/' + tempVector2.x, Mesh);
        //             temp.GetComponent. < Renderer > ().enabled = true;
        //             //temp.AddComponent(BoxCollider);
        //             //temp.name = "(" + array3dLoadJson[i][0] + ", " + array3dLoadJson[i][1]  + ", " + array3dLoadJson[i][2] + ")";

        //             temp.transform.position.x = tempVector3.x;
        //             temp.transform.position.y = tempVector3.y;
        //             temp.transform.position.z = tempVector3.z;
        //             temp.name = temp.transform.position.ToString("F0");
        //         }
        //     }
    }
}
