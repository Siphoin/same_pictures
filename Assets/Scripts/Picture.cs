using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

namespace SamePictures
{
    [RequireComponent(typeof(Image))]
    [RequireComponent (typeof(Button))]
    public class Picture : MonoBehaviour, IPicture
    {
        public event EventHandler OnSelect;

        [SerializeField] private Image _pictogram;

        private Button _button;
        public Sprite Sprite => _pictogram.sprite;

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

            _button.onClick.AddListener(Select);
        }

        private void Select()
        {
            OnSelect?.Invoke(this, new PictureEventArgs());
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
    }
}