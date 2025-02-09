﻿// Simple Scroll-Snap - https://assetstore.unity.com/packages/tools/gui/simple-scroll-snap-140884
// Copyright (c) Daniel Lochner

using UnityEngine;

namespace DanielLochner.Assets.SimpleScrollSnap
{
    public class SlotMachine : MonoBehaviour
    {
        #region Fields
        [SerializeField] private Simple_Scroll_Snap.Scripts.Runtime.Behaviours.SimpleScrollSnap[] slots;
        #endregion

        #region Methods
        public void Spin()
        {
            foreach (Simple_Scroll_Snap.Scripts.Runtime.Behaviours.SimpleScrollSnap slot in slots)
            {
                slot.Velocity += Random.Range(2500, 5000) * Vector2.up;
            }
        }
        #endregion
    }
}