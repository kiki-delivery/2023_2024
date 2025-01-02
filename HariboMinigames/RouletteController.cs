using System.Collections;
using TMPro;
using UniRx;
using UnityEngine;

public class RouletteController : MonoBehaviour
{
    readonly float _oneRoundDegree = 360;

    [SerializeField] GameObject _rouletteButton;
    [SerializeField] RouletteControllerFollower _rouletteControllerFollower;

    [SerializeField, Range(-360, 360)] int testDegree;


    // ������
    [SerializeField] TextMeshProUGUI _pickNumberDebug;
    [SerializeField] TextMeshProUGUI _successDebug;

    // �ܺ� ���� �ʿ�    
    int _successMinDegree = 226;
    int _successMaxDegree = 270;
    int _targetRotateCount;
    int _successRate = 13;    
    float _maxRotateSpeed = 1.3f;
    float _minRotateSpeed = 0.4f;        
    

    GameManager _gameManager;
    RectTransform _rectTransform;
    Vector2 _initAnchoredPosition;
    Vector2 _rotateStartAnchoredPosition;
    int _targetDegree;
    float _rotateSpeed = 1.3f;
    float _progressRate = 0;
    [SerializeField] float _startRotateSpeed = 0;
    bool _isRotateStart = false;
    bool _isSuccess = false;
    bool _isInputOver = false;

    WaitForFixedUpdate _waitForFixedUpdate = new WaitForFixedUpdate();

    int TargetDegree
    {
        get => _targetDegree;
        set => _targetDegree = value % 5 != 0 ? value : value - 1;        
    }

    float RotateSpeed
    {
        get => _rotateSpeed;
        set => _rotateSpeed = Mathf.Clamp(value, _minRotateSpeed, _maxRotateSpeed);
    }

    float ProgressRate
    {
        get => _progressRate;
        set => _progressRate = Mathf.Clamp(value, 0f, 1f);
    }

    private int sfxCount;
    void FixedUpdate()
    {
        if (_isInputOver)
        {
            Rotate();
        }

        // ����� �Է����� �������� ���� ��    
        // ���⼭ ������ �ӵ��� �� ���� ���� ���ϱ�
        // �ִ�ӵ� �˸� �ʿ�?
        void Rotate()
        {
            if (_isRotateStart)
            {
                return;
            }

            RotateSpeed = RotateSpeed + 0.01f;

            _rectTransform.anchoredPosition = Utility.GetRotatedVector2(_rectTransform.anchoredPosition, -RotateSpeed * Mathf.Deg2Rad);

            if (sfxCount > 60)
            {
                sfxCount = 0;
                AudioManager.Instance.PlaySFX(Constants.KEY_SFX_ROLLING_1);
            }
            sfxCount++;
        }
    }

    public void Setting(GameManager gameManager)
    {
        _gameManager = gameManager;
        _rectTransform = GetComponent<RectTransform>();
        _initAnchoredPosition = _rectTransform.anchoredPosition;

        _successRate = gameManager.SuccessRate;
        _successMinDegree = gameManager.SuccessMinDegree;
        _successMaxDegree = gameManager.SuccessMaxDegree;
        _targetRotateCount = gameManager.TargetRotateCount;

        _maxRotateSpeed = gameManager.StartRotateSpeed;
        _minRotateSpeed = gameManager.MinRotateSpeed;

        Init();
    }

    public void Init()
    {
        RotateSpeed = _maxRotateSpeed;
        _rectTransform.anchoredPosition = _initAnchoredPosition;        
        _isRotateStart = false;
        _isInputOver = false;
        _isTouchEnter = false;
        _rouletteControllerFollower.Init();

        if (Utility.isTest)
        {
            StopAllCoroutines();            
        }

        _pickNumberDebug.text = " ";        
        _successDebug.text = " ";
    }

    float _pushTime = 0;
    float _touchStartTime = 0;
    bool _isTouchEnter;

