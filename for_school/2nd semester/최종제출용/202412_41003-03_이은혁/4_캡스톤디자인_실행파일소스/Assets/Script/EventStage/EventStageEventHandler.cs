using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventStageEventNameSpace;
using System.IO;
using Newtonsoft.Json;
using System.Text;
using UnityEngine.SceneManagement;


public class EventStageEventHandler : MonoBehaviour
{
    public List<EventStageEvent> eventStageEvent= new List<EventStageEvent>();
    private EventStageEvent currentEvent;
    private UserController userController;
    private StageInfo stageBefore;//이전 스테이지의 정보를 저장하는 변수
    private string currentEventStageIDForLoading;//로딩을 위해 필요한 경우 현재 이벤트 스테이지의 ID를 저장하는 변수
    public List<int> passedOptionIndexes = new List<int>();//옵션 중 무엇이 표시되는지 번호를 저장해놓는 리스트
    private string targetAfterStage; //보통의 경우 MainPlay로 가야하지만 보스 스테이지 등의 특수 상황의 경우 어디로 갈지 목적지를 적어두는 변수
    void Start()
    {
        userController = FindObjectOfType<UserController>();
        stageBefore = userController.GetStageBefore();
        switch (stageBefore.stageKind)
        {
            case "BattleStage":
                LoadEventStageEvents("BattleEventStageList.json");
                break;
            case "TrainingStage":
                LoadEventStageEvents("TrainingEventStageList.json");
                break;
            case "BossStage":
                LoadEventStageEvents("BossEventStageList.json");
                break;
            default:
                LoadEventStageEvents(stageBefore.stageKind);
                break;
        }
        SetEvent(stageBefore.stageType);
    }//시작 설정과 마지막 스테이지의 정보를 가져옴(전투, 훈련, 보스 등). 해당 정보에 따라 필요한 스테이지 로드

