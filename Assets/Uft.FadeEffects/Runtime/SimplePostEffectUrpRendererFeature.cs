#nullable enable

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Uft.FadeEffects
{
    public class SimplePostEffectRendererFeature : ScriptableRendererFeature
    {
        SimplePostEffectPass? _pass;
        public override void Create() => this._pass = new SimplePostEffectPass();
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (this._pass == null) return;
            renderer.EnqueuePass(this._pass);
        }
#pragma warning disable CS0618 // 型またはメンバーが旧型式です
        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            if (this._pass == null) return;
            this._pass.SetTarget(renderer.cameraColorTargetHandle);
        }
#pragma warning restore CS0618 // 型またはメンバーが旧型式です
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            this._pass?.Dispose();
            this._pass = null;
        }

        class SimplePostEffectPass : ScriptableRenderPass
        {
            static readonly string profilerTag = nameof(SimplePostEffectPass);

            readonly ProfilingSampler _profilingSampler = new(profilerTag);
            RTHandle? _temp1;
            RTHandle? _temp2;

            RTHandle? _cameraColorTarget;

            public SimplePostEffectPass()
            {
                this.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
                this.ConfigureInput(ScriptableRenderPassInput.Color);
            }

            public void SetTarget(RTHandle colorTarget)
            {
                this._cameraColorTarget = colorTarget;
            }

            public void Dispose()
            {
                this._temp1?.Release();
                this._temp1 = null;
                this._temp2?.Release();
                this._temp2 = null;

                // cameraColorTarget は renderer 管理なので Release しない
                this._cameraColorTarget = null;
            }

#pragma warning disable CS0672 // メンバーは古い形式のメンバーをオーバーライドします
            public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
#pragma warning restore CS0672 // メンバーは古い形式のメンバーをオーバーライドします
            {
#pragma warning disable CS0618 // 型またはメンバーが旧型式です
                if (this._cameraColorTarget == null) return;

                this.ConfigureTarget(this._cameraColorTarget);
                var desc = renderingData.cameraData.cameraTargetDescriptor;
                desc.depthBufferBits = 0;
                RenderingUtils.ReAllocateIfNeeded(
                    ref this._temp1,
                    desc,
                    FilterMode.Bilinear,
                    TextureWrapMode.Clamp,
                    name: "_SimplePostEffectTemp1"
                );

                RenderingUtils.ReAllocateIfNeeded(
                    ref this._temp2,
                    desc,
                    FilterMode.Bilinear,
                    TextureWrapMode.Clamp,
                    name: "_SimplePostEffectTemp2"
                );
#pragma warning restore CS0618 // 型またはメンバーが旧型式です
            }

#pragma warning disable CS0672 // メンバーは古い形式のメンバーをオーバーライドします
            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
#pragma warning restore CS0672 // メンバーは古い形式のメンバーをオーバーライドします
            {
                if (renderingData.cameraData.isSceneViewCamera) return;
                var camera = renderingData.cameraData.camera;
                if (camera == null) return;

                if (!camera.TryGetComponent<SimplePostEffectCollection>(out var collection)) return;
                if (!collection.HasActiveEffects()) return;
                if (this._cameraColorTarget == null ||
                    this._temp1 == null ||
                    this._temp2 == null) return;

                var cmd = CommandBufferPool.Get(profilerTag);
                using (new ProfilingScope(cmd, this._profilingSampler))
                {
                    collection.RenderWithCommandBuffer(cmd, this._cameraColorTarget, this._cameraColorTarget, this._temp1, this._temp2);
                }
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                CommandBufferPool.Release(cmd);
            }
        }
    }
}
