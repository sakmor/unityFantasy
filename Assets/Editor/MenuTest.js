import System.IO;
import SimpleJSON;


// Add a menu item named "Do Something" to MyMenu in the menu bar.
@
MenuItem("==Menu==/saveGame")
static

function saveGame() {
    if (Application.isPlaying) {
        var tempJson: Color[] = PlayerPrefsX.GetColorArray("array3d");
        var Player = GameObject.Find("Cha_Knight");
        var json: String;
        json += '[';
        json += '{"length":' + tempJson.length + ',"playerX":' + Player.transform.position.x + ',"playerY":' + Player.transform.position.y + ',"playerZ":' + Player.transform.position.z + '},';
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
        Debug.Log('write end');

        // Create an instance of StreamWriter to write text to a file.
        sw = new StreamWriter("array3d.txt");
        sw.Write(json);
        sw.Close();

        // Add some text to the file.
        or = new StreamReader("tempJson.txt");
        or.Close();
    } else {
        UnityEditor.EditorUtility.DisplayDialog('oh,Come on ?', ' >_ < 請在播放模式使用該功能啦', '我明白了');
    }
}

@
MenuItem("==Menu==/saveGame", true)
static

function saveGameisPlay() {
    // Return false if no transform is selected.
    return Application.isPlaying;
}

@
MenuItem("==Menu==/LoadGame")
static

function LoadGame() {
    if (!Application.isPlaying) {
        or = new StreamReader("array3d.txt");
        var arrayText: String = or.ReadToEnd();
        var array3dLoadJson = JSON.Parse(arrayText);
        var Cube: GameObject = GameObject.Find("Cube");
        var Player = GameObject.Find("Cha_Knight");
        Player.transform.position.x = parseFloat(array3dLoadJson[0][1]);
        Player.transform.position.y = parseFloat(array3dLoadJson[0][2]);
        Player.transform.position.z = parseFloat(array3dLoadJson[0][3]);
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
    } else {
        UnityEditor.EditorUtility.DisplayDialog('oh,Come on ?', ' >_ < 請在編輯模式使用該功能啦', '我明白了');
    }
}

@
MenuItem("==Menu==/LoadGame", true)
static

function loadGameisPlay() {
    // Return false if no transform is selected.
    return !Application.isPlaying;
}

@
MenuItem("==Menu==/clearGame")
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
