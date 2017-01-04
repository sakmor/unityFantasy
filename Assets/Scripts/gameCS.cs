using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MiniJSON;

public class gameCS : MonoBehaviour
{
    //GameObject
    public GameObject mainCamera, mainCamera2, Player;
    GameObject cammeraPlateMouse, Cube, hitUIObject, movePlateMouse, movePlate, cammeraPlate, logText;

    //Dictionary、Array----------------------------
    List<int> cubeArrayTxt;
    GameObject[] allBiologys;
    Dictionary<Vector3, Vector2> cubesPosDictionary =
               new Dictionary<Vector3, Vector2>();

    //boolean----------------------------
    public bool touchScreen;
    bool cammeraPlatein2out, movePlatein2out, clickStart;

    //String----------------------------
    public string hitUIObjectName = "";

    //Vector2、Vector3----------------------------
    Vector2 myIputPostion;
    public Vector3 cameraRELtarget;
    Vector3 lastCameraPos, mouseStartPOS, mouseDragVector;

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
        mouseOrbitSet();
    }

    // Update is called once per frame
    void Update()
    {
        // allBioupdate(1);
        mainCamera2.transform.position = mainCamera.transform.position;
        mainCamera2.GetComponent<Camera>().fieldOfView = mainCamera.GetComponent(Camera).fieldOfView;
        mouseOrTouch();
        inputHitScene();
        // isCameraPosMove();
        // fellowPlayerCameraMove();
        // fellowPlayerCameraContorl();
        // buttonDetect();
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
        //讀取json檔案
        TextAsset json = Resources.Load("scene/s998") as TextAsset;
        scene scene = new scene();
        scene = JsonUtility.FromJson<scene>(json.text);

        //產生場景
        GameObject Cubes = GameObject.Find("Cubes");
        for (var i = 0; i < scene.cubeArray.Count; i += 5)
        {
            Vector3 tempVector3 = new Vector3(scene.cubeArray[i], scene.cubeArray[i + 1], scene.cubeArray[i + 2]);
            string name = tempVector3.ToString("F0");

            //檢查是否已經存在於unity scene中
            if (GameObject.Find("name"))
            {
                string meshname = GameObject.Find("name").GetComponent<MeshFilter>().sharedMesh.name;
                if (meshname == scene.cubeArray[i + 3].ToString("F0"))
                {
                    Debug.Log("break");
                    break;
                }
            }

            GameObject temp = Instantiate(GameObject.Find("Cube"));//todo:Cube可以不需要用Find的方式
            temp.transform.parent = Cubes.transform;
            temp.transform.position = tempVector3;
            temp.name = name;
            temp.GetComponent<MeshRenderer>().receiveShadows = true;
            temp.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            temp.GetComponent<Renderer>().enabled = true;
            Mesh mesh = (Mesh)Resources.Load("item/model/CUBE/" + scene.cubeArray[i + 3], typeof(Mesh));
            temp.GetComponent<MeshFilter>().mesh = mesh;

            switch (Mathf.FloorToInt(scene.cubeArray[i + 4]))
            {
                case 0:
                    temp.tag = "Cube";
                    break;
                case 1:
                    temp.tag = "Cube_WalkSMP";
                    break;
            }

        }
    }
    void mouseOrbitSet()
    {
        mainCamera.AddComponent<mouseOrbit>();
        mainCamera.GetComponent<mouseOrbit>().target = Player.transform;
        mainCamera.GetComponent<mouseOrbit>().targetMove = new Vector3(0, 2, 0);
    }
    void mouseOrTouch()
    {
        if (Input.GetMouseButton(0))
        {
            myIputPostion = Input.mousePosition;
            touchScreen = true;
        }
        else
        {
            touchScreen = false;
        }
    }
    void inputHitScene()
    {
        /*
        //點螢幕移動
        if (Input.GetMouseButtonUp(0))
        {
            playerBioJS._pick();

            groundPlane.Set3Points(
                Vector3(1.0, Player.transform.position.y, 0.0),
                Vector3(0.0, Player.transform.position.y, 1.0),
                Vector3(1.0, Player.transform.position.y, 1.0));
            //        mouseLineDecte();

            //滑鼠點擊取得做標點
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var rayDistance: float;
            if (hitUIObjectName == "" &&
                5.0 > Vector2.Distance(mouseStartPOS, Input.mousePosition) &&
                groundPlane.Raycast(ray, rayDistance))
            {
                var tempVector3 = ray.GetPoint(rayDistance);
                tempVector3.x = Mathf.Floor(tempVector3.x + 0.5);
                tempVector3.z = Mathf.Floor(tempVector3.z + 0.5);
                tempVector3.y = Mathf.Floor(tempVector3.y) - 0.5;

                if (checkArray(tempVector3) != false)
                {
                    var tempVector2: Vector2 = checkArray(tempVector3);
                    if (tempVector2.y == 1)
                    {
                        playerBioJS.Sphere3.transform.position = ray.GetPoint(rayDistance);
                        logg("前往座標：x:" + playerBioJS.Sphere3.transform.position.x.ToString("f2") + ",y:" + playerBioJS.Sphere3.transform.position.z.ToString("f2"));
                    }
                    else
                    {
                        logg("點擊到不可走區域了");
                    }
                }
            }



            //如果滑鼠左鍵按下，並點擊到plane，並沒有點擊到任何UI，也沒有從搖桿盤拖曳滑鼠出來
            if (Physics.Raycast(ray, mouseHitPlane) &&
                !EventSystem.current.IsPointerOverGameObject() &&
                hitUIObjectName != "cammeraPlate" &&
                hitUIObjectName != "movePlate"
            )
            {


                // Debug.Log('' + mouseHitPlane.transform.tag);
                switch (mouseHitPlane.transform.tag)
                {
                    case "Cube":
                        break;
                    case "biology":
                        logg("已選取名叫" + mouseHitPlane.collider.name + " 的生物");
                        //如果點擊到生物，停止移動
                        playerBioJS.Sphere2 = Player.transform.position;
                        playerBioJS.Sphere3.transform.position = Player.transform.position;
                        //如果點擊到生物，且該生物在攻擊範圍內
                        if (playerBioJS.attackDistance > Vector3.Distance(mouseHitPlane.transform.position, Player.transform.position))
                        {
                            var targetDir = mouseHitPlane.transform.position - Player.transform.position;
                            var newDir = Vector3.RotateTowards(this.transform.forward, targetDir, 300, 0.0);
                            Player.transform.rotation = Quaternion.LookRotation(newDir);
                            playerBioJS.bioAction = "Attack";
                        }
                        break;
                }
            }

        }*/
    }
}
public class scene
{
    public int length;
    public Vector3 playerPos;
    public List<float> cubeArray = new List<float>();
}

