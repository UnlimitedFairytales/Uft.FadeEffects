#nullable enable

using DG.Tweening;
using System;
using UnityEngine;

namespace Uft.FadeEffects
{
    [Serializable]
    public class FadeConfig
    {
        public float onTime_sec = 1.0f;
        public float offTime_sec = 1.0f;
        public Color color = Color.black;
        public Sprite? sprite = null;
        public Ease ease = Ease.OutQuad;
    }
}
