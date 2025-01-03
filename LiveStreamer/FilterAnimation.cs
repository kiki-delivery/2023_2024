using Mediapipe.Unity;
using System.Collections.Generic;
using UnityEngine;

// �ִϸ��̼� �ʱ�ȭ
// �ִϸ��̼� ���� ������ ����

public abstract class FilterAnimation : MonoBehaviour
{    
    protected Filter _filter;
    protected FaceLandmarkListWithIrisAnnotation_Custom _face;
    protected bool _isPlay;
    protected bool _isAnimationEnd;
    protected List<RectTransform> _iconRects = new List<RectTransform>();    

    void Awake()
    {        
        _filter = GetComponent<Filter>();
        FilterIcon[] filterIcons = GetComponentsInChildren<FilterIcon>();
        foreach(FilterIcon filterIcon in filterIcons)
        {
            _iconRects.Add(filterIcon.GetComponent<RectTransform>());
        }        
    }    

    public void Init(FaceLandmarkListWithIrisAnnotation_Custom face)
    {        
        _face = face;
    }

    protected void SetAngle()
    {        
        Vector3 angle = new Vector3(0, 0, _face.Angle);        
        foreach (RectTransform iconRect in _iconRects)
        {
            iconRect.eulerAngles = angle;
        }
    }
    
    public abstract void Ready();
    public abstract void Play();
    public abstract void OnFaceTrackingError();
    public abstract void OnFaceTrackingRecovery();
    protected abstract void OnAnimationEnd();
}
