using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class KioskManager : MonoBehaviour
{        
    [Header("Ű����ũ ��ü �ܰ�")]
    [SerializeField] GameObject _waitUI;
    [SerializeField] GameObject _readyUI;
    [SerializeField] GameObject _playUI;
    [SerializeField] GameObject _endUI;

    // ��ȣ ���� ������ ���缭 Ȱ��ȭ
    [Header("���� ���� ���ȭ��")]
    [SerializeField] GameObject _playUIOrangeBG;
    [SerializeField] GameObject _playUIGreenBG;
    
    [Header("���� ���ൿ�� ��Ʈ�� �ʿ��� �ֵ�")]
    [SerializeField] RectTransform _viewerParent;
    [SerializeField] RectTransform _viewer;
    [SerializeField] TextMeshProUGUI _nameTmp;
    [SerializeField] TextMeshProUGUI _sourceTmp;

    [SerializeField] Client _client;
    [Space(10f)]
    [SerializeField] UnityEvent OnGreenBusTagged;
    [SerializeField] UnityEvent OnOrangeBusTagged;

    Coroutine _sendStartSignalCoroutine;
    Image _viewImage;
    WaitForSeconds _noticeViewWaitTIme;

    bool _isOnlyCropMode;

    void Awake()
    {
        Screen.SetResolution(1920, 1080, true);
        Cursor.visible = false;

        INIDataManager iniDataManager = new INIDataManager();
        float noticeTime = iniDataManager.GetFloatData("KioskManager", "noticeTime", 3f);
        int isOnlyCropMode = iniDataManager.GetIntData("KioskManager", "onlyCropMode", 0);
        _isOnlyCropMode = isOnlyCropMode == 1 ? true : false;
        iniDataManager.INIClose();

        _viewImage = _viewer.GetComponent<Image>();
        _noticeViewWaitTIme = new WaitForSeconds(noticeTime);
    }

    void Update()
    {
        if (DataQueue.instance.GetDataCount() <= 0)
        {
            return;    
        }

        string signal = DataQueue.instance.DequeueData();
        if (signal.Contains("Orange"))
        {
            SendOrangeBusStartSignal();
        }
        else
        {
            SendGreenBusStartSignal();
        }
    }

    // ClientEvent���� ȣ��
    public void SetPlayUI(string Color)
    {
        _waitUI.SetActive(false);
        _readyUI.SetActive(false);
        _endUI.SetActive(false);
        _playUIOrangeBG.SetActive(Color.Contains("Orange"));
        _playUIGreenBG.SetActive(Color.Contains("Green"));

        _playUI.SetActive(true);
    }

    // ClientEvent���� ȣ��
    public void SetInfo(string msg)
    {                

        string[] tmpArray = msg.Split("_");
        int stationNumber = Convert.ToInt32(tmpArray[1]);

        if (tmpArray[0].Contains("Orange"))
        {            
            // �� �������� ��
            if(stationNumber > Data.Instance.OrangeStationData.Count)
            {                
                return;
            }

            _nameTmp.text = Data.Instance.OrangeStationData[stationNumber - 1].RelicsName;
            _sourceTmp.text = Data.Instance.OrangeStationData[stationNumber - 1].Source;
            _viewImage.sprite = Data.Instance.OrangeStationData[stationNumber - 1].Sprite;            
        }
        else
        {
            // �� �������� ��
            if (stationNumber > Data.Instance.GreenStationData.Count)
            {                
                return;
            }
            _nameTmp.text = Data.Instance.GreenStationData[stationNumber - 1].RelicsName;
            _sourceTmp.text = Data.Instance.GreenStationData[stationNumber - 1].Source;
            _viewImage.sprite = Data.Instance.GreenStationData[stationNumber - 1].Sprite;            
        }

        _viewImage.SetNativeSize();        
        _viewer.sizeDelta = Utility.GetBestFitClampSize(_viewer, _viewerParent);

        if(_isOnlyCropMode)
        {
            if (tmpArray[0].Contains("Orange") && _viewImage.sprite.name.Contains("10"))
            {                
                if (_viewer.sizeDelta.x > _viewerParent.sizeDelta.x + 10 || _viewer.sizeDelta.y > _viewerParent.sizeDelta.y + 10)
                {
                    _viewer.sizeDelta = Utility.GetBestDetailViewFitClampSize(_viewer, _viewerParent, _viewer.sizeDelta.x > _viewerParent.sizeDelta.x);
                }
            }
        }
        else
        {
            if (_viewer.sizeDelta.x > _viewerParent.sizeDelta.x + 10 || _viewer.sizeDelta.y > _viewerParent.sizeDelta.y + 10)
            {
                _viewer.sizeDelta = Utility.GetBestDetailViewFitClampSize(_viewer, _viewerParent, _viewer.sizeDelta.x > _viewerParent.sizeDelta.x);
            }
        }
    }

    // ClientEvent���� ȣ��
    public void End()
    {
        _endUI.SetActive(true);
    }

    // ClientEvent���� ȣ��
    public void Init()
    {
        _waitUI.SetActive(true);
        _readyUI.SetActive(false);
        _playUI.SetActive(false);
        _endUI.SetActive(false);
        _nameTmp.text = " ";
        _sourceTmp.text = " ";
    }

    public void SendOrangeBusStartSignal()
    {
        _client.SendData("Ready_OrangeBus");
        SetReadyUI();
        if (_sendStartSignalCoroutine != null)
        {
            StopCoroutine(_sendStartSignalCoroutine);
        }
        _sendStartSignalCoroutine = StartCoroutine(SendDataCoroutine("Start_OrangeBus"));
    }

    public void SendGreenBusStartSignal()
    {
        _client.SendData("Ready_GreenBus");
        SetReadyUI();
        if (_sendStartSignalCoroutine != null)
        {
            StopCoroutine(_sendStartSignalCoroutine);
        }
        _sendStartSignalCoroutine = StartCoroutine(SendDataCoroutine("Start_GreenBus"));
    }

    IEnumerator SendDataCoroutine(string text)
    {
        yield return _noticeViewWaitTIme;
        _client.SendData(text);
    }

    void SetReadyUI()
    {
        _waitUI.SetActive(false);
        _readyUI.SetActive(true);
        _playUI.SetActive(false);
        _endUI.SetActive(false);
    }
}
