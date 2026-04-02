// using FairyTales.Layer;
// using UnityEditor;
// using UnityEngine;
//
// namespace FairyTales.Trash
// {
// #if UNITY_EDITOR
//     public class TaeBoMiTool : EditorWindow
//     {
//         [MenuItem("Window/TaeBoMi")]
//         static void Init()
//         {
//             var window = (TaeBoMiTool)GetWindow(typeof(TaeBoMiTool));
//             window.Show();
//         }
//
//         private Vector3 _worldPos;
//
//         private void OnGUI()
//         {
//             GUILayout.Label("월드 스페이스로 옮기기", EditorStyles.boldLabel);
//             _worldPos = EditorGUILayout.Vector3Field("월드 스페이스 좌표", _worldPos);
//             if (GUILayout.Button("옮기기"))
//             {
//                 var transform = Selection.activeTransform;
//                 if (transform == null)
//                 {
//                     return;
//                 }
//
//                 transform.position = _worldPos;
//             }
//
//             if (GUILayout.Button("오버레이에 보이도록 변환"))
//             {
//                 var deathEventObject = Selection.activeGameObject.GetComponent<DeathEventObjectBase>();
//                 if (deathEventObject == null)
//                 {
//                     Debug.Log("DeathEventObjectBase 컴포넌트가 없음");
//                     return;
//                 }
//
//                 deathEventObject.ChangeMaterialAndLayer(DeathEventManager.Instance.DeathEventObjectMaterial,
//                     LayerCache.GetLayerInt(LayerName.Overlay));
//             }
//
//             if (GUILayout.Button("초기화 시키기"))
//             {
//             }
//         }
//
//         private void OnDestroy()
//         {
//         }
//     }
// #endif
// }