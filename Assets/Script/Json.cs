using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Json : MonoBehaviour
{
    public MapData BLOCK_STATE; //���̽� ������
    public MapData BLOCK_COLOR; //���̽� ������

    public int ClearCount = 0;
}


[System.Serializable]
public class MapX
{
    public int[] mapX = new int[9]; //���� x��
}
[System.Serializable]
public class MapData
{
    public MapX[] map = new MapX[9];
}