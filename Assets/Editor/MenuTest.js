import System.IO;
import MiniJSON;
import System.Collections.Generic;

// Add a menu item named "Do Something" to MyMenu in the menu bar.
@
MenuItem("==Menu==/saveGame")
static

function saveGame() {
    var json: String;
    var respawnPrefab: GameObject;
    var respawns: GameObject[];
    var Player = GameObject.Find("Cha_Knight");
    respawns = GameObject.FindGameObjectsWithTag("Cube");
    var step = 0;
    json = '{';
    for (var respawn: GameObject in respawns) {


        if (step != 0) {
            json += ',';
        }
        respawn.transform.position.x = Mathf.Floor(respawn.transform.position.x + 0.5);
        respawn.transform.position.z = Mathf.Floor(respawn.transform.position.z + 0.5);
        respawn.transform.position.y = Mathf.Floor(respawn.transform.position.y) + 0.5;
        respawn.name = respawn.transform.position.ToString("F0");
        json += '"' + step + '":[';
        json += respawn.transform.position.x;
        json += ',';
        json += respawn.transform.position.y;
        json += ',';
        json += respawn.transform.position.z;
        json += ',';
        json += respawn.GetComponent. < MeshFilter > ().sharedMesh.name[0];
        json += respawn.GetComponent. < MeshFilter > ().sharedMesh.name[1];
        json += respawn.GetComponent. < MeshFilter > ().sharedMesh.name[2];
        json += respawn.GetComponent. < MeshFilter > ().sharedMesh.name[3];
        json += respawn.GetComponent. < MeshFilter > ().sharedMesh.name[4];
        json += ']';
        step++;
    }
    json += ',';
    json += '"playerPos": [';
    json += Player.transform.position.x;
    json += ',';
    json += Player.transform.position.y;
    json += ',';
    json += Player.transform.position.z;
    json += ']';
    json += ',';

    json += '"length":' + (step) + '}';
    if (step != 1) {
        UnityEditor.EditorUtility.DisplayDialog('Save End ', ' --=== Save End ===--', '[OK]');
    }
    step = 0;

    sw = new StreamWriter("array3dictionary.txt");
    sw.Write(json);
    sw.Close();

}

@
MenuItem("==Menu==/LoadGame")
static

