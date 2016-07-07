// # pragma strict
var player: GameObject;
var Plane: GameObject;
var apple: GameObject;

//轉向目標使用
var speed: float;
var moveSpeed: float;

function Start() {
    player = GameObject.Find("Cha_Knight");
    Plane = GameObject.Find("Plane");
    apple = GameObject.Find("Sphere");
    speed = 8;
    moveSpeed = 0.02;

}

function Update() {
    contorlMangers();


}


/**
 * contorlMangers -輸入管理員
 * todo:
 * 	之後要獨立於另一個JS，並改為點擊地面移動
 * @return {[string]} keydown [return user input action]
 */
function contorlMangers() {

    //滑鼠點擊取得做標點
    var mouseHitPlane: RaycastHit;
    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    if (Physics.Raycast(ray, mouseHitPlane) && mouseHitPlane.transform.tag == "ground") {
        if (Input.GetMouseButtonDown(0)) {
            apple.transform.position = mouseHitPlane.point;
        }
    }


    //將生物轉向目標
    var targetDir = apple.transform.position - player.transform.position;
    var step = speed * Time.deltaTime;
    var newDir = Vector3.RotateTowards(player.transform.forward, targetDir, step, 0.0);
    player.transform.rotation = Quaternion.LookRotation(newDir);

    //將生物移動向目標
    player.transform.position = Vector3.MoveTowards(player.transform.position, apple.transform.position, moveSpeed);
    if (Vector3.Distance(player.transform.position, apple.transform.position) > 1) {
        player.GetComponent. < Animation > ().Play("Walk");
    }

    //使用WASD與上下左右鍵移動角色
    var keydown = "";
    if (Input.anyKey) {

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) {

            player.transform.Translate(0, 0, 1 * Time.deltaTime);

            keydown = "User typing W / UpArrow";
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) {
            player.transform.Translate(0, 0, -1 * Time.deltaTime);
            keydown = "User typing S / DownArrow";
        }
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) {
            player.transform.Translate(-1 * Time.deltaTime, 0, 0);
            keydown = "User typing A / LeftArrow";
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) {
            player.transform.Translate(1 * Time.deltaTime, 0, 0);
            keydown = "User typing D / RightArrow";
        }
        if (Input.GetKey(KeyCode.Space)) {
            apple.transform.position = player.transform.position;
            print("space");
            player.GetComponent. < Animation > ().Play("Attack");
            //            player.GetComponent. < Animation > ().PlayQueued("wait");
        } else {

        }
    } else {


    }
    return keydown;
}
