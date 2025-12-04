using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Uft.FadeEffects
{
    [Serializable]
    public class SimplePostEffectConfig
    {
        static readonly int AMOUNT_ID = Shader.PropertyToID("_Amount");
        static readonly int ANGLE_ID = Shader.PropertyToID("_Angle");

        [SerializeField] Material _materialPrototype;
        Material _material; public Material Material => this._material;

        [SerializeField, Range(0.0f, 1.0f)] float _amount;
        public float Amount
        {
            get => this._amount;
            set => this._amount = Mathf.Clamp01(value);
        }

        [SerializeField, Range(-360, +360)] int _angle;
        public int Angle
        {
            get => this._angle;
            set => this._angle = Mathf.Clamp(value, -360, +360);
        }

        public void Setup()
        {
            if (this._material == null && this._materialPrototype != null)
            {
                this._material = UnityEngine.Object.Instantiate(this._materialPrototype);
            }
        }

        public void Cleanup()
        {
            this.SafeDestroy(this._material);
            this._material = null;
        }

        public void UpdateFrame()
        {
            if (this._material == null) return;

            this._material.SetFloat(AMOUNT_ID, this.Amount);
            this._material.SetFloat(ANGLE_ID, this.Angle);
        }

        public void Blit(RenderTexture src, RenderTexture dst)
        {
            if (this._material == null)
            {
                Graphics.Blit(src, dst);
                return;
            }
            Graphics.Blit(src, dst, this._material);
        }

        public void BlitUrp(CommandBuffer cmd, RTHandle src, RTHandle dst)
        {
            if (this._material == null)
            {
                Blitter.BlitCameraTexture(cmd, src, dst);
                return;
            }
            Blitter.BlitCameraTexture(cmd, src, dst, this._material, 0);
        }

        void SafeDestroy(UnityEngine.Object obj)
        {
            if (obj == null) return;

#if UNITY_EDITOR
            if (!Application.isPlaying)
                UnityEngine.Object.DestroyImmediate(obj);
            else
#endif
                UnityEngine.Object.Destroy(obj);
        }
    }
}
