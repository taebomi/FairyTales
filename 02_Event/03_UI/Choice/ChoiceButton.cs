using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChoiceButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    // 버튼
    public Button button;
    public RectTransform rectTransform; // 버튼의 RectTransform
    [SerializeField] private Animator animator; // 버튼 애니메이터
    [SerializeField] private Image choiceButtonImage; // 버튼 이미지, 색상 변경 용도

    // 자식 오브젝트들
    [SerializeField] private TMP_Text choiceTMPText; // 버튼 내부 선택지 TMP text
    [SerializeField] private Image leftIconSpriteRenderer; // 버튼 내부 왼쪽 아이콘
    [SerializeField] private Image rightIconSpriteRenderer; // 버튼 내부 오른쪽 아이콘

    // 버튼 및 아이콘 스프라이트
    [SerializeField] private Sprite[] boxSpriteArray;       // 박스 색상별 스프라이트 리소스
    [SerializeField] private Sprite[] iconSpriteArray;      // 아이콘별 스프라이트 리소스

    // 현재 아이콘
    private Icon _currentIcon = Icon.Sword;
    
    
    
    public enum Icon
    {
        Standard = 1,
        Heart,
        Devil,
        Sword,
        Poop,
    }   // 버튼 아이콘 

    public enum Color
    {
        Red,
        Yellow,
        Green,
        Blue,
        Purple,
        Black,
        White,
    }   // 버튼 색상
    
    // 아이콘 색상 값 저장용도
    private static readonly UnityEngine.Color[] IconColorRGBArray = {
        new UnityEngine.Color(1f, 0.23f, 0.28f), // 빨강
        new UnityEngine.Color(1f, 0.75f, 0.26f), // 노랑
        new UnityEngine.Color(0.31f, 0.72f, 0.5f), // 초록
        new UnityEngine.Color(0.3f, 0.5f, 0.8f), // 파랑
        new UnityEngine.Color(0.62f, 0.3f, 0.68f), // 보라
        new UnityEngine.Color(0.23f, 0.23f, 0.23f), // 검정
        new UnityEngine.Color(1f, 1f, 1f) // 하양
    };      // 버튼 색상에 대응되는 컬러값 배열

    // ####################################  버튼 세팅  ####################################
    public void SetButton(string text, Color color, Icon icon)
    {
        choiceButtonImage.sprite = boxSpriteArray[(int) color];
        rightIconSpriteRenderer.color = leftIconSpriteRenderer.color = IconColorRGBArray[(int) color];
        rightIconSpriteRenderer.sprite = leftIconSpriteRenderer.sprite = iconSpriteArray[(int) icon - 1];
        _currentIcon = icon;
        choiceTMPText.text = text;
    }
    
    public void SetNavigation(Button prevBtn, Button nextBtn)
    {
        var customNav = new Navigation
        {
            mode = Navigation.Mode.Explicit,
            selectOnLeft = prevBtn,
            selectOnUp = prevBtn,
            selectOnRight = nextBtn,
            selectOnDown = nextBtn
        };
        button.navigation = customNav;
    }
    
    
    // ####################################  마우스 조작  #######################################
    public void OnPointerEnter(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (gameObject == EventSystem.current.currentSelectedGameObject)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    // ####################################  선택 / 해제 시  ####################################
    // 선택 시 아이콘 애니메이션 재생
    public void OnSelect(BaseEventData eventData)
    {
        animator.enabled = true;
        animator.SetInteger(AnimatorHash.Option, (int) _currentIcon);
    }

    // 선택 해제 시 아이콘 애니메이션 정지
    public void OnDeselect(BaseEventData eventData)
    {
        animator.enabled = false;
        rightIconSpriteRenderer.sprite = leftIconSpriteRenderer.sprite = iconSpriteArray[(int) _currentIcon -1];
    }

}