using UnityEngine;

namespace Uft.FadeEffects
{
    [RequireComponent(typeof(Camera))]
    public class SimplePostEffectCollection : MonoBehaviour
    {

        [SerializeField] protected SimplePostEffectConfig[] _simplePostEffects;

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
            if (this._simplePostEffects == null || this._simplePostEffects.Length == 0)
            {
                Graphics.Blit(src, dst);
                return;
            }

            // NOTE: Blitは重いため最適化のために把握
            var activeCount = 0;
            var lastIndex = 0;
            for (int i = 0; i < this._simplePostEffects.Length; i++)
            {
                if (this._simplePostEffects[i] == null) continue;
                if (this._simplePostEffects[i].Amount == 0) continue;
                activeCount++;
                lastIndex = i;
            }

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

            var desc = src.descriptor;
            var temp1 = RenderTexture.GetTemporary(desc);
            var temp2 = RenderTexture.GetTemporary(desc);
            RenderTexture currentSrc = src;
            RenderTexture currentDst = null;
            bool useTemp1 = true;
            for (int i = 0; i < this._simplePostEffects.Length; i++)
            {
                if (this._simplePostEffects[i] == null) continue;
                if (this._simplePostEffects[i].Amount == 0) continue;

                currentDst =
                    i == lastIndex ? dst :
                    useTemp1 ? temp1 :
                    temp2;

                this._simplePostEffects[i].Blit(currentSrc, currentDst);
                currentSrc = currentDst;
                useTemp1 = !useTemp1;
            }
            if (currentDst != dst)
            {
                Graphics.Blit(currentSrc, dst);
            }
            RenderTexture.ReleaseTemporary(temp1);
            RenderTexture.ReleaseTemporary(temp2);
        }
    }
}
