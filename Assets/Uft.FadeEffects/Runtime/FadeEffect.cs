﻿using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Uft.FadeEffects
{
    public sealed class FadeEffect : MonoBehaviour
    {
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

        [SerializeField] FadeConfig _defaultFadeConfig;
        [SerializeField] Canvas _canvas;
        [SerializeField] Image _image;

        // Unity event functions & event handlers

        void Reset()
        {
            this._canvas = this.GetComponentInChildren<Canvas>();
            this._image = this.GetComponentInChildren<Image>();
        }

        void Awake()
        {
            var uiLayer = LayerMask.NameToLayer("UI");
            GameObject objCanvas = this._canvas != null ? this._canvas.gameObject : null;
            if (objCanvas == null)
            {
                objCanvas = new GameObject("Canvas");
                if (0 <= uiLayer) objCanvas.layer = uiLayer;
                objCanvas.transform.SetParent(this.gameObject.transform, false);
                this._canvas = objCanvas.AddComponent<Canvas>();
                this._canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                this._canvas.sortingOrder = SORTING_ORDER_MAX;
                this._canvas.vertexColorAlwaysGammaSpace = true;
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

        public async UniTask StartFadeAsync(bool isOn, float? fadeTime_sec = null, Color? onColor = null, Color? offColor = null, Sprite sprite = null, Ease? ease = null)
        {
            if (!this.gameObject.activeSelf)
            {
                this.gameObject.SetActive(true);
            }
            fadeTime_sec ??= (isOn ? this._defaultFadeConfig.onTime_sec : this._defaultFadeConfig.offTime_sec);
            onColor ??= this._defaultFadeConfig.color;
            if (offColor == null)
            {
                var c = this._defaultFadeConfig.color;
                c.a = 0;
                offColor = c;
            }
            sprite ??= this._defaultFadeConfig.sprite;
            ease ??= this._defaultFadeConfig.ease;
            Color startColor = isOn ? offColor.Value : onColor.Value;
            Color endColor = isOn ? onColor.Value : offColor.Value;
            this._image.gameObject.SetActive(true);
            this._image.color = startColor;
            this._image.sprite = sprite;
            await this._image.DOColor(endColor, fadeTime_sec.Value).SetEase(ease.Value);
        }
    }
}
