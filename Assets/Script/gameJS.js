// # pragma strict
@
script RequireComponent(Animation)

import UnityEngine.EventSystems;
import System.Collections.Generic;
import System.IO;
import MiniJSON;

/****************
 < GameObject > ===========================
 *
 * Cube                 :場地的母體
 * CubePick             :從托盤拉出後的預覽物
 * Player               :由玩家控制的生物 (biology.js)
 * itemBagGameObject    :背包介面
 * mainCamera           :主要攝影機
 * rayCamera            :當Player生物被物件遮蔽時使用 (功能尚未撰寫)TODO
 * pickTouch            :顯示玩家在拖曳CUBE時，點選的CUBE
 * pickTouchSide        :顯示玩家在拖曳CUBE時，點選的CUBE那一面
 * cubePlateTimer       :以縮放圓形圖案的方式，顯示玩家在點選cubePlate按鈕時間
 *
 *
 *
 *
 *
 *
 *
 < Dictionary > ===========================
 *
 * cubesPosDictionary   :記錄場景CUBE的位置，checkArray、setArray、removeArray查找用
 * array3d              :紀錄場景CUBE的類型 (但需要再整理該功能)TODO
 * 顯示
 ** */
var Cube: GameObject;
var CubePick: GameObject;
var Player: GameObject;
var itemBagGameObject: GameObject;
var PlayerLight: GameObject;
var mainCamera: GameObject;
var rayCamera: GameObject;
var pickTouch: GameObject;
var pickTouchSide: GameObject;
var cubePlateMouse: GameObject;
var cubePlateTimer: GameObject;


var cubesPosDictionary: Dictionary. < Vector3, int > =
    new Dictionary. < Vector3,
    int > ();
var array3d = new Array();


var biologyJS: biology;
var itemBagJS: itemBag;
var mouseOrbitJS: mouseOrbit;
var PlayerPrefsX: PlayerPrefsX;

var myIputPostion: Vector2;
var touchScreen: boolean = false;

//紀錄滑鼠首次點擊座標
var mouseStartPOS: Vector3;
var clickStart: boolean = false;
var mouseDragVector: Vector3;
var mouseDragDist: float;

var cubePlateTimerCurrent: float;

//紀錄滑鼠首次按壓的UI
var hitUIObject: GameObject;
var hitUIObjectName: String = "";

//目前點擊的UI名稱
var nowButton: String;
var movePlateMouse: GameObject;

var movePlate: GameObject;
var cubePlate: GameObject;
var cammeraPlatein2out: boolean = false;
var cubePlatein2out: boolean = false;
var movePlatein2out = false;
var cammeraPlate: GameObject;
var cammeraPlateMouse: GameObject;

//Cylinder
var cylinder: GameObject;

//設定物件
var cubeArrayTxt = new Array();


//log用
var logText: GameObject;

//攝影機相對目標
var cameraRelativeTarget: Vector3;
var ray: Ray;
var mouseHitPlane: RaycastHit;


var groundPlane: Plane;
var markerObject: Transform;

var biologyList: TextAsset;

