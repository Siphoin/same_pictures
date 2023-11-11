using SamePictures.Services;
using UnityEngine;

namespace SamePictures.UI
{
    public class ErrorsView : DisplayerTextInfo
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
            _levelService.OnFailureSelect += OnErrorSelect;
        }

        private void OnDisable()
        {
            _levelService.OnFailureSelect -= OnErrorSelect;
        }

        private void OnErrorSelect(int error)
        {
            _animationComponent?.PlayAnimation();

            SetText(string.Format("Errors: {0}", error));
        }

    }

}