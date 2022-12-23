using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMAnager : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip clear; //클리어 사운드
    public AudioClip fail; //실패 사운드

    public AudioClip dontmove; //못움직일때 움직이라고 입력하면 사운드
    public AudioClip move; //움직일때 사운드

    public AudioClip coin; //색 채웠을때

    public AudioClip MouseOn;

    public static SoundMAnager instance;

    void Awake()
    {
        if (SoundMAnager.instance == null)
        {
            SoundMAnager.instance = this;
        }
    }

    //움직일때
    public void MoveSound()
    {
        audioSource.PlayOneShot(move);
    }
    //못움직이는거 움직이려고하면
    public void DontMoveSound()
    {
        audioSource.PlayOneShot(dontmove);
    }
    //클리어
    public void ClearSound()
    {
        audioSource.PlayOneShot(clear);
    }
    //졌을때
    public void FailSound()
    {
        audioSource.PlayOneShot(fail);
    }
    //코인
    public void CoinSound()
    {
        audioSource.PlayOneShot(coin);
    }
    //마우스 커서에 가져다 댔을때
    public void MouseOnSound()
    {
        audioSource.PlayOneShot(MouseOn);
    }
}