function Start() {
    biologyList = Resources.Load("db/biologyList");
    logText = GameObject.Find("logText");
    logg('This Device is:' + SystemInfo.deviceType);
    cubePlateTimer = GameObject.Find("cubePlateTimer");
    itemBagGameObject = GameObject.Find("itemBag");
    cylinder = GameObject.Find("cylinder");
    cubePlateMouse = GameObject.Find("cubePlateMouse");
    cubePlate = GameObject.Find("cubePlate");
    cammeraPlateMouse = GameObject.Find("cammeraPlateMouse");
    movePlateMouse = GameObject.Find("movePlateMouse");
    movePlate = GameObject.Find("movePlate");


    //宣告各個變數代表的gameObject
    PlayerLight = GameObject.Find("PlayerLight");
    pickTouch = GameObject.Find("pickTouch");
    Cube = GameObject.Find("Cube");
    CubePick = GameObject.Find("CubePick");

    Player = GameObject.Find("Cha_Knight");

    mainCamera = GameObject.Find("mainCamera");
    rayCamera = GameObject.Find("rayCamera");
    GameObject.Find("m101").AddComponent(biology);
    Player.AddComponent(biology);
    pickTouchSide = GameObject.Find("pickTouchSide");
    biologyJS = Player.GetComponent(biology);


    itemBagGameObject = GameObject.Find("itemBag");
    itemBagGameObject.AddComponent(itemBag);
    itemBagJS = itemBagGameObject.GetComponent(itemBag);

    var myButton = GameObject.Find("Button_LEFT");
    myButton.GetComponent(UI.Button).onClick.AddListener(Button_LEFT);

    var myButtonForward = GameObject.Find("Button_RIGHT");
    myButtonForward.GetComponent(UI.Button).onClick.AddListener(Button_RIGHT);

    var myButtonJump = GameObject.Find("Button_jump");
    myButtonJump.GetComponent(UI.Button).onClick.AddListener(Button_jump);

    var myButtonSave = GameObject.Find("Button_Save");
    myButtonSave.GetComponent(UI.Button).onClick.AddListener(Button_Save);

    var myButtonLoad = GameObject.Find("Button_Load");
    myButtonLoad.GetComponent(UI.Button).onClick.AddListener(Button_Load);

    var myButton_Next = GameObject.Find("Button_Next");
    myButton_Next.GetComponent(UI.Button).onClick.AddListener(Button_Next);

    cammeraPlate = GameObject.Find("cammeraPlate");
    cameraRelativeTarget = mainCamera.transform.position - Player.transform.position;
    loadResources();

    loadGame();
    //設定攝影機
    mouseOrbitSet();

    groundPlane.Set3Points(
        Vector3(1.0, 0.0, 0.0),
        Vector3(0.0, 0.0, 1.0),
        Vector3(0.0, 0.0, 0.0));
    var markerObject: Transform;


}

function Update() {

    rayCamera.transform.position = mainCamera.transform.position;
    rayCamera.transform.rotation = mainCamera.transform.rotation;
    rayCamera.GetComponent(Camera).fieldOfView = mainCamera.GetComponent(Camera).fieldOfView;
    mouseOrTouch();
    getMousehitGroupPos();
    //    fellowPlayerLight();
    fellowPlayerCameraMove();
    fellowPlayerCameraContorl();
    buttonDetect();



}

function clearCube() {
    var or = new StreamReader("array3dictionary.txt");
    var arrayText: String = or.ReadToEnd();
    var array3dLoadJson = Json.Deserialize(arrayText) as Dictionary. < String,
        System.Object > ;
    var tempi: int = array3dLoadJson["length"];
    for (var i = 1; i < tempi; i++) {
        var temp: Vector3;
        var tempColor: Color;
        tempColor.r = ((array3dLoadJson[i.ToString()]) as List. < System.Object > )[0];
        tempColor.g = ((array3dLoadJson[i.ToString()]) as List. < System.Object > )[1];
        tempColor.b = ((array3dLoadJson[i.ToString()]) as List. < System.Object > )[2];
        tempColor.a = ((array3dLoadJson[i.ToString()]) as List. < System.Object > )[3];
        temp.x = tempColor.r;
        temp.y = tempColor.g;
        temp.z = tempColor.b;
        DestroyImmediate(GameObject.Find(temp.ToString("F0")));
    }
    or.Close();
}

function Button_Save() {
    saveGame();
}

function Button_Next() {
    biologyJS.nextCube();
}

function Button_Load() {
    //    loadGame();
}

function Button_jump() {
    biologyJS.bioAction = "Jump";
}

function Button_LEFT() {
    biologyJS.bioAction = "Create";
}

function Button_RIGHT() {
    biologyJS.bioAction = "Action";
}

function setArray(a: Vector3, b: float) {
    cubesPosDictionary[a] = array3d.length;
    array3d.Push(Color(a.x, a.y, a.z, b));
    saveGame();
}

function removeArray(a: Vector3) {
    array3d[cubesPosDictionary[a]] = null;
    cubesPosDictionary[a] = 0;
    saveGame();
}

