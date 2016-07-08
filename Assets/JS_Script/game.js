// # pragma strict
var Plane: GameObject;
var Sphere: GameObject;
var Player: GameObject;

function Start() {

    Plane = GameObject.Find("Plane");
    Sphere = GameObject.Find("Sphere");
    Player = GameObject.Find("Cha_Knight");
    Player.AddComponent(biology);
    Player.GetComponent(biology).Sphere = Sphere;

}

function Update() {
    getMousehitGroupPos();

}
//========================================================

function getMousehitGroupPos() {
    //滑鼠點擊取得做標點
    var mouseHitPlane: RaycastHit;
    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    if (Physics.Raycast(ray, mouseHitPlane) && mouseHitPlane.transform.tag == "ground") {
        if (Input.GetMouseButtonDown(0)) {
            Sphere.transform.position = mouseHitPlane.point;
        }
    }
}
