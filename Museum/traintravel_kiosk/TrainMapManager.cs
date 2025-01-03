using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TrainMapManager : MonoBehaviour
{
    [HideInInspector] public bool isMainTrainReady = false;
    
    [SerializeField] Map _map;    
    [SerializeField] MainTrain _train;
    [SerializeField] LineManager[] _lineManagers;
    [SerializeField] GraphicRaycaster _graphicRaycaster;
    [SerializeField] Image _whiteOutBar;

    [SerializeField] AudioSource _bgm;
    [SerializeField] AudioSource _voice;
    [SerializeField] GameObject _clickBlock;

    AsyncOperation _asyncOperation;
    bool _isStartWhiteOut = false;

    public Map Map => _map;
    public MainTrain Train => _train;

    void Awake()
    {        
        StartCoroutine(PrepareScene());
        Init();
    }

    IEnumerator PrepareScene()
    {
        yield return null;
        _asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(2);
        _asyncOperation.allowSceneActivation = false;
    }

    void Init()
    {                
        foreach(LineManager lineManager in _lineManagers)
        {
            lineManager.Init(this);
        }

        _bgm.volume = GameInstance.Instance.SoundManager.sfxPower;
        _bgm.clip = GameInstance.Instance.SoundManager.GetAudioClip(EnumTypes.Sound.TrainMapBGM);
        _bgm.Play();
        _voice.volume = GameInstance.Instance.SoundManager.sfxPower;
        _voice.clip = GameInstance.Instance.SoundManager.GetAudioClip(EnumTypes.Sound.TrainMapIntroVoice);
        _voice.Play();

        _graphicRaycaster.enabled = true;
        _clickBlock.SetActive(false);
    }

    void Update()
    {        
        if (!isMainTrainReady)
        {
            return;
        }        

        _map.ZoomIn(_train.MovingProgress);

        if (_isStartWhiteOut)
        {
            return;
        }

        if (_train.MovingProgress > 0.85f)
        {
            
            StartWhiteOut();            
        }
    }

    public void BlockClick()
    {
        _graphicRaycaster.enabled = false;
        _clickBlock.SetActive(true);
    }

    void StartWhiteOut()
    {
        _isStartWhiteOut = true;
        _whiteOutBar.DOFade(1, 1f).OnComplete(PlayNextScene);
        DOTween.To(() => _bgm.volume, x => _bgm.volume = x, 0, 1f).OnComplete(_bgm.Stop);
        DOTween.To(() => _voice.volume, x => _voice.volume = x, 0, 1f).OnComplete(_voice.Stop);
    }

    void PlayNextScene()
    {
        DOTween.KillAll();
        _asyncOperation.allowSceneActivation = true;
    }
}
