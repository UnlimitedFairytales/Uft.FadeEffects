using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Uft.FadeEffects.Samples.FadeEffectSample
{
    public class FadeEffectSample : MonoBehaviour
    {
        [SerializeField] Sprite _sprite;
        FadeEffect _fadeEffect;
        FadeEffect _imgFadeEffect;
        bool _isOnEffect;
        bool _isOnimgFadeEffect;

        void Start()
        {
            var scene = SceneManager.GetActiveScene();
            var rootObjects = scene.GetRootGameObjects();
            foreach (var obj in rootObjects)
            {
                if (obj.name == "FadeEffect")
                {
                    this._fadeEffect = obj.GetComponent<FadeEffect>();
                    continue;
                }
                if (obj.name == "imgFadeEffect")
                {
                    this._imgFadeEffect = obj.GetComponent<FadeEffect>();
                    continue;
                }
            }
        }

        void Update()
        {
            if (Input.GetKeyUp(KeyCode.F))
            {
                UniTask.Action(async () =>
                {
                    this._isOnEffect = !this._isOnEffect;
                    var newValue = this._isOnEffect;
                    await this._fadeEffect.StartFadeAsync(newValue);
                    Debug.Log($"StartFadeAsync({newValue}) complete");
                })();
            }

            if (Input.GetKeyUp(KeyCode.G))
            {
                UniTask.Action(async () =>
                {
                    this._isOnimgFadeEffect = !this._isOnimgFadeEffect;
                    var newValue = this._isOnimgFadeEffect;
                    await this._imgFadeEffect.StartFadeAsync(newValue, 4.0f, new Color(0.5f, 0, 0, 1), new Color(0, 1, 0, 0.5f), _sprite, DG.Tweening.Ease.OutBounce);
                    Debug.Log($"StartFadeAsync({newValue}) complete");
                })();
            }
        }
    }
}
