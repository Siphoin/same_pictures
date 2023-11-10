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

        bool Equals(object other);

        void Deactivate();
        void ResetSprite();
        void SetSprite(Sprite sprite);

        Tween ResetColor();
        Tween SetColor(Color color);
        void Activate();
        void ResetSpriteToDefault();
    }
}
