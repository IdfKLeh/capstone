using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EventStageButtonController : MonoBehaviour
{
    [SerializeField] private EventStageCanvas eventStageCanvas;
    private EventStageEventHandler eventStageEventHandler;
    private void Start()
    {
        eventStageEventHandler = FindObjectOfType<EventStageEventHandler>();
        DisplayEvent();
    }//시작할 때 이벤트 전체 디스플레이
    private void DisplayEvent()
    {
        string eventDialogue = eventStageEventHandler.GetEventDialogue();
        SetEventOptions(eventStageEventHandler.VisibleOptionCount());
        SetEventText(eventDialogue);
    }//옵션과 다이얼로그 세팅
    private void SetEventText(string message)
    {
        // Scroll View의 Content에 있는 TextMeshProUGUI를 가져옵니다.
        TextMeshProUGUI textComponent = eventStageCanvas.eventStageDialogue
            .GetComponentInChildren<ScrollRect>() // Scroll View에서
            .content // Content RectTransform
            .GetComponentInChildren<TextMeshProUGUI>(); // Content 안의 TextMeshProUGUI

        textComponent.text = message;

        // 텍스트 길이에 따라 크기를 업데이트합니다.
        Canvas.ForceUpdateCanvases(); // 강제로 레이아웃 갱신
        LayoutRebuilder.ForceRebuildLayoutImmediate(textComponent.rectTransform);
    }//메시지를 전달 받으면 해당 메시지로 텍스트 설정
    private void SetEventOptions(int numberOfOptions){
        List<Button> buttons = new List<Button>();
        List<TextMeshProUGUI> buttonTexts = new List<TextMeshProUGUI>();

        switch (numberOfOptions)
        {
            case 1:
                eventStageCanvas.oneButtonGroup.SetActive(true);
                buttons.AddRange(eventStageCanvas.oneButtonGroup.GetComponentsInChildren<Button>());
                buttonTexts.AddRange(eventStageCanvas.oneButtonGroup.GetComponentsInChildren<TextMeshProUGUI>());
                break;
            case 2:
                eventStageCanvas.twoButtonGroup.SetActive(true);
                buttons.AddRange(eventStageCanvas.twoButtonGroup.GetComponentsInChildren<Button>());
                buttonTexts.AddRange(eventStageCanvas.twoButtonGroup.GetComponentsInChildren<TextMeshProUGUI>());
                break;
            case 3:
                eventStageCanvas.threeButtonGroup.SetActive(true);
                buttons.AddRange(eventStageCanvas.threeButtonGroup.GetComponentsInChildren<Button>());
                buttonTexts.AddRange(eventStageCanvas.threeButtonGroup.GetComponentsInChildren<TextMeshProUGUI>());
                break;
            case 4:
                eventStageCanvas.fourButtonGroup.SetActive(true);
                buttons.AddRange(eventStageCanvas.fourButtonGroup.GetComponentsInChildren<Button>());
                buttonTexts.AddRange(eventStageCanvas.fourButtonGroup.GetComponentsInChildren<TextMeshProUGUI>());
                break;
            default:
                Debug.Log("cant find options");
                break;
        }
        Debug.Log("buttonTexts count: " + buttonTexts.Count);

        for (int i = 0; i < numberOfOptions; i++)
        {
            Debug.Log("i = "+i);
            buttonTexts[i].text = eventStageEventHandler.GetOptionText(i,numberOfOptions);
            int index = i; // Capture index for closure
            buttons[i].onClick.RemoveAllListeners(); // Clear previous listeners
            buttons[i].onClick.AddListener(() => OnOptionSelection(index, numberOfOptions));
        }
    }//option 버튼들을 활성화 하는 함수.

    private void OnOptionSelection(int optionNumber, int numberOfOptions){
        eventStageEventHandler.ExecuteAction(optionNumber);

        switch(numberOfOptions)
        {
            case 1:
                eventStageCanvas.oneButtonGroup.SetActive(false);
                break;
            case 2:
                eventStageCanvas.twoButtonGroup.SetActive(false);
                break;
            case 3:
                eventStageCanvas.threeButtonGroup.SetActive(false);
                break;
            case 4:
                eventStageCanvas.fourButtonGroup.SetActive(false);
                break;
            default:
                Debug.Log("Dont know how many options are here");
                break;
        }
        DisplayAfterEvent(optionNumber);

    }//옵션 선택시 버튼들을 모두 없애고 이벤트 이후 텍스트 불러옴.

    private void DisplayAfterEvent(int optionNumber){
        SetEventText(eventStageEventHandler.GetEventAfterDialogue(optionNumber));

        eventStageCanvas.oneButtonGroup.SetActive(true);
        eventStageCanvas.oneButtonGroup.GetComponentInChildren<TextMeshProUGUI>().text = eventStageEventHandler.GetAfterOptionText(optionNumber); 

        eventStageCanvas.oneButtonGroup.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        eventStageCanvas.oneButtonGroup.GetComponentInChildren<Button>().onClick.AddListener(() => eventStageEventHandler.ChangeStage());
    }//옵션 선택 후에 나타나는 텍스트 설정 함수
}
//일단은 대강 된듯... 전투 씬 만들러가자..