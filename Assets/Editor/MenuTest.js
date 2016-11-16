import System.IO;
import SimpleJSON;

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
MenuItem("==Menu==/saveJson")

static

function saveJson() {
    var tempJson: Color[] = PlayerPrefsX.GetColorArray("array3d");
    var json: String;
    json += '[';
    json += '{"length":' + tempJson.length + '},';
    for (var i = 0; i < tempJson.length; i++) {

        if (i != 0) {
            json += ',';
        }
        json += '{';
        json += '"r":';
        json += tempJson[i].r;
        json += ',';

        json += '"g":';
        json += tempJson[i].g;
        json += ',';

        json += '"b":';
        json += tempJson[i].b;
        json += ',';

        json += '"a":';
        json += tempJson[i].a;
        json += '}';

    }
    json += ']';
    Debug.Log('last');
    Debug.Log('write end');

    // Create an instance of StreamWriter to write text to a file.
    sw = new StreamWriter("array3d.txt");
    sw.Write(json);
    sw.Close();

    // Add some text to the file.

    or = new StreamReader("tempJson.txt");
    //    or.Open();
    var arrayText: String = or.ReadToEnd();
    var luckArray = JSON.Parse(arrayText);
    or.Close();

}

@
MenuItem("==Menu==/loadJson")

static

function loadJson() {

    or = new StreamReader("array3d.txt");
    var arrayText: String = or.ReadToEnd();
    var array3dLoadJson = JSON.Parse(arrayText);
    var Cube: GameObject = GameObject.Find("Cube");
    Debug.Log(array3dLoadJson[0][0]);
    for (var i = 1; i < 1 + parseInt(array3dLoadJson[0][0]); i++) {
        if (GameObject.Find("(" + array3dLoadJson[i][0].ToString("F0") + ", " + array3dLoadJson[i][1].ToString("F0") + ", " + array3dLoadJson[i][2].ToString("F0") + ")") == null) {
            var temp = Instantiate(Cube);
            temp.GetComponent. < MeshRenderer > ().receiveShadows = true;
            temp.GetComponent. < Renderer > ().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            temp.GetComponent. < Renderer > ().enabled = true;
            //        temp.GetComponent. < MeshFilter > ().mesh = Resources.Load('item/model/CUBE/' + Path.GetFileNameWithoutExtension(cubeArrayTxt[array3dLoadJson[i][3]]), Mesh);
            temp.GetComponent. < MeshFilter > ().mesh = Resources.Load('item/model/CUBE/' + array3dLoadJson[i][3], Mesh);
            temp.GetComponent. < Renderer > ().enabled = true;
            temp.AddComponent(BoxCollider);
            //temp.name = "(" + array3dLoadJson[i][0] + ", " + array3dLoadJson[i][1]  + ", " + array3dLoadJson[i][2] + ")";

            temp.transform.position.x = parseFloat(array3dLoadJson[i][0]);
            temp.transform.position.y = parseFloat(array3dLoadJson[i][1]);
            temp.transform.position.z = parseFloat(array3dLoadJson[i][2]);
            temp.name = temp.transform.position.ToString("F0");
            //setArray(temp.transform.position, array3dLoadJson[i][3]);
        }
    }

    or.Close();

}

@
MenuItem("==Menu==/clear")
static

function clearCube() {
    or = new StreamReader("array3d.txt");
    var arrayText: String = or.ReadToEnd();
    var array3dclearJson = JSON.Parse(arrayText);
    for (var i = 1; i < 1 + parseInt(array3dclearJson[0][0]); i++) {
        var tempVector3: Vector3;
        tempVector3.x = parseFloat(array3dclearJson[i][0]);
        tempVector3.y = parseFloat(array3dclearJson[i][1]);
        tempVector3.z = parseFloat(array3dclearJson[i][2]);
        DestroyImmediate(GameObject.Find(tempVector3.ToString("F0")));
        Debug.Log(array3dclearJson[0][0]);
    }
    or.Close();
}
