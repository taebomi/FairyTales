using UnityEditor;

#if UNITY_EDITOR
namespace FairyTales.EventSystem
{
    [CustomEditor(typeof(DialogueClip))]
    [CanEditMultipleObjects]
    public class DialogueClipInspector : Editor
    {
        private SerializedProperty _idProperty;
        private SerializedProperty _playModeProperty;
        private SerializedProperty _effectProperty;
        private SerializedProperty _textColorProperty;
        private SerializedProperty _printIntervalProperty;
        private SerializedProperty _canSkipProperty;
        private SerializedProperty _pauseDurationProperty;
        private SerializedProperty _timeToJumpToProperty;

        private void OnEnable()
        {
            var dialogueProperty = serializedObject.FindProperty("template.dialogueInfo");
            _idProperty = dialogueProperty.FindPropertyRelative("id");
            _playModeProperty = dialogueProperty.FindPropertyRelative("playMode");
            _effectProperty = dialogueProperty.FindPropertyRelative("effect");
            _textColorProperty = dialogueProperty.FindPropertyRelative("textColor");
            _printIntervalProperty = dialogueProperty.FindPropertyRelative("printInterval");
            _canSkipProperty = dialogueProperty.FindPropertyRelative("canSkip");
            _pauseDurationProperty = dialogueProperty.FindPropertyRelative("pauseDuration");
            _timeToJumpToProperty = dialogueProperty.FindPropertyRelative("timeToJumpTo");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_idProperty);
            if (_idProperty.intValue == 0)
            {
                _idProperty.intValue = 10001;
            }

            EditorGUILayout.PropertyField(_playModeProperty);
            EditorGUILayout.PropertyField(_effectProperty);
            EditorGUILayout.PropertyField(_textColorProperty);

            EditorGUILayout.PropertyField(_printIntervalProperty);
            if (_printIntervalProperty.floatValue == 0f)
            {
                _printIntervalProperty.floatValue = 0.05f;
            }

            EditorGUILayout.PropertyField(_canSkipProperty);

            if (_canSkipProperty.boolValue)
            {
                EditorGUILayout.Separator();
                EditorGUILayout.LabelField("# 자동 설정 값 #");
                EditorGUILayout.PropertyField(_pauseDurationProperty);
                EditorGUILayout.PropertyField(_timeToJumpToProperty);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif