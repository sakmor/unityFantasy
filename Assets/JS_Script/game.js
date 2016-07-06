// # pragma strict
var player: GameObject;
var Plane: GameObject;
var apple: GameObject;

//轉向目標使用
var speed: float;

function Start() {
    player = GameObject.Find("Cha_Knight");
    Plane = GameObject.Find("Plane");
    apple = GameObject.Find("Sphere");
    speed = 5;

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
    if (Physics.Raycast(ray, mouseHitPlane)) {
        if (Input.GetMouseButtonDown(0)) {
            apple.transform.position = mouseHitPlane.point;
        }
    }


    //轉向目標

    var targetDir = apple.transform.position - player.transform.position;
    var step = speed * Time.deltaTime;
    var newDir = Vector3.RotateTowards(player.transform.forward, targetDir, step, 0.0);
    player.transform.rotation = Quaternion.LookRotation(newDir);


    var keydown = "";
    if (Input.anyKey) {

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) {

            player.transform.Translate(0, 0, 1 * Time.deltaTime);
            player.GetComponent. < Animation > ().Play("Walk");
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
            print("space");
            player.GetComponent. < Animation > ().Play("Attack");
            //            player.GetComponent. < Animation > ().PlayQueued("wait");
        } else {

        }
    } else {


    }
    return keydown;
}
