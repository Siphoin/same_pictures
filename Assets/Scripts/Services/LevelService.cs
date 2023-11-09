using Cysharp.Threading.Tasks;
using SamePictures.Extensions;
using SamePictures.Repositories;
using SiphoinUnityHelpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace SamePictures.Services
{
    public class LevelService : IService
    {
        private bool _isInitialized;
        private int _maxCountCards;
        private readonly int _maxCountSelectingCards = 2;
        private IPicture[] _selectedPictures;

        private int _errorsCount;
        private int _score;
        private int _requireCountEquals;
        private int _selectedCountCards;

        public event Action OnEnd;
        public event Action<int> OnSuccessSelect;
        public event Action<int> OnFailureSelect;

        private GridLayoutGroup _grid;

        private PictureRepository _pictureRepository;

        private Queue<Sprite> _spritesVariants;

        private IPicture[] _pictures;

        public async void Initialize()
        {
            _pictureRepository = Startup.GetRepository<PictureRepository>();

            await UniTask.WaitUntil(() => _pictureRepository.IsLoadedAllSprites);

            _maxCountCards = _pictureRepository.CountSprites;

            _pictures = new IPicture[_maxCountCards];
            _selectedPictures = new IPicture[_maxCountSelectingCards];
        }

        public async UniTask InitializeCards()
        {
            await UniTask.WaitUntil(() => _pictureRepository.IsLoadedAllSprites);

            var picture = await AddressablesHelperUniTask.GetPrefab<Picture>();

            for (int i = 0; i < _pictures.Length; i++)
            {
                IPicture newPicture = Object.Instantiate(picture, _grid.transform);

                newPicture.OnSelect += OnSelect;

                _pictures[i] = newPicture;
            }

            HideCards();

        }

        private void OnSelect(object sender, EventArgs e)
        {
            IPicture picture = sender as IPicture;

            _selectedCountCards++;

            if (_selectedCountCards < _maxCountSelectingCards)
            {
                _selectedPictures[_selectedCountCards - 1] = picture;
            }

            else if (_selectedCountCards == _maxCountCards)
            {
                IPicture firstPicture = _selectedPictures[0];

                IPicture lastPicture = _selectedPictures[_selectedPictures.Length - 1];

                bool equals = CompareCards(firstPicture, lastPicture);

                if (equals)
                {
                    firstPicture.Deactivate();
                    lastPicture.Deactivate();
                }

                _selectedCountCards = 0;
            }
        }

        public void SetGrid (GridLayoutGroup grid)
        {
            if (!grid)
            {
                throw new ArgumentNullException("canvas is null");
            }

            _grid = grid;
        }

        public void NewLevel()
        {
            ResetCards();

            HideCards();

            int countCards = UnityEngine.Random.Range(2, _maxCountCards + 1);

            _requireCountEquals = countCards;

            SetPicturesForCards(countCards);
        }

        private void SetPicturesForCards(int countCards)
        {
            _spritesVariants = new Queue<Sprite>(_pictureRepository.Sprites.Shuffle());

            

            for (int i = 0; i < countCards / 2; i++)
            {
                System.Random random = new System.Random();

                var sprite = _spritesVariants.Dequeue();

                if (sprite == null)
                {
                    Debug.Log("sprite null");
                }

                var first = _pictures.Where(s => s.IsEmpty).OrderBy(s => random.Next()).First();

                var second = _pictures.Where(s => s.IsEmpty).OrderBy(s => random.Next()).First();

                IPicture[] pictures = new IPicture[]
                {
                    first, second,
                };

                for (int j = 0; j < pictures.Length; j++)
                {
                    pictures[j].Show();
                    pictures[j].SetSprite(sprite);
                }
            }
        }

        private void HideCards ()
        {
            foreach (var picture in _pictures)
            {
                picture.Hide();
            }
        }

        private void ResetCards()
        {
            foreach (var picture in _pictures)
            {
                picture.ResetSprite();
            }
        }

        private bool CompareCards (IPicture pictureA, IPicture pictureB)
        {
            bool equals = pictureA.Equals(pictureB);

            if (equals)
            {
                _score++;

                OnSuccessSelect?.Invoke(_score);
            }

            else
            {
                _errorsCount++;

                OnFailureSelect?.Invoke(_errorsCount);
            }
            return equals;
        }
    }
}
