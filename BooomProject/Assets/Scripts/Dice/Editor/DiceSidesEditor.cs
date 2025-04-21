//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;

//[CustomEditor(typeof(DiceSides))]
//public class DiceSideEditor : Editor
//{
//    SerializedProperty diceSides;

//    private void OnEnable()
//    {
//        diceSides = serializedObject.FindProperty("sides");
//    }

//    public override void OnInspectorGUI()
//    {
//        serializedObject.Update();

//        ShowDiceSideInspectorGUI();

//        serializedObject.ApplyModifiedProperties();
//        if(GUILayout.Button("计算面"))
//        {
//            CalculateSides();
//        }
//    }

//    private void ShowDiceSideInspectorGUI()
//    {
//        EditorGUILayout.LabelField("Dice Editor", EditorStyles.boldLabel);
//        // 增加缩进
//        EditorGUI.indentLevel++;
//        for(int i = 0; i < diceSides.arraySize; i ++ )
//        {
//            ShowDiceSideUI(i);
//        }
//        EditorGUI.indentLevel--;
//    }

//    private void ShowDiceSideUI(int index)
//    {
//        SerializedProperty side = diceSides.GetArrayElementAtIndex(index);
//        SerializedProperty value = side.FindPropertyRelative("val");

//        EditorGUILayout.BeginHorizontal();
//        EditorGUILayout.PropertyField(value, new GUIContent("Side " + (index + 1)));

//        if (GUILayout.Button("Show", GUILayout.Width(70)))
//        {
//            RotateDiceToSide(index);
//        }

//        EditorGUILayout.EndHorizontal();
//    }

//    private void RotateDiceToSide(int index)
//    {
//        DiceSides sides = target as DiceSides;
//        sides.transform.rotation = sides.GetWorldRotationFor(index);
//        SceneView.RepaintAll();
        
//    }
//}
