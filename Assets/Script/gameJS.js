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

//紀錄滑鼠首次點擊座標
var mouseStartPOS: Vector3;
var clickStart = false;
var mouseDragVector: Vector3;
var mouseDragDist: float;
var cameraAngle: float;

function Start() {
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
    _input();

}

function _input() {
    if (Input.anyKey) {
        if (Input.GetKey(KeyCode.Space)) {
            //            Sphere.transform.position = this.transform.position;
            //            this.bioAction = "Action";

            PlayerCamera.transform.RotateAround(Player.transform.position, Vector3.up, 200 * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.F)) {
            //            this.bioAction = "Jump";

        }
        if (Input.GetKey(KeyCode.A)) {
            transform.Rotate(0, -3, 0);
        }
        if (Input.GetKey(KeyCode.D)) {
            transform.Rotate(0, 3, 0);
        }
        if (Input.GetKey(KeyCode.W)) {
            PlayerCamera.transform.position.x += 1;
        }
        if (Input.GetKey(KeyCode.S)) {
            Sphere.transform.position.x = this.transform.position.x - transform.forward.x * 2.5;
            Sphere.transform.position.z = this.transform.position.z - transform.forward.z * 2.5;
        }
    }


}
//========================================================

function fellowPlayerCameraMove() {
    //    print(Vector3.Distance(PlayerCamera.transform.position, Player.transform.position));
    if (Vector3.Distance(PlayerCamera.transform.position, Player.transform.position) > 20) {
        PlayerCamera.transform.position -= (PlayerCamera.transform.position - Player.transform.position) * 0.01;
        PlayerCamera.transform.position.y = 10;
        print('forward');
    }
    if (Vector3.Distance(PlayerCamera.transform.position, Player.transform.position) < 18) {
        PlayerCamera.transform.position += (PlayerCamera.transform.position - Player.transform.position) * 0.01;
        PlayerCamera.transform.position.y = 10;
        print('Backward');
    }
    PlayerCamera.transform.LookAt(Vector3(Player.transform.position.x, Player.transform.position.y + 1.0, Player.transform.position.z));

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
                    Sphere.transform.position = Player.transform.position;
                }
            }
        }
    } else {

        //預設滑鼠未點擊時，操控球sphere要在角色底下
        //todo:寫在這裡太難擴充了 之後要改
        Sphere.transform.position = Player.transform.position;

        //如果滑鼠左鍵按下，並點擊到plane，並沒有點擊到任何UI
        if (Input.GetMouseButton(0) && Physics.Raycast(ray, mouseHitPlane) && !EventSystem.current.IsPointerOverGameObject()) {
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
        //如果滑鼠右鍵按下，並點擊到plane，並沒有點擊到任何UI
        //clickStart:如果是false狀態，則將現在點擊的座標視為原點，並將狀態改為true
        //所以clickStart=true時，表示現在是滑鼠拖拉狀態
        if (Input.GetMouseButton(1)) {
            if (!clickStart) {
                clickStart = true;
                mouseStartPOS = Input.mousePosition;
            }
            mouseDragVector.x = (Input.mousePosition.x - mouseStartPOS.x);
            mouseDragVector.z = (Input.mousePosition.y - mouseStartPOS.y);
            mouseDragDist = Vector3.Distance(Input.mousePosition, mouseStartPOS);
        } else {
            clickStart = false;
            //            mouseDragDist = 0;
        }
    }

    //    Cube.layer = 0;
    Sphere.layer = 0;
    Player.layer = 0;

}
