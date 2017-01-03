using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class editorMenuCS : MonoBehaviour
{
    public class Cube
    {
        public float x, y, z;
        public int g, smp;

        public Cube(float p1, float p2, float p3, int p4, int p5)
        {
            x = p1;
            y = p2;
            z = p3;
            g = p4;
            smp = p5;
        }
    }

    // [Serializable]
    public class scene
    {
        public int length;

        public List<Cube> cubeArray = new List<Cube>();
        Cube g = new Cube(1, 1, 1, 1, 1);



        static void Main()
        {

        }
    }

    [MenuItem("[Menu]/LoadJson")]
    static void LoadJson()
    {
        TextAsset arrayText = Resources.Load("scene/s998") as TextAsset;
        scene myObject = new scene();

    }

}