using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class editorMenuCS : MonoBehaviour
{
    public class biologyList
    {
        public List<string> drawNumber = new List<string>();
        public List<float> biodata = new List<float>();
    }
    // [Serializable]
    public class scene
    {
        public int length;
        public Vector3 playerPos;
        public List<float> cubeArray = new List<float>();
    }


    [MenuItem("[Menu]/讀取地圖")]
    static void LoadJson()
    {
        UnityEditor.AssetDatabase.Refresh();

        //清除場景
        clearMap();

        //讀取json檔案
        TextAsset json = Resources.Load("scene/s998") as TextAsset;
        scene scene = new scene();
        scene = JsonUtility.FromJson<scene>(json.text);


        //產生場景
        GameObject Cubes = GameObject.Find("Cubes");


        for (var i = 0; i < scene.cubeArray.Count; i += 5)
        {
            GameObject temp = Instantiate(GameObject.Find("Cube"));//todo:Cube可以不需要用Find的方式
            Vector3 tempVector3 = new Vector3(scene.cubeArray[i], scene.cubeArray[i + 1], scene.cubeArray[i + 2]);
            temp.transform.parent = Cubes.transform;
            temp.transform.position = tempVector3;
            temp.name = temp.transform.position.ToString("F1");
            temp.GetComponent<MeshRenderer>().receiveShadows = true;
            temp.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            temp.GetComponent<Renderer>().enabled = true;
            Mesh mesh = (Mesh)Resources.Load("item/model/CUBE/" + scene.cubeArray[i + 3], typeof(Mesh));
            temp.GetComponent<MeshFilter>().mesh = mesh;
            temp.GetComponent<Renderer>().sharedMaterial.mainTexture.filterMode = FilterMode.Point;



            switch (Mathf.FloorToInt(scene.cubeArray[i + 4]))
            {
                case 0:
                    temp.tag = "Cube";
                    break;
                case 1:
                    temp.tag = "Cube_WalkSMP";
                    break;
            }
        }

    }


    [MenuItem("[Menu]/儲存地圖")]
    static void SaveJson()
    {
        scene scene = new scene();

        //儲存tag ==Cube
        GameObject[] respawnsCube;
        respawnsCube = GameObject.FindGameObjectsWithTag("Cube");
        foreach (GameObject respawn in respawnsCube)
        {
            scene.cubeArray.Add(respawn.transform.position.x);
            scene.cubeArray.Add(respawn.transform.position.y);
            scene.cubeArray.Add(respawn.transform.position.z);
            string name = respawn.GetComponent<MeshFilter>().sharedMesh.name;
            scene.cubeArray.Add(float.Parse(name));
            scene.cubeArray.Add(0);
        }

        //儲存tag ==Cube_WalkSMP
        GameObject[] respawnsSMP;
        respawnsSMP = GameObject.FindGameObjectsWithTag("Cube_WalkSMP");
        foreach (GameObject respawn in respawnsSMP)
        {
            scene.cubeArray.Add(respawn.transform.position.x);
            scene.cubeArray.Add(respawn.transform.position.y);
            scene.cubeArray.Add(respawn.transform.position.z);
            string name = respawn.GetComponent<MeshFilter>().sharedMesh.name;
            scene.cubeArray.Add(float.Parse(name));
            scene.cubeArray.Add(1);
        }

        //儲存長度
        //雖然非必要，但便於觀看場景資訊
        scene.length = scene.cubeArray.Count;

        //儲存角色位置
        //todo:角色名稱不一定是Cha_Kinight
        scene.playerPos = GameObject.Find("Cha_Knight").transform.position;

        //將Object轉存為json，並存檔
        string json = JsonUtility.ToJson(scene);
        StreamWriter file = new StreamWriter(Application.dataPath + "/Resources/scene/s998.json");
        file.Write(json);
        file.Close();

        UnityEditor.EditorUtility.DisplayDialog("Save End ", " --=== Save End ===--", "[OK]");


    }

    [MenuItem("[Menu]/清除場景")]
    static void clearMap()
    {
        var g = GameObject.Find("Cubes").transform;

        var children = new List<GameObject>();
        foreach (Transform child in g) children.Add(child.gameObject);
        children.ForEach(child => DestroyImmediate(child));
    }

    [MenuItem("[Menu]/選取物件歸正")]
    static void Normalized()
    {

        var respawns = Selection.gameObjects.OfType<GameObject>().ToList();
        if (respawns.Count == 1)
        {
            UnityEditor.EditorUtility.DisplayDialog("oh,Come on ?", " Select GameObject First!", "OK");
        }
        var step = 0;
        foreach (GameObject respawn in respawns)
        {
            if (respawn.tag == "Cube" || respawn.tag == "Cube_WalkSMP")
            {
                Vector3 temp;
                temp = respawn.transform.position;
                Debug.Log(respawn.transform.position);
                temp.x = Mathf.Floor(temp.x);
                temp.z = Mathf.Floor(temp.z);
                temp.y = Mathf.Floor(temp.y);
                respawn.transform.position = temp;
                respawn.transform.eulerAngles = new Vector3(-90, 0, 0);

                respawn.name = temp.ToString("F1");
            }
        }
        int ss = 0;

        for (var i = 0; i < respawns.Count; i++)
        {
            if (respawns[i] == null) continue;
            for (var j = 0; j < respawns.Count; j++)
            {
                if (respawns[j] == null || respawns[i] == null) continue;
                if (respawns[j] != respawns[i])
                {
                    if (respawns[j].name == respawns[i].name)
                    {
                        DestroyImmediate(respawns[i]);
                        ss++;
                    }
                }
            }
        }

        UnityEditor.EditorUtility.DisplayDialog("Normalized End ", " --=== Normalized End ===--", "OK");
        Debug.Log("Normalized:" + step);
        Debug.Log("DestroyImmediate:" + ss);
    }
    [MenuItem("[Menu]/show|hide-SMP")]
    static void showHideSmp()
    {

        GameObject[] smps;
        smps = GameObject.FindGameObjectsWithTag("SMP");

        if (smps.Length > 0)
        {
            Debug.Log("hided");
            //hide
            foreach (GameObject smp in smps)
            {
                DestroyImmediate(smp);
            }
        }
        else
        {
            Debug.Log("showed");
            //show
            GameObject[] respawns;
            respawns = GameObject.FindGameObjectsWithTag("Cube_WalkSMP");
            foreach (GameObject respawn in respawns)
            {
                GameObject temp = Instantiate(GameObject.Find("tempPick2"));
                temp.transform.parent = GameObject.Find("WalkSMP").transform;
                temp.tag = ("SMP");
                temp.transform.position = respawn.transform.position;
                temp.name = respawn.name + "_SMP";
                respawns = Selection.gameObjects;
            }
        }
    }

}