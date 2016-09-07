// # pragma strict
import UnityEngine.EventSystems;
import System.Collections.Generic;
var Plane: GameObject;

var Sphere: GameObject;
var Cube: GameObject;
var Player: GameObject;
var PlayerLight: GameObject;
var PlayerCamera: GameObject;
var array3d: Dictionary. < Vector3, boolean > = new Dictionary. < Vector3,
    boolean > ();
var myButton: GameObject;
var myButtonJump: GameObject;
var myButtonForward: GameObject;
var myButtonBackward: GameObject;
var myButtonLeft: GameObject;
var myButtonRight: GameObject;
var pickTouch: GameObject;
var pickTouchSide: GameObject;
var cammeraPlate: GameObject;
var biologyJS: biology;

//紀錄滑鼠首次點擊座標
var mouseStartPOS: Vector3;
var clickStart = false;
var mouseDragVector: Vector3;
var mouseDragDist: float;
var cameraAngle: float;


//紀錄滑鼠首次按壓的UI
var hitUIObject: GameObject;
var hitUIObjectName: String = "";

//目前點擊的UI名稱
var nowButton: String;
var cammeraPlateMouse: GameObject;
var movePlateMouse: GameObject;
var movePlate: GameObject;
var cammeraPlatein2out: boolean = false;
var movePlatein2out = false;

function Start() {
    cammeraPlateMouse = GameObject.Find("cammeraPlateMouse");
    movePlateMouse = GameObject.Find("movePlateMouse");
    movePlate = GameObject.Find("movePlate");
    cameraAngle = cameraAngle || 45.0;

    //宣告各個變數代表的gameObject
    PlayerLight = GameObject.Find("PlayerLight");
    Plane = GameObject.Find("Plane");
    pickTouch = GameObject.Find("pickTouch");
    Sphere = GameObject.Find("Sphere");
    Cube = GameObject.Find("Cube");
    Player = GameObject.Find("Cha_Knight");
    PlayerCamera = GameObject.Find("PlayerCamera");
    Sphere.transform.position = Player.transform.position;
    Player.AddComponent(biology);
    Player.GetComponent(biology).Sphere = Sphere;
    pickTouchSide = GameObject.Find("pickTouchSide");
    biologyJS = Player.GetComponent(biology);

    myButton = GameObject.Find("Button_LEFT");
    myButton.GetComponent(UI.Button).onClick.AddListener(Button_LEFT);
    myButtonJump = GameObject.Find("Button_jump");
    myButtonJump.GetComponent(UI.Button).onClick.AddListener(Button_jump);
    myButtonForward = GameObject.Find("Button_RIGHT");
    myButtonForward.GetComponent(UI.Button).onClick.AddListener(Button_RIGHT);
    myButtonBackward = GameObject.Find("Button_down");


    cammeraPlate = GameObject.Find("cammeraPlate");

}





function Button_LEFT() {
    biologyJS.bioAction = "Create";
    print("ButtonLeft");
}

function Button_jump() {
    biologyJS.bioAction = "Jump";
}

function Button_RIGHT() {
    biologyJS.bioAction = "Action";
}


function setArray(a: Vector3) {
    array3d[a] = true;
}

function removeArray(a: Vector3) {
    array3d[a] = false;
}

function checkArray(a: Vector3) {
    if (array3d.ContainsKey(a)) {
        if (array3d[a]) {
            return true;
        }
    }
}
var tempVector: Vector3;

