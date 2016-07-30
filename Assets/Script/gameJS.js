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
var biologyJS: biology;

function Start() {

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
    myButtonBackward.GetComponent(UI.Button).onClick.AddListener(Button_down);
    myButtonLeft = GameObject.Find("Button_left");
    myButtonLeft.GetComponent(UI.Button).onClick.AddListener(Button_left);
    myButtonRight = GameObject.Find("Button_right");
    myButtonRight.GetComponent(UI.Button).onClick.AddListener(Button_right);
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

function Button_down() {
    Sphere.transform.position.x = biologyJS.transform.position.x - biologyJS.transform.forward.x * 2.5;
    Sphere.transform.position.z = biologyJS.transform.position.z - biologyJS.transform.forward.z * 2.5;
}

function Button_left() {
    biologyJS.transform.Rotate(0, -3, 0);
}

function Button_right() {
    biologyJS.transform.Rotate(0, 3, 0);
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

function Update() {
    getMousehitGroupPos();
    fellowPlayerLight();
    fellowPlayerCameraMove();
    fellowPlayerCameraContorl();

}

//========================================================

function fellowPlayerCameraMove() {
    PlayerCamera.transform.position.x = Player.transform.position.x + -12;
    PlayerCamera.transform.position.z = Player.transform.position.z + -12;
    PlayerCamera.transform.position.y = Player.transform.position.y + 9;

}

function fellowPlayerCameraContorl() {
    //滑鼠滾輪縮放攝影機
    if (Input.GetAxis("Mouse ScrollWheel") < 0) // forward
    {
        if (Camera.main.orthographicSize > 1)
            Camera.main.orthographicSize = Mathf.Min(Camera.main.orthographicSize - 1, 10);
    }
    if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
    {
        Camera.main.orthographicSize = Mathf.Min(Camera.main.orthographicSize + 1, 10);
    }
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
                }
            }
        }
    } else {
        if (Physics.Raycast(ray, mouseHitPlane) && !EventSystem.current.IsPointerOverGameObject()) {
            if (Input.GetMouseButton(0)) {
                Sphere.transform.position = mouseHitPlane.point;

                pickTouchSide.transform.position.x = Mathf.Floor(Sphere.transform.position.x + 0.5 / 1);
                pickTouchSide.transform.position.y = Mathf.Floor(Sphere.transform.position.y + 0.5 / 1) + 0.5;
                pickTouchSide.transform.position.z = Mathf.Floor(Sphere.transform.position.z + 0.5 / 1);
//                pickTouchSide.transform.position = pickTouch.transform.position;

                if (mouseHitPlane.transform.tag == "Cube") {
                    pickTouch.transform.position = mouseHitPlane.transform.gameObject.transform.position;
                    pickTouchSide.transform.position = mouseHitPlane.transform.gameObject.transform.position;
                    var tempVector: Vector3 = mouseHitPlane.transform.position - Sphere.transform.position;
                    if (Sphere.transform.position.x - mouseHitPlane.transform.position.x >= 0.5 &&
                        Sphere.transform.position.y - mouseHitPlane.transform.position.y <= 0.5 &&
                        Sphere.transform.position.z - mouseHitPlane.transform.position.z <= 0.5) {
                        pickTouchSide.transform.position.x += 1.0;
                    } else
                    if (mouseHitPlane.transform.position.x - Sphere.transform.position.x >= 0.5 &&
                        mouseHitPlane.transform.position.y - Sphere.transform.position.y <= 0.5 &&
                        mouseHitPlane.transform.position.z - Sphere.transform.position.z <= 0.5) {
                        pickTouchSide.transform.position.x -= 1.0;
                    } else
                    if (Sphere.transform.position.x - mouseHitPlane.transform.position.x <= 0.5 &&
                        Sphere.transform.position.y - mouseHitPlane.transform.position.y <= 0.5 &&
                        Sphere.transform.position.z - mouseHitPlane.transform.position.z >= 0.5) {
                        pickTouchSide.transform.position.z += 1.0;
                    } else
                    if (mouseHitPlane.transform.position.x - Sphere.transform.position.x <= 0.5 &&
                        mouseHitPlane.transform.position.y - Sphere.transform.position.y <= 0.5 &&
                        mouseHitPlane.transform.position.z - Sphere.transform.position.z >= 0.5) {
                        pickTouchSide.transform.position.z -= 1.0;
                    } else
                    if (Sphere.transform.position.x - mouseHitPlane.transform.position.x <= 0.5 &&
                        Sphere.transform.position.y - mouseHitPlane.transform.position.y >= 0.5 &&
                        Sphere.transform.position.z - mouseHitPlane.transform.position.z <= 0.5) {
                        pickTouchSide.transform.position.y += 1.0;
                    } else
                    if (mouseHitPlane.transform.position.x - Sphere.transform.position.x <= 0.5 &&
                        mouseHitPlane.transform.position.y - Sphere.transform.position.y >= 0.5 &&
                        mouseHitPlane.transform.position.z - Sphere.transform.position.z <= 0.5) {
                        pickTouchSide.transform.position.y -= 1.0;
                    }
                    //                    print(tempVector);

                    //                    print("Pick: " + pickTouchSide.transform.position);
                    //                    print("mouse: " + mouseHitPlane.point);
                } else {
                    //                    pickTouchSide.transform.position = pickTouch.transform.position;

                }
            } else {
                Sphere.transform.position = Player.transform.position;

            }
        }
    }

    //    Cube.layer = 0;
    Sphere.layer = 0;
    Player.layer = 0;

}
