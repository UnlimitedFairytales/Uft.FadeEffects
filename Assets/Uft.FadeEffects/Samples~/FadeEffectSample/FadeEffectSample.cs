using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Uft.FadeEffects.Samples.FadeEffectSample
{
    public class FadeEffectSample : MonoBehaviour
    {
        [SerializeField] Sprite _sprite;
        [SerializeField] TMP_Text _txtRaycastInfo;


        FadeEffect _fadeEffect;
        FadeEffect _imgFadeEffect;
        bool _isOnEffect;
        bool _isOnimgFadeEffect;
        readonly List<RaycastResult> _results = new(32);

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
                if (obj.name == "FadeEffect1280x720")
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
                    await this._imgFadeEffect.StartFadeAsync(newValue, 4.0f, new Color(0.5f, 0, 0, 1), new Color(0, 1, 0, 0.5f), this._sprite, DG.Tweening.Ease.OutBounce);
                    Debug.Log($"StartFadeAsync({newValue}) complete");
                })();
            }

            if (EventSystem.current == null)
            {
                this._txtRaycastInfo.text = "EventSystem.current == null";
            }
            else
            {
                var eventData = new PointerEventData(EventSystem.current)
                {
                    position = Input.mousePosition, // 旧InputManager
                };
                this._results.Clear();
                EventSystem.current.RaycastAll(eventData, this._results);
                var sb = new StringBuilder(512);
                sb.AppendLine($"UI Raycast hits: {this._results.Count}");
                for (int i = 0; i < this._results.Count && i < 10; i++)
                {
                    var r = this._results[i];
                    sb.AppendLine($"{i}: {r.gameObject.name} (module={r.module}, dist={r.distance})");
                }
                this._txtRaycastInfo.text = sb.ToString();
            }
        }
    }
}
