// #pragma strict
var moveSpeed: float;

var moveSpeedMax: float;
var rotateSpeed: float;
var bioAction: String;

private
var _bioAction: String;
private
var _backward: boolean;

var Sphere: GameObject;
var Pick: GameObject;
var Cube: GameObject;
var anim: Animation;
var WalkSteptweek: float;
private
var mainGame: GameObject;
private
var mainGamejs: gameJS;
var TextMesh: TextMesh;
var pickPlayer: GameObject;

var Plane_touch: GameObject;

function Start() {
    Plane_touch = GameObject.Find("Plane_touch");
    pickPlayer = GameObject.Find("pickPlayer");
    mainGame = GameObject.Find("mainGame");
    mainGamejs = GameObject.Find("mainGame").GetComponent(gameJS);

    _backward = false;
    WalkSteptweek = WalkSteptweek || 100;
    moveSpeed = moveSpeed || 0.08;
    moveSpeedMax = moveSpeed;
    rotateSpeed = rotateSpeed || 10;
    Pick = GameObject.Find("pick");
    Cube = GameObject.Find("Cube");
    Cube.GetComponent. < Renderer > ().enabled = false;
    anim = this.GetComponent. < Animation > ();


}

function Update() {
    this._input();
    this._movment();
    this._animations();
    this._autoJump();
    _pick();
    Plane_touch.transform.position.y = this.transform.position.y - 0.2;
}

function _autoJump() {
    if (this.bioAction == "Walk" && moveSpeed > 0.04) {

        var tempPOS: Vector3;
        tempPOS = Pick.transform.position;
        //        tempPOS.y = Pick.transform.position.y;
        //        tempPOS.y = this.transform.position.y;
        //        tempPOS.z = Pick.transform.position.z;

        if ((Pick.transform.position.y - this.transform.position.y) > 0.6) {
            print(Pick.transform.position.y - this.transform.position.y);

            this.transform.position.y += 0.05;
        }
        if (mainGamejs.checkArray(Vector3(tempPOS.x, tempPOS.y, tempPOS.z)) == true) {
            this.transform.position.y += 0.05;
        }
    }
}

function _input() {
    if (Input.anyKey) {
        _backward = false;
        if (Input.GetKey(KeyCode.Space)) {
            Sphere.transform.position = this.transform.position;
            this.bioAction = "Attack";
        }
        if (Input.GetKey(KeyCode.F)) {
            this.bioAction = "Jump";

        }
        if (Input.GetKey(KeyCode.A)) {
            transform.Rotate(0, -3, 0);
        }
        if (Input.GetKey(KeyCode.D)) {
            transform.Rotate(0, 3, 0);
        }
        if (Input.GetKey(KeyCode.W)) {
            Sphere.transform.position.x = this.transform.position.x + transform.forward.x * 2.5;
            Sphere.transform.position.z = this.transform.position.z + transform.forward.z * 2.5;
        }
        if (Input.GetKey(KeyCode.S)) {
            _backward = true;
            Sphere.transform.position.x = this.transform.position.x - transform.forward.x * 2.5;
            Sphere.transform.position.z = this.transform.position.z - transform.forward.z * 2.5;
        }
    }


}

function _createCube() {

    var tempPOS: Vector3 = Pick.transform.position;
    if (mainGamejs.checkArray(Vector3(tempPOS.x, tempPOS.y, tempPOS.z)) == true) {
        mainGamejs.removeArray(tempPOS);
        Destroy(GameObject.Find(tempPOS.ToString("F0")));
    } else {
        if (mainGamejs.checkArray(Vector3(tempPOS.x, tempPOS.y - 1, tempPOS.z)) == true ||
            mainGamejs.checkArray(Vector3(tempPOS.x, tempPOS.y + 1, tempPOS.z)) == true ||
            mainGamejs.checkArray(Vector3(tempPOS.x - 1, tempPOS.y, tempPOS.z)) == true ||
            mainGamejs.checkArray(Vector3(tempPOS.x + 1, tempPOS.y, tempPOS.z)) == true || mainGamejs.checkArray(Vector3(tempPOS.x, tempPOS.y, tempPOS.z - 1)) == true ||
            mainGamejs.checkArray(Vector3(tempPOS.x, tempPOS.y, tempPOS.z + 1)) == true || Pick.transform.position.y == 0.5) {
            Cube.transform.position = Pick.transform.position;
            var temp = Instantiate(Cube);
            temp.GetComponent. < Renderer > ().enabled = true;
            temp.AddComponent(BoxCollider);
            temp.name = temp.transform.position.ToString("F0");
            mainGamejs.setArray(temp.transform.position);
            print(JsonUtility.ToJson(this));
        }
    }
}


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


    //如果生物腳下有方塊，且pick底下正好為空時
    var temp: Vector3;
    temp = pickPlayer.transform.position;
    temp.y -= 1;
    if (mainGamejs.checkArray(temp) &&
        !mainGamejs.checkArray(Vector3(Pick.transform.position.x, Pick.transform.position.y - 1, Pick.transform.position.z))) {
        Pick.transform.position.y -= 1;
    }

}

function _animations() {
    //對應生物所處狀態，播放對應動作
    if (!anim.IsPlaying("Attack")) {
        switch (this.bioAction) {
        case "Attack":
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
            this.bioAction = "Wait";
            break;
        }
    }
    if (anim.IsPlaying("Wait") &&
        this.bioAction == "Attack") {
        this.bioAction = "Wait";
    }
}

function _movment() {

    //將生物移動向目標
    if (Vector3.Distance(this.transform.position, Sphere.transform.position) > 0.5) {

        moveSpeed = moveSpeedMax;
        this.bioAction = "Walk";

        //依照目標距離調整移動速度
        if (Vector3.Distance(this.transform.position, Sphere.transform.position) < 5) {
            moveSpeed = moveSpeed * (Vector3.Distance(this.transform.position, Sphere.transform.position) / 5);
            if (moveSpeed < 0.02) {
                moveSpeed = 0;
            }
        }

        //移動生物到目標點
        Sphere.transform.position.y = this.transform.position.y;
        this.transform.position = Vector3.MoveTowards(this.transform.position, Sphere.transform.position, moveSpeed);

        //調整步伐
        anim["Walk"].speed = WalkSteptweek * moveSpeed;

    } else {

        if (this.bioAction == "Walk") {
            this.bioAction = "Wait";
        }
    }


    //將生物轉向目標
    if (this.bioAction == "Walk") {
        var targetDir = Sphere.transform.position - this.transform.position;
        var step = rotateSpeed * Time.deltaTime;
        var newDir = Vector3.RotateTowards(this.transform.forward, targetDir, step, 0.0);
        this.transform.rotation = Quaternion.LookRotation(newDir);
    }
}
