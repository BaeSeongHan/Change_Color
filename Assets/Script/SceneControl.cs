using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneControl : MonoBehaviour
{
    //public BlockRoot block_root = null;
    public GameObject block_root = null;
    void Start()
    {
        // BlockRoot ��ũ��Ʈ�� �����´�.
        // this.block_root = this.gameObject.GetComponent<BlockRoot>();

        /*
        if (GameManager.instance.sceneNum == 1)
        {

        }
        else if (GameManager.instance.sceneNum == 2)
        {

        }
        else if (GameManager.instance.sceneNum == 3)
        {

        }
        else if (GameManager.instance.sceneNum == 4)
        {

        }
        else if (GameManager.instance.sceneNum == 5)
        {

        }
        else if (GameManager.instance.sceneNum == 6)
        {

        }*/

        // BlockRoot ��ũ��Ʈ�� initialSetUp()�� ȣ���Ѵ�.
        this.block_root.GetComponent<BlockRoot>().initialSetUp();
    }
}
