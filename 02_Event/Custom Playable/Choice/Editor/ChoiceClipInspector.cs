using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

#if UNITY_EDITOR
namespace FairyTales.EventSystem
{
    [CustomEditor(typeof(ChoiceClip))]
    public class ChoiceClipInspector : Editor
    {
        private SerializedProperty _idProperty;
        private SerializedProperty _choiceInfoProperty;
        
        private void OnEnable()
        {
            var choiceProperty = serializedObject.FindProperty("template");
            _idProperty = choiceProperty.FindPropertyRelative("id");
            _choiceInfoProperty = choiceProperty.FindPropertyRelative("choiceInfo");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_idProperty);

            var stageEventData = EventManager.GetStageEventData();
            
            if (ChoiceClip.EventName == null)
            {
                EditorGUILayout.LabelField("마커를 움직여주면 내용이 출력됨.");
                serializedObject.ApplyModifiedProperties();
                return;
            }
            
            if (!stageEventData.TryGetValue(ChoiceClip.EventName, out var currentEventData))
            {
                EditorGUILayout.LabelField($"{ChoiceClip.EventName} 이름의 이벤트 존재하지 않음.");
                serializedObject.ApplyModifiedProperties();
                return;
            }

            if (!currentEventData.choiceSet.TryGetValue(_idProperty.intValue, out var choiceTexts))
            {
                EditorGUILayout.LabelField($"{_idProperty.intValue} 아이디인 선택지 존재하지 않음.");
                serializedObject.ApplyModifiedProperties();
                return;
            }
            
            
            _choiceInfoProperty.arraySize = currentEventData.choiceSet[_idProperty.intValue].Length;
            
            for (var i = 0; i < _choiceInfoProperty.arraySize; i++)
            {
                EditorGUILayout.PropertyField(_choiceInfoProperty.GetArrayElementAtIndex(i),
                    new GUIContent(choiceTexts[i]));
            }


            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}