function checkArray(a: Vector3) {
    if (cubesPosDictionary.ContainsKey(a)) {
        if (cubesPosDictionary[a] != 0) {
            return true;
        }
    }
}

function mouseOrTouch() {
    //    if(SystemInfo.deviceType)
    //    if (Input.touches.length > 1) {
    //        for (var touch: Touch in Input.touches) {
    //            touchScreen = true;
    //            myIputPostion.x = touch.position.x;
    //            myIputPostion.y = touch.position.y;
    //        }
    //    } else
    if (Input.GetMouseButton(0)) {
        myIputPostion = Input.mousePosition;
        touchScreen = true;
    } else {
        touchScreen = false;
    }
}


function loadResources() {
    //將特定資料夾內的fbx檔名存入陣列之中
    var tempObject: GameObject;
    var tempMesh: Mesh = new Mesh();
    var temptext: String;

    //todo：之後要讀設定檔
    cubeArrayTxt.push(10001);
    cubeArrayTxt.push(10002);
    cubeArrayTxt.push(10003);
    cubeArrayTxt.push(10004);
    cubeArrayTxt.push(10005);
    cubeArrayTxt.push(10017);
    cubeArrayTxt.push(10020);
    cubeArrayTxt.push(10045);
    cubeArrayTxt.push(10098);

    //    var filePaths: String[] = Directory.GetFiles("Assets/Resources/item/model/CUBE", "*.fbx");
    //    for (var i = 0; i < filePaths.length; i++) {
    //        cubeArrayTxt.push(filePaths[i]);
    //    }
    //    tempMesh = Resources.Load('item/model/' + Path.GetFileNameWithoutExtension(cubeArrayTxt[0]), Mesh);
    //    Cube.GetComponent. < MeshFilter > ().mesh = tempMesh;
}

function loadGame() {
    //    clearCube();
    //    var array3dLoad: Color[] = PlayerPrefsX.GetColorArray("array3d");
    //    Player.transform.position = array3dLoadJson["PlayerPOS"];
    var or = new StreamReader("array3dictionary.txt");
    var arrayText: String = or.ReadToEnd();
    var array3dLoadJson = Json.Deserialize(arrayText) as Dictionary. < String,
        System.Object > ;
    var Cube: GameObject = GameObject.Find("Cube");
    var tempi: int = array3dLoadJson["length"];
    for (var i = 1; i < tempi; i++) {
        var tempColor: Color;
        tempColor.r = ((array3dLoadJson[i.ToString()]) as List. < System.Object > )[0];
        tempColor.g = ((array3dLoadJson[i.ToString()]) as List. < System.Object > )[1];
        tempColor.b = ((array3dLoadJson[i.ToString()]) as List. < System.Object > )[2];
        tempColor.a = ((array3dLoadJson[i.ToString()]) as List. < System.Object > )[3];

        //建立目錄cubesPosDictionary
        cubesPosDictionary[Vector3(tempColor.r, tempColor.g, tempColor.b)] = tempColor.a;

        //重建CUBE
        if (GameObject.Find("(" + tempColor.r.ToString("F0") + ", " + tempColor.g.ToString("F0") + ", " + tempColor.b.ToString("F0") + ")") == null) {
            var temp = Instantiate(Cube);
            temp.tag = "Cube";
            temp.GetComponent. < MeshRenderer > ().receiveShadows = true;
            temp.GetComponent. < Renderer > ().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            temp.GetComponent. < Renderer > ().enabled = true;
            //        temp.GetComponent. < MeshFilter > ().mesh = Resources.Load('item/model/CUBE/' + Path.GetFileNameWithoutExtension(cubeArrayTxt[array3dLoadJson[i][3]]), Mesh);
            temp.GetComponent. < MeshFilter > ().mesh = Resources.Load('item/model/CUBE/' + tempColor.a, Mesh);
            temp.GetComponent. < Renderer > ().enabled = true;
            //temp.AddComponent(BoxCollider);
            //temp.name = "(" + array3dLoadJson[i][0] + ", " + array3dLoadJson[i][1]  + ", " + array3dLoadJson[i][2] + ")";

            temp.transform.position.x = tempColor.r;
            temp.transform.position.y = tempColor.g;
            temp.transform.position.z = tempColor.b;
            temp.name = temp.transform.position.ToString("F0");
            setArray(temp.transform.position, tempColor.a);
        }
    }
    or.Close();
    //    GameObject.Find("Cubes").GetComponent("DrawCallMinimizer").enabled = true;
}

