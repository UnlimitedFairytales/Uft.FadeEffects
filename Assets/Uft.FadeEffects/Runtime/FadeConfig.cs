using DG.Tweening;
using System;
using UnityEngine;

namespace Uft.FadeEffects
{
    [Serializable]
    public class FadeConfig
    {
        public float inTime_sec = 1.0f;
        public float outTime_sec = 1.0f;
        public Color color = Color.black;
        public Ease ease = Ease.OutQuad;
    }
}
