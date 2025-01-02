using HKY;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SensorInputController : MonoBehaviour
{    
    [Header("연속 입력 방지 시간")]
    public int reInputDefenceTime;

    [Header("같은 위치로 취급할 거리")]
    public int sameInputDistance;

    [Header("인식이 필요한 시간")]
    public float checkTime = 0.001f;

    [Header("오프셋")]
    public Vector2Int offSet;

    [Header("정적 데이터")]
    [SerializeField] HomoGraphyData homoGraphyData;

    [Header("동적 데이터")]
    [SerializeField] InputEvents inputEvents;    
            
    [HideInInspector] public UnityEvent<Vector2> inputEvent;
    [HideInInspector] public bool isDragMode;
    [HideInInspector] public float lastBirthTime = 0;

    [SerializeField] SensorDetectorManager sensorDetectorManager;
    [SerializeField] Toggle toggle;

    InputFilterData inputFilterData;
    WaitForSeconds waitTime;

    List<DefenceArea> defenceAreas;
    List<Vector2> preTouchPositions;
    List<ProcessedObject> inputList;
        
    void Awake()
    {
        inputFilterData = GetComponent<InputFilterData>();
        inputList = new List<ProcessedObject>();
        defenceAreas = inputFilterData.defenceAreas;
        preTouchPositions = inputFilterData.preTouchPositions;
        waitTime = new WaitForSeconds(reInputDefenceTime);
        AddInteractionEvent();
    }

    public void Update()
    {
        SetInputList();
        FilterInputList();
        inputList.Clear();
    }

    void SetInputList()
    {
        for (int i = 0; i < sensorDetectorManager.useSensorDetectors.Count; i++)
        {
            inputList.AddRange(sensorDetectorManager.useSensorDetectors[i].GetObjects(checkTime));   
        }
    }

    void FilterInputList()
    {
        for (int i = 0; i < inputList.Count; i++)
        {
            if (!isDragMode)
            {
                if (IsOldInput(i))
                {
                    continue;
                }
            }

            lastBirthTime = inputList[i].birthTime;

            Vector2 inputCaliPo = GetCalibrationPosition(inputList[i].position, inputList[i].sensorNum);

            if (IsDefencePositionTouch(inputCaliPo))
            {
                continue;
            }

            if (IsSamePositionTouch(inputCaliPo))
            {
                continue;
            }

            inputCaliPo = inputCaliPo + offSet;

            DoInputEvent(inputCaliPo);
        }
    }

    bool IsOldInput(int i)
    {
        return inputList[i].birthTime <= lastBirthTime;
    }

    bool IsDefencePositionTouch(Vector2 inputMousePosition)
    {
        if(defenceAreas.Count == 0)
        {
            return false;
        }

        for(int i = 0; i < defenceAreas.Count; i++)
        {            
            if (inputMousePosition.x < defenceAreas[i].defenceInputArea.defenceStartPosition.x) continue;
            if (inputMousePosition.x > defenceAreas[i].defenceInputArea.defenceEndPosition.x) continue;
            if (inputMousePosition.y < defenceAreas[i].defenceInputArea.defenceStartPosition.y) continue;
            if (inputMousePosition.y < defenceAreas[i].defenceInputArea.defenceEndPosition.y) return true;            
        }

        return false;
    }

    bool IsSamePositionTouch(Vector2 inputMousePosition)
    {
        for(int i = 0; i < preTouchPositions.Count; i++)
        {
            float distance = Vector2.Distance(inputMousePosition, preTouchPositions[i]);            

            if (distance < sameInputDistance)
            {
                return true;                
            }
        }

        return false;
    }

    void DoInputEvent(Vector2 inputCaliPo)
    {
        inputEvent.Invoke(inputCaliPo);

        preTouchPositions.Add(inputCaliPo);
        StartCoroutine(PopReservation(inputCaliPo));
    }

    IEnumerator PopReservation(Vector2 popItem)
    {
        // todo 실전에서는 미리 만들어 놓기
        yield return new WaitForSeconds(reInputDefenceTime);
        preTouchPositions.Remove(popItem);
    }


    Vector2 GetCalibrationPosition(Vector2 inputMousePosition, int sensorNum)
    {
        float newX = (inputMousePosition.x * homoGraphyData.homographies[sensorNum].arr[0]) + (inputMousePosition.y * homoGraphyData.homographies[sensorNum].arr[1]) + homoGraphyData.homographies[sensorNum].arr[2];
        float newY = (inputMousePosition.x * homoGraphyData.homographies[sensorNum].arr[3]) + (inputMousePosition.y * homoGraphyData.homographies[sensorNum].arr[4]) + homoGraphyData.homographies[sensorNum].arr[5];
        float newZ = (inputMousePosition.x * homoGraphyData.homographies[sensorNum].arr[6]) + (inputMousePosition.y * homoGraphyData.homographies[sensorNum].arr[7]) + homoGraphyData.homographies[sensorNum].arr[8];

        Vector2 caliPo = new Vector2(newX / newZ, newY / newZ);
        return caliPo;
    }

    public void AddInteractionEvent()
    {
        inputEvent.AddListener(inputEvents.inputEvent.Invoke);
    }

    public void RemoveInteractionEvent()
    {
        inputEvent.RemoveListener(inputEvents.inputEvent.Invoke);
    }



    // ini 저장용 코드

    public void SaveSettingData(INIParser ini)
    {        
        ini.WriteValue("InputManager", "DistanceLimit", sameInputDistance);
        ini.WriteValue("InputManager", "DefenceSameInputTime", reInputDefenceTime);
        ini.WriteValue("InputManager", "IsDragMode", isDragMode);
        ini.WriteValue("InputManager", "CheckTime", checkTime);
        ini.WriteValue("InputManager", "offsetX", offSet.x);
        ini.WriteValue("InputManager", "offsetY", offSet.y);
    }

    public void LoadSettingData(INIParser ini)
    {
        sameInputDistance = ini.ReadValue("InputManager", "DistanceLimit", 50);
        reInputDefenceTime = ini.ReadValue("InputManager", "DefenceSameInputTime", 1);                
        int iIsDragMode = ini.ReadValue("InputManager", "IsDragMode", 0);
        isDragMode = iIsDragMode == 0 ? false : true;
        toggle.isOn = isDragMode;
        double dcheckTime = ini.ReadValue("InputManager", "CheckTime", 0f);
        checkTime = Convert.ToSingle(dcheckTime);

        int offsetX = ini.ReadValue("InputManager", "offsetX", 0);
        int offsetY = ini.ReadValue("InputManager", "offsetY", 0);
        offSet = new Vector2Int(offsetX, offsetY);
    }                    

}

