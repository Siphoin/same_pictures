using Cysharp.Threading.Tasks;
using SamePictures.Extensions;
using SiphoinUnityHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SamePictures.Repositories
{
    public abstract class SpriteRepository : IRepository
    {
        private string _key;

        private Sprite[] _sprites;

        public SpriteRepository(string key)
        {
            _key = key;
        }

        public async void Initialize()
        {
            var sprites = await AddressablesHelperUniTask.GetMore<Sprite>(false, _key);

            _sprites = new Sprite[sprites.Count()];

            for (int i = 0; i < _sprites.Length; i++)
            {
                _sprites[i] = sprites.ElementAt(i);
            }

            Debug.Log($"{GetType().Name} loaded {_sprites.Length} {nameof(Sprite)}s");
        }

        public async UniTask<Sprite> GetRandomSprite()
        {
            await UniTask.WaitUntil(() => _sprites != null);

            return _sprites.GetRandomElement();
        }
    }
}
