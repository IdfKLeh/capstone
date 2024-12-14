using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using TMPro;


public class StartEventButtonController : MonoBehaviour
{
    [SerializeField] private StartEventCanvas startEventCanvas; //creating Canvas instance
    private StartEventEventHandler startEventEventHandler; //startEventEventHandler의 함수 사용 위함

    private void Start()
    {
        startEventEventHandler = FindObjectOfType<StartEventEventHandler>();
        DisplayNextDialogue();
    }//시작과 동시에 첫 dialogue 실행
    public void OnPointerClick()
    {
        DisplayNextDialogue();
    }//클릭시 다음 dialogue 출력
    private void DisplayNextDialogue()
    {
        if (startEventEventHandler.HasMoreDialogues())
        {
            string nextDialogue = startEventEventHandler.GetNextDialogue();
            setGrannyText(nextDialogue);
            if(!startEventEventHandler.HasMoreDialogues()){
                OpenOptions(startEventEventHandler.GetNumberOfOptions());
                // 다이얼로그가 끝났을 때의 처리를 여기서 수행
                Debug.Log("All dialogues completed.");
            }
        }
        
    }//event에 dialogue가 더 있는 경우 그것을 grannytext(startevent에서의 메인 dialogue)의 text로 설정. 마지막 dialogue라면 option이 떠오르도록 했음. 근데 만약 어떤 방식으로든 버그가 난다? 그럼 이새끼 때문인가보다 하셈.
    private void setGrannyText(string message)
    {
        startEventCanvas.grannyText.text = message;
    }//grannyText를 message로 설정
    private void OpenOptions(int numberOfOptions)
    {
        List<Button> buttons = new List<Button>();
        List<TextMeshProUGUI> buttonTexts = new List<TextMeshProUGUI>();

        switch (numberOfOptions)
        {
            case 1:
                buttons.Add(startEventCanvas.oneChoice);
                buttonTexts.Add(startEventCanvas.oneChoice.GetComponentInChildren<TextMeshProUGUI>());
                break;
            case 2:
                buttons.Add(startEventCanvas.twoChoices0);
                buttons.Add(startEventCanvas.twoChoices1);
                buttonTexts.Add(startEventCanvas.twoChoices0Text);
                buttonTexts.Add(startEventCanvas.twoChoices1Text);
                break;
            case 3:
                //미련하게 TMPRO를 안넣어서 canvas에 일일히 object를 지정해줬는데 굳이 그러지 말자. 아래 문장 사용하기.
                //startEventCanvas.threeChoices0.GetComponentInChildren<TextMeshProUGUI>().text = startEventEventHandler.GetOptionDialogue(0);
                buttons.Add(startEventCanvas.threeChoices0);
                buttons.Add(startEventCanvas.threeChoices1);
                buttons.Add(startEventCanvas.threeChoices2);
                buttonTexts.Add(startEventCanvas.threeChoices0Text);
                buttonTexts.Add(startEventCanvas.threeChoices1Text);
                buttonTexts.Add(startEventCanvas.threeChoices2Text);
                break;
            default:
                Debug.Log("cant find options");
                break;
        }

        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].gameObject.SetActive(true);
            buttonTexts[i].text = startEventEventHandler.GetOptionDialogue(i);
            int index = i; // Capture index for closure
            buttons[i].onClick.RemoveAllListeners(); // Clear previous listeners
            buttons[i].onClick.AddListener(() => OnOptionSelection(index, numberOfOptions));
        }
    }//option 버튼들을 활성화 하는 함수
    private void OnOptionSelection(int optionNumber,int numberOfOptions)
    {
        bool isThisLastEvent = startEventEventHandler.IsThisLastEvent();//코드가 멍청이 코드라서 여기에서 미리 체크를 한다음 변수에 저장해놓을 수 밖에 없음. 안그러면 option action실행과 동시에 다음 이벤트로 넘어가버리기 때문.
        startEventEventHandler.OnOptionSelected(optionNumber);
        List<Button> buttons = new List<Button>();
        switch (numberOfOptions)
        {
            case 1:
                buttons.Add(startEventCanvas.oneChoice);
                break;
            case 2:
                buttons.Add(startEventCanvas.twoChoices0);
                buttons.Add(startEventCanvas.twoChoices1);
                break;
            case 3:
                buttons.Add(startEventCanvas.threeChoices0);
                buttons.Add(startEventCanvas.threeChoices1);
                buttons.Add(startEventCanvas.threeChoices2);
                break;
            default:
                Debug.Log("cant erase options");
                break;
        }
        foreach (var button in buttons)
        {
            button.gameObject.SetActive(false);
        }
        if(isThisLastEvent)
        {
            Debug.Log("This is the final event!");
            SceneChange("MainPlay");
        }
        else
        {
            DisplayNextDialogue();
        }
        
    }//option 버튼들이 선택 이후 비활성화 되고, 마지막 이벤트일시 화면이 전환되도록 하는 함수
    private void openPopUpWindow(string message)//when PopUp is opened
    {
        startEventCanvas.popUpWindow.gameObject.SetActive(true);
        startEventCanvas.popUpWindow.yesButton.onClick.AddListener(onYesButtonClicked);
        startEventCanvas.popUpWindow.noButton.onClick.AddListener(onNoButtonClicked);
        startEventCanvas.popUpWindow.messageText.text = message;
    }//popup여는 함수

    private void onYesButtonClicked()//when clicking the yes button
    {
        startEventCanvas.popUpWindow.gameObject.SetActive(false);
        Debug.Log("yes clicked");
    }//pop의 yes가 눌렸을 때
 
    private void onNoButtonClicked()//when clicking the no button
    {
        startEventCanvas.popUpWindow.gameObject.SetActive(false);
        Debug.Log("no clicked");
    }//popup의 no가 눌렸을 때

    private void SceneChange(string targetLevel)//function for Scene Change through button clicks
    {
        startEventEventHandler.EndStartEvent(targetLevel);
    }//scene 전환 함수
}
