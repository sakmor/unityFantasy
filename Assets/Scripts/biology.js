//pragma strict
@
script RequireComponent(Animation)

@script RequireComponent(Rigidbody)

@script RequireComponent(BoxCollider)

//class biologyInfo {
//    public
//    var name: String;
//    public
//    var walkStep: float;
//    public
//    var BoxCollider_CenterY: float;
//    public
//    var BoxCollider_SizeY: float;
//    public
//    var effectScale: float;
//    public static
//
//    function CreateFromJSON(jsonString: String) {
//        return JsonUtility.FromJson. < biologyInfo > (jsonString);
//    }
//}
//    var myJsonBiology: biologyInfo = new biologyInfo().CreateFromJSON(maingameJS.biologyList.text);
//    Debug.Log(myJsonBiology.name);

var attackDistance = 1.5;
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

var Sphere: Vector3;
var Sphere2: Vector3;
var Sphere3: GameObject;
var anim: Animation;
var WalkSteptweek: float;
private
var mainGame: GameObject;
private
var maingameJS: gameJS;
var pickPlayer: GameObject;

var lastAttackTime: float;
var collisionCubes: GameObject[];
var nameText: GameObject;
collisionCubes = new GameObject[28];

var nametextScreenPos: Vector3;
var targetName: String = "";
var startPos: Vector3;
var targetLine: GameObject;

//-------------------
//與AI相關的參數
var runBackDist = 15;
var runBack = false;
var seeMax = 10;
var catchSpeed = 0.05;
var attackCoolDown = 3000;
var dectefrequency = 1.00;
var bais: Number;
//-------------------
//Pick           --在玩家面前的選取框
//PickPlayer     --玩家所在的位置
//PickTouch      --滑鼠點選地面位置,或是點擊的Cube選取框


function Start() {
    bais = Mathf.Floor(Random.value * 10);
    nameText = Instantiate(GameObject.Find("nameText"));
    nameText.name = this.name + "_nameText";
    nameText.transform.parent = GameObject.Find("4-UI/Canvas").transform;

    nameText.GetComponent. < UnityEngine.UI.Text>().text = this.name;

    anim = GetComponent. < Animation>();
    bioAction = "Wait";
    handCube = 0;
    mainGame = GameObject.Find("mainGame");
    targetLine = Instantiate(GameObject.Find("targetLine"));
    targetLine.transform.parent = this.transform;
    Sphere3 = Instantiate(GameObject.Find("Sphere3"));
    Sphere3.name = this.name + '_Sphere3';
    Sphere3.transform.parent = GameObject.Find("Biology/Items").transform;
    Sphere3.transform.position = this.transform.position;
    Sphere3.transform.position.y = 1;
    maingameJS = GameObject.Find("mainGame").GetComponent(gameJS);
    pickPlayer = Instantiate(GameObject.Find("pickPlayer"));
    pickPlayer.transform.parent = GameObject.Find("Biology/Items").transform;
    pickPlayer.name = this.name + 'pickPlayer';
    _backward = false;
    WalkSteptweek = WalkSteptweek || 40;
    moveSpeed = moveSpeed || 0.09;
    moveSpeedMax = moveSpeed;
    rotateSpeed = rotateSpeed || 10;

    var tempVector3: Vector3 = pickPlayer.transform.position;
    var collisionCubeOBJ: GameObject;
    collisionCubeOBJ = new GameObject(this + '_collisionCubeOBJ');
    collisionCubeOBJ.transform.parent = GameObject.Find("Biology/Items").transform;
    //    collisionCubes.push(Instantiate(GameObject.Find("pickPlayer")));
    for (var i = 0; i <= 27; i++) {
        collisionCubes[i] = Instantiate(GameObject.Find("pickPlayer"));
        collisionCubes[i].name = 'dynamicCollision_' + i;
        collisionCubes[i].AddComponent(BoxCollider);
        collisionCubes[i].transform.parent = collisionCubeOBJ.transform;
        collisionCubes[i].GetComponent. < Renderer>().enabled = false;
    }
    startPos = this.transform.position;
    //更新pick狀態
    _pick();
    //抓取動作檔案
    AnimationClip();
    dynamicCollision();

}

function Update() {

    if (this.name == maingameJS.Player.name) {
        //        this._catchPlayer();
        this._movment();
        this._bioStatus();
    }
}

function updateUI() {
    nametextScreenPos = Camera.main.WorldToScreenPoint(Vector3(
        this.transform.position.x,
        this.transform.position.y + 2.5,
        this.transform.position.z));

    nameText.transform.position.x = nametextScreenPos.x;
    nameText.transform.position.y = nametextScreenPos.y;
    nameText.transform.position.z = nametextScreenPos.z;
}

