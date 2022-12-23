using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public enum BTN_StageType
{
    Start,
    Stage,
    Manul,
    Game,
    Back,
    Exit
}


public class MainUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public BTN_StageType currentType;
    public Transform buttonScale;
    Vector3 defaultScale;

    public CanvasGroup mainGroup;
    public CanvasGroup manulGroup;
    public CanvasGroup stageGroup;

    public int stageNum; //몇스테이지인지 알려주기

    private void Start()
    {
        defaultScale = buttonScale.localScale;
    }

    


    public void OnStageBtnClick()
    {
        switch (currentType)
        {
            case BTN_StageType.Start:
                Time.timeScale = 1;
                GameManager.instance.IsPause = false;
                LoadingSceneController.LoadScene("Main");
                break;
            case BTN_StageType.Stage:
                CanvasGroupOff(mainGroup);
                CanvasGroupOff(manulGroup);
                CanvasGroupOn(stageGroup);
                break;
            case BTN_StageType.Manul:
                CanvasGroupOff(mainGroup);
                CanvasGroupOn(manulGroup);
                CanvasGroupOff(stageGroup);
                break;
            case BTN_StageType.Game:
                Time.timeScale = 1;
                GameManager.instance.sceneNum = stageNum;

                if (GameManager.instance.sceneNum == 1)
                {
                    LoadingSceneController.LoadScene("Game1");
                }
                else if (GameManager.instance.sceneNum == 2)
                {
                    LoadingSceneController.LoadScene("Game2");
                }
                else if (GameManager.instance.sceneNum == 3)
                {
                    LoadingSceneController.LoadScene("Game3");
                }
                else if (GameManager.instance.sceneNum == 4)
                {
                    LoadingSceneController.LoadScene("Game4");
                }
                else if (GameManager.instance.sceneNum == 5)
                {
                    LoadingSceneController.LoadScene("Game5");
                }
                else if (GameManager.instance.sceneNum == 6)
                {
                    LoadingSceneController.LoadScene("Game6");
                }
                else if (GameManager.instance.sceneNum == 7)
                {
                    LoadingSceneController.LoadScene("Game7");
                }
                else if (GameManager.instance.sceneNum == 8)
                {
                    LoadingSceneController.LoadScene("Game8");
                }
                else if (GameManager.instance.sceneNum == 9)
                {
                    LoadingSceneController.LoadScene("Game9");
                }
                else if (GameManager.instance.sceneNum == 10)
                {
                    LoadingSceneController.LoadScene("Game10");
                }

                break;
            case BTN_StageType.Back:
                CanvasGroupOn(mainGroup);
                CanvasGroupOff(manulGroup);
                CanvasGroupOff(stageGroup);
                break;
            case BTN_StageType.Exit:
                Application.Quit();
                Debug.Log("종료");
                break;
            default:
                break;
        }
    }


    public void CanvasGroupOn(CanvasGroup cg)
    {
        cg.alpha = 1;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }
    public void CanvasGroupOff(CanvasGroup cg)
    {
        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }



    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundMAnager.instance.MouseOnSound();
        //SoundManger.instance.MouseOnSound();
        buttonScale.localScale = defaultScale * 1.1f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonScale.localScale = defaultScale;
    }
}
