// #pragma strict
var speed: float;
var moveSpeed: float;
var rotateSpeed: float;
var bioAction: String;
var Sphere: GameObject;

function Start() {

    moveSpeed = moveSpeed || 0.01;
    speed = speed || 8;
}

function Update() {
    this._movment();
    this._animations();

}

function _animations() {
    //撠??????拇??????嚗??剜?曉?????雿?
    var nowAnimation: AnimationState;
    switch (this.bioAction) {
    case "attack":
        this.GetComponent. < Animation > ().Play("Attack");
        break;
    case "hurt":
        break;
    case "Walk":
        this.GetComponent. < Animation > ().Play("Walk");
        break;
    case "picking":
        break;
    case "Wait":
        this.GetComponent. < Animation > ().Play("Wait");
        break;
    }
}

function _movment() {
    //將生物移動向目標
    this.transform.position = Vector3.MoveTowards(this.transform.position, Sphere.transform.position, moveSpeed);
    if (Vector3.Distance(this.transform.position, Sphere.transform.position) > 1) {
        this.bioAction = "Walk";
    } else {
        this.bioAction = "Wait";
    }

    //將生物轉向目標
    var targetDir = Sphere.transform.position - this.transform.position;
    var step = speed * Time.deltaTime;
    var newDir = Vector3.RotateTowards(this.transform.forward, targetDir, step, 0.0);
    this.transform.rotation = Quaternion.LookRotation(newDir);
}
