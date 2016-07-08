// #pragma strict
private
var moveSpeed: float;
var moveSpeedMax: float;
var rotateSpeed: float;
var bioAction: String;
private
var _bioAction: String;
var Sphere: GameObject;
var Cube: GameObject;
var anim: Animation;
var WalkSteptweek: float;


function Start() {
    WalkSteptweek = WalkSteptweek || 50;
    moveSpeed = moveSpeed || 0.07;
    moveSpeedMax = moveSpeed;
    rotateSpeed = rotateSpeed || 10;
    Cube = GameObject.Find("Cube");
    anim = this.GetComponent. < Animation > ();

}

function Update() {
    this._input();
    this._movment();
    this._animations();

}

function _putCube() {
    this.transform.position = Cube.transForm.postion;

}

function _input() {
    if (Input.GetKey(KeyCode.Space)) {
        Sphere.transform.position = this.transform.position;
        this.bioAction = "Attack";
    }
    if (Input.GetKey(KeyCode.F)) {
        Sphere.transform.position = this.transform.position;
        this.bioAction = "Damage";
    }
}

function _Attack() {
    print('Attack');

}

function _Damage() {


}

function _animations() {
    //對應生物所處狀態，播放對應動作
    if (this.bioAction != this._bioAction) {
        switch (this.bioAction) {
        case "Attack":
            anim.CrossFade("Attack");
            anim.CrossFadeQueued("Wait");
            this.bioAction = "Wait";
            _Attack();
            break;
        case "Damage":
            anim.CrossFade("Dead");
            anim.CrossFadeQueued("Wait");
            this.bioAction = "Wait";
            _Damage();
            break;
        case "Walk":
            anim.CrossFade("Walk");
            break;
        case "picking":
            break;
        case "Wait":
            anim.CrossFade("Wait");
            break;
        }

        this._bioAction = this.bioAction;
    }
}

function _movment() {

    //將生物移動向目標
    if (Vector3.Distance(this.transform.position, Sphere.transform.position) > 0.5) {
        moveSpeed = moveSpeedMax;
        this.bioAction = "Walk";
        //依照目標距離調整移動速度
        if (Vector3.Distance(this.transform.position, Sphere.transform.position) < 1.5) {
            moveSpeed = moveSpeed * (Vector3.Distance(this.transform.position, Sphere.transform.position) / 1.5);
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
    var targetDir = Sphere.transform.position - this.transform.position;
    var step = rotateSpeed * Time.deltaTime;
    var newDir = Vector3.RotateTowards(this.transform.forward, targetDir, step, 0.0);
    this.transform.rotation = Quaternion.LookRotation(newDir);

}
