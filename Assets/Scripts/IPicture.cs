using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace SamePictures
{
    public interface IPicture
    {
        event EventHandler OnSelect;

        Sprite Sprite { get; }
        bool IsEmpty { get; }

        void Show();

        void Hide();
        void Activate();
        void ResetSpriteToDefault();
        void Deactivate();
        void ResetSprite();
        void SetSprite(Sprite sprite);

        bool Equals(object other);

        Tween ResetColor();
        Tween SetColor(Color color);
    }
}
