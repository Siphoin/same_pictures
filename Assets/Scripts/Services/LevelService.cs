using Cysharp.Threading.Tasks;
using SiphoinUnityHelpers;
using System;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace SamePictures.Services
{
    public class LevelService : IService
    {
        private readonly int _maxCountCards = 110;
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

        private IPicture[] _pictures;

        public void Initialize()
        {
            _pictures = new IPicture[_maxCountCards];
            _selectedPictures = new IPicture[_maxCountSelectingCards];
        }

        public async UniTask InitializeCards()
        {
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
            HideCards();

            int countCards = UnityEngine.Random.Range(0, _maxCountCards + 1);

            _requireCountEquals = countCards;

            ShowCards(countCards);
        }

        private void HideCards ()
        {
            foreach (var picture in _pictures)
            {
                picture.Hide();
            }
        }

        private void ShowCards (int count)
        {
            for (int i = 0; i < count; i++)
            {
                _pictures[i].Show();
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
