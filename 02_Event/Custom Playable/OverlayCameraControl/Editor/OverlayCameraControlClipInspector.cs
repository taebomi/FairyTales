using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace FairyTales.EventSystem
{
    [CustomEditor(typeof(OverlayCameraControlClip))]
    public class OverlayCameraControlClipInspector : Editor
    {
        private SerializedProperty _positionProperty;
        private SerializedProperty _sizeProperty;

        private void OnEnable()
        {
            var templateProperty = serializedObject.FindProperty("template");
            _positionProperty = templateProperty.FindPropertyRelative("position");
            _sizeProperty = templateProperty.FindPropertyRelative("size");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_positionProperty);
            EditorGUILayout.PropertyField(_sizeProperty);
            if (Mathf.Approximately(_sizeProperty.floatValue, 0f))
            {
                _sizeProperty.floatValue = CameraManager.DefaultCameraSize;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif