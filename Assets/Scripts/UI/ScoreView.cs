using DG.Tweening;
using SamePictures.Services;
using UnityEngine;

namespace SamePictures.UI
{
    public class ScoreView : DisplayerTextInfo
    {
        private LevelService _levelService;
        private IScoreAnimationComponent _animationComponent;

        private void Awake()
        {
            _levelService = Startup.GetService<LevelService>();
        }

        protected override void Start()
        {
            base.Start();
            if (!TryGetComponent(out _animationComponent))
            {
                Debug.Log($"animation component not found on {GetType().Name}");
            }
        }

        private void OnEnable()
        {
            _levelService.OnSuccessSelect += OnSuccessSelect;
        }

        private void OnDisable()
        {
            _levelService.OnSuccessSelect -= OnSuccessSelect;
        }

        private void OnSuccessSelect(int score)
        {
            _animationComponent?.PlayAnimation();

            SetText(string.Format("Score: {0}", score));
        }
    }
}