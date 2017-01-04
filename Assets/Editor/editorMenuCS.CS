using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class editorMenuCS : MonoBehaviour
{

    // [Serializable]
    public class scene
    {
        public int length;
        public Vector3 playerPos;
        public List<float> cubeArray = new List<float>();
    }

    [MenuItem("[Menu]/LoadJson")]
    static void LoadJson()
    {
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
            temp.transform.position = tempVector3;

            temp.GetComponent<MeshRenderer>().receiveShadows = true;
            temp.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            temp.GetComponent<Renderer>().enabled = true;

            //fixme:目前這版沒辦法指定正確mesh
            temp.GetComponent<MeshFilter>().mesh = Resources.Load("item/model/CUBE/" + scene.cubeArray[i + 3]) as Mesh;

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


    [MenuItem("[Menu]/SaveJson")]
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

}