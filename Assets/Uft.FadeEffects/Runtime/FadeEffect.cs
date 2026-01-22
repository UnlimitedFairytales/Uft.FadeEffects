#nullable enable

using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Uft.FadeEffects
{
    public class FadeEffect : MonoBehaviour
    {
        const string NAME = "[" + nameof(FadeEffect) + "]";

        // Static

        public const int SORTING_ORDER_MAX = 32767;

        static void SetStretch(RectTransform rectTransform, float left, float top, float right, float bottom)
        {
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.offsetMin = new Vector2(left, bottom);
            rectTransform.offsetMax = new Vector2(right, top);
        }

        // Instance

        [SerializeField] protected FadeConfig? _defaultFadeConfig;
        [SerializeField] protected Canvas? _canvas;
        [SerializeField] protected Image? _image;

        protected int _fadeVersion;

        // Unity event functions & event handlers

        protected virtual void Reset()
        {
            this._defaultFadeConfig ??= new FadeConfig();
            if (this._canvas == null) this._canvas = this.GetComponentInChildren<Canvas>();
            if (this._image == null) this._image = this.GetComponentInChildren<Image>();
        }

        protected virtual void Awake()
        {
            var uiLayer = LayerMask.NameToLayer("UI");
            var objCanvas = this._canvas != null ? this._canvas.gameObject : null;
            if (objCanvas == null)
            {
                objCanvas = new GameObject("Canvas");
                if (0 <= uiLayer) objCanvas.layer = uiLayer;
                objCanvas.transform.SetParent(this.gameObject.transform, false);
                this._canvas = objCanvas.AddComponent<Canvas>();
                this._canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                this._canvas.sortingOrder = SORTING_ORDER_MAX;
                this._canvas.vertexColorAlwaysGammaSpace = true;
                objCanvas.AddComponent<GraphicRaycaster>();
            }

            var objImage = this._image != null ? this._image.gameObject : null;
            if (objImage == null)
            {
                objImage = new GameObject("Image");
                if (0 <= uiLayer) objImage.layer = uiLayer;
                var rectTransform = objImage.AddComponent<RectTransform>();
                SetStretch(rectTransform, 0, 0, 0, 0);
                objImage.transform.SetParent(objCanvas.transform, false);
                this._image = objImage.AddComponent<Image>();
                this._image.color = new Color(0, 0, 0);
            }
        }

        // Pure code

        public async UniTask StartFadeAsync(bool isOn, float? fadeTime_sec = null, Color? onColor = null, Color? offColor = null, Sprite? sprite = null, Ease? ease = null, bool? raycastTarget = null)
        {
            if (!this.gameObject.activeSelf)
            {
                this.gameObject.SetActive(true);
            }

            if (this._defaultFadeConfig == null ||
                this._canvas == null ||
                this._image == null)
            {
                Debug.LogWarning($"{NAME} Before Awake(). {nameof(StartFadeAsync)} does not work.");
                return;
            }

            fadeTime_sec ??= (isOn ? this._defaultFadeConfig.onTime_sec : this._defaultFadeConfig.offTime_sec);
            onColor ??= this._defaultFadeConfig.color;
            if (offColor == null)
            {
                var c = onColor.Value;
                c.a = 0;
                offColor = c;
            }
            sprite = sprite != null ? sprite : this._defaultFadeConfig.sprite;
            ease ??= this._defaultFadeConfig.ease;
            raycastTarget ??= this._defaultFadeConfig.raycastTarget;

            Color startColor = isOn ? offColor.Value : onColor.Value;
            Color endColor = isOn ? onColor.Value : offColor.Value;

            var version = ++this._fadeVersion;
            this._image.DOKill();
            this._image.gameObject.SetActive(true);
            this._image.color = startColor;
            this._image.sprite = sprite;
            this._image.raycastTarget = raycastTarget.Value;
            await this._image.DOColor(endColor, fadeTime_sec.Value).SetEase(ease.Value);
            if (version != this._fadeVersion) return;

            // NOTE: 1/256未満なら実質透明
            if (this._image.color.a <= (1f / 256f))
            {
                this._image.raycastTarget = false;
                this.gameObject.SetActive(false);
            }
        }
    }
}
