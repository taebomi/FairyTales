using System;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class CameraControlBehavior : PlayableBehaviour
{
    public Vector2 position;
    public float size;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            var mainCamera = Camera.main;
            if (mainCamera)
            {
                mainCamera.transform.position = new Vector3(position.x, position.y, mainCamera.transform.position.z);
                mainCamera.orthographicSize = size;
            }
            return;
        }
#endif

        CameraManager.Instance.ControlMainCamera(position, size);
    }
}