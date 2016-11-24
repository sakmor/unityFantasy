// #pragma strict

var moveSpeed: float;

//目前所選擇的材質
var handCube: float;
var moveSpeedMax: float;
var rotateSpeed: float;
var bioAction: String;

private
var _bioAction: String;
private
var _backward: boolean;

var Sphere: GameObject;
var Sphere2: GameObject;
var Pick: GameObject;
var Cube: GameObject;
var anim: Animation;
var WalkSteptweek: float;
private
var mainGame: GameObject;
private
var maingameJS: gameJS;
var TextMesh: TextMesh;
var pickPlayer: GameObject;
var Plane_touch: GameObject;
var pickTouch: GameObject;
var pickTouchSide: GameObject;
var onAir: boolean;

var collisionCubes: GameObject[];
collisionCubes = new GameObject[28];

//Pick           --在玩家面前的選取框
//PickPlayer     --玩家所在的位置
//PickTouch      --滑鼠點選地面位置,或是點擊的Cube選取框
//PickTouchSide  --滑鼠點擊Cube的某一面


function Start() {
    anim = GetComponent. < Animation > ();
    bioAction = "Wait";
    handCube = 0;
    mainGame = GameObject.Find("mainGame");
    Sphere = Instantiate(GameObject.Find("Sphere2"));
    Sphere.name = this.name + '_Sphere';
    Sphere2 = Instantiate(GameObject.Find("Sphere2"));
    Sphere2.name = this.name + '_Sphere2';
    Sphere2.transform.parent = GameObject.Find("Biology").transform;
    maingameJS = GameObject.Find("mainGame").GetComponent(gameJS);
    pickTouch = maingameJS.pickTouch;
    pickTouchSide = maingameJS.pickTouchSide;
    Plane_touch = GameObject.Find("Plane_touch");
    pickPlayer = GameObject.Find("pickPlayer");
    _backward = false;
    WalkSteptweek = WalkSteptweek || 40;
    moveSpeed = moveSpeed || 0.07;
    moveSpeedMax = moveSpeed;
    rotateSpeed = rotateSpeed || 10;
    Pick = GameObject.Find("pick");
    Cube = GameObject.Find("Cube");

    var tempVector3: Vector3 = GameObject.Find("pickPlayer").transform.position;
    var collisionCubeOBJ: GameObject;
    collisionCubeOBJ = new GameObject(this + '_collisionCubeOBJ');
    collisionCubeOBJ.transform.parent = GameObject.Find("Biology").transform;
    //    collisionCubes.push(Instantiate(GameObject.Find("pickPlayer")));
    for (var i = 0; i <= 27; i++) {
        collisionCubes[i] = Instantiate(GameObject.Find("pickPlayer"));
        collisionCubes[i].name = 'dynamicCollision_' + i;
        collisionCubes[i].AddComponent(BoxCollider);
        collisionCubes[i].transform.parent = collisionCubeOBJ.transform;
    }
    //更新pick狀態
    _pick();
    //抓取動作檔案
    AnimationClip();

}

function Update() {

    if (Input.GetKeyDown("k")) {
        _createCube();
    }
    if (Input.GetKeyDown("a")) {
        this.bioAction = "Action";
    }
    this._movment();
    this._bioStatus();
    //    this._autoJump();
    this._cubeHead();

    dynamicCollision();
}


function dynamicCollision() {



}
//function OnCollisionEnter(collision: Collision) {
//    print(collision.gameObject.transform.position.y - this.transform.position.y);
//    if (collision.gameObject.transform.position.y - this.transform.position.y > 1) {
//        this.transform.position.y += 0.1;
//    }
//}
//
//function OnCollisionStay(collision: Collision) {
//
//    if (collision.relativeVelocity.y > 0) {
//        onAir = false;
//    }
//}
//
//function OnCollisionExit(collision: Collision) {
//
//    if (collision.relativeVelocity.y > 0) {
//        onAir = true;
//    }
//}

function _autoJump() {
    if (this.bioAction == "Walk" && moveSpeed > 0.06) {
        print("autoJump");
        var tempPOS: Vector3;
        tempPOS = Pick.transform.position;
        this.transform.position.y += 0.1;

    }
}
/*************************
 *_cubeHead 角色頭上旋轉旋轉
 ************************/
function _cubeHead() {
    Cube.transform.position.x = 0;
    Cube.transform.position.z = 0;
    Cube.transform.position.y = 299.8;
    //    Cube.transform.Rotate(Vector3.up * Time.deltaTime * 100.0, Space.World);
    Cube.GetComponent. < MeshFilter > ().mesh = Resources.Load('item/model/CUBE/' + maingameJS.cubeArrayTxt[handCube], Mesh);
    //    Cube.GetComponent. < MeshFilter > ().mesh = tempMesh;
}

function nextCube() {
    print("change");
    if (handCube == maingameJS.cubeArrayTxt.length - 1) {
        handCube = 0;
    } else {
        handCube++;
    }

}

