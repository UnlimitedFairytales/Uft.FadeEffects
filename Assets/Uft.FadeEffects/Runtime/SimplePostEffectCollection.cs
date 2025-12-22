#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Uft.FadeEffects
{
    [RequireComponent(typeof(Camera))]
    public class SimplePostEffectCollection : MonoBehaviour
    {

        [SerializeField] protected SimplePostEffectConfig[]? _simplePostEffects; public IReadOnlyList<SimplePostEffectConfig> SimplePostEffects => this._simplePostEffects;

        void Reset()
        {
            this._simplePostEffects ??= Array.Empty<SimplePostEffectConfig>();
        }

        void Awake()
        {
            if (this._simplePostEffects == null) return;

            for (int i = 0; i < this._simplePostEffects.Length; i++)
            {
                if (this._simplePostEffects[i] == null) continue;
                this._simplePostEffects[i].Setup();
            }
        }

        void OnDestroy()
        {
            if (this._simplePostEffects == null) return;

            for (int i = 0; i < this._simplePostEffects.Length; i++)
            {
                if (this._simplePostEffects[i] == null) continue;
                this._simplePostEffects[i].Cleanup();
            }
        }

        void Update()
        {
            if (this._simplePostEffects == null) return;

            for (int i = 0; i < this._simplePostEffects.Length; i++)
            {
                if (this._simplePostEffects[i] == null) continue;
                this._simplePostEffects[i].UpdateFrame();
            }
        }

        void OnRenderImage(RenderTexture src, RenderTexture dst)
        {
            this.RenderWithRenderTexture(src, dst);
        }

        public bool HasActiveEffects()
        {
            this.GetActiveEffectInfo(out int activeCount, out _);
            return 0 < activeCount;
        }

        /// <summary>レガシーでも利用可能</summary>
        public void RenderWithRenderTexture(RenderTexture src, RenderTexture dst)
        {
            if (this._simplePostEffects == null || this._simplePostEffects.Length == 0)
            {
                Graphics.Blit(src, dst);
                return;
            }

            this.GetActiveEffectInfo(out int activeCount, out int lastIndex);
            if (activeCount == 0)
            {
                Graphics.Blit(src, dst);
                return;
            }
            if (activeCount == 1)
            {
                this._simplePostEffects[lastIndex].Blit(src, dst);
                return;
            }

            RenderTexture? temp1 = null;
            RenderTexture? temp2 = null;
            try
            {
                var desc = src.descriptor;
                temp1 = RenderTexture.GetTemporary(desc);
                temp2 = RenderTexture.GetTemporary(desc);
                RenderTexture currentSrc = src;
                RenderTexture? currentDst = null;
                bool useTemp1 = true;

                for (int i = 0; i < this._simplePostEffects.Length; i++)
                {
                    var effect = this._simplePostEffects[i];
                    if (effect == null) continue;
                    if (effect.Amount == 0f) continue;

                    currentDst =
                        i == lastIndex ? dst :
                        useTemp1 ? temp1 :
                        temp2;

                    effect.Blit(currentSrc, currentDst);
                    currentSrc = currentDst;
                    useTemp1 = !useTemp1;
                }
                if (currentDst != dst)
                {
                    Graphics.Blit(currentSrc, dst);
                }
            }
            finally
            {
                if (temp1 != null) RenderTexture.ReleaseTemporary(temp1);
                if (temp2 != null) RenderTexture.ReleaseTemporary(temp2);
            }
        }

        /// <summary>URP用</summary>
        public void RenderWithCommandBuffer(
            CommandBuffer cmd,
            RTHandle src,
            RTHandle dst,
            RTHandle temp1,
            RTHandle temp2)
        {
            if (this._simplePostEffects == null || this._simplePostEffects.Length == 0)
            {
                Blitter.BlitCameraTexture(cmd, src, dst);
                return;
            }

            this.GetActiveEffectInfo(out int activeCount, out int lastIndex);
            if (activeCount == 0)
            {
                Blitter.BlitCameraTexture(cmd, src, dst);
                return;
            }
            if (activeCount == 1)
            {
                var effect = this._simplePostEffects[lastIndex];
                if (ReferenceEquals(src, dst))
                {
                    effect.BlitUrp(cmd, src, temp1);
                    Blitter.BlitCameraTexture(cmd, temp1, dst);
                }
                else
                {
                    effect.BlitUrp(cmd, src, dst);
                }
                return;
            }

            var currentSrc = src;
            RTHandle? currentDst = null;
            bool useTemp1 = true;
            for (int i = 0; i < this._simplePostEffects.Length; i++)
            {
                var effect = this._simplePostEffects[i];
                if (effect == null) continue;
                if (effect.Amount == 0f) continue;

                currentDst =
                    i == lastIndex ? dst :
                    useTemp1 ? temp1 :
                    temp2;

                effect.BlitUrp(cmd, currentSrc, currentDst);
                currentSrc = currentDst;
                useTemp1 = !useTemp1;
            }
            if (currentDst != dst)
            {
                Blitter.BlitCameraTexture(cmd, currentSrc, dst);
            }
        }

        void GetActiveEffectInfo(out int activeCount, out int lastIndex)
        {
            activeCount = 0;
            lastIndex = 0;
            if (this._simplePostEffects == null) return;

            for (int i = 0; i < this._simplePostEffects.Length; i++)
            {
                var effect = this._simplePostEffects[i];
                if (effect == null) continue;
                if (effect.Amount == 0f) continue;
                activeCount++;
                lastIndex = i;
            }
        }
    }
}
