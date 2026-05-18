using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FairyTales.Layer;
using UnityEditor;
using UnityEngine;

public partial class PlayerScript
{
    private void InitializeInteraction()
    {
        // CheckInteractableObject().Forget();
    }

    private void ActivateInteraction()
    {
        
    }


    private async UniTaskVoid CheckInteractableObject()
    {
        while (true)
        {
            var currentInteractableObject =
                Physics2D.OverlapCircle(transform.position, 0.75f, LayerCache.GetLayerMask(LayerName.Object));
            Debug.DrawRay(transform.position, transform.up * 0.75f);
            if (currentInteractableObject)
            {
                Debug.Log(currentInteractableObject.name);
            }
            else
            {
                
            }
        
            await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
        }
    }
}