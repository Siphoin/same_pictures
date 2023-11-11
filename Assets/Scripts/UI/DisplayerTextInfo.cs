using System;
using TMPro;
using UnityEngine;

namespace SamePictures.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public abstract class DisplayerTextInfo : MonoBehaviour
    {
        private TextMeshProUGUI _guiText;

        protected TextMeshProUGUI TextGUI => _guiText;

        protected virtual void Start()
        {
            if (!TryGetComponent(out _guiText))
            {
                throw new NullReferenceException("view monney collected must have component TMProGUI Text");
            }
        }

        protected void SetText (string text) => _guiText.text = text;
    }
}