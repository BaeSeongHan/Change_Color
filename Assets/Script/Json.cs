using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Json : MonoBehaviour
{
    public MapData BLOCK_STATE; //제이슨 데이터
    public MapData BLOCK_COLOR; //제이슨 데이터

    public int ClearCount = 0;
}


[System.Serializable]
public class MapX
{
    public int[] mapX = new int[9]; //맵의 x값
}
[System.Serializable]
public class MapData
{
    public MapX[] map = new MapX[9];
}