function BioUpdate() {
    if (this.name != maingameJS.Player.name) {
        this._catchPlayer(5 + bais);
        this._movment();
        this._bioStatus();
    }
    //    this._autoJump();
}

function dynamicCollision() {
    //更新碰撞物狀態
    var g = 0;
    var tempVector3: Vector3 = pickPlayer.transform.position;
    for (var x = -1; x < 2; x++) {
        for (var y = -1; y < 2; y++) {
            for (var z = -1; z < 2; z++) {
                g++;
                if (maingameJS.checkArray(Vector3(tempVector3.x + x, tempVector3.y + y, tempVector3.z + z)) != false) {
                    collisionCubes[g].transform.position = Vector3(tempVector3.x + x, tempVector3.y + y, tempVector3.z + z);
                }
            }
        }
    }
}

/*************************
 *_pick 類似DQM的操作模式
 ************************/
function _pick() {

    //正規化生物座標
    pickPlayer.transform.position.x = Mathf.Floor(this.transform.position.x + 0.5);
    pickPlayer.transform.position.z = Mathf.Floor(this.transform.position.z + 0.5);
    pickPlayer.transform.position.y = Mathf.Floor(this.transform.position.y + 0.5) + 0.5;


}

function _bioStatus() {

    //對應生物所處狀態，播放對應動作
    if (!anim.IsPlaying("Attack")) {
        switch (this.bioAction) {
            case "Attack":
                anim.CrossFade("Attack");
                break;
            case "Damage":
                anim.CrossFade("Damage");
                break;
            case "Walk":
                anim.CrossFade("Walk");
                break;
            case "picking":
                break;
            case "Wait":
                anim.CrossFade("Wait");
                runBack = false;
                Sphere2 = this.transform.position;
                Sphere3.GetComponent. < Renderer>().enabled = false;
                break;
            case "Jump":
                this.GetComponent. < Rigidbody>().velocity.y = 5;
                break;
        }
    }
    if (anim.IsPlaying("Wait")) {
        this.bioAction = "Wait";
    }
}

function _movment() {

    var SphereDistance: float;

    if (this.bioAction == "Walk") {
        this.bioAction = "Wait";
    }

    //如果使用者操作搖桿
    if (maingameJS.clickStart &&
        maingameJS.hitUIObjectName == 'movePlate' &&
        maingameJS.Player.name == this.name) {

        this.bioAction = "Walk";

        //自搖桿取得的移動向量直
        Sphere.x = maingameJS.mouseDragVector.x * 0.02;
        Sphere.z = maingameJS.mouseDragVector.z * 0.02;

        //將spere2依據攝影機位置做座標轉換
        var tempAngel = Vector3.Angle(maingameJS.mainCamera.transform.forward, (Sphere - this.transform.position));
        tempAngel = -maingameJS.mainCamera.transform.eulerAngles.y * Mathf.PI / 180;
        Sphere2.x = Sphere.x * Mathf.Cos(tempAngel) - Sphere.z * Mathf.Sin(tempAngel) + this.transform.position.x;
        Sphere2.z = Sphere.x * Mathf.Sin(tempAngel) + Sphere.z * Mathf.Cos(tempAngel) + this.transform.position.z;
        Sphere2.y = this.transform.position.y;

        SphereDistance = Vector3.Distance(this.transform.position, Sphere2);
    }
    //如果使用者點擊螢幕操作
    else {
        if (maingameJS.Player.name == this.name) {
            Sphere3.GetComponent. < Renderer>().enabled = true;
        }
        SphereDistance = Vector3.Distance(this.transform.position, Sphere3.transform.position);
        var Sphere2Distance = Vector3.Distance(this.transform.position, Sphere2);
        if (SphereDistance > 0.05) {
            this.bioAction = "Walk";
            if (SphereDistance < 1) {
                Sphere2 = Sphere3.transform.position;
            } else {
                if (Sphere2Distance < 1) {
                    // Debug.Log("PathfindingCS");
                    Sphere2 = maingameJS.PathfindingCS.FindPath_Update(this.transform, Sphere3.transform);
                    if (Sphere2 == Vector3(-999, -999, -999)) {
                        maingameJS.logg("目前無法移動至該處");
                        Sphere2 = this.transform.position;
                        Sphere3.transform.position = this.transform.position;
                    }
                    // GameObject.Find("Sphere2").transform.position = Sphere2;
                    // GameObject.Find("Sphere2").transform.position.y = 1;
                }

            }
        }
    }

    //將生物轉向目標
    Sphere2.y = this.transform.position.y;
    Sphere.y = this.transform.position.y;
    var targetDir = Sphere2 - this.transform.position;
    var step = rotateSpeed * Time.deltaTime;
    var newDir = Vector3.RotateTowards(this.transform.forward, targetDir, step, 0.0);
    this.transform.rotation = Quaternion.LookRotation(newDir);

    //依照目標距離調整移動速度
    moveSpeed = moveSpeedMax;
    if (maingameJS.hitUIObjectName ==
        'movePlate' && SphereDistance < 5) {
        moveSpeed = moveSpeed * (SphereDistance / 5);
    }

    //更新pick狀態
    _pick();
    dynamicCollision();

    //移動生物到目標點
    this.transform.position = Vector3.MoveTowards(this.transform.position, Sphere2, moveSpeed * Time.deltaTime * 50);

    //調整步伐
    anim["Walk"].speed = WalkSteptweek * moveSpeed;

}