    public void OnTouchEnter()
    {        
        if (_isRotateStart)
        {
            return;
        }
        _touchStartTime = Time.time;
        _isInputOver = true;
        sfxCount = int.MaxValue;
        _isTouchEnter = true;
    }

    public void OnTouchOut()
    {
        if (!_isTouchEnter)
        {
            return;
        }

        _pushTime = Time.time - _touchStartTime;
        _pushTime = Mathf.Floor(_pushTime * 10f) * 0.1f;
        

        StartAutoRotate();
    }

    public void StartAutoRotate()
    {             
        if(!_isInputOver)
        {
            return;
        }

        if(_isRotateStart)
        {
            return;
        }
        
        _isRotateStart = true;
        _rotateStartAnchoredPosition = _rectTransform.anchoredPosition;
        _startRotateSpeed = RotateSpeed;
        SetTargetDegree();
        StartCoroutine(RotateByAuto());

        // ���⼭ Ÿ�� ��׸��� -�� ���� ������
        void SetTargetDegree()
        {
            Random.InitState((int)(System.DateTime.Now.Ticks));
            int pickNumber = Random.Range(1, 101);
            _pickNumberDebug.text = pickNumber.ToString();
            _isSuccess = pickNumber <= _successRate;

            _successDebug.text = _isSuccess == true ? "����" : "����";

            TargetDegree = _isSuccess ? GetSuccessDegree() : GetFailDegree();
            TargetDegree = TargetDegree < GetStartDegree() ? TargetDegree - (GetStartDegree() - 360) : TargetDegree - GetStartDegree();

            // ����� min�� 0���� Ŭ ��

            int GetSuccessDegree()
            {
                if(_successMinDegree < 0)
                {
                    UnityEngine.Random.InitState((int)(System.DateTime.Now.Ticks));
                    return Random.Range(0, 2) == 0 ? Random.Range(0, _successMaxDegree) : Random.Range(360 + _successMinDegree, 360);
                }
                else
                {
                    return Random.Range(_successMinDegree, _successMaxDegree);
                }
            }

            int GetFailDegree()
            {
                if (_successMinDegree < 0)
                {
                    int value = Random.Range(0, 360);
                    while (360 + _successMinDegree - 5 <= value || value <= _successMaxDegree + 5)
                    {
                        value = Random.Range(0, 360);
                    }
                    return value;
                }
                else
                {
                    int value = Random.Range(0, 360);
                    while (_successMinDegree + 5 <= value && value <= _successMaxDegree - 5)
                    {
                        value = Random.Range(0, 360);
                    }
                    return value;
                }
            }

            // ����ڰ� �귿 ���� ��ġ
            int GetStartDegree()
            {
                float dot = Vector2.Dot(_initAnchoredPosition.normalized, _rotateStartAnchoredPosition.normalized);
                float radian = Mathf.Acos(dot);
                int startDegree = Mathf.FloorToInt(radian * Mathf.Rad2Deg);
                return _initAnchoredPosition.normalized.x >= _rotateStartAnchoredPosition.normalized.x ? startDegree : 360 - startDegree;
            }
        }

        // ��ư ������ �ڵ����� ���� ��
        IEnumerator RotateByAuto()
        {
            if (Utility.isTest)
            {
                //int startDegree = GetStartDegree();
                //TargetDegree = testDegree < startDegree ? testDegree - (startDegree - 360) : testDegree - startDegree;
                TargetDegree = testDegree;
            }


            float targetRadian = TargetDegree * Mathf.Deg2Rad;
            Vector2 targetPosition = Utility.GetRotatedVector2(_rectTransform.anchoredPosition, targetRadian);
            // ���� ������ ���̳ʽ��ε� ���� ��׸��� +�����̶� �ʿ��� �۾�
            TargetDegree = 360 - TargetDegree;
            float targetMovedDegree = _oneRoundDegree * _targetRotateCount + TargetDegree;
            float totalMovedDegree = 0;
            float currentDegree = 0;
            float rotatedRadian = 0;
            int currentRotateCount = 0;

            int count = sfxCount;

            // n���� ����
            while (_targetRotateCount > currentRotateCount)
            {
                while (_oneRoundDegree > currentDegree)
                {
                    totalMovedDegree += RotateSpeed;
                    currentDegree += RotateSpeed;
                    rotatedRadian = -RotateSpeed * Mathf.Deg2Rad;
                    ProgressRate = totalMovedDegree / targetMovedDegree;
                    RotateSpeed = Mathf.Lerp(_startRotateSpeed, _minRotateSpeed, ProgressRate);

                    //_rectTransform.anchoredPosition = Utility.GetRotatedVector2(_rectTransform.anchoredPosition, rotatedRadian);
                    Vector2 from = _rectTransform.anchoredPosition;
                    Vector2 to = Utility.GetRotatedVector2(_rectTransform.anchoredPosition, rotatedRadian);
                    _rectTransform.anchoredPosition = to;

                    float dist = Vector2.Distance(from, to);

                    if (count > 60)
                    {
                        count = 0;

                        if (dist > 36)
                        {
                            AudioManager.Instance.PlaySFX(Constants.KEY_SFX_ROLLING_1);
                        }
                        else if (dist > 18)
                        {
                            AudioManager.Instance.PlaySFX(Constants.KEY_SFX_ROLLING_2);
                        }
                        else if (dist > 4)
                        {
                            AudioManager.Instance.PlaySFX(Constants.KEY_SFX_ROLLING_3);
                        }
                        else
                        {
                            count = int.MinValue;
                            AudioManager.Instance.PlaySFX(Constants.KEY_SFX_ROLLING_4);
                        }
                    }
                    count++;

                    yield return _waitForFixedUpdate;
                }

                currentDegree = 0;
                currentRotateCount++;
                _rectTransform.anchoredPosition = _rotateStartAnchoredPosition;
            }


            // �������� ������ ������ŭ ����
            while (TargetDegree > currentDegree)
            {
                totalMovedDegree += RotateSpeed;
                currentDegree += RotateSpeed;
                rotatedRadian = -RotateSpeed * Mathf.Deg2Rad;
                ProgressRate = totalMovedDegree / targetMovedDegree;
                RotateSpeed = Mathf.Lerp(_startRotateSpeed, _minRotateSpeed, ProgressRate);

                //_rectTransform.anchoredPosition = Utility.GetRotatedVector2(_rectTransform.anchoredPosition, rotatedRadian);
                Vector2 from = _rectTransform.anchoredPosition;
                Vector2 to = Utility.GetRotatedVector2(_rectTransform.anchoredPosition, rotatedRadian);
                _rectTransform.anchoredPosition = to;

                float dist = Vector2.Distance(from, to);

                if (count > 60)
                {
                    count = 0;

                    if (dist > 36)
                    {
                        AudioManager.Instance.PlaySFX(Constants.KEY_SFX_ROLLING_1);
                    }
                    else if (dist > 18)
                    {
                        AudioManager.Instance.PlaySFX(Constants.KEY_SFX_ROLLING_2);
                    }
                    else if (dist > 4)
                    {
                        AudioManager.Instance.PlaySFX(Constants.KEY_SFX_ROLLING_3);
                    }
                    else
                    {
                        count = int.MinValue;
                        AudioManager.Instance.PlaySFX(Constants.KEY_SFX_ROLLING_4);
                    }
                }
                count++;

                yield return _waitForFixedUpdate;
            }

            _rectTransform.anchoredPosition = targetPosition;

            if (Utility.isTest)
            {
                yield break;
            }

            yield return new WaitForSeconds(0.5f);

            if (_isSuccess)
            {
                _gameManager.Success();
            }
            else
            {
                _gameManager.Fail();
            }
        }
    }
}
