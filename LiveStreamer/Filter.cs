using EnumTypes;
using Mediapipe.Unity;
using UnityEngine;
using UnityEngine.Events;

// 얘가 face 인식여부 확인해서 애니메이션 멈춰야할듯
public class Filter : MonoBehaviour
{
    [SerializeField] FoodName _foodName;
    [SerializeField] Mask _mask;

    public RectTransform RectTransform { get; private set; }    
    public FaceLandmarkListWithIrisAnnotation_Custom Face { get; private set; }
    public CanvasGroup CanvasGroup { get; private set; }    
    public FoodName FoodName => _foodName;
    public Mask Mask => _mask;

    public UnityEvent AnimationEnd;

    FilterAnimation _animation;

    void Awake()
    {        
        RectTransform = GetComponent<RectTransform>();
        CanvasGroup = GetComponent<CanvasGroup>();
        _animation = GetComponent<FilterAnimation>();        
    }    

    public void SetFace(FaceLandmarkListWithIrisAnnotation_Custom face)
    {        
        gameObject.SetActive(true);        
        face.FaceTracked.AddListener(_animation.OnFaceTrackingRecovery);
        face.FaceTrackError.AddListener(_animation.OnFaceTrackingError);
        Face = face;        
        CanvasGroup.alpha = 1;        
        _animation.Init(face);
        _animation.Ready();
        _animation.Play();        
    }

    void OnDisable()
    {
        AnimationEnd.AddListener(Face.FaceTracked.RemoveAllListeners);
        AnimationEnd.AddListener(Face.FaceTrackError.RemoveAllListeners);
    }

    //public void Ready()
    //{
    //    Using = false;
    //}
}
