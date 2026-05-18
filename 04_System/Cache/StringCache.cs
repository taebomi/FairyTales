using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FairyTales.Layer
{
    public enum LayerName
    {
        Object,
        Wall,
        Room,
        Overlay,
    }

    public static class LayerCache
    {
        private static readonly Dictionary<LayerName, int> NameToLayerDictionary = new();

        public static int GetLayerInt(LayerName layerName)
        {
            if (!NameToLayerDictionary.TryGetValue(layerName, out var layerInt))
            {
                NameToLayerDictionary.Add(layerName,
                    layerInt = LayerMask.NameToLayer(Enum.GetName(typeof(LayerName), layerName)));
            }

            return layerInt;
        }

        private static readonly Dictionary<LayerName, LayerMask> LayerMaskDictionary = new();

        public static LayerMask GetLayerMask(LayerName layerName)
        {
            if (!LayerMaskDictionary.TryGetValue(layerName, out var layerMask))
            {
                LayerMaskDictionary.Add(layerName,
                    layerMask = LayerMask.GetMask(Enum.GetName(typeof(LayerName), layerName)));
            }

            return layerMask;
        }
    }
}