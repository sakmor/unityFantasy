// # pragma strict
import UnityEngine.EventSystems;
import System.Collections.Generic;
var Plane: GameObject;

var Sphere: GameObject;
var Cube: GameObject;
var CubePick: GameObject;
var Player: GameObject;
var PlayerLight: GameObject;
var PlayerCamera: GameObject;
var dictionary3d: Dictionary. < Vector3, int > =
    new Dictionary. < Vector3,
    int > ();
var array3d = new Array();
var pickTouch: GameObject;
var pickTouchSide: GameObject;
var biologyJS: biology;
var mouseOrbitJS: mouseOrbit;
var PlayerPrefsX: PlayerPrefsX;

var myIputPostion: Vector2;
var touchScreen: boolean = false;

//紀錄滑鼠首次點擊座標
var mouseStartPOS: Vector3;
var clickStart: boolean = false;
var mouseDragVector: Vector3;
var mouseDragDist: float;


//紀錄滑鼠首次按壓的UI
var hitUIObject: GameObject;
var hitUIObjectName: String = "";

//目前點擊的UI名稱
var nowButton: String;
var movePlateMouse: GameObject;
var movePlate: GameObject;
var cammeraPlatein2out: boolean = false;
var movePlatein2out = false;
var cammeraPlate: GameObject;
var cammeraPlateMouse: GameObject;

//設定物件
var cubeArrayTxt = new Array();


//攝影機相對目標
var cameraRelativeTarget: Vector3;
var ray: Ray;
var mouseHitPlane: RaycastHit;

function Start() {



    cammeraPlateMouse = GameObject.Find("cammeraPlateMouse");
    movePlateMouse = GameObject.Find("movePlateMouse");
    movePlate = GameObject.Find("movePlate");


    //宣告各個變數代表的gameObject
    PlayerLight = GameObject.Find("PlayerLight");
    Plane = GameObject.Find("Plane");
    pickTouch = GameObject.Find("pickTouch");
    Sphere = GameObject.Find("Sphere");
    Cube = GameObject.Find("Cube");
    CubePick = GameObject.Find("CubePick");

    Player = GameObject.Find("Cha_Knight");
    PlayerCamera = GameObject.Find("PlayerCamera");
    Sphere.transform.position = Player.transform.position;
    Player.AddComponent(biology);
    Player.GetComponent(biology).Sphere = Sphere;
    pickTouchSide = GameObject.Find("pickTouchSide");
    biologyJS = Player.GetComponent(biology);

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
    cameraRelativeTarget = PlayerCamera.transform.position - Player.transform.position;
    loadResources();
    loadGame();
    //設定攝影機
    mouseOrbitSet();
}

function Update() {
    mouseOrTouch();
    getMousehitGroupPos();
    fellowPlayerLight();
    fellowPlayerCameraMove();
    fellowPlayerCameraContorl();
    buttonDetect();
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
    dictionary3d[a] = array3d.length;
    array3d.Push(Color(a.x, a.y, a.z, b));
    saveGame();
}

function removeArray(a: Vector3) {
    array3d[dictionary3d[a]] = null;
    dictionary3d[a] = 0;
}

function checkArray(a: Vector3) {
    if (dictionary3d.ContainsKey(a)) {
        if (dictionary3d[a] != 0) {
            return true;
        }
    }
}