    void LoadEventStageEvents(string fileName)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath,"EventStages", fileName);
        
        Debug.Log("Loading events from: "+ filePath);

        if (File.Exists(filePath))
        {
            try
            {
                string json = File.ReadAllText(filePath);
                Debug.Log("JSON content: " + json);

                var root = JsonConvert.DeserializeObject<Root>(json);
                if(root != null)
                {
                    eventStageEvent = root.events;
                    Debug.Log("Events loaded successfully.");
                }
                else
                {
                    Debug.LogError("Failed to parse Json.");
                }
            }
            catch (JsonException ex)
            {
                Debug.LogError("Error parsing Json: "+ ex.Message);
            }
            
        }
        else
        {
            Debug.LogError("Cannot find file"+filePath);
        }
    }//json에서 이벤트들을 읽어 eventStageEvent에 저장함.

    void SetEvent(string type){
        if(userController.GetCurrentEvent() != "Empty" )
        {
            currentEventStageIDForLoading = userController.GetCurrentEvent();
            SetCurrentEvent(GetEventsByID(currentEventStageIDForLoading));
            return;
        }

        string karma = userController.GetKarma("medStat", false);

        List<EventStageEvent> events = GetEventsByTypeAndKarma(type,karma);
        
        if (events == null || events.Count == 0)
        {
            Debug.LogError("No events found for stage " + type);
            return; // Null이나 빈 리스트가 있을 때 처리
        }

        SetCurrentEvent(events[GetRandomEventNumber(events.Count)]);
    }//현재 이벤트로 특정 타입과 카르마의 이벤트 중 랜덤한 걸 뽑아 설정하는 함수.

    void SetCurrentEvent(EventStageEvent gameEvent)
    {
        if (gameEvent == null)
        {
            Debug.LogError("The gameEvent is null.");
            return;
        }
        currentEvent = gameEvent;
        userController.SetCurrentEvent(currentEvent.eventID);//로딩을 위해 현재 이벤트 스테이지의 ID를 저장
        userController.SaveData();
        Debug.Log("Current event set: " + currentEvent.eventID);
    }//대상 EventStageEvent를 currentEvent로 설정하는 함수

    int GetRandomEventNumber(int eventListCount)
    {
        return Random.Range(0, eventListCount);
    }//주어진 이벤트 수 중에서 랜덤한 번호 출력

    EventStageEvent GetEventsByID(string eventID)
    {
        Debug.Log("Setting event for ID: "+ eventID);
        if (eventStageEvent == null)
        {
            Debug.LogError("eventStageEvent list is null.");
            return null;
        }
        foreach(EventStageEvent e in eventStageEvent)
        {
            if (e == null)
            {
                Debug.LogWarning("Encountered a null StartEvent object.");
                continue;
            }
            if (e.eventID == eventID)
            {
                return e;
            }
        }
        Debug.LogWarning("No events found for the given ID.");
        return null;
    }

    List<EventStageEvent> GetEventsByTypeAndKarma(string type, string karma){
        Debug.Log("Setting event for type: "+ type+ ", and karma: "+ karma);
        if (eventStageEvent == null)
        {
            Debug.LogError("eventStageEvent list is null.");
            return null;
        }

        List<EventStageEvent> eventsForType = new List<EventStageEvent>();

        foreach(EventStageEvent e in eventStageEvent)
        {
            if (e == null)
            {
                Debug.LogWarning("Encountered a null StartEvent object.");
                continue;
            }

            if (e.eventType == type && e.eventKarma == karma)
            {
                eventsForType.Add(e);
            }
        }
        if (eventsForType.Count == 0)
        {
            Debug.LogWarning("No events found for the given type and karma.");
        }
        return eventsForType;
    }//type(전 이벤트가 승리인지 패배인지)와 karma(medStat등의 스탯에 따라 운으로 정해지는 값)에 따라 이벤트들을 걸러내는 함수

    public string GetEventDialogue(){
        
        if(currentEvent != null)
        {
            return DialogueExtractor(currentEvent.dialogues);
        }
        return null;
    }//이벤트 시작시 첫 dialogue를 반환하는 함수
    
    public string DialogueExtractor(List<Dialogue> dialogues){

        StringBuilder completedDialogue = new StringBuilder();

        foreach(Dialogue e in dialogues){
                bool allRestrictionsPassed = true; // 모든 restriction이 통과했는지 확인하는 플래그

                foreach(Restriction a in e.restriction)
                {
                    if(!userController.IsRestrictionPassed(a))
                    {
                        allRestrictionsPassed = false; // 하나라도 통과하지 못하면 false로 설정
                        break; // 더 이상 확인할 필요가 없으므로 루프 탈출
                    }
                }

                if(allRestrictionsPassed)
                {
                    completedDialogue.Append(e.text);
                    completedDialogue.Append(" ");
                }  
            }
            return completedDialogue.ToString();
    }//각 dialogue를 추가하기 전에 restriction을 체크하여 통과했을시에만 표시할 string에 추가하는 함수

    public int VisibleOptionCount(){

        passedOptionIndexes.Clear();

        int optionCount = 0;
        if(currentEvent == null){
            Debug.LogError("The gameEvent is null.");
            return 0;
        }
        for(int i = 0; i < currentEvent.allOptions.Count; i++){
            Option e = currentEvent.allOptions[i];
            bool allRestrictionsPassed = true; // 모든 restriction이 통과했는지 확인하는 플래그

            foreach(Restriction a in e.restriction)
            {
                if(!userController.IsRestrictionPassed(a))
                {
                    allRestrictionsPassed = false; // 하나라도 통과하지 못하면 false로 설정
                    break; // 더 이상 확인할 필요가 없으므로 루프 탈출
                }
            }
            if (allRestrictionsPassed)
            {
                optionCount += 1;
                passedOptionIndexes.Add(i);
            }
        }
        return optionCount;
    }//유저가 볼 수 있는 옵션을 세고, 어떤 옵션들이 들어갈 것인지 index를 기록해놓는 함수

    public string GetOptionText(int optionIndex, int numberOfOptions){
            return currentEvent.allOptions[passedOptionIndexes[optionIndex]].text;
    }//옵션의 텍스트를 반환하는 함수

    public void ExecuteAction(int selectedOptionIndex){
        int realOptionIndex = passedOptionIndexes[selectedOptionIndex];

        foreach(Action e in currentEvent.allOptions[realOptionIndex].action){
            switch(e.type)
            {
                case "statChange":
                    userController.ChangeStat(e.amount,e.stats);
                    break;
                case "itemGet":
                    userController.AddItem(e.item);
                    break;
                case "healthChange":
                    userController.AddCurrentHealth(e.amount);
                    break;
                case "moneyGet":
                    userController.AddMoney(e.amount);
                    break;
                default:
                    Debug.Log("Unknown type of action"+e.type);
                    break;
            }
        }
        userController.EmptyCurrentEvent();
        userController.SetStageBefore("EventStage", "Neutral");
        userController.SaveData();
    }//action 실행 함수. **이건 나중 아이디어, 만약 후에 또 다른 이벤트로 이어지는 이벤트를 만들거라면, 액션의 종류를 "eventChange" 이런 걸로 넣어서 다른 이벤트로 연결되도록 설정.

    public string GetEventAfterDialogue(int selectedOptionIndex){
        int realOptionIndex = passedOptionIndexes[selectedOptionIndex];

        return DialogueExtractor(currentEvent.allOptions[realOptionIndex].afterText);
    }//이벤트에서 옵션 선택 후 나오는 dialogue를 반환하는 함수

    public string GetAfterOptionText(int selectedOptionIndex){
        return currentEvent.allOptions[passedOptionIndexes[selectedOptionIndex]].afterOptionText;
    }//afterOptionText, 즉 선택후 아래 버튼에 나오는 텍스트 반환하는 함수. 코드가 거지같아 보이지만 사실 최선임. 굳이 메모리 할당하기 싫었고, 위의 옵션text반환 함수랑 합쳐볼까 했는데 비효율적.

    public void ChangeStage(){

        switch(targetAfterStage)
        {
            case "MainPlay":
                SceneChange("MainPlay");
                break;
            case "Battle":
                break;
            default:
                SceneChange("MainPlay");
                break;
        }
    }//다음 스테이지가 뭔지 확인하고 바꿔주는 함수

    private void SceneChange(string stageName){
        userController.SetStageBefore("EventStage","Neutral");
        userController.SaveData();

        SceneManager.LoadScene(stageName);
    }//다음 스테이지를 전달받아 정보 저장후 그걸로 바꿔주는 함수.
}
