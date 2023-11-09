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

        void Deactivate();
        void ResetSprite();
        void SetSprite(Sprite sprite);
    }
}
