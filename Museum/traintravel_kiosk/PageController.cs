using UnityEngine;
using UnityEngine.UI;

public class PageController : MonoBehaviour
{
    [SerializeField] GraphicRaycaster _graphicRaycaster;    
    [SerializeField, Range(0f, 1f)] float _transitionProgress;

    [SerializeField] AnimatorController _animatorController;


    [Header("트렌지션에 쓰이는 애들")]
    [SerializeField] AnimationCurve _animationInCurve;    
    [SerializeField] MainPage _mainPage;    
    [SerializeField] ViewCountPage _countViewPage;
    [SerializeField] PositionGuidePage _positionPage;
    [SerializeField] DetailPage _detailPage;    
    [SerializeField] RectTransform _detailViewBtn;
    [SerializeField] RectTransform _countViewBtn;
    [SerializeField] RectTransform _positionViewBtn;
    // 인트로 애니메이션 구조상 _shine을 메인 페이지 아래로 넣을 수 없음
    [SerializeField] CanvasGroup _mainShine;
    [SerializeField] CanvasGroup _detailShine;

    RelicsViewScenePage _currentPage;                
    Vector2 _zoomStartPosition;
    int _transitionDirection;
    bool _isTransitioning;    



    public float TransitionProgress
    {
        get => _transitionProgress;
        set => _transitionProgress = Mathf.Clamp(value, 0, 1);        
    }

    // 
    void Update()
    {
        if(!_isTransitioning)
        {
            return;
        }

        TransitionProgress += Time.deltaTime * GameInstance.Instance.transitionSpeed;
        float value = _animationInCurve.Evaluate(TransitionProgress);
        Transition(value);
        CheckTransitionEnd();
    }
    
    void Transition(float progress)
    {
        float transitionProgress = _transitionDirection == 1 ? progress : 1 - progress;        

        Vector2 targetZoomSize = new Vector2(GameInstance.Instance.mainPageZoomSize, GameInstance.Instance.mainPageZoomSize);        
        Vector2 zoomSize = Vector2.Lerp(Vector2.one, targetZoomSize, transitionProgress);
        Vector2 zoomPosition = Vector2.Lerp(_zoomStartPosition, Vector2.zero, transitionProgress);
        Vector2 zoomOutSize = new Vector2(GameInstance.Instance.targetPageZoomOutSize, GameInstance.Instance.targetPageZoomOutSize);

        _mainPage.Rect.localScale = zoomSize;
        _mainPage.Rect.anchoredPosition = zoomPosition;
        _currentPage.Rect.localScale = Vector2.Lerp(zoomOutSize, Vector3.one, transitionProgress);

        // 사라질 때는 더 빨리 사라지도록
        _mainPage.CanvasGroup.alpha = _transitionDirection == 1 ? 1 - transitionProgress * 1.2f : 1 - transitionProgress;
        _mainShine.alpha = _transitionDirection == 1 ? 1 - transitionProgress * 1.2f : 1 - transitionProgress;        

        _currentPage.CanvasGroup.alpha = _transitionDirection == 1 ? transitionProgress : transitionProgress * 1.2f;
        _detailShine.alpha = _transitionDirection == 1 ? transitionProgress : transitionProgress * 1.2f;
    }

    void CheckTransitionEnd()
    {
        if(TransitionProgress != 1)
        {
            return;
        }

        if (_transitionDirection == -1)
        {
            _mainPage.OnOpened();
            _currentPage.OnClosed();
            _isTransitioning = false;
        }

        if (_transitionDirection == 1)
        {
            _mainPage.OnClosed();
            _currentPage.OnOpened();
            _isTransitioning = false;
        }

        TransitionProgress = 0;
    }

    public void Init(RelicsViewManager relicsViewManager)
    {
        _mainPage.Init(relicsViewManager);
        _countViewPage.Init(relicsViewManager);
        _positionPage.Init(relicsViewManager);
        _detailPage.Init(relicsViewManager);        
    }    

    public void OpenMainPage()
    {
        _mainPage.OnOpen();
        _currentPage.OnClose();

        _transitionDirection = -1;
        _isTransitioning = true;        
        SetClickBlock(true);
        // 트랜지션 역재생 해야함        
    }

    public void OpenViewCountPage()
    {
        _currentPage = _countViewPage;
        _mainPage.OnClose();
        _currentPage.OnOpen();        
        ReadyTransition(_countViewBtn);
        _animatorController.DissolveSound();
    }

    public void OpenPositionPage()
    {        
        _currentPage = _positionPage;
        _mainPage.OnClose();
        _currentPage.OnOpen();
        ReadyTransition(_positionViewBtn);
        _animatorController.DissolveSound();
    }

    public void OpenDetailPage()
    {        
        _currentPage = _detailPage;
        _mainPage.OnClose();
        _currentPage.OnOpen();
        ReadyTransition(_detailViewBtn);
        _animatorController.DissolveSound();
    }    

    void ReadyTransition(RectTransform stationRect)
    {        
        _transitionDirection = 1;
        SetZoomInPosition(stationRect);        
        SetClickBlock(true);
        _isTransitioning = true;
    }    
    
    void SetZoomInPosition(RectTransform stationRect)
    {
        Vector2 size = _mainPage.Rect.sizeDelta;
        Vector2 halfSize = size * 0.5f;
        Vector2 normalizePosition = new Vector2(stationRect.anchoredPosition.x / size.x, stationRect.anchoredPosition.y / size.y);
        _mainPage.Rect.pivot = normalizePosition;

        // 피벗 바꿨을 때 포지션도 맞춰서 바뀌어야함
        float x = Mathf.Lerp(-halfSize.x, halfSize.x, _mainPage.Rect.pivot.x);
        float y = Mathf.Lerp(-halfSize.y, halfSize.y, _mainPage.Rect.pivot.y);
        _mainPage.Rect.anchoredPosition += new Vector2(x, y);
        _zoomStartPosition = _mainPage.Rect.anchoredPosition;
    }

    public void SetClickBlock(bool isBlock)
    {
        _graphicRaycaster.enabled = !isBlock;
    }
}