function saveGame() {
    //存檔前先將空格移除
    if (array3d.length > 0) {
        for (var i = 0; i < array3d.length; i++) {
            if (array3d[i] == null) {
                array3d.splice(i, 1);
                i--;
            }
        }
        var array3dColor = new Color[array3d.length];
        for (i = 0; i < array3d.length; i++) {
            array3dColor[i] = array3d[i];
        }
        PlayerPrefsX.SetVector3("playerPos", Player.transform.position);
        PlayerPrefsX.SetColorArray("array3d", array3dColor);
        PlayerPrefs.Save();

        //        print("save game");
    } else {
        //        print("Not game data saved");
    }
}


function getIntersections(ax: float, ay: float, bx: float, by: float, cx: float, cy: float, cz: float) {
    var a = [ax, ay];
    var b = [bx, by];
    var c = [cx, cy, cz];
    // Calculate the euclidean distance between a & b
    var eDistAtoB = Mathf.Sqrt(Mathf.Pow(b[0] - a[0], 2) + Mathf.Pow(b[1] - a[1], 2));


    // compute the direction vector d from a to b
    var d = [(b[0] - a[0]) / eDistAtoB, (b[1] - a[1]) / eDistAtoB];


    // Now the line equation is x = dx*t + ax, y = dy*t + ay with 0 <= t <= 1.

    // compute the value t of the closest point to the circle center (cx, cy)
    var t = (d[0] * (c[0] - a[0])) + (d[1] * (c[1] - a[1]));


    // compute the coordinates of the point e on line and closest to c
    var ecoords0 = (t * d[0]) + a[0];
    var ecoords1 = (t * d[1]) + a[1];

    // Calculate the euclidean distance between c & e
    var eDistCtoE = Mathf.Sqrt(Mathf.Pow(ecoords0 - c[0], 2) + Mathf.Pow(ecoords1 - c[1], 2));

    // test if the line intersects the circle
    if (eDistCtoE < c[2]) {
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
        var finalAnswer: Vector2 = Vector2(fcoords0, fcoords1);

        // check if g lies on the line
        return (finalAnswer);


    } else if (parseInt(eDistCtoE) == parseInt(c[2])) {
        // console.log("Only one intersection");
        return {
            //            points: false,
            //            pointOnLine: e
        };
    } else {
        // console.log("No intersection");
        return {
            //            points: false,
            //            pointOnLine: e
        };
    }
    /**/
}

function logg(n: String) {
    logText.GetComponent. < UI.Text > ().text += '\n';
    logText.GetComponent. < UI.Text > ().text += n;

}

