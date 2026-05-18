using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace FairyTales.EventSystem
{
    [CustomEditor(typeof(AnimationClip))]
    [CanEditMultipleObjects]
    public class AnimationClipInspector : Editor
    {
    }
}
#endif