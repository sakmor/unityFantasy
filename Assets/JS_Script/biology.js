// #pragma strict
var moveSpeed: float;
var moveSpeedMax: float;
var rotateSpeed: float;
var rotateSpeedMax: float;
var bioAction: String;
var Sphere: GameObject;
var anim: Animation;
var WalkSteptweek: float;


function Start() {
    WalkSteptweek = WalkSteptweek || 50;
    moveSpeed = moveSpeed || 0.1;
    moveSpeedMax = moveSpeed;
    rotateSpeed = rotateSpeed || 10;
    rotateSpeedMax = rotateSpeed;
    anim = this.GetComponent. < Animation > ();

}

function Update() {
    this._movment();
    this._animations();
}



function _animations() {
    //對應生物所處狀態，播放對應動作
    switch (this.bioAction) {
    case "attack":
        break;
    case "hurt":
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
        this.bioAction = "Wait";
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

    //    print(Vector3.Angle(targetDir, this.transform.forward));
}