function _createCube() {

    var tempPOS: Vector3 = pickTouchSide.transform.position;

    //給原本DQM操方式
    //if (maingameJS.checkArray(Vector3(tempPOS.x, tempPOS.y - 1, tempPOS.z)) == true ||
    //        maingameJS.checkArray(Vector3(tempPOS.x, tempPOS.y + 1, tempPOS.z)) == true ||
    //        maingameJS.checkArray(Vector3(tempPOS.x - 1, tempPOS.y, tempPOS.z)) == true ||
    //        maingameJS.checkArray(Vector3(tempPOS.x + 1, tempPOS.y, tempPOS.z)) == true || maingameJS.checkArray(Vector3(tempPOS.x, tempPOS.y, tempPOS.z - 1)) == true ||
    //        maingameJS.checkArray(Vector3(tempPOS.x, tempPOS.y, tempPOS.z + 1)) == true ||
    //        tempPOS.y == 0.5) {
    //        Cube.transform.position = tempPOS;
    //        var temp = Instantiate(Cube);
    //        temp.transform.eulerAngles = Vector3(-90, 0, 0);
    //        temp.GetComponent. < Renderer > ().enabled = true;
    //        temp.AddComponent(BoxCollider);
    //        temp.name = temp.transform.position.ToString("F0");
    //        //        maingameJS.setArray(temp.transform.position, float.parseFloat(maingameJS.cubeArrayTxt[handCube]));
    //        maingameJS.setArray(temp.transform.position, maingameJS.cubeArrayTxt[handCube]);
    //    }
    if (!maingameJS.checkArray(tempPOS)) {

        var temp = Instantiate(Cube);
        temp.tag = "Cube";
        temp.transform.eulerAngles = Vector3(-90, 0, 0);
        temp.transform.position = tempPOS;
        temp.GetComponent. < MeshRenderer > ().receiveShadows = true;
        temp.GetComponent. < Renderer > ().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        temp.GetComponent. < Renderer > ().enabled = true;
        temp.AddComponent(BoxCollider);
        temp.name = temp.transform.position.ToString("F0");
        //        maingameJS.setArray(temp.transform.position, float.parseFloat(maingameJS.cubeArrayTxt[handCube]));
        maingameJS.setArray(temp.transform.position, maingameJS.cubeArrayTxt[handCube]);
    }
}

function _removeCube() {

    var tempPOS: Vector3 = pickTouch.transform.position;
    maingameJS.removeArray(tempPOS);
    Destroy(GameObject.Find(tempPOS.ToString("F0")));

}

/*************************
 *_pick 類似DQM的操作模式
 ************************/
function _pick() {

    //正規化生物座標
    pickPlayer.transform.position.x = Mathf.Floor(this.transform.position.x + 0.5 / 1);
    pickPlayer.transform.position.z = Mathf.Floor(this.transform.position.z + 0.5 / 1);
    pickPlayer.transform.position.y = Mathf.Floor(this.transform.position.y + 0.5 / 1) + 0.5;

    //TODO:效能可以調整
    //將依據生物面相角度，將Pick放在角色正前方
    var tempInt = Mathf.Floor(this.transform.eulerAngles.y / 45);
    Pick.transform.position.y = pickPlayer.transform.position.y;
    switch (tempInt) {
    case 0:
        Pick.transform.position.x = pickPlayer.transform.position.x + 0;
        Pick.transform.position.z = pickPlayer.transform.position.z + 1;
        break;
    case 1:
        Pick.transform.position.x = pickPlayer.transform.position.x + 1;
        Pick.transform.position.z = pickPlayer.transform.position.z + 1;
        break;
    case 2:
        Pick.transform.position.x = pickPlayer.transform.position.x + 1;
        Pick.transform.position.z = pickPlayer.transform.position.z + 0;
        break;
    case 3:
        Pick.transform.position.x = pickPlayer.transform.position.x + 1;
        Pick.transform.position.z = pickPlayer.transform.position.z + -1;
        break;
    case 4:
        Pick.transform.position.x = pickPlayer.transform.position.x + 0;
        Pick.transform.position.z = pickPlayer.transform.position.z + -1;
        break;
    case 5:
        Pick.transform.position.x = pickPlayer.transform.position.x + -1;
        Pick.transform.position.z = pickPlayer.transform.position.z + -1;
        break;
    case 6:
        Pick.transform.position.x = pickPlayer.transform.position.x + -1;
        Pick.transform.position.z = pickPlayer.transform.position.z + 0;
        break;
    case 7:
        Pick.transform.position.x = pickPlayer.transform.position.x + -1;
        Pick.transform.position.z = pickPlayer.transform.position.z + 1;
        break;
    }

    //更新碰撞物狀態
    var g = 0;
    var tempVector3: Vector3 = GameObject.Find("pickPlayer").transform.position;
    for (var x = -1; x < 2; x++) {
        for (var y = -1; y < 2; y++) {
            for (var z = -1; z < 2; z++) {
                g++;
                if (maingameJS.checkArray(Vector3(tempVector3.x + x, tempVector3.y + y, tempVector3.z + z))) {

                    collisionCubes[g].transform.position = Vector3(tempVector3.x + x, tempVector3.y + y, tempVector3.z + z);
                }
            }
        }
    }


}

