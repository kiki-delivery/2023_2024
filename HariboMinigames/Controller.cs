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

        // 0�� �� ������ ��Ʈ�ѳѹ� �����ֳ� Ȯ���غ���
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
                    
                // �̰� ���� ������ �� �ȶ��� ���� �ؾߵǴ� ����
                case TouchPhase.Ended:
                    foreach (Jelly jelly in _jellys)
                    {
                        jelly.RemoveControllTouchNumber(touch.fingerId);
                    }
                    break;                
            }            
        }            
    }    
    
    // JellyControllCanvas�Ʒ� ��Ʈ�ѷ��� Event Trigger�� ��
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

        // ���� �ְ� �ƴ϶� ���� ��ġ���� ��� �ְ� ��
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

        // ���̵� �� �̰ɷ� �������� �ڿ��������
        if(Input.GetMouseButton(0))
        {
            _mouseTestJelly.Move(1, Input.GetAxis("Mouse X") * 10, Input.GetAxis("Mouse Y") * 10);
        }
    }

}
