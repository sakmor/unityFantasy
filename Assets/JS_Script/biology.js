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
var Cube: GameObject;
var anim: Animation;
var WalkSteptweek: float;
var mainGame: GameObject;
var mainGamejs: gameJS;
var TextMesh: TextMesh;

function Start() {

    mainGame = GameObject.Find("mainGame");
    mainGamejs = GameObject.Find("mainGame").GetComponent(gameJS);

    _backward = false;
    WalkSteptweek = WalkSteptweek || 100;
    moveSpeed = moveSpeed || 0.08;
    moveSpeedMax = moveSpeed;
    rotateSpeed = rotateSpeed || 10;
    Cube = GameObject.Find("Cube");
    anim = this.GetComponent. < Animation > ();


}

function Update() {
    this._input();
    this._movment();
    this._animations();


    _pick();

}

function _input() {
    if (Input.anyKey) {
        _backward = false;
        if (Input.GetKey(KeyCode.Space)) {
            Sphere.transform.position = this.transform.position;
            this.bioAction = "Attack";
        }
        if (Input.GetKey(KeyCode.F)) {
            this.bioAction = "Damage";
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

function _crateCube() {
    var temp = Instantiate(Cube);
    temp.AddComponent(BoxCollider);
    temp.name = temp.transform.position.ToString("F0");
    mainGamejs.setArray(temp.transform.position);


}

function _removeCube() {
    //檢查下方是否有方塊
    var tempPOS: Vector3 = Cube.transform.position;
    var tempHight: int = 0;
    if (mainGamejs.checkArray(Vector3(tempPOS.x, tempPOS.y, tempPOS.z)) == true) {
        Destroy(GameObject.Find(tempPOS.ToString("F0")));
    }
}

function _pick() {
    //TODO:效能可以調整
    //將座標放在角色正前方
    Cube.transform.position.x = this.transform.position.x + transform.forward.x;
    Cube.transform.position.z = this.transform.position.z + transform.forward.z;

    //正規化座標位置
    Cube.transform.position.x = Mathf.Floor(Cube.transform.position.x / 1);
    Cube.transform.position.z = Mathf.Floor(Cube.transform.position.z / 1);

    //檢查下方是否有方塊
    var tempPOS: Vector3 = Cube.transform.position;
    tempPOS.y = 0;
    var tempHight: int = 0;
    for (var i: int = 0; i < 5; i++) {
        if (mainGamejs.checkArray(Vector3(tempPOS.x, tempPOS.y + i, tempPOS.z)) == true) {
            tempHight++;
        }
    }
    Cube.transform.position.y = tempHight + 0.5;
}

function _animations() {
    //對應生物所處狀態，播放對應動作
    if (this.bioAction != this._bioAction) {
        switch (this.bioAction) {
        case "Attack":
            anim.CrossFade("Attack");
            anim.CrossFadeQueued("Wait");
            _crateCube();
            break;
        case "Damage":
            //jump
            this.GetComponent. < Rigidbody > ().velocity.y = 5;
            _removeCube();
            break;
        case "Walk":
            anim.CrossFade("Walk");
            break;
        case "picking":
            break;
        case "Wait":
            Sphere.transform.position.x = this.transform.position.x;
            Sphere.transform.position.z = this.transform.position.z;
            anim.CrossFade("Wait");
            break;
        }
        this._bioAction = this.bioAction;
    }
    //若生物不是撥特定動作時，恢復Wait狀態
    if (!anim.IsPlaying("Attack")) {
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
    } else {
        if (this.bioAction != "Attack" && this.bioAction != "Damage") {
            this.bioAction = "Wait";
        }
    }

    //移動生物到目標點
    this.transform.position = Vector3.MoveTowards(this.transform.position, Sphere.transform.position, moveSpeed);

    //調整步伐
    anim["Walk"].speed = WalkSteptweek * moveSpeed;

    //將生物轉向目標
    if (!_backward) {
        var targetDir = Sphere.transform.position - this.transform.position;
        var step = rotateSpeed * Time.deltaTime;
        var newDir = Vector3.RotateTowards(this.transform.forward, targetDir, step, 0.0);
        this.transform.rotation = Quaternion.LookRotation(newDir);
    }
}