function AnimationClip() {
    var nameShort: String;
    var animationsName = [
        'Attack',
        'Damage',
        'Dead',
        'Wait',
        'Walk'
    ];
    var bioName: String = this.name;
    var bioFlodr: String;
    if (bioName[0] == 'm') {
        bioFlodr = 'Biology';
        nameShort = '' + bioName[0] + bioName[1] + bioName[2] + bioName[3];
    } else if (bioName[0] == 'C') {
        bioFlodr = 'char/' + bioName;
        nameShort = bioName;
    }

    for (var name: String in animationsName) {
        var mdl: GameObject = Resources.Load(bioFlodr + "/Animation/" + nameShort + "@" + name);
        var anim: Animation = this.GetComponent. < Animation>();
        var aClip = mdl.GetComponent. < Animation>().clip;
        anim.AddClip(aClip, name);
    }

    //讀取生物清單表
    var array3dLoadJson = Json.Deserialize(maingameJS.biologyList.text) as Dictionary. < String,
        System.Object>;
    this.WalkSteptweek = ((array3dLoadJson[nameShort]) as List. < System.Object>)[0];
    this.GetComponent. < BoxCollider>().center.y = ((array3dLoadJson[nameShort]) as List. < System.Object>)[1];
    this.GetComponent. < BoxCollider>().size.x = ((array3dLoadJson[nameShort]) as List. < System.Object>)[2];
    this.GetComponent. < BoxCollider>().size.y = ((array3dLoadJson[nameShort]) as List. < System.Object>)[3];
    this.GetComponent. < BoxCollider>().size.z = ((array3dLoadJson[nameShort]) as List. < System.Object>)[2];
    this.transform.localScale.x = ((array3dLoadJson[nameShort]) as List. < System.Object>)[4];
    this.transform.localScale.y = ((array3dLoadJson[nameShort]) as List. < System.Object>)[4];
    this.transform.localScale.z = ((array3dLoadJson[nameShort]) as List. < System.Object>)[4];

    this.GetComponent. < Rigidbody>().freezeRotation = true;
}

function _catchPlayer(n: float) {
    if (10 * Time.fixedTime % n < 0.05) {
        var playerDistance = Vector3.Distance(maingameJS.Player.transform.position, this.transform.position);
        var startPosDis = Vector3.Distance(this.transform.position, startPos);

        if (startPosDis > runBackDist) {
            _runback();
        }
        if (playerDistance < seeMax && !runBack) {

            if (targetName != maingameJS.Player.name) {
                targetName = maingameJS.Player.name;
                maingameJS.logg(this.name + "開始追擊你了");
            }
            if (playerDistance > attackDistance) {
                this.Sphere3.transform.position = maingameJS.Player.transform.position;
            }

            this.moveSpeedMax = catchSpeed;
            nameText.GetComponent. < UnityEngine.UI.Text>().color = Color.red;

            if (playerDistance < attackDistance) {
                if (Time.time * 1000 - lastAttackTime > attackCoolDown) {
                    lastAttackTime = Time.time * 1000;
                    this.Sphere3.transform.position = this.transform.position;
                    nameText.GetComponent. < UnityEngine.UI.Text>().color = Color.yellow;
                    bioAction = "Attack";
                    maingameJS.logg(this.name + "攻擊！");
                }
            }
        } else {
            if (targetName != "") {
                targetName = "";
                maingameJS.logg(this.name + "放棄追擊你");
                _runback();
            }
            nameText.GetComponent. < UnityEngine.UI.Text>().color = Color.white;
            //        this.Sphere3.transform.position = this.transform.position;
            //        this.Sphere3.transform.position.y = 1;
            //        maingameJS.logg("here");

        }


    }
}

function _runback() {
    runBack = true;
    this.Sphere3.transform.position = startPos;
}