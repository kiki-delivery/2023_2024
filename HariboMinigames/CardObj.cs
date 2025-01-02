using Enums;
using UnityEngine;
using UnityEngine.UI;

public class Card
{
    public Card(Sprite frontBGImage, Sprite frontOBJImage, Sprite backImage, int number)
    {        
        this.frontBGImage = frontBGImage;
        this.frontOBJImage = frontOBJImage;
        this.backImage = backImage;
        this.number = number;
    }

    public int number;
    public Sprite frontBGImage;
    public Sprite frontOBJImage;
    public Sprite backImage;
}

public class CardObj : MonoBehaviour
{    
    [SerializeField] Image _frontBG;
    [SerializeField] Image _frontObj;
    [SerializeField] Image _backImage;
    [SerializeField] Animator _cardParentAnimator;
    [SerializeField] Animator _cardAnimator;
    [SerializeField] CardAnimationEventManager _cardAnimationEventManager;

    Button _button;
    Card _card;
    CardManager _cardManager;    
    CardState _currentState = CardState.Idle;

    bool _isTurned = false;
    bool _isTurnning = false;

    public int Number => _card.number;
    public CardManager CardManager => _cardManager;
    public bool IsTurnning => _isTurnning;
    public bool IsTurned
    {
        get => _isTurned;
        set
        {
            _isTurned = value;
            _button.interactable = !_isTurned;
        }
    }      

    public void Setting(Card card, CardManager cardManager)
    {
        _card = card;
        _cardManager = cardManager;        
        SetCard();

        void SetCard()
        {
            _button = GetComponent<Button>();
            _frontBG.sprite = _card.frontBGImage;
            _frontObj.sprite = _card.frontOBJImage;
            _backImage.sprite = _card.backImage;
            _button.onClick.AddListener(Click);

            void Click()
            {
                SetState(CardState.Turn);                
            }
        }
    }

    public void SetState(CardState state)
    {
        if (_currentState == state)
            return;

        _currentState = state;

        switch (_currentState)
        {
            case CardState.Turn:
                IsTurned = true;
                _cardAnimator.SetTrigger("Card_Turn");
                break;
            case CardState.Wrong:
                _cardAnimator.SetTrigger("Card_Wrong");
                break;
            case CardState.Correct:
                _cardAnimator.SetTrigger("Card_Correct");
                break;
            case CardState.Idle:
                IsTurned = false;
                _cardAnimator.SetTrigger("Card_Idle");
                break;
            case CardState.Start:
                _cardParentAnimator.SetTrigger("Card_Start");
                break;
            case CardState.End:
                _cardParentAnimator.SetTrigger("Card_End");
                break;
        }
    }        

    public void IsPreviewMode(bool isPreviewMode)
    {
        _cardAnimationEventManager.isPreviewMode = isPreviewMode;
    }    

    public void SetPickCard()
    {
        _isTurnning = true;
        _cardManager.PickCard(this);        
    }    

    public void ActionOfResult()
    {
        _isTurnning = false;
        _cardManager.ActionOfResult();
    }

    public void SetImage(Sprite frongImage, Sprite frontObjImage)
    {
        _frontBG.sprite = frongImage;
        _frontObj.sprite = frontObjImage;
    } 
}
