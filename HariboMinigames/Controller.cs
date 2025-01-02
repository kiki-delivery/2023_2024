using UnityEngine;

public class Controller : MonoBehaviour
{    
    [SerializeField] Jelly[] _jellys;        

    [SerializeField] bool _isMouseMode;
    [SerializeField] HandGuideController _handGuideController;

    public HandGuideController HandGuideController => _handGuideController;

    void Awake()
    {
        INIDataManager iNIDataManager = new INIDataManager();
        int value = iNIDataManager.GetIntData("JellyManager", "isMouseMode", 0);
        _isMouseMode = value == 1 ? true : false;
    }

    void Update()
    {
        if (_isMouseMode)
        {
            MouseTest2();
        }

        // 0일 때 젤리에 컨트롤넘버 남아있나 확인해보기
        if (Input.touches.Length <= 0)
        {
            return;
        }

        foreach (Touch touch in Input.touches)
        {
            switch(touch.phase)
            {
                case TouchPhase.Began:
                    
                    break;
                case TouchPhase.Moved:
                    foreach (Jelly jelly in _jellys)
                    {
                        // jelly.Move(touch.fingerId, Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                        jelly.Move(touch.fingerId, touch.deltaPosition.x, touch.deltaPosition.y);                        
                    }
                    break;            
                    
                // 이건 젤리 위에서 손 안떼도 삭제 해야되니 맞음
                case TouchPhase.Ended:
                    foreach (Jelly jelly in _jellys)
                    {
                        jelly.RemoveControllTouchNumber(touch.fingerId);
                    }
                    break;                
            }            
        }            
    }    
    
    // JellyControllCanvas아래 컨트롤러들 Event Trigger에 들어감
    public void SetJellyControllTouchNumber(Jelly jelly)
    {
        if (!_handGuideController.IsInteracted)
        {

            //AudioManager.Instance.PlaySFX(Constants.)
            _handGuideController.IsInteracted = true;

            if (!jelly.gameObject.name.Contains("DolfinJelly"))
            {
                _handGuideController.GuideJelly.ReturnHome();
            }
        }        

        // 누른 애가 아니라 현재 터치중인 모든 애가 들어감
        foreach (Touch touch in Input.touches)
        {
            jelly.AddControllTouchNumber(touch.fingerId);
        }

        if(_isMouseMode)
        {
            jelly.AddControllTouchNumber(1);
        }
    }


    Jelly _mouseTestJelly; 

    public void MouseTest(Jelly jelly)
    {
        if (jelly.gameObject.name.Contains("DolfinJelly"))
        {
            _handGuideController.IsInteracted = true;
        }

        if (_isMouseMode)
        {
            _mouseTestJelly = jelly;
            _mouseTestJelly.AddControllTouchNumber(1);
        }
    }

    void MouseTest2()
    {
        if(_mouseTestJelly == null)
        {
            return;
        }        

        if(Input.GetMouseButtonUp(0))
        {
            _mouseTestJelly.RemoveControllTouchNumber(1);
            _mouseTestJelly = null;
        }

        // 가이드 때 이걸로 움직여야 자연스러울듯
        if(Input.GetMouseButton(0))
        {
            _mouseTestJelly.Move(1, Input.GetAxis("Mouse X") * 10, Input.GetAxis("Mouse Y") * 10);
        }
    }

}
