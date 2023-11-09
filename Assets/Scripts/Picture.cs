using UnityEngine;
using UnityEngine.UI;
using System;

namespace SamePictures
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(Button))]
    public class Picture : MonoBehaviour, IPicture
    {
        public event EventHandler OnSelect;

        [SerializeField] private Image _pictogram;

        private Button _button;
        private Sprite _defaultSprite;
        private Sprite _setedSprite;

        public Sprite Sprite => _pictogram.sprite;

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

            _defaultSprite = _pictogram.sprite;

            _button.onClick.AddListener(Select);
        }

        private void Select()
        {
            OnSelect?.Invoke(this, new PictureEventArgs());

            _pictogram.sprite = _setedSprite;
        }

        public override bool Equals(object other)
        {
            if (other is IPicture)
            {
                var equalsPicture = other as IPicture;

                return equalsPicture.Sprite.Equals(_pictogram.sprite);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Deactivate()
        {
            _button.interactable = false;
        }

        public void ResetSprite()
        {
            _pictogram.sprite = _defaultSprite;

            _setedSprite = null;
        }

        public void SetSprite(Sprite sprite)
        {
            _setedSprite = sprite;
        }
    }
}