function LoadGame() {
    if (!Application.isPlaying) {
        var Cubes = GameObject.Find("Cubes");
        or = new StreamReader("array3dictionary.txt");
        var arrayText: String = or.ReadToEnd();
        var array3dLoadJson = Json.Deserialize(arrayText) as Dictionary. < String,
            System.Object > ;
        var Player = GameObject.Find("Cha_Knight");
        //        Player.transform.position.x = parseFloat(array3dLoadJson[0][1]);
        //        Player.transform.position.y = parseFloat(array3dLoadJson[0][2]);
        //        Player.transform.position.z = parseFloat(array3dLoadJson[0][3]);
        for (var i = 1; i < array3dLoadJson["length"]; i++) {
            Debug.Log("load" + i);
            var tempColor: Color;
            tempColor.r = ((array3dLoadJson[i.ToString()]) as List. < System.Object > )[0];
            tempColor.g = ((array3dLoadJson[i.ToString()]) as List. < System.Object > )[1];
            tempColor.b = ((array3dLoadJson[i.ToString()]) as List. < System.Object > )[2];
            tempColor.a = ((array3dLoadJson[i.ToString()]) as List. < System.Object > )[3];
            Debug.Log('Color:' + tempColor);
            if (GameObject.Find("(" + tempColor.r.ToString("F0") + ", " + tempColor.g.ToString("F0") + ", " + tempColor.b.ToString("F0") + ")") == null) {
                var temp = Instantiate(GameObject.Find("Cube"));
                temp.transform.parent = Cubes.transform;
                temp.tag = "Cube";
                temp.GetComponent. < MeshRenderer > ().receiveShadows = true;
                temp.GetComponent. < Renderer > ().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                temp.GetComponent. < Renderer > ().enabled = true;
                //        temp.GetComponent. < MeshFilter > ().mesh = Resources.Load('item/model/CUBE/' + Path.GetFileNameWithoutExtension(cubeArrayTxt[array3dLoadJson[i][3]]), Mesh);
                temp.GetComponent. < MeshFilter > ().mesh = Resources.Load('item/model/CUBE/' + tempColor.a, Mesh);
                temp.GetComponent. < Renderer > ().enabled = true;
                //                temp.AddComponent(BoxCollider);
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


@
MenuItem("==Menu==/Normalized")
static

function Normalized() {
    var respawns: GameObject[];
    respawns = Selection.gameObjects;
    if (respawns.length == 1) {
        UnityEditor.EditorUtility.DisplayDialog('oh,Come on ?', ' Select GameObject First!', 'OK');
    }
    var json: String;
    json = '{';
    var step = 0;
    for (var respawn: GameObject in respawns) {
        if (step != 0) {
            json += ',';
        }
        if (respawn.tag == "Cube") {
            respawn.transform.position.x = Mathf.Floor(respawn.transform.position.x + 0.5);
            respawn.transform.position.z = Mathf.Floor(respawn.transform.position.z + 0.5);
            respawn.transform.position.y = Mathf.Floor(respawn.transform.position.y) + 0.5;
            respawn.transform.eulerAngles = Vector3(-90, 0, 0);
            respawn.name = respawn.transform.position.ToString("F0");
        }
    }
    var ss = 0;
    respawns = Selection.gameObjects;
    for (var respawnA: GameObject in respawns) {
        for (var respawnB: GameObject in respawns) {
            if (respawnB != respawnA) {
                if (respawnB.name == respawnA.name) {
                    DestroyImmediate(respawnB);
                    ss++;
                }
            }
        }
    }

    UnityEditor.EditorUtility.DisplayDialog('Normalized End ', ' --=== Normalized End ===--', 'OK');
    Debug.Log("Normalized:" + step);
    Debug.Log("DestroyImmediate:" + ss);

}@
MenuItem("==Menu==/detectOcclusion")
static

function detectOcclusion() {
    var dictionary3d: Dictionary. < Vector3, int > =
        new Dictionary. < Vector3,
        int > ();
    or = new StreamReader("array3dictionary.txt");
    var arrayText: String = or.ReadToEnd();
    var array3dLoadJson = Json.Deserialize(arrayText) as Dictionary. < String,
        System.Object > ;

    //建立目錄dictionary3d
    for (var i = 1; i < array3dLoadJson["length"]; i++) {
        var tempColor: Vector3;
        tempColor.x = ((array3dLoadJson[i.ToString()]) as List. < System.Object > )[0];
        tempColor.y = ((array3dLoadJson[i.ToString()]) as List. < System.Object > )[1];
        tempColor.z = ((array3dLoadJson[i.ToString()]) as List. < System.Object > )[2];
        dictionary3d[tempColor] = 1;
    }

    //透過目錄檢查是否Cube是完全封閉
    var os = 0;
    for (var j = 1; j < array3dLoadJson["length"]; j++) {
        tempColor.x = ((array3dLoadJson[j.ToString()]) as List. < System.Object > )[0];
        tempColor.y = ((array3dLoadJson[j.ToString()]) as List. < System.Object > )[1];
        tempColor.z = ((array3dLoadJson[j.ToString()]) as List. < System.Object > )[2];
        var tempA: Vector3 = Vector3(tempColor.x + 1, tempColor.y, tempColor.z);
        var tempB: Vector3 = Vector3(tempColor.x - 1, tempColor.y, tempColor.z);
        var tempC: Vector3 = Vector3(tempColor.x, tempColor.y + 1, tempColor.z);
        var tempD: Vector3 = Vector3(tempColor.x, tempColor.y - 1, tempColor.z);
        var tempE: Vector3 = Vector3(tempColor.x, tempColor.y, tempColor.z + 1);
        var tempF: Vector3 = Vector3(tempColor.x, tempColor.y, tempColor.z - 1);
        if (dictionary3d.ContainsKey(tempA) &&
            dictionary3d.ContainsKey(tempB) &&
            dictionary3d.ContainsKey(tempC) &&
            dictionary3d.ContainsKey(tempD) &&
            dictionary3d.ContainsKey(tempE) &&
            dictionary3d.ContainsKey(tempF)) {

            DestroyImmediate(GameObject.Find(tempColor.ToString("F0")));
            os++;

        }
    }
    Debug.Log('detectOcclusion Over: ' + os);
}

@
MenuItem("==Menu==/loadJson")
static

function loadJson() {


    or = new StreamReader("Assets/Resources/db/biologyList.json");
    var loadJsonString: String = or.ReadToEnd();
    or.Close();
    Debug.Log(loadJsonString);
    or.Close();
}
