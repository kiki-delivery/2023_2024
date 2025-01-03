using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CircleAnimator : MonoBehaviour
{
    [Header("애니메이션 설정")]
    [SerializeField] float _circleFillTime;
    [SerializeField] float _circleStarScaleDownTime;
    [SerializeField] float _circleStarScaleUpTime;
    [SerializeField] float _circleStarMovingTime;
    [SerializeField] AnimationCurve _circleStarScaleDownGraph;
    [SerializeField] AnimationCurve _circleStarScaleUpGraph;
    [SerializeField] AnimationCurve _circleStarMoveGraph;
    [SerializeField] AnimationCurve _circleStarMoveScaleGraph;

    [Header("애니메이션 하는 애들")]
    [SerializeField] Image _bigCircleGage;
    [SerializeField] Image _smallCircleGage;
    [SerializeField] Image _circleHeart;    
    [SerializeField] Sprite _colorHeartSprite;    
    [SerializeField] RouteManager _circleStartRouteManager;
    [SerializeField] GameObject _heartParticle;
    [SerializeField] RectTransform _circleStarRect;

    Sprite _circleStarOriginalSprite;    
    Vector3 _circleStarStartScale;
    Vector3 _circleStarStartPos;
    
    TweenCallback _AnimationEndCallback;    

    void Awake()
    {
        // 이게 동작 안함
        //_circleStarRect = _circleStar.GetComponent<RectTransform>();
        _circleStarOriginalSprite = _circleHeart.sprite;
        _circleStarStartScale = _circleStarRect.localScale;
        _circleStarStartPos = _circleStarRect.anchoredPosition;
        _circleStartRouteManager.Init();        
    }

    public void SetAnimationTime(float circleFillTime, float circleStarScaleDownTime, float circleStarScaleUpTime, float circleStarMovingTime)
    {
        _circleFillTime = circleFillTime;
        _circleStarScaleDownTime = circleStarScaleDownTime;
        _circleStarScaleUpTime = circleStarScaleUpTime;
        _circleStarMovingTime = circleStarMovingTime;
    }
    
    public void SetAnimationEndCallback(TweenCallback callback)
    {
        _AnimationEndCallback = callback;
    }

    public void SetActiveCircleStar(bool isActive)
    {
        _circleHeart.gameObject.SetActive(isActive);
    }

    public void FillCircle()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(_bigCircleGage.DOFillAmount(1, _circleFillTime))
            .Join(_smallCircleGage.DOFillAmount(1, _circleFillTime))
            .Insert(_circleFillTime * 0.9f, _circleStarRect.DOScale(0.5f, _circleStarScaleDownTime).SetEase(_circleStarScaleDownGraph))
                                                           .OnComplete(SizeUpCircleHeart);
    }

    void SizeUpCircleHeart()
    {
        //_heartParticle.SetActive(true);
        _circleHeart.sprite = _colorHeartSprite;
//        _fireWork.SetActive(true);

        Sequence sequence = DOTween.Sequence();
        sequence.Append(_circleStarRect.DOScale(1, _circleStarScaleUpTime).SetEase(_circleStarScaleUpGraph))
            .Insert(GameInstance.Instance.circleStarScaleUpTime * 0.9f,
                    _circleStarRect.DOScale(1, _circleStarMovingTime).SetEase(_circleStarMoveScaleGraph).OnPlay(MoveCircleHeart));
    }

    void MoveCircleHeart()
    {
        Route route = _circleStartRouteManager.GetRoute(1);
        _circleHeart.transform.DOLocalPath(route.Vector3Array, _circleStarMovingTime, PathType.CubicBezier, PathMode.TopDown2D, 20)
            .SetEase(_circleStarMoveGraph).OnComplete(_AnimationEndCallback);
    }


    public void ReSetting()
    {
        _bigCircleGage.fillAmount = 0;
        _smallCircleGage.fillAmount = 0;
        _circleStarRect.localScale = _circleStarStartScale;
        _circleHeart.sprite = _circleStarOriginalSprite;
        _circleStarRect.anchoredPosition = _circleStarStartPos;
        _circleHeart.gameObject.SetActive(true);
        _heartParticle.SetActive(false);
    }
}
