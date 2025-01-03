using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// to do : ������ ���ڱ��� ���� �տ��� �׻� �������� �� ���..
// �ϳ��� Ȯ���ϸ鼭 endPoint�� ���� �� �ָ��ָ� �Ǳ��ҵ�...
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
            Debug.Log("���޹��� ��Ʈ�� �����");
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
        // ��Ʈ�� ù Start���� ����ϵ���        
        transform.localPosition = _currentRoute.Vector3Array[1];
        ResetState();
        //Debug.Log("�̵� �Ÿ���  " + distance);
        SpwanFootPrint();
        transform.DOLocalPath(_currentRoute.Vector3Array, distance / (GameInstance.Instance.walkerSpeed * 0.02f), PathType.CubicBezier, PathMode.TopDown2D, 50)
                   .SetEase(Ease.Linear).OnUpdate(SpwanFootPrint).SetLookAt(0.001f, default, Vector3.right).OnComplete(_moveEndedEvent);
    }

    public void SetGuideEndedEvent(TweenCallback tweenCallback)
    {
        _moveEndedEvent = tweenCallback;
    }

    //// �ٷ� ������ �� �� ��쿡 ���
    //// ���� �ʿ��ϸ� �� Ÿ�ֿ̹� �ֱ�
    //public void StartGuide(TweenCallback tweenCallback)
    //{
    //    Move(tweenCallback);
    //}

    //public void Move(TweenCallback tweenCallback)
    //{
    //    float distance = _currentRoute.MoveDistance * 0.001f;
    //    transform.localPosition = _currentRoute.Vector3Array[1];
    //    ResetState();
    //    //Debug.Log("�̵� �Ÿ���  " + distance);
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
        // ���ڱ� �Ÿ� ������ ���⼭ position �����ϸ� ��
        Vector3 position = transform.position;

        _preFootPrints.Add(Instantiate(foot, position, transform.rotation, _parent));
        _isLeftFoot = !_isLeftFoot;
        _currentTime = 0;        
    }
}
