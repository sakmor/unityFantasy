import System.IO;
import MiniJSON;
import System.Collections.Generic;

// Add a menu item named "Do Something" to MyMenu in the menu bar.
@
MenuItem("==Menu==/saveGame")
static

function saveGame() {
    if (Application.isPlaying) {
        var tempJson: Color[] = PlayerPrefsX.GetColorArray("array3d");
        var Player = GameObject.Find("Cha_Knight");
        var json: String;
        json += '{';
        for (var i = 0; i < tempJson.length; i++) {

            if (i != 0) {
                json += ',';
            }
            json += '"' + i + '":[';
            json += tempJson[i].r;
            json += ',';
            json += tempJson[i].g;
            json += ',';
            json += tempJson[i].b;
            json += ',';
            json += tempJson[i].a;
            json += ']';

        }
        json += ',"length":' + (tempJson.length - 1) + '}';
        Debug.Log('write end');

        // Create an instance of StreamWriter to write text to a file.
        sw = new StreamWriter("array3dictionary.txt");
        sw.Write(json);
        sw.Close();


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

        or = new StreamReader("array3dictionary.txt");
        var arrayText: String = or.ReadToEnd();
        var array3dLoadJson = Json.Deserialize(arrayText) as Dictionary. < String,
            System.Object > ;
        var Cube: GameObject = GameObject.Find("Cube");
        var Player = GameObject.Find("Cha_Knight");
        //        Player.transform.position.x = parseFloat(array3dLoadJson[0][1]);
        //        Player.transform.position.y = parseFloat(array3dLoadJson[0][2]);
        //        Player.transform.position.z = parseFloat(array3dLoadJson[0][3]);
        for (var i = 1; i < array3dLoadJson["length"]; i++) {
            var tempColor: Color;
            tempColor.r = ((array3dLoadJson[i.ToString()]) as List. < System.Object > )[0];
            tempColor.g = ((array3dLoadJson[i.ToString()]) as List. < System.Object > )[1];
            tempColor.b = ((array3dLoadJson[i.ToString()]) as List. < System.Object > )[2];
            tempColor.a = ((array3dLoadJson[i.ToString()]) as List. < System.Object > )[3];
            Debug.Log('Color:' + tempColor);
            if (GameObject.Find("(" + tempColor.r.ToString("F0") + ", " + tempColor.g.ToString("F0") + ", " + tempColor.b.ToString("F0") + ")") == null) {
                var temp = Instantiate(Cube);
                temp.GetComponent. < MeshRenderer > ().receiveShadows = true;
                temp.GetComponent. < Renderer > ().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                temp.GetComponent. < Renderer > ().enabled = true;
                //        temp.GetComponent. < MeshFilter > ().mesh = Resources.Load('item/model/CUBE/' + Path.GetFileNameWithoutExtension(cubeArrayTxt[array3dLoadJson[i][3]]), Mesh);
                temp.GetComponent. < MeshFilter > ().mesh = Resources.Load('item/model/CUBE/' + tempColor.a, Mesh);
                temp.GetComponent. < Renderer > ().enabled = true;
                temp.AddComponent(BoxCollider);
                //temp.name = "(" + array3dLoadJson[i][0] + ", " + array3dLoadJson[i][1]  + ", " + array3dLoadJson[i][2] + ")";

                temp.transform.position.x = tempColor.r;
                temp.transform.position.y = tempColor.g;
                temp.transform.position.z = tempColor.b;
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
    or = new StreamReader("array3dictionary.txt");
    var arrayText: String = or.ReadToEnd();
    var array3dLoadJson = Json.Deserialize(arrayText) as Dictionary. < String,
        System.Object > ;
    for (var i = 1; i < array3dLoadJson["length"]; i++) {
        var temp: Vector3;
        var tempColor: Color;
        tempColor.r = ((array3dLoadJson[i.ToString()]) as List. < System.Object > )[0];
        tempColor.g = ((array3dLoadJson[i.ToString()]) as List. < System.Object > )[1];
        tempColor.b = ((array3dLoadJson[i.ToString()]) as List. < System.Object > )[2];
        tempColor.a = ((array3dLoadJson[i.ToString()]) as List. < System.Object > )[3];
        temp.x = tempColor.r;
        temp.y = tempColor.g;
        temp.z = tempColor.b;
        DestroyImmediate(GameObject.Find(temp.ToString("F0")));
    }
    Debug.Log('clear cubs: ' + array3dLoadJson["length"]);
    or.Close();
}
