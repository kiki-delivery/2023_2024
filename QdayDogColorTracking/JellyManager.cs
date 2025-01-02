using UnityEngine;
using System.Collections;

public class JellyManager : MonoBehaviour
{
    [SerializeField] SettingData settingData;
    [Tooltip("�޽��� ������ Ÿ�̹� �����ؾ���")]
    [SerializeField] JellyMessageManager jellyMessageManager;

    [Header("���� ������Ʈ ����")]
    [SerializeField] GameObject[] jellys;
    [SerializeField] Material[] jellyMetarials;
    [SerializeField] GameObject[] jellyAniStartBtns;
    
    [Header("==========���� ����==========")]    
    [SerializeField, Range(1, 100)] int alphaChangeSpeed = 6;

    [Header("������ ����")]    
    [SerializeField] bool useRandomMotion;
    [SerializeField, Range(1, 10)] int motionSpeed = 5;
    [SerializeField, Range(0.1f, 1)] float sellectTime;
    
    bool isAnswering = false;
    WaitForSeconds waitMotionTime;

    void Start()
    {
        Setting();
    }

    void Setting()
    {
        motionSpeed = settingData.motionSpeed;
        waitMotionTime = new WaitForSeconds(0.01f * motionSpeed);
        alphaChangeSpeed = settingData.alphaChangeSpeed;
        useRandomMotion = settingData.useRandomMotion;
        sellectTime = settingData.sellectTime;
    }

    public void ActiveJelly(int jellyNum)
    {                
        if(useRandomMotion)
        {            
            StartCoroutine(IDoRandomMotion(jellyNum));
        }
        else
        {
            StartCoroutine(IAppearJelly(jellyNum));
        }
    }

    IEnumerator IDoRandomMotion(int jellyNum)
    {
        float currentTime = 0;

        while(currentTime < sellectTime)
        {
            for (int i = 0; i < jellys.Length; i++)
            {
                jellys[i].SetActive(true);
                yield return waitMotionTime;
                jellys[i].SetActive(false);
            }
            currentTime = currentTime + 0.1f;

            if (IsAnswerTiming(currentTime, sellectTime - 0.1f))
            {
                isAnswering = true;                
                jellyMessageManager.Answer(jellyNum);
            }

            //Debug.Log("���� �̴���");
        }

        jellys[jellyNum].SetActive(true);
        jellyAniStartBtns[jellyNum].SetActive(true);        
        //Debug.Log("���� ����");
    }    
    
    IEnumerator IAppearJelly(int jellyNum)
    {
        jellys[jellyNum].SetActive(true);
        Material mat = jellyMetarials[jellyNum];
        float alpha = 0;
        mat.SetFloat("_AlphaRange", alpha);

        while (alpha < 1)
        {
            alpha = alpha + 0.001f * alphaChangeSpeed;
            mat.SetFloat("_AlphaRange", alpha);            
            yield return null;

            if (IsAnswerTiming(alpha, 0.8f))
            {
                isAnswering = true;
                jellyMessageManager.Answer(jellyNum);
            }
            //Debug.Log("���� ������ ��");
        }

        mat.SetFloat("_AlphaRange", 1);
        //Debug.Log("���� ����");
    }

    bool IsAnswerTiming(float changeValue, float answerTimingThreshold)
    {
        if (isAnswering)
        {
            return false;
        }
        return changeValue > answerTimingThreshold;
    }    

    public void DissolveJelly(int jellyNum)
    {
        jellyMessageManager.Question();
        isAnswering = false;
        StartCoroutine(IDissolveJelly(jellyNum));
    }

    IEnumerator IDissolveJelly(int jellyNum)
    {
        Material mat = jellyMetarials[jellyNum];
        float alpha = 1;

        while (alpha > 0)
        {            
            alpha = alpha - 0.001f * alphaChangeSpeed;
            mat.SetFloat("_AlphaRange", alpha);            
            yield return null;
        }
        
        jellys[jellyNum].SetActive(false);        
        jellyAniStartBtns[jellyNum].SetActive(false);
        mat.SetFloat("_AlphaRange", 1);                
    }    
}
