using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;

namespace SamePictures
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(Button))]
    public class Picture : MonoBehaviour, IPicture
    {
        private float _animationDuration = 0.5f;
        [SerializeField, Min(0)] private float _speedScaling = 10;

        private Color _defaultColor;
        private Color _clearColor;

        public event EventHandler OnSelect;

        private Image _shirt;
        [SerializeField] private Image _pictogram;

        private Button _button;
        private Sprite _defaultSprite;
        private Sprite _setedSprite;

        public Sprite Sprite => _setedSprite;

        public bool IsEmpty => _setedSprite is null;

        private void Awake()
        {
            if (!_pictogram)
            {
                throw new NullReferenceException("pictogram not seted");
            }

            if (!TryGetComponent(out _button))
            {
                throw new NullReferenceException($"{nameof(Picture)} must have component Button");
            }

            if (!TryGetComponent(out _shirt))
            {
                throw new NullReferenceException($"{nameof(Picture)} must have component Imagr");
            }

            _defaultColor = _shirt.color;

            _clearColor = _pictogram.color;

            _clearColor.a = 0;

            _defaultSprite = _pictogram.sprite;

            _button.onClick.AddListener(Select);
        }

        private void FixedUpdate()
        {
            transform.localRotation = Quaternion.identity;
        }

        private void Select()
        {
            _pictogram.sprite = _setedSprite;

            _pictogram.color = _clearColor;

            FlipOver();

            Sequence sequence = DOTween.Sequence();
            sequence.Join(_pictogram.transform.DOPunchScale(transform.localScale / 2, _animationDuration));
            sequence.Join(ResetColor());
            sequence.Play();

            Deactivate();

            OnSelect?.Invoke(this, new PictureEventArgs());
        }

        private Tween Scale (float scale)
        {
            return transform.DOScale(scale, _animationDuration + (transform.GetSiblingIndex() / _speedScaling));
        }

        private void FlipOver()
        {
            Vector3 flipVector = new Vector3(0, 180, 0);

            Sequence sequence = DOTween.Sequence();
            sequence.Join(transform.DORotate(flipVector, _animationDuration, RotateMode.Fast));
            sequence.Append(transform.DORotate(Vector3.zero, _animationDuration, RotateMode.Fast));
            sequence.Play();
        }

        public override bool Equals(object other)
        {
            if (other is IPicture)
            {
                var equalsPicture = other as IPicture;

                return equalsPicture.Sprite.Equals(Sprite);
            }

            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public void Show()
        {
            gameObject.SetActive(true);
            transform.localScale = Vector3.zero;
            Scale(1);
        }

        public void Hide()
        {
            gameObject?.SetActive(false);
        }

        public void Deactivate()
        {
            _button.interactable = false;
        }


        public void Activate()
        {
            _button.interactable = true;
        }
        public void ResetSpriteToDefault()
        {
            _pictogram.sprite = _defaultSprite;
        }

        public void ResetSprite()
        {
            _setedSprite = null;

            ResetSpriteToDefault();
        }

        public Tween ResetColor()
        {
            return SetColor(_defaultColor);
        }

        public void SetSprite(Sprite sprite)
        {
            _setedSprite = sprite;
        }

        public Tween SetColor(Color color)
        {
            return _pictogram.DOColor(color, _animationDuration);
        }
    }
}