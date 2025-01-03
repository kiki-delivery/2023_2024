using TMPro;
using UnityEngine;

public class JellyManager : MonoBehaviour
{    
    [SerializeField] Jelly[] _tropifruttiJellys;
    [SerializeField] Jelly[] _goldbarenJellys;
    [SerializeField] Jelly[] _starmixJellys;
    
    [SerializeField] TextMeshProUGUI _krInfoTmp;
    [SerializeField] TextMeshProUGUI _engInfoTmp;
    [SerializeField] CameraRotation _cameraRotation;    

    Jelly[] _jellys;
    int _currentJellyNumber;
    int _preNewFaceJellyNumber = 99;

    string _packageID;

    public string PackageName => _packageID;

    public void Setting()
    {        
        JellyManagerSettingData jellyManagerSettingData = GetComponent<JellyManagerSettingData>();        

        switch (jellyManagerSettingData.JellyGroupNumber)
        {
            case 1:
                _jellys = new Jelly[_tropifruttiJellys.Length];
                _jellys = _tropifruttiJellys;
                _packageID = "11";
                break;
            case 2:
                _jellys = new Jelly[_goldbarenJellys.Length];
                _jellys = _goldbarenJellys;
                _packageID = "12";
                break;
            case 3:
                _jellys = new Jelly[_starmixJellys.Length];
                _jellys = _starmixJellys;
                _packageID = "13";
                break;
        }
        
        _cameraRotation.yAxisRotationLimit = jellyManagerSettingData.YAxisRotationLimit;
        _cameraRotation.rotationSpeed = jellyManagerSettingData.RotationSpeed;
    }

    // 새로운 사람이라 젤리 새로 뽑을 때 호출(FaceRecognizer.cs)
    public int GetJellyNumber()
    {
        int jellyNumber = Random.Range(0, _jellys.Length);        

        while (jellyNumber == _preNewFaceJellyNumber)
        {
            jellyNumber = Random.Range(0, _jellys.Length);
        }

        _preNewFaceJellyNumber = jellyNumber;
        _cameraRotation.Init();
        return jellyNumber;
    }
    
    public void SetJelly(int jellyNumber)
    {        
        _currentJellyNumber = jellyNumber;
        _krInfoTmp.text = _jellys[_currentJellyNumber].KrInfo;
        _engInfoTmp.text = _jellys[_currentJellyNumber].EngInfo;
        AppearJelly();
    }

    public void AppearJelly()
    {
        _jellys[_currentJellyNumber].gameObject.SetActive(true);
        _cameraRotation.isRotate = true;
    }

    public void HideJelly()
    {
        _jellys[_currentJellyNumber].gameObject.SetActive(false);
        _cameraRotation.isRotate = false;
        // 회전 각도 초기화 필요?
    }
}
