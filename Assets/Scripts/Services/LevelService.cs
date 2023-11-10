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
        private int _level = 1;
        private readonly int _maxCountSelectingCards = 2;
        private IPicture[] _selectedPictures;

        private int _errorsCount;
        private int _score;
        private int _requireCountEquals;
        private int _selectedCountCards;
        public event Action OnNew;
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

            _selectedPictures[_selectedCountCards - 1] = picture;

            if (_selectedCountCards == _maxCountSelectingCards)
            {
                
                IPicture firstPicture = _selectedPictures[0];

                IPicture lastPicture = _selectedPictures[_selectedPictures.Length - 1];

                bool equals = CompareCards(firstPicture, lastPicture);

                Color color = equals ? Color.green : Color.red;
                firstPicture.Deactivate();
                lastPicture.Deactivate();
                firstPicture.SetColor(color);
                lastPicture.SetColor(color);

                if (!equals)
                {
                    ResetCards(firstPicture, lastPicture).Forget();
                }

                else
                {
                    _requireCountEquals -= _maxCountSelectingCards;
                }
                _selectedCountCards = 0;
            }
            if (_requireCountEquals == 0)
            {
                NextLevel().Forget();
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

            int countCards = UnityEngine.Random.Range(_maxCountSelectingCards, _level * 2 + 1);

            while (countCards % _maxCountSelectingCards != 0)
            {
                countCards++;
            }

            _requireCountEquals = countCards;

            SetPicturesForCards(countCards);

            OnNew?.Invoke();
        }

        private void SetPicturesForCards(int countCards)
        {
            _spritesVariants = new Queue<Sprite>(_pictureRepository.Sprites.Shuffle());

            Queue<IPicture> pictures = new Queue<IPicture>(_pictures.Shuffle());

            for (int i = 0; i < countCards / _maxCountSelectingCards; i++)
            {
                var sprite = _spritesVariants.Dequeue();
                IPicture[] selectedPictures = new IPicture[]
                {
                    pictures.Dequeue(),
                    pictures.Dequeue()
                };
                for (int j = 0; j < selectedPictures.Length; j++)
                {
                    var picture = selectedPictures[j];

                    picture.Show();
                    picture.SetSprite(sprite);
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
                picture.ResetSpriteToDefault();
                picture.ResetSprite();
                picture.ResetColor();
                picture.Activate();
            }
        }

        private bool CompareCards (IPicture a, IPicture b)
        {
            bool equals = a.Equals(b);

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

        private async UniTask ResetCards (params IPicture[] pictures)
        {
             TimeSpan timeSpan = TimeSpan.FromSeconds(0.5f);

            await UniTask.Delay(timeSpan);

            foreach (var picture in pictures)
            {
                picture.ResetColor().ToUniTask().Forget();
                picture.ResetSpriteToDefault();
                picture.Activate();
            }
        }

        private async UniTask NextLevel()
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(2);

            await UniTask.Delay(timeSpan);

            _level++;

            NewLevel();
        }
    }
}
