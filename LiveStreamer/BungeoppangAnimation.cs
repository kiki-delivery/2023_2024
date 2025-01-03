using DG.Tweening;
using UnityEngine;

public class BungeoppangAnimation : FilterAnimation
{
    [Header("설정 값")]
    [SerializeField, Range(0, 2f)] float _noseSmokeTime;
    [SerializeField, Range(0, 2f)] float _faceSmokeTime;
    [SerializeField, Range(1, 10)] int _scaleFactor;

    [Header("위치 조정, 애니메이션에 사용")]
    [SerializeField] FilterIcon _nose;    
    [SerializeField] GameObject _leftNoseSmoke;
    [SerializeField] GameObject _rightNoseSmoke;
    [SerializeField] GameObject _faceSmoke;
    [SerializeField] GameObject _beanParticle;
    [SerializeField] RectTransform _particleScale;

    void Update()
    {
        //SetAngle();
        var angle = Quaternion.Euler(_face.Angle2);
        _particleScale.localRotation = angle;
        SetScale();
        SetPosition();
    }

    void SetPosition()
    {
        _nose.Rect.anchoredPosition = _face.meshVertices[5];
    }

    void SetScale()
    {
        _particleScale.localScale = new Vector3(_face.Distance, _face.Distance, _face.Distance) * _scaleFactor;
    }

    public override void Ready()
    {
        _leftNoseSmoke.SetActive(false);
        _rightNoseSmoke.SetActive(false);
        _faceSmoke.SetActive(false);
        _beanParticle.SetActive(false);
        _isAnimationEnd = false;        
    }

    public override void Play()
    {
        Sequence mySequence = DOTween.Sequence();

        mySequence.AppendCallback(StartNoseSmoke);
        mySequence.AppendInterval(_noseSmokeTime);
        mySequence.AppendCallback(StartMouseSmoke);
        mySequence.AppendInterval(_faceSmokeTime);
        mySequence.AppendCallback(StartBeanParticle);
        mySequence.AppendInterval(_faceSmokeTime);
        mySequence.AppendCallback(EndAnimation);
    }

    void StartNoseSmoke()
    {
        _leftNoseSmoke.SetActive(true);
        _rightNoseSmoke.SetActive(true);
    }

    void StartMouseSmoke()
    {
        _faceSmoke.SetActive(true);
    }

    void StartBeanParticle()
    {
        _beanParticle.transform.localPosition = new Vector3(_nose.Rect.anchoredPosition.x, 600, 0);
        _beanParticle.SetActive(true);
    }

    public void EndAnimation()
    {
        _isAnimationEnd = true;
        OnAnimationEnd();
    }

    protected override void OnAnimationEnd()
    {        
        _filter.AnimationEnd.Invoke();
        gameObject.SetActive(false);
    }

    public override void OnFaceTrackingError()
    {
        _filter.CanvasGroup.alpha = 0;
        if (_filter.Mask != EnumTypes.Mask.None)
        {
            _face.meshRenderer.material.SetFloat("_Alpha", 0);
        }
    }

    public override void OnFaceTrackingRecovery()
    {
        if (_isAnimationEnd)
        {
            return;
        }

        _filter.CanvasGroup.alpha = 1;
        if (_filter.Mask != EnumTypes.Mask.None)
        {
            _face.meshRenderer.material.SetFloat("_Alpha", 1);
        }
    }
}
