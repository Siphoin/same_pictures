using System;
using UnityEngine;
using UnityEngine.Events;

namespace SamePictures
{
    public interface IPicture
    {
        event EventHandler OnSelect;

        Sprite Sprite { get; }

        void Show();

        void Hide();

        void Deactivate();
    }
}