function Update() {
    getMousehitGroupPos();
    fellowPlayerLight();
    fellowPlayerCameraMove();
    fellowPlayerCameraContorl();
    buttonDetect();
    tempVector = PlayerCamera.transform.position - Player.transform.position;
    Debug.DrawLine(Vector3.zero, tempVector, Color.green);
    tempVector = Vector3(tempVector.y, tempVector.x, tempVector.z);
    Debug.DrawLine(Vector3.zero, tempVector, Color.red);
    tempVector = Quaternion.Euler(0, 0, 90) * tempVector;
    tempVector.y = 0;
    Debug.DrawLine(Vector3.zero, tempVector, Color.blue);

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
        //        var g = {
        //            coords: [],
        //            onLine: false
        //        };
        var gcoords0 = ((t + dt) * d[0]) + a[0];
        var gcoords1 = ((t + dt) * d[1]) + a[1];
        var finalAnswer: Vector2 = Vector2(fcoords0, fcoords1);
        // check if g lies on the line
        //        g.onLine = is_on(a, b, g.coords);
        return (finalAnswer);
        //        return {
        //            points: {
        //                intersection1: f,
        //                intersection2: g
        //            }
        //        };

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

    if (Input.GetMouseButton(0)) {


        //取得按壓的物件名稱
        if (EventSystem.current.IsPointerOverGameObject()) {
            hitUIObject = EventSystem.current.currentSelectedGameObject;
            hitUIObjectName = hitUIObject.name;
        }

        //如果點選到了搖桿
        if (hitUIObjectName == 'cammeraPlate') {
            var _sprite = hitUIObject.GetComponent. < UI.Image > ().sprite;
            var _rect = hitUIObject.GetComponent. < RectTransform > ().rect;
            var temp: Vector2;
            var UIObjectRGB: Color;
            var imageScale: Vector2 = hitUIObject.GetComponent. < RectTransform > ().localScale;


            //取得使用者滑鼠點擊處的Alpha值(為了不規則的按鈕)
            temp.x = Input.mousePosition.x - hitUIObject.transform.position.x + _rect.width * 0.5;
            temp.y = Input.mousePosition.y - hitUIObject.transform.position.y + _rect.height * 0.5;
            UIObjectRGB = _sprite.texture.GetPixel(Mathf.FloorToInt(temp.x * _sprite.texture.width / (_rect.width * imageScale.x)), Mathf.FloorToInt(temp.y * _sprite.texture.height / (_rect.height * imageScale.y)));

            if (UIObjectRGB.a != 0 && Vector2.Distance(Input.mousePosition, hitUIObject.transform.position) < _rect.width * 0.5) {
                cammeraPlatein2out = true;
                cammeraPlateMouse.transform.position = Input.mousePosition;
            } else if (cammeraPlatein2out) {
                //如果拖拉滑鼠盤脫離搖桿盤的範圍，取得圓的交點
                var a: Vector2 = Vector2(Input.mousePosition.x, Input.mousePosition.y);
                var b: Vector2 = Vector2(hitUIObject.transform.position.x, hitUIObject.transform.position.y);
                var c: Vector3 = Vector3(hitUIObject.transform.position.x, hitUIObject.transform.position.y, _rect.width * 0.5);
                var x: Vector2 = getIntersections(a.x, a.y, b.x, b.y, c.x, c.y, c.z);
                cammeraPlateMouse.transform.position.x = x.x;
                cammeraPlateMouse.transform.position.y = x.y;
            }

            //控制攝影機
            //            PlayerCamera.transform.RotateAround(Player.transform.position, Vector3.up, (hitUIObject.transform.position.x - cammeraPlateMouse.transform.position.x) * Time.deltaTime);

            //            Debug.DrawLine(Vector3.zero, tempVector, Color.red);
            tempVector = Quaternion.Euler(0, 0, 90) * tempVector;
            tempVector.y = 0;
            PlayerCamera.transform.RotateAround(Player.transform.position, tempVector, (hitUIObject.transform.position.y - cammeraPlateMouse.transform.position.y) * Time.deltaTime);
        }
        if (hitUIObjectName == 'movePlate') {
            _sprite = hitUIObject.GetComponent. < UI.Image > ().sprite;
            _rect = hitUIObject.GetComponent. < RectTransform > ().rect;
            imageScale = hitUIObject.GetComponent. < RectTransform > ().localScale;


            //取得使用者滑鼠點擊處的Alpha值(為了不規則的按鈕)
            temp.x = Input.mousePosition.x - hitUIObject.transform.position.x + _rect.width * 0.5;
            temp.y = Input.mousePosition.y - hitUIObject.transform.position.y + _rect.height * 0.5;
            UIObjectRGB = _sprite.texture.GetPixel(Mathf.FloorToInt(temp.x * _sprite.texture.width / (_rect.width * imageScale.x)), Mathf.FloorToInt(temp.y * _sprite.texture.height / (_rect.height * imageScale.y)));

            if (UIObjectRGB.a != 0 && Vector2.Distance(Input.mousePosition, hitUIObject.transform.position) < _rect.width * 0.5) {
                movePlatein2out = true;
                movePlateMouse.transform.position = Input.mousePosition;
            } else if (movePlatein2out) {
                //如果拖拉滑鼠盤脫離搖桿盤的範圍，取得圓的交點
                a = Vector2(Input.mousePosition.x, Input.mousePosition.y);
                b = Vector2(hitUIObject.transform.position.x, hitUIObject.transform.position.y);
                c = Vector3(hitUIObject.transform.position.x, hitUIObject.transform.position.y, _rect.width * 0.5);
                x = getIntersections(a.x, a.y, b.x, b.y, c.x, c.y, c.z);
                movePlateMouse.transform.position.x = x.x;
                movePlateMouse.transform.position.y = x.y;
            }

            //控制生物移動
            if (Vector2.Distance(Input.mousePosition, hitUIObject.transform.position) > 0) {
                if (!clickStart) {
                    clickStart = true;
                    mouseStartPOS = Input.mousePosition;
                }
                mouseDragVector.x = (Input.mousePosition.x - mouseStartPOS.x) * 2.5;
                mouseDragVector.z = (Input.mousePosition.y - mouseStartPOS.y) * 2.5;
                mouseDragDist = Vector3.Distance(Input.mousePosition, mouseStartPOS);
            }
        }


    } else {
        cammeraPlateMouse.transform.position = cammeraPlate.transform.position;
        movePlateMouse.transform.position = movePlate.transform.position;
        hitUIObject = null;
        hitUIObjectName = "";
        clickStart = false;
        cammeraPlatein2out = false;
        movePlatein2out = false;
    }

}



