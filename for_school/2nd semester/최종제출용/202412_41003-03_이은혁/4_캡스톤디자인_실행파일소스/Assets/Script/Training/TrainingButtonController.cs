using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class TrainingButtonController : MonoBehaviour
{

    //[SerializeField] private PopUpWindow popUpWindow; // creating PopUpWindow instance
    [SerializeField] private TrainingCanvas trainingCanvas; //creating Canvas instance

    private void Awake()
    {
        trainingCanvas.mainMenuButton.onClick.AddListener(onFirstButtonClicked);
        //openPopUpWindow("are you sure?");
    }
    private void onFirstButtonClicked()
    {
        openPopUpWindow("first button clicked");
    }
    private void openPopUpWindow(string message)//when PopUp is opened
    {
        trainingCanvas.popUpWindow.gameObject.SetActive(true);

        //기존 리스너 삭제
        trainingCanvas.popUpWindow.yesButton.onClick.RemoveAllListeners();
        trainingCanvas.popUpWindow.noButton.onClick.RemoveAllListeners();

        //리스너 추가
        trainingCanvas.popUpWindow.yesButton.onClick.AddListener(() => onYesButtonClicked());
        trainingCanvas.popUpWindow.noButton.onClick.AddListener(() => onNoButtonClicked());
        trainingCanvas.popUpWindow.messageText.text = message;
    }

    private void onYesButtonClicked()//when clicking the yes button
    {
        trainingCanvas.popUpWindow.gameObject.SetActive(false);
        Debug.Log("yes clicked");
    }

    private void onNoButtonClicked()//when clicking the no button
    {
        trainingCanvas.popUpWindow.gameObject.SetActive(false);
        Debug.Log("no clicked");
    }

    public void sceneChange(string targetLevel)//fuction for Scene Change through button clicks
    {
        SceneManager.LoadScene(targetLevel);
    }
}
