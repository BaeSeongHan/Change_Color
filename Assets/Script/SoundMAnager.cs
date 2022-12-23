using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMAnager : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip clear; //Ŭ���� ����
    public AudioClip fail; //���� ����

    public AudioClip dontmove; //�������϶� �����̶�� �Է��ϸ� ����
    public AudioClip move; //�����϶� ����

    public AudioClip coin; //�� ä������

    public AudioClip MouseOn;

    public static SoundMAnager instance;

    void Awake()
    {
        if (SoundMAnager.instance == null)
        {
            SoundMAnager.instance = this;
        }
    }

    //�����϶�
    public void MoveSound()
    {
        audioSource.PlayOneShot(move);
    }
    //�������̴°� �����̷����ϸ�
    public void DontMoveSound()
    {
        audioSource.PlayOneShot(dontmove);
    }
    //Ŭ����
    public void ClearSound()
    {
        audioSource.PlayOneShot(clear);
    }
    //������
    public void FailSound()
    {
        audioSource.PlayOneShot(fail);
    }
    //����
    public void CoinSound()
    {
        audioSource.PlayOneShot(coin);
    }
    //���콺 Ŀ���� ������ ������
    public void MouseOnSound()
    {
        audioSource.PlayOneShot(MouseOn);
    }
}
