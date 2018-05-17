using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CylinderProjector))]
public class CylinderProjectorEditor : Editor {
    public override void OnInspectorGUI()
    {
        var target = this.target as CylinderProjector;
//        EditorGUILayout.BeginVertical();
//        DrawDefaultInspector();
//        EditorGUILayout.EndVertical();
        DrawDefaultInspector();
        EditorGUILayout.LabelField("Preview:");
        var height = EditorGUIUtility.currentViewWidth * target.texture.height / (float) target.texture.width;
        var rect = EditorGUILayout.GetControlRect(false,height);
        rect.height = height;
        EditorGUI.DrawPreviewTexture(rect, target.texture);
        
    }
}
