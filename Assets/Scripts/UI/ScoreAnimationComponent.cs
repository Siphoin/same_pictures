using DG.Tweening;
using UnityEngine;

namespace SamePictures.UI
{
    public class ScoreAnimationComponent : MonoBehaviour, IScoreAnimationComponent
    {
        [SerializeField, Min(0.1f)] private float _durationAnimationNewScore = 1;
        [SerializeField, Min(0.3f)] private float _divideScale = 4;

        public void PlayAnimation ()
        {
            transform.DOPunchScale(transform.localScale / _divideScale, _durationAnimationNewScore);
        }
    }
}