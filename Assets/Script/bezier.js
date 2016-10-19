////using UnityEngine;
////using System.Collections.Generic;
////[RequireComponent(typeof (LineRenderer))]
//
//var controlPoints = new Array();
////    public Transform[] controlPoints;
//var LineRenderer: lineRenderer;
//
//private
//var curveCount: int = 0;
//private
//var layerOrder: int = 0;
//private
//var SEGMENT_COUNT: int = 50;
//
//function start() {
//    //    if (!lineRenderer) {
//    //        lineRenderer = GetComponent < LineRenderer > ();
//    //    }
//    lineRenderer.sortingLayerID = layerOrder;
//    //    curveCount = (int) controlPoints.Length / 3;
//    curveCount = Mathf.FloorToInt(controlPoints.Length / 3);
//}
//
//function Update() {
//    DrawCurve();
//}
//
//function DrawCurve() {
//    for (var j = 0; j < curveCount; j++) {
//        for (var i = 1; i <= SEGMENT_COUNT; i++) {
//            //            var t: float = i / (float) SEGMENT_COUNT;
//            var t: float = i / SEGMENT_COUNT;
//            var nodeIndex: int = j * 3;
//            var pixel: Vector3 = CalculateCubicBezierPoint(t, controlPoints[nodeIndex].position, controlPoints[nodeIndex + 1].position, controlPoints[nodeIndex + 2].position, controlPoints[nodeIndex + 3].position);
//            lineRenderer.SetVertexCount(((j * SEGMENT_COUNT) + i));
//            lineRenderer.SetPosition((j * SEGMENT_COUNT) + (i - 1), pixel);
//        }
//
//    }
//}
//
//function CalculateCubicBezierPoint(t: float, p0: Vector3, p1: Vector3, p2: Vector3, p3: Vector3) {
//    var u: float = 1 - t;
//    var tt: float = t * t;
//    var uu: float = u * u;
//    var uuu: float = uu * u;
//    var ttt: float = tt * t;
//
//    var p: Vector3 = uuu * p0;
//    p += 3 * uu * t * p1;
//    p += 3 * u * tt * p2;
//    p += ttt * p3;
//
//    return p;
//}