function _bioStatus() {
    //對應生物所處狀態，播放對應動作
    if (!anim.IsPlaying("Attack")) {
        switch (this.bioAction) {
        case "Action":
            anim.CrossFade("Attack");
            anim.CrossFadeQueued("Wait");
            _removeCube();
            break;
        case "Create":
            anim.CrossFade("Attack");
            anim.CrossFadeQueued("Wait");
            _createCube();
            break;
        case "Damage":
            //jump
            this.bioAction = 'Jump';
            break;
        case "Walk":
            anim.CrossFade("Walk");
            break;
        case "picking":
            break;
        case "Wait":
            anim.CrossFade("Wait");
            break;
        case "Jump":
            this.GetComponent. < Rigidbody > ().velocity.y = 5;
            onAir = true;
            break;
        }
    }
    if (anim.IsPlaying("Wait")) {
        this.bioAction = "Wait";
    }
}

function _movment() {


    //轉換sphere座標，轉換成螢幕座標
    if (maingameJS.clickStart && maingameJS.hitUIObjectName == 'movePlate') {
        Sphere.transform.position.x = maingameJS.mouseDragVector.x * 0.02;
        Sphere.transform.position.z = maingameJS.mouseDragVector.z * 0.02;

        //var tempAngel = maingameJS.cameraAngle;
        var tempAngel = Vector3.Angle(maingameJS.mainCamera.transform.forward, (Sphere.transform.position - this.transform.position));

        //角度轉徑度
        //弳度=角度*pi/180
        tempAngel = -maingameJS.mainCamera.transform.eulerAngles.y * Mathf.PI / 180;
        // x′=xcosθ-ysinθ,
        //y′=xsin⁡θ+ycos⁡θ....
        Sphere2.transform.position.x = Sphere.transform.position.x * Mathf.Cos(tempAngel) - Sphere.transform.position.z * Mathf.Sin(tempAngel) + this.transform.position.x;
        Sphere2.transform.position.z = Sphere.transform.position.x * Mathf.Sin(tempAngel) + Sphere.transform.position.z * Mathf.Cos(tempAngel) + this.transform.position.z;

    }

    //將生物移動向目標
    if (
        Vector3.Distance(this.transform.position, Sphere2.transform.position) > 0.5) {
        if (maingameJS.hitUIObjectName != 'movePlate') {
            Sphere2.GetComponent. < Renderer > ().enabled = true;
        }
        moveSpeed = moveSpeedMax;
        this.bioAction = "Walk";

        //依照目標距離調整移動速度
        if (maingameJS.hitUIObjectName ==
            'movePlate' && Vector3.Distance(this.transform.position, Sphere2.transform.position) < 5) {
            moveSpeed = moveSpeed * (Vector3.Distance(this.transform.position, Sphere2.transform.position) / 5);
        } else if (Vector3.Distance(this.transform.position, Sphere2.transform.position) < 1) {
            moveSpeed = moveSpeed * (Vector3.Distance(this.transform.position, Sphere2.transform.position) / 1.2);
        }

        //更新pick狀態
        _pick();

        //移動生物到目標點
        Sphere2.transform.position.y = this.transform.position.y;
        Sphere.transform.position.y = this.transform.position.y;
        this.transform.position = Vector3.MoveTowards(this.transform.position, Sphere2.transform.position, moveSpeed);
        //調整步伐
        anim["Walk"].speed = WalkSteptweek * moveSpeed;

    } else {
        Sphere2.GetComponent. < Renderer > ().enabled = false;
        if (this.bioAction == "Walk") {
            this.bioAction = "Wait";
        }
    }


    //將生物轉向目標
    if (this.bioAction == "Walk") {
        var targetDir = Sphere2.transform.position - this.transform.position;
        var step = rotateSpeed * Time.deltaTime;
        var newDir = Vector3.RotateTowards(this.transform.forward, targetDir, step, 0.0);
        this.transform.rotation = Quaternion.LookRotation(newDir);
    }
}

function AnimationClip() {
    var animationsName = [
            'Attack',
            'Damage',
            'Dead',
            'Wait',
            'Walk'
        ];
    var bioName = 'm101';
    for (var name: String in animationsName) {
        Debug.Log(name);
        var mdl: GameObject = Resources.Load("Biology/Animation/" + bioName + "@" + name);
        var anim: Animation = GameObject.Find(bioName).GetComponent. < Animation > ();
        var aClip = mdl.GetComponent. < Animation > ().clip;
        anim.AddClip(aClip, name);
    }
}