//========================================================

function fellowPlayerCameraMove() {
    //    print(Vector3.Distance(PlayerCamera.transform.position, Player.transform.position));
    if (Vector3.Distance(PlayerCamera.transform.position, Player.transform.position) > 15) {
        PlayerCamera.transform.position -= (PlayerCamera.transform.position - Player.transform.position) * 0.01;
        PlayerCamera.transform.position.y = 5;
        print('forward');
    }
    if (Vector3.Distance(PlayerCamera.transform.position, Player.transform.position) < 10) {
        PlayerCamera.transform.position += (PlayerCamera.transform.position - Player.transform.position) * 0.01;
        PlayerCamera.transform.position.y = 5;
        print('Backward');
    }
    PlayerCamera.transform.LookAt(Vector3(Player.transform.position.x, Player.transform.position.y + 1.0, Player.transform.position.z));

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

function _pickAuto() {
    //新增pickAuto，讓選取框會自動往下格動
}

function fellowPlayerLight() {
    PlayerLight.transform.position = Vector3(Player.transform.position.x, Player.transform.position.y + 8, Player.transform.position.z);
}

function getMousehitGroupPos() {

    //    Cube.layer = 2;
    //    Plane.transform.position.y = Player.transform.position.y - 1;
    Sphere.layer = 2;
    Player.layer = 2;

    //滑鼠點擊取得做標點
    var mouseHitPlane: RaycastHit;
    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    if (Input.touches.length > 0) {
        for (var touch: Touch in Input.touches) {
            var id = touch.fingerId;
            if (Physics.Raycast(ray, mouseHitPlane) && !EventSystem.current.IsPointerOverGameObject(id)) {
                if (Input.GetMouseButton(0)) {
                    Sphere.transform.position = mouseHitPlane.point;
                } else {
                    Sphere.transform.position = Player.transform.position;
                    Sphere.transform.position = Player.transform.position;
                }
            }
        }
    } else {

        //預設滑鼠未點擊時，操控球sphere要在角色底下
        //todo:寫在這裡太難擴充了 之後要改
        Sphere.transform.position = Player.transform.position;

        //如果滑鼠左鍵按下，並點擊到plane，並沒有點擊到任何UI，也沒有從搖桿盤拖曳滑鼠出來
        if (Input.GetMouseButton(0) &&
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

    }

    //    Cube.layer = 0;
    Sphere.layer = 0;
    Player.layer = 0;

}
