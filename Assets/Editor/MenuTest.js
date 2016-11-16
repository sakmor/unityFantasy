// Add a menu item named "Do Something" to MyMenu in the menu bar.
@
MenuItem("==Menu==/loadGame")
static
function DoSomething() {
    var Cube: GameObject = GameObject.Find("Cube");
    var array3dLoad: Color[] = PlayerPrefsX.GetColorArray("array3d");
    for (var i = 0; i < array3dLoad.length; i++) {
        if (GameObject.Find("(" + array3dLoad[i].r.ToString("F0") + ", " + array3dLoad[i].g.ToString("F0") + ", " + array3dLoad[i].b.ToString("F0") + ")") == null) {
            var temp = Instantiate(Cube);
            temp.GetComponent. < MeshRenderer > ().receiveShadows = true;
            temp.GetComponent. < Renderer > ().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            temp.GetComponent. < Renderer > ().enabled = true;
            //        temp.GetComponent. < MeshFilter > ().mesh = Resources.Load('item/model/CUBE/' + Path.GetFileNameWithoutExtension(cubeArrayTxt[array3dLoad[i].a]), Mesh);
            temp.GetComponent. < MeshFilter > ().mesh = Resources.Load('item/model/CUBE/' + array3dLoad[i].a, Mesh);
            temp.GetComponent. < Renderer > ().enabled = true;
            temp.AddComponent(BoxCollider);
            temp.name = "(" + array3dLoad[i].r.ToString("F0") + ", " + array3dLoad[i].g.ToString("F0") + ", " + array3dLoad[i].b.ToString("F0") + ")";
            temp.transform.position.x = array3dLoad[i].r;
            temp.transform.position.y = array3dLoad[i].g;
            temp.transform.position.z = array3dLoad[i].b;
            //setArray(temp.transform.position, array3dLoad[i].a);
        }
    }
}

@
MenuItem("==Menu==/sayHello")
static
function deleteMap() {
    Debug.Log(GameObject.Find("Cube"));
}
