using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Bus : MonoBehaviour
{
    public float initTime = 3;
    public float stayStationTime;
    public float speed;
    public float scale = 0.65f;

    [SerializeField] BusRouteManager _busRouteManager;
    [SerializeField] Server _server;    
    // ó�� ��Ʈ�� �������� �����ؼ� ���� ��� �� Ȱ��ȭ
    [SerializeField] GameObject _startPin;
    
    BusMove _busMove;
    public BusRouteObj _currentRouteObj;
    BusMovingRouteRenderer _busMovingRouteRenderer;
    Coroutine _initCoroutine;
    Coroutine _waitStartStationCoroutine;
    Image _image;
    WaitForSeconds _startStationStayTime;

    int _connectionKioskID;
    bool _isRacing;

    public bool IsRacing => _isRacing;
    public int ConnectionKioskID => _connectionKioskID;
    
    [SerializeField] UnityEvent OnInited;

    void Start()
    {
        SetComponents();

        GetComponent<RectTransform>().localScale = new Vector3(scale, scale, scale);
        _busMove.Init();
        _busMovingRouteRenderer.Init();

        //if (Testor.instance.IsTest)
        //{
        //    GoNextStation();
        //    ControllImage();
        //}

        _image.enabled = false;
        _startStationStayTime = new WaitForSeconds(stayStationTime);
    }

    public void StartRace(int id)
    {
        //Debug.Log("����?");
        if (_isRacing)
        {
            StopRace();
            _server.SendInitSignal(ConnectionKioskID);
        }

        _connectionKioskID = id;
        _server.SendStartSignal(gameObject.name, _connectionKioskID);
        _isRacing = true;
        _image.enabled = true;

        // ����ϸ鼭 ���ֹڹ��� �� �ɱ�
        _startPin.SetActive(true);
        _busMovingRouteRenderer.AddStartPosition();
        _server.SendStationNumber(gameObject.name, _connectionKioskID, 1);

        _waitStartStationCoroutine = StartCoroutine(WaitStartStationCoroutine());
    }

    IEnumerator WaitStartStationCoroutine()
    {
        yield return _startStationStayTime;
        GoNextStation();        
    }

    public void GoNextStation()
    {
        _currentRouteObj = _busRouteManager.GetNextRoute();
        
        if(_currentRouteObj == null)
        {            
            EndRace();            
            return;
        }

        float stayTime = _currentRouteObj.IsEndPoint ? 0 : stayStationTime;
        _busMove.Move(_currentRouteObj.BusRoute, stayTime);
        _busMovingRouteRenderer.StartRender();
    }

    public void OnNearingNextStation()
    {
        _server.SendStationNumber(gameObject.name, ConnectionKioskID, _currentRouteObj.StationNumber);        
    }

    public void OnArriveStation()
    {
        //_server.SendStationNumber(gameObject.name, ConnectionID, _currentRouteObj.StationNumber);
        _currentRouteObj.SetActivePin(true);
        _busMovingRouteRenderer.StopRender();
    }   

    public void StopRace()
    {
        if (_initCoroutine != null)
        {
            StopCoroutine(_initCoroutine);
        }

        if (_waitStartStationCoroutine != null)
        {
            StopCoroutine(_waitStartStationCoroutine);
        }

        _busMovingRouteRenderer.StopRender();
        Init();
    }

    void SetComponents()
    {
        _busMove = GetComponent<BusMove>();
        _busMovingRouteRenderer = GetComponent<BusMovingRouteRenderer>();
        _image = GetComponent<Image>();
    }

    void EndRace()
    {
        _busMovingRouteRenderer.StopRender();
        _server.SendEndSignal(ConnectionKioskID);        
        _initCoroutine = StartCoroutine(InitCorouine());        
    }

    void Init()
    {
        _busMove.Stop();
        _busMovingRouteRenderer.ClearLine();
        _busRouteManager.Init();
        _startPin.SetActive(false);
        _isRacing = false;
        _image.enabled = false;        
    }

    IEnumerator InitCorouine()
    {
        yield return new WaitForSeconds(initTime);
        _server.SendInitSignal(ConnectionKioskID);
        Init();
        // �����ؼ� �ʱ�ȭ �� ���� �ҷ�����
        OnInited.Invoke();
    }
}
