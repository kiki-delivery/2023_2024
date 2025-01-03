using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// to do : 마지막 발자국이 유물 앞에서 항상 찍히도록 할 방법..
// 하나씩 확인하면서 endPoint를 조금 더 멀리주면 되긴할듯...
public class Guide : MonoBehaviour
{
    [SerializeField] Transform _parent;
    [SerializeField] GameObject _leftFootPrint;
    [SerializeField] GameObject _rightFootPrint;
    
    List<GameObject> _preFootPrints = new List<GameObject>();        
    Quaternion _startQuaternion;
    Route _currentRoute;
    TweenCallback _moveEndedEvent;

    bool _isLeftFoot;
    float _currentTime;    

    public void Init()
    {                
        GetComponent<Image>().enabled = false;        
        _currentTime = 0;        
    }

    public void SetRoute(Route route)
    {
        if (route == null)
        {
            Debug.Log("전달받은 루트가 없어요");
        }

        _currentRoute = route;
    }

    public void ResetState()
    {
        transform.DOKill();
        foreach(GameObject footPrint in _preFootPrints)
        {
            Destroy(footPrint);
        }
        
        transform.rotation = _startQuaternion;
        _isLeftFoot = true;
        _currentTime = 0;        
    }    

    public void StartGuide()
    {        
        Move();
    }
    
    void Move()
    {
        float distance = _currentRoute.MoveDistance * 0.001f;
        // 루트의 첫 Start에서 출발하도록        
        transform.localPosition = _currentRoute.Vector3Array[1];
        ResetState();
        //Debug.Log("이동 거리는  " + distance);
        SpwanFootPrint();
        transform.DOLocalPath(_currentRoute.Vector3Array, distance / (GameInstance.Instance.walkerSpeed * 0.02f), PathType.CubicBezier, PathMode.TopDown2D, 50)
                   .SetEase(Ease.Linear).OnUpdate(SpwanFootPrint).SetLookAt(0.001f, default, Vector3.right).OnComplete(_moveEndedEvent);
    }

    public void SetGuideEndedEvent(TweenCallback tweenCallback)
    {
        _moveEndedEvent = tweenCallback;
    }

    //// 바로 유물로 안 갈 경우에 사용
    //// 사운드 필요하면 이 타이밍에 넣기
    //public void StartGuide(TweenCallback tweenCallback)
    //{
    //    Move(tweenCallback);
    //}

    //public void Move(TweenCallback tweenCallback)
    //{
    //    float distance = _currentRoute.MoveDistance * 0.001f;
    //    transform.localPosition = _currentRoute.Vector3Array[1];
    //    ResetState();
    //    //Debug.Log("이동 거리는  " + distance);
    //    SpwanFootPrint();
    //    transform.DOLocalPath(_currentRoute.Vector3Array, distance / (GameInstance.Instance.walkerSpeed * 0.02f), PathType.CubicBezier, PathMode.TopDown2D, 50)
    //               .SetEase(Ease.Linear).OnUpdate(SpwanFootPrint).SetLookAt(0.001f, default, Vector3.right).OnComplete(tweenCallback);
    //}


    void SpwanFootPrint()
    {
        _currentTime += Time.deltaTime;        

        if (_currentTime < 5f / GameInstance.Instance.footPrintSpawnSpeed)
        {
            return;
        }

        GameObject foot = _isLeftFoot ? _leftFootPrint : _rightFootPrint;
        // 발자국 거리 같은거 여기서 position 조정하면 됨
        Vector3 position = transform.position;

        _preFootPrints.Add(Instantiate(foot, position, transform.rotation, _parent));
        _isLeftFoot = !_isLeftFoot;
        _currentTime = 0;        
    }
}
