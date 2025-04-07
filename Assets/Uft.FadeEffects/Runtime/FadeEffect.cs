using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Uft.FadeEffects
{
    public sealed class FadeEffect : MonoBehaviour
    {
        static void SetStretch(RectTransform rectTransform, float left, float top, float right, float bottom)
        {
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.offsetMin = new Vector2(left, bottom);
            rectTransform.offsetMax = new Vector2(right, top);
        }

        // static / instance

        [SerializeField] FadeConfig _defaultFadeConfig;
        [SerializeField] int _canvasSortOrder = 32767;

        Canvas _canvas;
        RawImage _rawImage;

        void Awake()
        {
            var objCanvas = new GameObject("Canvas");
            objCanvas.transform.SetParent(gameObject.transform, false);
            _canvas = objCanvas.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = _canvasSortOrder;
            _canvas.vertexColorAlwaysGammaSpace = true;

            var objRawImage = new GameObject("RawImage");
            var rectTransform = objRawImage.AddComponent<RectTransform>();
            SetStretch(rectTransform, 0, 0, 0, 0);
            objRawImage.transform.SetParent(objCanvas.transform, false);
            _rawImage = objRawImage.AddComponent<RawImage>();
            _rawImage.color = _defaultFadeConfig.color;
        }

        // Unity event functions & event handlers / pure code

        public async UniTask StartFadeAsync(bool isFadeIn, float? fadeTime_sec = null, Color? color = null, Ease? ease = null)
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
            var duration_sec = fadeTime_sec ?? (isFadeIn ? _defaultFadeConfig.inTime_sec : _defaultFadeConfig.outTime_sec);
            var startColor = color ?? _defaultFadeConfig.color;
            startColor.a = isFadeIn ? 1 : 0;
            var endColor = startColor;
            endColor.a = isFadeIn ? 0 : 1;
            var easeValue = ease ?? _defaultFadeConfig.ease;

            _rawImage.gameObject.SetActive(true);
            _rawImage.color = startColor;
            await _rawImage.DOColor(endColor, duration_sec).SetEase(easeValue);
        }
    }
}
