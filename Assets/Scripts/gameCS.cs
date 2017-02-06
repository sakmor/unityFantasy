using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class gameCS : MonoBehaviour
{
    Dictionary<Vector3, Vector2> cubesDictionary = new Dictionary<Vector3,
        Vector2>();

    //GameObject
    public GameObject mainCamera, mainCamera2, Player, cameraRightBTN, cameraLeftBTN;
    GameObject clickPoint, cammeraStickMouse, Cube, hitUIObject, moveStickMouse, moveStick, cammeraStick, logText, fpsText;
    GameObject[] players = new GameObject[2];

    //Dictionary、Array----------------------------
    GameObject[] allBiologys;

    //boolean----------------------------
    public bool touchScreen;
    public bool cammeraStickin2out, moveStickin2out, clickStart;

    //float----------------------------
    float stickSensitive = 5;

    //String----------------------------
    public string hitUIObjectName = "";

    //Vector2、Vector3----------------------------
    public Vector2 myIputPostion;
    public Vector3 cameraRELtarget, mouseDragVector;
    Vector3 lastCameraPos, mouseStartPOS;

    //Script 自定義----------------------------
    public Pathfinding PathfindingCS;
    public List<biologyCS> playerBioCSList;
    public biologyList biologyList;

    //UnityEngine ----------------------------
    Camera camera1, camera2;
    // Use this for initialization
    void Start()
    {
        clickPoint = GameObject.Find("Sphere3");
        setBio();
        logText = GameObject.Find("logText");
        fpsText = GameObject.Find("fpsText");
        fpsText.AddComponent<FramesPerSecond>();
        logg("This Device is:" + SystemInfo.deviceType);

        cammeraStickMouse = GameObject.Find("cammeraStickMouse");
        moveStickMouse = GameObject.Find("moveStickMouse");
        moveStick = GameObject.Find("moveStick");
        Cube = GameObject.Find("Cube");

        Player = GameObject.Find("Cha_Knight");//todo:玩家不一定是用Cha_Knight

        mainCamera = GameObject.Find("mainCamera");
        mainCamera2 = GameObject.Find("mainCamera2");
        playerBioCSList.Add(Player.GetComponent<biologyCS>());
        cammeraStick = GameObject.Find("cammeraStick");

        cameraRELtarget = mainCamera.transform.position - Player.transform.position;
        camera1 = mainCamera.GetComponent<Camera>();
        camera2 = mainCamera2.GetComponent<Camera>();
        camera1.enabled = true;
        camera2.enabled = false;

        loadResources();
        loadGame();
        mouseOrbitSet();
        setUIpos();
        setPlayerBioCSList();
        // GameObject.Find("nodeInfo").AddComponent<nodeInfo>();
    }

    // Update is called once per frame
    void Update()
    {
        // allBioupdate(1);
        cameraUpdate();
        mouseOrTouch();
        inputHitScene();
        fellowPlayerCameraMove();
        fellowPlayerCameraContorl();
        lineDecte();
        buttonDetect();
        clickPointPos();
        keyboard();

    }
    void keyboard()
    {
        if (Input.GetKeyDown("e"))
        {
            rightCamera();
        }
        if (Input.GetKeyDown("q"))
        {
            leftCamera();
        }
    }

    void setPlayerBioCSList()
    {
        playerBioCSList[0].gameObject.AddComponent<DrawCircle>();
        playerBioCSList[0].gameObject.GetComponent<LineRenderer>().startWidth = 0.3f;
        playerBioCSList[0].gameObject.GetComponent<LineRenderer>().endWidth = 0.3f;
        playerBioCSList[0].gameObject.GetComponent<LineRenderer>().material = Resources.Load("item/model/Materials/LineBeam", typeof(Material)) as Material;
    }

    void clickPointPos()
    {
        if (playerBioCSList[0].getBioAnimation() == "mWalk")
        {
            clickPoint.transform.position = playerBioCSList[0].getDestination();
            clickPoint.GetComponent<Renderer>().enabled = true;
        }
        else
        {
            clickPoint.GetComponent<Renderer>().enabled = true;
        }
    }
    void setUIpos()
    {
        cameraRightBTN = GameObject.Find("cameraRightBTN");
        cameraLeftBTN = GameObject.Find("cameraLeftBTN");

        cameraLeftBTN.transform.position = getuiPosByScreen(cameraLeftBTN, 1, "left", "upper");
        cameraRightBTN.transform.position = getuiPosByScreen(cameraRightBTN, 1, "right", "upper");
        moveStick.transform.position = getuiPosByScreen(moveStick, 15, "left", "lower");
        fpsText.transform.position = getuiPosByScreen(fpsText, 1, "left", "lower");

        logText.transform.position = getuiPosByScreen(logText, 0, "right", "center");
        GameObject.Find("playerINFO").transform.position = getuiPosByScreen(GameObject.Find("playerINFO"), 1, "right", "lower");
        GameObject.Find("ActionBar").transform.position = getuiPosByScreen(GameObject.Find("ActionBar"), 15, "right", "upper");

        // GameObject.Find("playerINFO").transform.position = new Vector3(UnityEngine.Screen.width - 230, 30, 0);

    }

    Vector3 getuiPosByScreen(GameObject uiobj, float cap, string alignX, string alignY)
    {
        var screenHeight = UnityEngine.Screen.height;
        var screenWidth = UnityEngine.Screen.width;
        var objHeight = uiobj.GetComponent<RectTransform>().sizeDelta.y * uiobj.GetComponent<RectTransform>().localScale.y;
        var objWidth = uiobj.GetComponent<RectTransform>().sizeDelta.x * uiobj.GetComponent<RectTransform>().localScale.x;
        Vector3 pos = new Vector3(0, 0, 0);
        switch (alignX)
        {
            case "left":
                pos = new Vector3(cap + objWidth * 0.5f, 0, 0);
                break;
            case "right":
                pos = new Vector3(screenWidth - cap - objWidth * 0.5f, 0, 0);
                break;
            case "middie":
                pos = new Vector3(screenWidth * 0.5f, 0, 0);
                break;
        }
        switch (alignY)
        {
            case "upper":
                pos += new Vector3(0, screenHeight - cap - objHeight * 0.5f + cap, 0);
                break;
            case "center":
                pos += new Vector3(0, screenHeight * 0.5f + cap, 0);
                break;
            case "lower":
                pos += new Vector3(0, objHeight * 0.5f + cap, 0);
                break;
        }

        return pos;

    }
    void cameraUpdate()
    {
        mainCamera2.transform.position = mainCamera.transform.position;
        mainCamera2.GetComponent<Camera>().fieldOfView = mainCamera.GetComponent<Camera>().fieldOfView;
        mainCamera2.transform.rotation = mainCamera.transform.rotation;
    }

    void buttonDetect()
    {

        if (touchScreen)
        {
            lineDecte();
            //取得按壓的物件名稱
            if (EventSystem.current.IsPointerOverGameObject())
            {
                hitUIObject = EventSystem.current.currentSelectedGameObject;
                if (hitUIObject)
                {
                    hitUIObjectName = hitUIObject.name;
                }
            }
            else if (EventSystem.current.IsPointerOverGameObject(0))
            {
                hitUIObject = EventSystem.current.currentSelectedGameObject;
                if (hitUIObject)
                {
                    hitUIObjectName = hitUIObject.name;
                }
            }

            //取得首次點擊座標
            if (!clickStart)
            {
                clickStart = true;
                mouseStartPOS = myIputPostion;
            }
            //如果點選到了攝影機搖桿
            if (hitUIObjectName == "cammeraStick")
            {
                Sprite _sprite = hitUIObject.GetComponent<Image>().sprite;
                Rect _rect = hitUIObject.GetComponentInParent<RectTransform>().rect;
                Vector2 temp;
                Color UIObjectRGB;
                Vector2 imageScale = hitUIObject.GetComponent<RectTransform>().localScale;

                //取得使用者滑鼠點擊處的Alpha值(為了不規則的按鈕)
                temp.x = myIputPostion.x - hitUIObject.transform.position.x + _rect.width * 0.5f;
                temp.y = myIputPostion.y - hitUIObject.transform.position.y + _rect.height * 0.5f;

                UIObjectRGB = _sprite.texture.GetPixel(Mathf.FloorToInt(temp.x * _sprite.texture.width / (_rect.width * imageScale.x)), Mathf.FloorToInt(temp.y * _sprite.texture.height / (_rect.height * imageScale.y)));

                if (UIObjectRGB.a != 0 && Vector2.Distance(myIputPostion, hitUIObject.transform.position) < _rect.width * 0.5)
                {
                    cammeraStickin2out = true;
                    cammeraStickMouse.transform.position = myIputPostion;
                }
                else if (cammeraStickin2out)
                {
                    //如果拖拉滑鼠盤脫離搖桿盤的範圍，取得圓的交點
                    Vector2 a = new Vector2(myIputPostion.x, myIputPostion.y);
                    Vector2 b = new Vector2(hitUIObject.transform.position.x, hitUIObject.transform.position.y);
                    Vector3 c = new Vector3(hitUIObject.transform.position.x, hitUIObject.transform.position.y, _rect.width * 0.5f);
                    Vector2 x = getIntersections(a.x, a.y, b.x, b.y, c.x, c.y, c.z);
                    cammeraStickMouse.transform.position = new Vector3(x.x, x.y, 0);
                }

                //控制攝影機--香菇頭左右
                mainCamera.transform.RotateAround(Player.transform.position, Vector3.up, (hitUIObject.transform.position.x - cammeraStickMouse.transform.position.x) * Time.deltaTime);

                //控制攝影機--香菇頭上下
                var camera2PlayerVector = mainCamera.transform.position - Player.transform.position;
                var tempVector = camera2PlayerVector;
                tempVector.y = 0;
                tempVector = Quaternion.Euler(0, 90, 0) * tempVector;

                //限制攝影機上下移動的角度
                if (hitUIObject.transform.position.y - cammeraStickMouse.transform.position.y < 0)
                {
                    if (Vector3.Angle(camera2PlayerVector, Vector3.up) >= 10)
                    {
                        mainCamera.transform.RotateAround(Player.transform.position, tempVector, (hitUIObject.transform.position.y - cammeraStickMouse.transform.position.y) * Time.deltaTime);
                    }
                }
                else
                if (Vector3.Angle(camera2PlayerVector, Vector3.up) <= 160)
                {
                    mainCamera.transform.RotateAround(Player.transform.position, tempVector, (hitUIObject.transform.position.y - cammeraStickMouse.transform.position.y) * Time.deltaTime);
                }

                //更新攝影機與目標的相對位置
                cameraRELtarget = mainCamera.transform.position - Player.transform.position;

            }
            //如果點選到了移動搖桿
            if (hitUIObjectName == "moveStick")
            {

                Sprite _sprite = hitUIObject.GetComponent<Image>().sprite;
                Rect _rect = hitUIObject.GetComponent<RectTransform>().rect;
                Vector3 imageScale = hitUIObject.GetComponent<RectTransform>().localScale;

                Vector2 temp;
                //取得使用者滑鼠點擊處的Alpha值(為了不規則的按鈕)
                temp.x = myIputPostion.x - hitUIObject.transform.position.x + _rect.width * 0.5f;
                temp.y = myIputPostion.y - hitUIObject.transform.position.y + _rect.height * 0.5f;
                Color UIObjectRGB = _sprite.texture.GetPixel(Mathf.FloorToInt(temp.x * _sprite.texture.width / (_rect.width * imageScale.x)), Mathf.FloorToInt(temp.y * _sprite.texture.height / (_rect.height * imageScale.y)));

                if (UIObjectRGB.a != 0 && Vector2.Distance(myIputPostion, hitUIObject.transform.position) < _rect.width * 0.5)
                {
                    moveStickin2out = true;
                    moveStickMouse.transform.position = myIputPostion;
                }
                else if (moveStickin2out)
                {
                    //如果拖拉滑鼠盤脫離搖桿盤的範圍，取得圓的交點
                    Vector2 a = new Vector2(myIputPostion.x, myIputPostion.y);
                    Vector2 b = new Vector2(hitUIObject.transform.position.x, hitUIObject.transform.position.y);
                    Vector3 c = new Vector3(hitUIObject.transform.position.x, hitUIObject.transform.position.y, _rect.width * 0.5f);
                    Vector2 x = getIntersections(a.x, a.y, b.x, b.y, c.x, c.y, c.z);
                    moveStickMouse.transform.position = new Vector3(x.x, x.y, 0);
                }

                //控制生物移動
                if (Vector2.Distance(myIputPostion, hitUIObject.transform.position) > 0)
                {
                    mouseDragVector.x = moveStickMouse.transform.localPosition.x / (_rect.height * 0.5f);
                    mouseDragVector.z = moveStickMouse.transform.localPosition.y / (_rect.width * 0.5f);
                }
            }

        }
        else
        {
            cammeraStickMouse.transform.position = cammeraStick.transform.position;
            moveStickMouse.transform.position = moveStick.transform.position;
            hitUIObject = null;
            //放開滑鼠時...如果前一個按鍵removeStick，則移除
            //放開滑鼠時...如果前一個按鍵cubeStick，則新增
            if (hitUIObjectName != "")
            {

                if (hitUIObjectName == "moveStick")
                {
                    playerBioCSList[0].bioStop();
                }
                hitUIObjectName = "";
            }
            clickStart = false;
            cammeraStickin2out = false;
            moveStickin2out = false;
        }

    }
    public void rightCamera()
    {
        mainCamera.GetComponent<mouseOrbit>()._right();
    }

    public void leftCamera()
    {
        mainCamera.GetComponent<mouseOrbit>()._left();
    }

    //todo:要在角色或攝影機有移動時才徵測
    void lineDecte()
    {
        float yScaleUP = mainCamera.GetComponent<mouseOrbit>().targetMove.y;
        Vector3 target = Player.transform.position;
        target.y += yScaleUP;

        Vector3 tempPick;
        tempPick = target;
        Vector3 myVector = target - mainCamera.transform.position;
        float mylength = Mathf.Round(Vector3.Distance(target, mainCamera.transform.position));
        Debug.DrawLine(target, mainCamera.transform.position);

        for (var i = 0; i < mylength; i++)
        {
            tempPick = target - myVector.normalized * i;

            if (cubesDictionary.ContainsKey(normalized(tempPick)))
            {
                mainCamera2.transform.position = tempPick;
                camera2.enabled = true;
                camera1.enabled = false;
                break;
            }
            else
            {
                camera1.enabled = true;
                camera2.enabled = false;
            }

        }
    }
    //todo:好像沒用到了
    // void mouseLineDecte()
    // {
    //     if (Input.GetMouseButtonUp(0))
    //     {
    //         Vector3 tempPick, tempPick2;
    //         //滑鼠點擊取得做標點
    //         Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //         if (hitUIObjectName == "" &&
    //             5.0f > Vector2.Distance(mouseStartPOS, Input.mousePosition))
    //         {
    //             for (var i = 0; i < Mathf.Floor(mainCamera.transform.position.y); i++)
    //             {
    //                 tempPick = ray.GetPoint(i);
    //                 tempPick2.x = Mathf.Floor(tempPick.x + 0.5f);
    //                 tempPick2.z = Mathf.Floor(tempPick.z + 0.5f);
    //                 tempPick2.y = Mathf.Floor(tempPick.y) + 0.5f;

    //                 if (GameObject.Find(tempPick2.ToString("F0")))
    //                 {
    //                     groundPlane.Set3Points(
    //                         new Vector3(1.0f, tempPick.y + 0.5f, 0.0f),
    //                         new Vector3(0.0f, tempPick.y + 0.5f, 1.0f),
    //                         new Vector3(1.0f, tempPick.y + 0.5f, 1.0f));
    //                     break;
    //                 }
    //             }
    //         }

    //     }
    // }

    void fellowPlayerCameraContorl()
    {
        //滑鼠滾輪縮放攝影機
        if (Input.GetAxis("Mouse ScrollWheel") < 0) // forward
        {
            if (Camera.main.fieldOfView > 1
                && Camera.main.orthographicSize > 1)
            {
                Camera.main.fieldOfView = Mathf.Min(Camera.main.fieldOfView - 1, 60);
                Camera.main.orthographicSize = Mathf.Min(Camera.main.orthographicSize - 1, 60);
            }

        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
        {
            if (Camera.main.fieldOfView < 50
                && Camera.main.orthographicSize < 6)
            {
                Camera.main.fieldOfView = Mathf.Min(Camera.main.fieldOfView + 1, 60);
                Camera.main.orthographicSize = Mathf.Min(Camera.main.orthographicSize + 1, 60);
            }
        }
    }
    void fellowPlayerCameraMove()
    {
        mainCamera.transform.position = cameraRELtarget + Player.transform.position;
        mainCamera.transform.LookAt(new Vector3(Player.transform.position.x, Player.transform.position.y + 2.0f, Player.transform.position.z));

    }

    public void logg(string n)
    {
        string tempString = logText.GetComponent<Text>().text;
        int loggLine = tempString.Split('\n').Length - 1;
        int loggLineMax = 5;
        if (loggLine >= loggLineMax)
        {
            logText.GetComponent<Text>().text += '\n';
            logText.GetComponent<Text>().text += n;
            int firstLine = 0;
            tempString = logText.GetComponent<Text>().text;
            logText.GetComponent<Text>().text = "";
            firstLine = tempString.IndexOf('\n');
            for (int i = 0; i < tempString.Length; i++)
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
        }
    }
    void loadResources()
    {
        //將特定資料夾內的fbx檔名存入陣列之中
        Mesh tempMesh = new Mesh();

        //todo：之後要讀設定檔
        // cubeArrayTxt.Add("10001");
        // cubeArrayTxt.Add("10002");
        // cubeArrayTxt.Add("10003");
        // cubeArrayTxt.Add("10004");
        // cubeArrayTxt.Add("10005");
        // cubeArrayTxt.Add("10017");
        // cubeArrayTxt.Add("10020");
        // cubeArrayTxt.Add("10045");
        // cubeArrayTxt.Add("10098");

    }
    void setBio()
    {        //將所有生物套上biologyCS;
        allBiologys = GameObject.FindGameObjectsWithTag("biology");
        foreach (GameObject thisBiology in allBiologys)
        {
            biologyCS temp = new biologyCS(this);
            thisBiology.AddComponent<biologyCS>();
        }
        TextAsset json = Resources.Load("db/biologyList", typeof(TextAsset)) as TextAsset;
        biologyList = JsonUtility.FromJson<biologyList>(json.text);
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
            // if (GameObject.Find(name))
            // {
            //     string meshname = GameObject.Find(name).GetComponent<MeshFilter>().sharedMesh.name;
            //     if (meshname == scene.cubeArray[i + 3].ToString("F0"))
            //     {
            //         Debug.Log("break");
            //         break;
            //     }
            // }

            //新增CUBE
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
                    //建立目錄cubesDictionary
                    cubesDictionary[new Vector3(tempVector3.x, tempVector3.y, tempVector3.z)] = new Vector2(float.Parse(mesh.name), 0);
                    break;
                case 1:
                    temp.tag = "Cube_WalkSMP";
                    cubesDictionary[new Vector3(tempVector3.x, tempVector3.y, tempVector3.z)] = new Vector2(float.Parse(mesh.name), 1);
                    break;
            }

        }
        GameObject.Find("aStart").AddComponent<Pathfinding>();
        PathfindingCS = GameObject.Find("aStart").GetComponent<Pathfinding>();
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
        Plane groundPlane = new Plane();

        //點螢幕移動
        if (Input.GetMouseButtonUp(0))
        {
            groundPlane.Set3Points(
                new Vector3(1.0f, Player.transform.position.y, 0.0f),
                new Vector3(0.0f, Player.transform.position.y, 1.0f),
                new Vector3(1.0f, Player.transform.position.y, 1.0f));
            //        mouseLineDecte();

            //滑鼠點擊取得做標點
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float rayDistance;      //todo:這裡沒有指定距離真的沒問題？
            if (hitUIObjectName == "" &&
                5.0 > Vector2.Distance(mouseStartPOS, Input.mousePosition) &&
                groundPlane.Raycast(ray, out rayDistance))
            {
                var tempVector3 = ray.GetPoint(rayDistance);
                tempVector3 = normalized(tempVector3);
                //todo:留者參考用，沒問題可刪除
                //tempVector3.x = Mathf.Floor(tempVector3.x + 0.5f);
                //tempVector3.z = Mathf.Floor(tempVector3.z + 0.5f);
                //tempVector3.y = Mathf.Floor(tempVector3.y) - 0.5f;

                if (cubesDictionary.ContainsKey(normalized(tempVector3)))
                {
                    float tag = cubesDictionary[tempVector3].y;
                    if (tag == 1)
                    {
                        var temp3 = ray.GetPoint(rayDistance);
                        playerBioCSList[0].bioGoto(ray.GetPoint(rayDistance));
                        logg("前往座標：x:" + temp3.x.ToString("f2") + ",y:" + temp3.z.ToString("f2"));
                    }
                    else
                    {
                        logg("<color=red>點擊到不可走區域了</color>");
                    }
                }
            }

            RaycastHit mouseHitPlane;
            //如果滑鼠左鍵按下，並點擊到plane，並沒有點擊到任何UI，也沒有從搖桿盤拖曳滑鼠出來
            if (Physics.Raycast(ray, out mouseHitPlane) &&
                !EventSystem.current.IsPointerOverGameObject() &&
                hitUIObjectName != "cammeraStick" &&
                hitUIObjectName != "moveStick"
            )
            {

                // Debug.Log('' + mouseHitPlane.transform.tag);
                switch (mouseHitPlane.transform.tag)
                {
                    case "Cube":
                        break;
                    case "biology":
                        logg("已選取名叫" + mouseHitPlane.collider.name + " 的生物");
                        break;
                }
            }

        }
    }
    public Vector3 normalized(Vector3 pos)
    {
        Vector3 temp;
        //正規化生物座標
        temp.x = Mathf.Floor(pos.x);
        temp.z = Mathf.Floor(pos.z);
        temp.y = Mathf.Floor(pos.y);

        return temp;
    }
    Vector2 getIntersections(float ax, float ay, float bx, float by, float cx, float cy, float cz)
    {
        float[] a = { ax, ay }, b = { bx, by }, c = { cx, cy, cz };
        // Calculate the euclidean distance between a & b
        float eDistAtoB = Mathf.Sqrt(Mathf.Pow(b[0] - a[0], 2) + Mathf.Pow(b[1] - a[1], 2));

        // compute the direction vector d from a to b
        float[] d = { (b[0] - a[0]) / eDistAtoB, (b[1] - a[1]) / eDistAtoB };

        // Now the line equation is x = dx*t + ax, y = dy*t + ay with 0 <= t <= 1.

        // compute the value t of the closest point to the circle center (cx, cy)
        var t = (d[0] * (c[0] - a[0])) + (d[1] * (c[1] - a[1]));

        // compute the coordinates of the point e on line and closest to c
        var ecoords0 = (t * d[0]) + a[0];
        var ecoords1 = (t * d[1]) + a[1];

        // Calculate the euclidean distance between c & e
        var eDistCtoE = Mathf.Sqrt(Mathf.Pow(ecoords0 - c[0], 2) + Mathf.Pow(ecoords1 - c[1], 2));

        // test if the line intersects the circle
        if (eDistCtoE < c[2])
        {
            // compute distance from t to circle intersection point
            var dt = Mathf.Sqrt(Mathf.Pow(c[2], 2) - Mathf.Pow(eDistCtoE, 2));

            // compute first intersection point
            var fcoords0 = ((t - dt) * d[0]) + a[0];
            var fcoords1 = ((t - dt) * d[1]) + a[1];
            // check if f lies on the line
            //        f.onLine = is_on(a, b, f.coords);

            // compute second intersection point
            var gcoords0 = ((t + dt) * d[0]) + a[0];
            var gcoords1 = ((t + dt) * d[1]) + a[1];
            Vector2 finalAnswer = new Vector2(fcoords0, fcoords1);

            // check if g lies on the line
            return (finalAnswer);

        }

        return (new Vector2());

    }
    public bool checkCubesDictionary(Vector3 n)
    {
        return cubesDictionary.ContainsKey(n);
    }
    public Dictionary<Vector3, Vector2> getCubesDictionary()
    {
        return cubesDictionary;
    }
    public bool checkPlayerBioCSListByName(string n)
    {
        foreach (var i in playerBioCSList)
        {
            if (i.name == n)
            {
                return true;
            }

        }
        return false;
    }
    public GameObject[] getAllBiologys()
    {
        return allBiologys;

    }

}
public class scene
{
    public int length;
    public Vector3 playerPos;
    public List<float> cubeArray = new List<float>();
}

public class biologyList
{
    public string[] drawNumber;
    public float[] biodata;
}