function buttonDetect() {
    //當滑鼠按壓，並點選到UI時

    if (touchScreen) {
        //取得按壓的物件名稱
        if (EventSystem.current.IsPointerOverGameObject()) {
            hitUIObject = EventSystem.current.currentSelectedGameObject;
            if (hitUIObject) {
                hitUIObjectName = hitUIObject.name;
            }
        } else if (EventSystem.current.IsPointerOverGameObject(0)) {
            hitUIObject = EventSystem.current.currentSelectedGameObject;
            if (hitUIObject) {
                hitUIObjectName = hitUIObject.name;
            }
        }

        //取得首次點擊座標
        if (!clickStart) {
            clickStart = true;
            mouseStartPOS = myIputPostion;
        }
        //如果點選到了攝影機搖桿
        if (hitUIObjectName == 'cammeraPlate') {
            var _sprite = hitUIObject.GetComponent. < UI.Image > ().sprite;
            var _rect = hitUIObject.GetComponent. < RectTransform > ().rect;
            var temp: Vector2;
            var UIObjectRGB: Color;
            var imageScale: Vector2 = hitUIObject.GetComponent. < RectTransform > ().localScale;


            //取得使用者滑鼠點擊處的Alpha值(為了不規則的按鈕)
            temp.x = myIputPostion.x - hitUIObject.transform.position.x + _rect.width * 0.5;
            temp.y = myIputPostion.y - hitUIObject.transform.position.y + _rect.height * 0.5;

            UIObjectRGB = _sprite.texture.GetPixel(Mathf.FloorToInt(temp.x * _sprite.texture.width / (_rect.width * imageScale.x)), Mathf.FloorToInt(temp.y * _sprite.texture.height / (_rect.height * imageScale.y)));

            if (UIObjectRGB.a != 0 && Vector2.Distance(myIputPostion, hitUIObject.transform.position) < _rect.width * 0.5) {
                cammeraPlatein2out = true;
                cammeraPlateMouse.transform.position = myIputPostion;
            } else if (cammeraPlatein2out) {
                //如果拖拉滑鼠盤脫離搖桿盤的範圍，取得圓的交點
                var a: Vector2 = Vector2(myIputPostion.x, myIputPostion.y);
                var b: Vector2 = Vector2(hitUIObject.transform.position.x, hitUIObject.transform.position.y);
                var c: Vector3 = Vector3(hitUIObject.transform.position.x, hitUIObject.transform.position.y, _rect.width * 0.5);
                var x: Vector2 = getIntersections(a.x, a.y, b.x, b.y, c.x, c.y, c.z);
                cammeraPlateMouse.transform.position.x = x.x;
                cammeraPlateMouse.transform.position.y = x.y;
            }

            //控制攝影機--香菇頭左右
            mainCamera.transform.RotateAround(Player.transform.position, Vector3.up, (hitUIObject.transform.position.x - cammeraPlateMouse.transform.position.x) * Time.deltaTime);

            //控制攝影機--香菇頭上下
            var camera2PlayerVector = mainCamera.transform.position - Player.transform.position;
            var tempVector = camera2PlayerVector;
            tempVector.y = 0;
            tempVector = Quaternion.Euler(0, 90, 0) * tempVector;

            //限制攝影機上下移動的角度
            if (hitUIObject.transform.position.y - cammeraPlateMouse.transform.position.y < 0) {
                if (Vector3.Angle(camera2PlayerVector, Vector3.up) >= 10) {
                    mainCamera.transform.RotateAround(Player.transform.position, tempVector, (hitUIObject.transform.position.y - cammeraPlateMouse.transform.position.y) * Time.deltaTime);
                }
            } else
            if (Vector3.Angle(camera2PlayerVector, Vector3.up) <= 160) {
                mainCamera.transform.RotateAround(Player.transform.position, tempVector, (hitUIObject.transform.position.y - cammeraPlateMouse.transform.position.y) * Time.deltaTime);
            }

            //更新攝影機與目標的相對位置
            cameraRelativeTarget = mainCamera.transform.position - Player.transform.position;

        }
        //如果點選到了移動搖桿
        if (hitUIObjectName == 'movePlate') {

            _sprite = hitUIObject.GetComponent. < UI.Image > ().sprite;
            _rect = hitUIObject.GetComponent. < RectTransform > ().rect;
            imageScale = hitUIObject.GetComponent. < RectTransform > ().localScale;


            //取得使用者滑鼠點擊處的Alpha值(為了不規則的按鈕)
            temp.x = myIputPostion.x - hitUIObject.transform.position.x + _rect.width * 0.5;
            temp.y = myIputPostion.y - hitUIObject.transform.position.y + _rect.height * 0.5;
            UIObjectRGB = _sprite.texture.GetPixel(Mathf.FloorToInt(temp.x * _sprite.texture.width / (_rect.width * imageScale.x)), Mathf.FloorToInt(temp.y * _sprite.texture.height / (_rect.height * imageScale.y)));

            if (UIObjectRGB.a != 0 && Vector2.Distance(myIputPostion, hitUIObject.transform.position) < _rect.width * 0.5) {
                movePlatein2out = true;
                movePlateMouse.transform.position = myIputPostion;
            } else if (movePlatein2out) {
                //如果拖拉滑鼠盤脫離搖桿盤的範圍，取得圓的交點
                a = Vector2(myIputPostion.x, myIputPostion.y);
                b = Vector2(hitUIObject.transform.position.x, hitUIObject.transform.position.y);
                c = Vector3(hitUIObject.transform.position.x, hitUIObject.transform.position.y, _rect.width * 0.5);
                x = getIntersections(a.x, a.y, b.x, b.y, c.x, c.y, c.z);
                movePlateMouse.transform.position.x = x.x;
                movePlateMouse.transform.position.y = x.y;
            }

            //控制生物移動
            if (Vector2.Distance(myIputPostion, hitUIObject.transform.position) > 0) {
                mouseDragVector.x = (myIputPostion.x - mouseStartPOS.x) * 2.5;
                mouseDragVector.z = (myIputPostion.y - mouseStartPOS.y) * 2.5;
                mouseDragDist = Vector3.Distance(myIputPostion, mouseStartPOS);
            }
        }
        //如果點選到了CUBE按鈕
        if (hitUIObjectName == 'cubePlate') {
            if (cubePlateTimerCurrent == 0) {
                cubePlateTimerCurrent = Time.time;
            }
            if (cubePlateTimer.transform.localScale.x < 0.95) {
                cubePlateTimer.transform.localScale = Vector3.MoveTowards(cubePlateTimer.transform.localScale, Vector3(1, 1, 1), 0.05);
            } else {
                itemBagJS.itemBagON = true;
            }
            _sprite = hitUIObject.GetComponent. < UI.Image > ().sprite;
            _rect = hitUIObject.GetComponent. < RectTransform > ().rect;
            imageScale = hitUIObject.GetComponent. < RectTransform > ().localScale;


            //取得使用者滑鼠點擊處的Alpha值(為了不規則的按鈕)
            temp.x = myIputPostion.x - hitUIObject.transform.position.x + _rect.width * 0.5;
            temp.y = myIputPostion.y - hitUIObject.transform.position.y + _rect.height * 0.5;
            UIObjectRGB = _sprite.texture.GetPixel(Mathf.FloorToInt(temp.x * _sprite.texture.width / (_rect.width * imageScale.x)), Mathf.FloorToInt(temp.y * _sprite.texture.height / (_rect.height * imageScale.y)));

            if (UIObjectRGB.a != 0 && Vector2.Distance(myIputPostion, hitUIObject.transform.position) < _rect.width * 0.5) {
                cubePlatein2out = true;
                cubePlateMouse.transform.position = myIputPostion;
                pickTouchSide.transform.position = Vector3(-100, -100, 0.5);
            } else if (cubePlatein2out) {
                //如果拖拉滑鼠盤脫離搖桿盤的範圍
                cubePlateMouse.transform.position = cubePlate.transform.position;
                cubePlateMouse.GetComponent. < UI.Graphic > ().color.a = 0.55;
                cubePlateTimer.transform.localScale = Vector3(0, 0, 0);
            }
        }

        //如果點選到了CUBE按鈕
        if (hitUIObjectName == 'cubePlate') {
            _sprite = hitUIObject.GetComponent. < UI.Image > ().sprite;
            _rect = hitUIObject.GetComponent. < RectTransform > ().rect;
            imageScale = hitUIObject.GetComponent. < RectTransform > ().localScale;


            //取得使用者滑鼠點擊處的Alpha值(為了不規則的按鈕)
            temp.x = myIputPostion.x - hitUIObject.transform.position.x + _rect.width * 0.5;
            temp.y = myIputPostion.y - hitUIObject.transform.position.y + _rect.height * 0.5;
            UIObjectRGB = _sprite.texture.GetPixel(Mathf.FloorToInt(temp.x * _sprite.texture.width / (_rect.width * imageScale.x)), Mathf.FloorToInt(temp.y * _sprite.texture.height / (_rect.height * imageScale.y)));

            if (UIObjectRGB.a != 0 && Vector2.Distance(myIputPostion, hitUIObject.transform.position) < _rect.width * 0.5) {
                cubePlatein2out = true;
                cubePlateMouse.transform.position = myIputPostion;
                pickTouchSide.transform.position = Vector3(-100, -100, 0.5);
            } else if (cubePlatein2out) {
                //如果拖拉滑鼠盤脫離搖桿盤的範圍
                cubePlateMouse.transform.position = cubePlate.transform.position;
                cubePlateMouse.GetComponent. < UI.Graphic > ().color.a = 0.55;
                //                itemBag.GetComponent. < UI.RawImage > ().color.a = 0.0;
            }
        }
        //如果點選到了itemBag介面
        if (hitUIObjectName == 'itemBag') {
            itemBagJS.drag(mouseStartPOS, myIputPostion);
        }

        //如果點選到了itemBag介面
        if (hitUIObjectName == '') {
            if (itemBagJS.itemBagON) {
                itemBagJS.itemBagON = false;
            }
        }
    } else {

        //點螢幕移動
        if (Input.GetMouseButtonUp(0)) {
            biologyJS._pick();
            groundPlane.Set3Points(
                Vector3(1.0, biologyJS.pickPlayer.transform.position.y, 0.0),
                Vector3(0.0, biologyJS.pickPlayer.transform.position.y, 1.0),
                Vector3(1.0, biologyJS.pickPlayer.transform.position.y, 1.0));

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var rayDistance: float;
            if (hitUIObjectName == "" &&
                5.0 > Vector2.Distance(mouseStartPOS, Input.mousePosition) &&
                groundPlane.Raycast(ray, rayDistance)) {
                biologyJS.Sphere2.transform.position = ray.GetPoint(rayDistance);
            }
        }
        cubePlateTimerCurrent = 0;
        cubePlateMouse.GetComponent. < UI.Graphic > ().color.a = 1.0;
        cammeraPlateMouse.transform.position = cammeraPlate.transform.position;
        cubePlateMouse.transform.position = cubePlate.transform.position;
        movePlateMouse.transform.position = movePlate.transform.position;
        hitUIObject = null;
        cubePlateTimer.transform.localScale = Vector3(0.0, 0.0, 1);
        //放開滑鼠時...如果前一個按鍵removePlate，則移除
        //放開滑鼠時...如果前一個按鍵cubePlate，則新增
        if (hitUIObjectName != "") {
            if (hitUIObjectName == 'removePlate') {
                biologyJS.bioAction = "Action";
            }
            if (hitUIObjectName == 'cubePlate') {
                biologyJS.bioAction = "Create";
            }
            if (hitUIObjectName == 'movePlate') {
                biologyJS.Sphere2.transform.position = biologyJS.transform.position;
            }
            hitUIObjectName = "";
        }
        clickStart = false;
        cammeraPlatein2out = false;
        cubePlatein2out = false;
        movePlatein2out = false;
    }

}

