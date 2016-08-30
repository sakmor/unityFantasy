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
var mousePlate: GameObject;
var biologyJS: biology;

//紀錄滑鼠首次點擊座標
var mouseStartPOS: Vector3;
var clickStart = false;
var mouseDragVector: Vector3;
var mouseDragDist: float;
var cameraAngle: float;

//目前點擊的UI名稱
var nowButton: String;

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

    cammeraPlate = GameObject.Find("cammeraPlate");

    mousePlate = GameObject.Find("mousePlate");
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
    PlayerCamera.transform.RotateAround(Player.transform.position, Vector3.up, 800 * Time.deltaTime);
}

function Button_right() {
    PlayerCamera.transform.RotateAround(Player.transform.position, Vector3.up, -800 * Time.deltaTime);
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
    buttonDetect();


}



function getSpritePixel() {


}

function buttonDetect() {
    //當滑鼠按壓，並點選到UI時
    if (Input.GetMouseButton(0) && EventSystem.current.IsPointerOverGameObject()) {


        //        Input.mousePosition.x -
        var _sprite = cammeraPlate.GetComponent. < UI.Image > ().sprite;
        var _rect = cammeraPlate.GetComponent. < RectTransform > ().rect;

        var temp: Vector2;
        temp.x = Input.mousePosition.x -
            cammeraPlate.transform.position.x + _rect.width * 0.5;
        temp.y = Input.mousePosition.y -
            cammeraPlate.transform.position.y + _rect.height * 0.5;
        //        print(temp);
        var imageScale: Vector2 = cammeraPlate.GetComponent. < RectTransform > ().localScale;
        //        print("RGB" + _sprite.texture.GetPixel(Mathf.Floor(temp.x / imageScale.x), Mathf.Floor(temp.y / imageScale.y)));
        print(imageScale);

        print("RGB_11" + _sprite.texture.GetPixel(Mathf.Floor(temp.x * _sprite.texture.width / _rect.width / imageScale.x), Mathf.Floor(temp.y * _sprite.texture.height / _rect.height / imageScale.y)));
        print(temp);
        //        print(cammeraPlate.GetComponent. < RectTransform > ().pivot.x * cammeraPlate.GetComponent. < RectTransform > ().rect.width);
        //            print(EventSystem.current.currentSelectedGameObject.name);
        nowButton = EventSystem.current.currentSelectedGameObject.name;

        //        print(cammeraPlate.RawImage.texture.texture2D.GetPixel(0, 0));
        switch (nowButton) {
        case "cammeraPlate":
            mousePlate.transform.position = Input.mousePosition;
            break;
        }
    } else {
        nowButton = "";
    }

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
            PlayerCamera.transform.RotateAround(Player.transform.position, Vector3.up, 200 * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D)) {
            PlayerCamera.transform.RotateAround(Player.transform.position, Vector3.up, -200 * Time.deltaTime);
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
