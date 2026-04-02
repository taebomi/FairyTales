// using UnityEditor;
// using UnityEngine;
//
// namespace FairyTales.Trash
// {
// #if UNITY_EDITOR
//     [CustomEditor(typeof(DeathEventManager))]
//     public class DeathEventManagerInspector : Editor
//     {
//         public override void OnInspectorGUI()
//         {
//             base.OnInspectorGUI();
//             if (GUILayout.Button("오버레이 카메라 0,0으로 이동하기"))
//             {
//                 CameraManager.Instance.ControlOverlayCamera(Vector2.zero, CameraManager.DefaultCameraSize);
//                 CameraManager.Instance.ActivateOverlayCamera(true);
//             }
//         }
//     }
// #endif
// }
