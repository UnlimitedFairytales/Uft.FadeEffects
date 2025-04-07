using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Uft.FadeEffects.Samples.FadeEffectSample
{
    public class FadeEffectSample : MonoBehaviour
    {
        FadeEffect _fadeEffect;
        bool _isFadeIn = true;

        void Start()
        {
            var scene = SceneManager.GetActiveScene();
            var rootObjects = scene.GetRootGameObjects();
            foreach (var obj in rootObjects)
            {
                if (obj.name == "FadeEffect")
                {
                    _fadeEffect = obj.GetComponent<FadeEffect>();
                    break;
                }
            }
        }

        void Update()
        {
            if (Input.GetKeyUp(KeyCode.F))
            {
                UniTask.Action(async () =>
                {
                    _isFadeIn = !_isFadeIn;
                    await _fadeEffect.StartFadeAsync(_isFadeIn);
                    Debug.Log("StartFadeAsync complete");
                })();
            }
        }
    }
}
