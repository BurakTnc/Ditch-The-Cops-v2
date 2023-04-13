// Simple Scroll-Snap - https://assetstore.unity.com/packages/tools/gui/simple-scroll-snap-140884
// Copyright (c) Daniel Lochner

using Simple_Scroll_Snap.Scripts.Core.Runtime.Behaviours.TransitionEffect;
using UnityEngine;

namespace DanielLochner.Assets.SimpleScrollSnap
{
    public class TranslateZ : TransitionEffectBase<RectTransform>
    {
        public override void OnTransition(RectTransform rectTransform, float distance)
        {
            rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y, distance);
        }
    }
}