using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneControl : MonoBehaviour
{
    //public BlockRoot block_root = null;
    public GameObject block_root = null;
    void Start()
    {
        // BlockRoot 스크립트를 가져온다.
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

        // BlockRoot 스크립트의 initialSetUp()을 호출한다.
        this.block_root.GetComponent<BlockRoot>().initialSetUp();
    }
}