function fellowPlayerCameraMove() {
    mainCamera.transform.position = cameraRelativeTarget + Player.transform.position;
    mainCamera.transform.LookAt(Vector3(Player.transform.position.x, Player.transform.position.y + 2.0, Player.transform.position.z));

}

function fellowPlayerCameraContorl() {
    //滑鼠滾輪縮放攝影機
    if (Input.GetAxis("Mouse ScrollWheel") < 0) // forward
    {
        if (Camera.main.fieldOfView > 1)
            Camera.main.fieldOfView = Mathf.Min(Camera.main.fieldOfView - 1, 60);
    }
    if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
    {
        Camera.main.fieldOfView = Mathf.Min(Camera.main.fieldOfView + 1, 60);
    }
}

function fellowPlayerLight() {
    PlayerLight.transform.position = Vector3(Player.transform.position.x, Player.transform.position.y + 1300, Player.transform.position.z);
}

function getMousehitGroupPos() {



    switch (hitUIObjectName) {
    case "cubePlate":
        pickTouchSide.GetComponent. < Renderer > ().enabled = true;
        pickTouch.GetComponent. < Renderer > ().enabled = true;
        break;
    case "removePlate":
        pickTouchSide.GetComponent. < Renderer > ().enabled = false;
        pickTouch.GetComponent. < Renderer > ().enabled = true;
        break;
    default:
        CubePick.GetComponent. < Renderer > ().enabled = false;
        pickTouchSide.GetComponent. < Renderer > ().enabled = false;
        pickTouch.GetComponent. < Renderer > ().enabled = false;
    }



    //滑鼠點擊取得做標點
    ray = Camera.main.ScreenPointToRay(myIputPostion);

    //如果滑鼠左鍵按下，並點擊到plane，並沒有點擊到任何UI，也沒有從搖桿盤拖曳滑鼠出來
    if (touchScreen &&
        Physics.Raycast(ray, mouseHitPlane) &&
        !EventSystem.current.IsPointerOverGameObject() &&
        hitUIObjectName != "cammeraPlate" &&
        hitUIObjectName != "movePlate"
    ) {
        pickTouchSide.transform.position = mouseHitPlane.point;

        pickTouchSide.transform.position.x = Mathf.Floor(pickTouchSide.transform.position.x + 0.5 / 1);
        pickTouchSide.transform.position.y = Mathf.Floor(pickTouchSide.transform.position.y + 0.5 / 1) + 0.5;
        pickTouchSide.transform.position.z = Mathf.Floor(pickTouchSide.transform.position.z + 0.5 / 1);
        //                pickTouchSide.transform.position = pickTouch.transform.position;


        switch (mouseHitPlane.transform.tag) {
        case "Cube":
            pickTouch.transform.position = mouseHitPlane.transform.gameObject.transform.position;
            pickTouchSide.transform.position = mouseHitPlane.transform.gameObject.transform.position;

            if (mouseHitPlane.point.x - mouseHitPlane.transform.position.x >= 0.5 &&
                mouseHitPlane.point.y - mouseHitPlane.transform.position.y <= 0.5 &&
                mouseHitPlane.point.z - mouseHitPlane.transform.position.z <= 0.5) {
                pickTouchSide.transform.position.x += 1.0;
            } else
            if (mouseHitPlane.transform.position.x - mouseHitPlane.point.x >= 0.5 &&
                mouseHitPlane.transform.position.y - mouseHitPlane.point.y <= 0.5 &&
                mouseHitPlane.transform.position.z - mouseHitPlane.point.z <= 0.5) {
                pickTouchSide.transform.position.x -= 1.0;
            } else
            if (mouseHitPlane.point.x - mouseHitPlane.transform.position.x <= 0.5 &&
                mouseHitPlane.point.y - mouseHitPlane.transform.position.y <= 0.5 &&
                mouseHitPlane.point.z - mouseHitPlane.transform.position.z >= 0.5) {
                pickTouchSide.transform.position.z += 1.0;
            } else
            if (mouseHitPlane.transform.position.x - mouseHitPlane.point.x <= 0.5 &&
                mouseHitPlane.transform.position.y - mouseHitPlane.point.y <= 0.5 &&
                mouseHitPlane.transform.position.z - mouseHitPlane.point.z >= 0.5) {
                pickTouchSide.transform.position.z -= 1.0;
            } else
            if (mouseHitPlane.point.x - mouseHitPlane.transform.position.x <= 0.5 &&
                mouseHitPlane.point.y - mouseHitPlane.transform.position.y >= 0.5 &&
                mouseHitPlane.point.z - mouseHitPlane.transform.position.z <= 0.5) {
                pickTouchSide.transform.position.y += 1.0;
            } else
            if (mouseHitPlane.transform.position.x - mouseHitPlane.point.x <= 0.5 &&
                mouseHitPlane.transform.position.y - mouseHitPlane.point.y >= 0.5 &&
                mouseHitPlane.transform.position.z - mouseHitPlane.point.z <= 0.5) {
                pickTouchSide.transform.position.y -= 1.0;
            }
            break;
        case "biology":

            break;
        }
    }


    CubePick.transform.position = pickTouchSide.transform.position;
    CubePick.GetComponent. < MeshFilter > ().mesh = Cube.GetComponent. < MeshFilter > ().mesh;

}

function mouseOrbitSet() {
    mainCamera.AddComponent(mouseOrbit);
    mainCamera.GetComponent(mouseOrbit).target = Player.transform;
    mainCamera.GetComponent(mouseOrbit).targetMove = Vector3(0, 2, 0);
}
