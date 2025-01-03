using DG.Tweening;
using EnumTypes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public class ChatController : MonoBehaviour
{    
    [SerializeField] Chat _chatAPrefab;
    [SerializeField] Chat _chatBPrefab;
    [SerializeField] Chat _chatCPrefab;
    [SerializeField] Chat _chatDPrefab;

    [SerializeField] GameObject _chatAParent;
    [SerializeField] GameObject _chatBParent;
    [SerializeField] GameObject _chatCParent;
    [SerializeField] GameObject _chatDParent;

    [SerializeField, Range(0.1f, 2)] float _chatUpTime;
    [SerializeField, Range(5, 20)] int _chatSpace;
    [Header("입력창하고 거리")]
    [SerializeField, Range(90, 150)] int _distance;

    List<Chat> _chatList = new List<Chat>();
    IObjectPool<Chat> _chatAPool;
    IObjectPool<Chat> _chatBPool;
    IObjectPool<Chat> _chatCPool;
    IObjectPool<Chat> _chatDPool;

    IObjectPool<Chat> _currentPool;

    // 실전에서는 직렬화 필요없음
    [SerializeField] string _receiveID;
    [SerializeField, TextArea(3, 5)] string _receiveText;    

    bool _isNewChatMaking;
    bool _isReady;

    void Awake()
    {
        _chatAPool = new ObjectPool<Chat>(CreateAChat, OnGetChat, OnReleaseChat, OnDestroyChat);
        _chatBPool = new ObjectPool<Chat>(CreateBChat, OnGetChat, OnReleaseChat, OnDestroyChat);
        _chatCPool = new ObjectPool<Chat>(CreateCChat, OnGetChat, OnReleaseChat, OnDestroyChat);
        _chatDPool = new ObjectPool<Chat>(CreateDChat, OnGetChat, OnReleaseChat, OnDestroyChat);
    }

    Chat CreateAChat()
    {
        Chat chat = Instantiate(_chatAPrefab, _chatAParent.transform);        
        return chat;
    }

    Chat CreateBChat()
    {
        Chat chat = Instantiate(_chatBPrefab, _chatBParent.transform);
        return chat;
    }

    Chat CreateCChat()
    {
        Chat chat = Instantiate(_chatCPrefab, _chatCParent.transform);
        return chat;
    }

    Chat CreateDChat()
    {
        Chat chat = Instantiate(_chatDPrefab, _chatDParent.transform);
        return chat;
    }

    void OnGetChat(Chat chat)
    {        
        chat.gameObject.SetActive(true);
        chat.CanvasGroup.alpha = 1;
    }

    void OnReleaseChat(Chat chat)
    {
        chat.gameObject.SetActive(false);        
    }

    void OnDestroyChat(Chat chat)
    {
        Destroy(chat);
    }

    public void ReadyChat(BroadcastTheme theme)
    {
        switch(theme)
        {
            case BroadcastTheme.A:
                _currentPool = _chatAPool;
                break;
            case BroadcastTheme.B:
                _currentPool = _chatBPool;
                break;
            case BroadcastTheme.C:
                _currentPool = _chatCPool;
                break;
            case BroadcastTheme.D:
                _currentPool = _chatDPool;
                break;
        }

        MakeFirstChat();
        _isReady = true;
    }
    
    void MakeFirstChat()
    {
        _isNewChatMaking = true;
        Chat chat = _currentPool.Get();
        chat.SetChatText(id : "님만 기다림", text : "1등");
        _chatList.Add(chat);
        chat.RectTransform.anchoredPosition = Vector2.zero;
        chat.RectTransform.DOLocalMoveY(_distance, _chatUpTime).OnComplete(OnReadyFirstChat);        
    }

    void OnReadyFirstChat()
    {
        _isNewChatMaking = false;        
        GameInstance.Instance.MainCtrl.chatReceived.AddListener(OnReveivedChat);
        //GameInstance.Instance.IsMainSceneChatReady = true;
    }

    public void OnReveivedChat(string id, string message)
    {                
        MakeChat(id, message);
    }

    public void MakeChat(string id, string text)
    {
        //if (_isNewChatMaking || !_isReady)
        //{
        //    return;
        //}
        GameInstance.Instance.chatCount++;        

        _isNewChatMaking = true;

        Chat chat = _currentPool.Get();
        chat.SetChatText(id, text);
        SetChatPosition(chat);
        _chatList.Add(chat);

        MoveChat();

        //_subChatController.MakeChat(id, text);

    }

    void SetChatPosition(Chat chat)
    {        
        Chat lastChat = _chatList.Last();
        chat.RectTransform.anchoredPosition = new Vector2(chat.RectTransform.anchoredPosition.x, lastChat.YPosition - lastChat.YScale - _chatSpace);        
    }

    // 77 ~ 358
    void MoveChat()
    {
        Chat newChat = _chatList.Last();
        float moveRange = newChat.YScale;

        foreach (Chat chat in _chatList)
        {
            float targetAlpha = Mathf.InverseLerp(358, 77, chat.YPosition);

            if (chat == newChat)
            {
                chat.RectTransform.DOLocalMoveY(chat.YPosition + moveRange + _chatSpace, _chatUpTime).OnComplete(() => _isNewChatMaking = false).SetId("Chat");
                chat.CanvasGroup.DOFade(targetAlpha, _chatUpTime).OnComplete(() => ReleaseOldChat(chat)).SetId("Chat");
            }
            else
            {
                chat.RectTransform.DOLocalMoveY(chat.YPosition + moveRange + _chatSpace, _chatUpTime).SetId("Chat");
                chat.CanvasGroup.DOFade(targetAlpha, _chatUpTime).OnComplete(() => ReleaseOldChat(chat)).SetId("Chat");
            }
        }
    }

    void ReleaseOldChat(Chat chat)
    {
        if (chat.CanvasGroup.alpha > 0)
        {
            return;
        }

        _chatList.Remove(chat);
        _currentPool.Release(chat);
    }

    public void ClearAllChat()
    {
        GameInstance.Instance.MainCtrl.chatReceived.RemoveListener(OnReveivedChat);
        foreach (Chat chat in _chatList)
        {
            _currentPool.Release(chat);
        }

        _chatList.Clear();        
        _isReady = false;
    }
}
