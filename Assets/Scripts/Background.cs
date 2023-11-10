using UnityEngine;
using System;
using SamePictures.Repositories;
using SamePictures.Services;

namespace SamePictures
{
    public class Background : MonoBehaviour
    {
        private const int DEPTH = 10;

        private SpriteRenderer _spriteRenderer;
        private BackgroundRepository _repository;
        private LevelService _levelService;
        private void Start()
        {
            if (!TryGetComponent(out _spriteRenderer))
            {
                throw new NullReferenceException("background must have component Sprite Renderer");
            }

            _repository = Startup.GetRepository<BackgroundRepository>();

            _levelService = Startup.GetService<LevelService>();
            _levelService.OnNew += Refresh;
        }

        public async void Refresh ()
        {
            _spriteRenderer.sprite = await _repository.GetRandomSprite();

            Resize();
        }

        private void Resize ()
        {
            float halfFovRadians = Camera.main.fieldOfView * Mathf.Deg2Rad / 2f;

            float visibleHeightAtDepth = DEPTH * Mathf.Tan(halfFovRadians) * 2f;

            float spriteHeight = _spriteRenderer.sprite.rect.height
                               / _spriteRenderer.sprite.pixelsPerUnit;

            float scaleFactor = visibleHeightAtDepth / spriteHeight;

            _spriteRenderer.transform.localScale = Vector3.one * scaleFactor;
        }
    }
}