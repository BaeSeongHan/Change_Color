using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum MBTN_Type
{
    First,
    Second,
    Third,
    Fourth,
    Game
}


public class ManulUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public MBTN_Type currentType;
    public Transform buttonScale;
    Vector3 defaultScale;

    public GameObject first;
    public GameObject second;
    public GameObject third;
    public GameObject fourth;
    public GameObject game;


    private void Start()
    {
        defaultScale = buttonScale.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        switch (currentType)
        {
            case MBTN_Type.First:
                first.SetActive(true);
                second.SetActive(false);
                third.SetActive(false);
                fourth.SetActive(false);
                break;
            case MBTN_Type.Second:
                first.SetActive(false);
                second.SetActive(true);
                third.SetActive(false);
                fourth.SetActive(false);
                break;
            case MBTN_Type.Third:
                first.SetActive(false);
                second.SetActive(false);
                third.SetActive(true);
                fourth.SetActive(false);
                break;
            case MBTN_Type.Fourth:
                first.SetActive(false);
                second.SetActive(false);
                third.SetActive(false);
                fourth.SetActive(true);
                break;
            
            default:
                break;
        }

        SoundMAnager.instance.MouseOnSound();
        //SoundManger.instance.MouseOnSound();
        buttonScale.localScale = defaultScale * 1.1f;
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        if (first != null && second != null && third != null && fourth != null)
        {
            first.SetActive(false);
            second.SetActive(false);
            third.SetActive(false);
            fourth.SetActive(false);
        }
        

        buttonScale.localScale = defaultScale;
    }


    public void GameManul()
    {

        game.SetActive(!game.activeSelf);

    }
}
