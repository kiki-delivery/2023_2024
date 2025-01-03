using Structs;
using UnityEngine;

// 노선받아서 이동
// 2차, 3차 구분 하는것 보다 3차의 포인트 2개를 겹처서 2차처럼 쓰는게 처리 간단할듯
// 컨트롤 하는거 분리할 방법 생각
public class BusMove : MonoBehaviour
{        
    Bus _bus;
    RectTransform _rectTransform;

    Vector2 _initPositon;
    Vector2 _startPosition;
    Vector2 _middlePoint1;
    Vector2 _middlePoint2;
    Vector2 _endPosition;

    bool _isRouteReady;
    bool _isNearTarget;
    float ratio;    
    float _currentStayTime;
    float _distance;
    float _targetStayTime = 0;

    public bool IsNearTarget
    {
        get => _isNearTarget;
        set
        {
            if(_isNearTarget == value)
            {
                return;
            }

            _isNearTarget = value;

            if(!_isNearTarget)
            {
                return;
            }

            _bus.OnNearingNextStation();
        }
    }        

    void Update()
    {
        if (!_isRouteReady)
        {
            return;
        }

        ratio += Time.deltaTime * _bus.speed * (1 / _distance) * 100;

        MoveBezierCurve();

        if (ratio >= 0.95)
        {
            IsNearTarget = true;
        }

        if (!Testor.instance.IsTest)
        {
            if (ratio >= 1)
            {                                                
                _bus.OnArriveStation();
                _currentStayTime += Time.deltaTime;                

                if (_currentStayTime > _targetStayTime)
                {                    
                    _isRouteReady = false;
                    IsNearTarget = false;
                    _currentStayTime = 0;
                    ratio = 0;                    
                    _bus.GoNextStation();
                }
            }
        }
        else
        {
            IsNearTarget = false;
        }
    }

    public void Init()
    {
        _rectTransform = GetComponent<RectTransform>();
        _bus = GetComponent<Bus>();
        _initPositon = _rectTransform.anchoredPosition;
    }    

    public void Move(BusRoute route, float stayTime)
    {
        _startPosition = route.StartPosition;
        _middlePoint1 = route.MiddlePoint1;
        _middlePoint2 = route.MiddlePoint2;
        _endPosition = route.EndPosition;        
        _distance = Vector2.Distance(_startPosition, _endPosition);
        _targetStayTime = stayTime;
        
        _isRouteReady = true;
    }

    public void Stop()
    {
        _isRouteReady = false;
        IsNearTarget = false;
        _currentStayTime = 0;
        ratio = 0;
        _rectTransform.anchoredPosition = _initPositon;
    }

    void MoveBezierCurve()
    {
        Vector2 a = GetLerpMovedPosition(_startPosition, _middlePoint1);
        Vector2 b = GetLerpMovedPosition(_middlePoint1, _middlePoint2);
        Vector2 c = GetLerpMovedPosition(_middlePoint2, _endPosition);

        Vector2 d = GetLerpMovedPosition(a, b);
        Vector2 e = GetLerpMovedPosition(b, c);

        _rectTransform.anchoredPosition = GetLerpMovedPosition(d, e);
    }
    
    Vector2 GetLerpMovedPosition(Vector2 startPosition, Vector2 endPosition)
    {
        return Vector2.Lerp(startPosition, endPosition, ratio);
    }
}
