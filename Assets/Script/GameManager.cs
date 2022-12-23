using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int sceneNum = 0;   //�� �ѹ�
    public int sceneState = 0;   //�� �ѹ�
    public bool IsPause = true;

    private void Awake()
    {
        if (instance != null)
        {

            Destroy(gameObject);
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