function mouseOrTouch() {
    if (Input.touchSupported) {
        if (Input.touchCount > 0) {
            myIputPostion = Input.GetTouch(0).position;
            touchScreen = true;
        }
    } else
    if (Input.GetMouseButton(0)) {
        myIputPostion = myIputPostion;
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
    var array3dLoad: Color[] = PlayerPrefsX.GetColorArray("array3d");
    Player.transform.position = PlayerPrefsX.GetVector3("playerPos");
    //    Player.transform.Rotate = PlayerPrefsX.GetVector3("playerRotate");
    for (var i = 0; i < array3dLoad.length; i++) {
        var temp = Instantiate(Cube);
        print(array3dLoad[i].a);
        //        temp.GetComponent. < MeshFilter > ().mesh = Resources.Load('item/model/CUBE/' + Path.GetFileNameWithoutExtension(cubeArrayTxt[array3dLoad[i].a]), Mesh);
        temp.GetComponent. < MeshFilter > ().mesh = Resources.Load('item/model/CUBE/' + array3dLoad[i].a, Mesh);
        temp.GetComponent. < Renderer > ().enabled = true;
        temp.AddComponent(BoxCollider);
        temp.name = "(" + array3dLoad[i].r.ToString("F0") + ", " + array3dLoad[i].g.ToString("F0") + ", " + array3dLoad[i].b.ToString("F0") + ")";
        temp.transform.position.x = array3dLoad[i].r;
        temp.transform.position.y = array3dLoad[i].g;
        temp.transform.position.z = array3dLoad[i].b;
        setArray(temp.transform.position, array3dLoad[i].a);
    }
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

        print("save game");
    } else {
        print("Not game data saved");
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

function buttonDetect() {
    //當滑鼠按壓，並點選到UI時

    if (touchScreen) {


        //取得按壓的物件名稱
        if (EventSystem.current.IsPointerOverGameObject()) {
            hitUIObject = EventSystem.current.currentSelectedGameObject;
            if (hitUIObject) {
                hitUIObjectName = hitUIObject.name;
            }
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
            PlayerCamera.transform.RotateAround(Player.transform.position, Vector3.up, (hitUIObject.transform.position.x - cammeraPlateMouse.transform.position.x) * Time.deltaTime);

            //控制攝影機--香菇頭上下
            var camera2PlayerVector = PlayerCamera.transform.position - Player.transform.position;
            var tempVector = camera2PlayerVector;
            tempVector.y = 0;
            tempVector = Quaternion.Euler(0, 90, 0) * tempVector;

            //限制攝影機上下移動的角度
            if (hitUIObject.transform.position.y - cammeraPlateMouse.transform.position.y < 0) {
                if (Vector3.Angle(camera2PlayerVector, Vector3.up) >= 10) {
                    PlayerCamera.transform.RotateAround(Player.transform.position, tempVector, (hitUIObject.transform.position.y - cammeraPlateMouse.transform.position.y) * Time.deltaTime);
                }
            } else
            if (Vector3.Angle(camera2PlayerVector, Vector3.up) <= 160) {
                PlayerCamera.transform.RotateAround(Player.transform.position, tempVector, (hitUIObject.transform.position.y - cammeraPlateMouse.transform.position.y) * Time.deltaTime);
            }

            //更新攝影機與目標的相對位置
            cameraRelativeTarget = PlayerCamera.transform.position - Player.transform.position;

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
                if (!clickStart) {
                    clickStart = true;
                    mouseStartPOS = myIputPostion;
                }
                mouseDragVector.x = (myIputPostion.x - mouseStartPOS.x) * 2.5;
                mouseDragVector.z = (myIputPostion.y - mouseStartPOS.y) * 2.5;
                mouseDragDist = Vector3.Distance(myIputPostion, mouseStartPOS);
            }
        }
        //如果點選到了CUBE按鈕
        if (hitUIObjectName == 'cubePlate') {
            print('cube ');
        }

    } else {
        cammeraPlateMouse.transform.position = cammeraPlate.transform.position;
        movePlateMouse.transform.position = movePlate.transform.position;
        hitUIObject = null;

        if (hitUIObjectName != "") {

            if (hitUIObjectName == 'removePlate') {
                biologyJS.bioAction = "Action";
            }
            if (hitUIObjectName == 'cubePlate') {
                biologyJS.bioAction = "Create";
            }
            hitUIObjectName = "";
        }
        clickStart = false;
        cammeraPlatein2out = false;
        movePlatein2out = false;
    }

}

function fellowPlayerCameraMove() {
    PlayerCamera.transform.position = cameraRelativeTarget + Player.transform.position;
    PlayerCamera.transform.LookAt(Vector3(Player.transform.position.x, Player.transform.position.y + 2.0, Player.transform.position.z));

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
    PlayerLight.transform.position = Vector3(Player.transform.position.x, Player.transform.position.y + 8, Player.transform.position.z);
}

function getMousehitGroupPos() {

    if (hitUIObjectName == "cubePlate") {
        CubePick.GetComponent. < Renderer > ().enabled = true;
        pickTouchSide.GetComponent. < Renderer > ().enabled = true;
        pickTouch.GetComponent. < Renderer > ().enabled = true;
    } else if (hitUIObjectName == "removePlate") {
        CubePick.GetComponent. < Renderer > ().enabled = false;
        pickTouchSide.GetComponent. < Renderer > ().enabled = false;
        pickTouch.GetComponent. < Renderer > ().enabled = true;
    } else {
        CubePick.GetComponent. < Renderer > ().enabled = false;
        pickTouchSide.GetComponent. < Renderer > ().enabled = false;
        pickTouch.GetComponent. < Renderer > ().enabled = false;

    }
    //    Cube.layer = 2;
    //    Plane.transform.position.y = Player.transform.position.y - 1;
    Sphere.layer = 2;
    Player.layer = 2;

    //滑鼠點擊取得做標點

    ray = Camera.main.ScreenPointToRay(myIputPostion);


    //預設滑鼠未點擊時，操控球sphere要在角色底下
    //todo:寫在這裡太難擴充了 之後要改
    Sphere.transform.position = Player.transform.position;

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

        if (mouseHitPlane.transform.tag == "Cube") {
            pickTouch.transform.position = mouseHitPlane.transform.gameObject.transform.position;
            pickTouchSide.transform.position = mouseHitPlane.transform.gameObject.transform.position;
            var tempVector: Vector3 = mouseHitPlane.transform.position - mouseHitPlane.point;
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
        }
    }


    CubePick.transform.position = pickTouchSide.transform.position;
    CubePick.GetComponent. < MeshFilter > ().mesh = Cube.GetComponent. < MeshFilter > ().mesh;

    //    Cube.layer = 0;
    Sphere.layer = 0;
    Player.layer = 0;

}

function mouseOrbitSet() {
    PlayerCamera.AddComponent(mouseOrbit);
    PlayerCamera.GetComponent(mouseOrbit).target = Player.transform;
    PlayerCamera.GetComponent(mouseOrbit).targetMove = Vector3(0, 2, 0);
}
