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
        static readonly int SOFTNESS_ID = Shader.PropertyToID("_Softness");
        static readonly int SUB_TEX_ID = Shader.PropertyToID("_SubTex");
        static readonly int SUB_TEX_COLOR_ID = Shader.PropertyToID("_SubTexColor");
        static readonly int RULE_TEX_ID = Shader.PropertyToID("_RuleTex");
        static readonly int INVERT_ID = Shader.PropertyToID("_Invert");

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

        [SerializeField, Range(0, 1.0f)] float _softness;
        public float Softness
        {
            get => this._softness;
            set => this._softness = Mathf.Clamp(value, 0, 1.0f);
        }

        [SerializeField] Texture _subTex;
        public Texture SubTex
        {
            get => this._subTex;
            set => this._subTex = value;
        }

        [SerializeField] Color _subTexColor;
        public Color SubTexColor
        {
            get => this._subTexColor;
            set => this._subTexColor = value;
        }

        [SerializeField] Texture _ruleTex;
        public Texture RuleTex
        {
            get => this._ruleTex;
            set => this._ruleTex = value;
        }

        [SerializeField, Range(0, 1)] int _invert;
        public int Invert
        {
            get => this._invert;
            set => this._invert = value;
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
            this._material.SetFloat(SOFTNESS_ID, this.Softness);
            this._material.SetTexture(SUB_TEX_ID, this.SubTex);
            this._material.SetColor(SUB_TEX_COLOR_ID, this.SubTexColor);
            this._material.SetTexture(RULE_TEX_ID, this.RuleTex);
            this._material.SetFloat(INVERT_ID, this.Invert);
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
