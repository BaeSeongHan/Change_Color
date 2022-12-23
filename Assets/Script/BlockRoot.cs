using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockRoot : MonoBehaviour
{
    public bool isDie = false; //죽었는지 체크
    public bool isClear = false; //클리어 했는지 체크
    public bool isDieTrue = false; 
    public bool isClearTrue = false;

    public Text dietext;


    public GameObject C; //클리어 UI
    public GameObject D; //다이 UI

    public GameObject J; //제이슨 파일 가져올거

    public GameObject BlockPrefab = null; // 만들어낼 블록의 프리팹.

    public BlockControl[,] blocks; // 그리드.
                                   // 블록을 만들어 내고 가로 9칸, 세로 9칸에 배치한다.

    ///블록 픽업--------------
    private GameObject main_camera = null; // 메인 카메라.
    private BlockControl grabbed_block = null; // 잡은 블록.
    //---------------------

    void Start()
    {
        this.main_camera = GameObject.FindGameObjectWithTag("MainCamera");
    } // 카메라로부터 마우스 커서를 통과하는 광선을 쏘기 위해서 필요

    void Update()
    { // 마우스 좌표와 겹치는지 체크한다. 잡을 수 있는 상태의 블록을 잡는다.
        Vector3 mouse_position; // 마우스 위치.
        this.unprojectMousePosition(out mouse_position, Input.mousePosition); // 마우스 위치를 가져온다.
        Vector2 mouse_position_xy = new Vector2(mouse_position.x, mouse_position.y); // 가져온 마우스 위치를 하나의 Vector2로 모은다.
        if (this.grabbed_block == null)
        { // 잡은 블록이 비었으면.
          // if(!this.is_has_falling_block()) { // 나중에 주석 해제
            if (Input.GetMouseButtonDown(0))
            { // 마우스 버튼이 눌렸으면
                foreach (BlockControl block in this.blocks)
                { // blocks 배열의 모든 요소를 차례로 처리한다.
                    if (!block.isGrabbable())
                    { // 블록을 잡을 수 없다면.
                        continue;
                    } // 루프의 처음으로 점프한다.
                    if (!block.isContainedPosition(mouse_position_xy))
                    { // 마우스 위치가 블록 영역 안이 아니면.
                        continue;
                    } // 루프의 처음으로 점프한다.
                    this.grabbed_block = block; // 처리 중인 블록을 grabbed_block에 등록한다.
                    this.grabbed_block.beginGrab(); break;
                } // 잡았을 때의 처리를 실행한다.
            } // }
        }
        else
        {

            //블록 교체------------------------------------------------------------------------------------
            do
            {
                if (grabbed_block.GetComponent<BlockControl>().prefab == (Block.PREFAB)1)
                {
                    BlockControl swap_target = this.getNextBlock(grabbed_block, grabbed_block.slide_dir); // 슬라이드할 곳의 블록을 가져온다.
                    if (swap_target == null)
                    { // 슬라이드할 곳 블록이 비어 있으면.
                        break;
                    } // 루프 탈출.
                    if (!swap_target.isGrabbable())
                    { // 슬라이드할 곳의 블록이 잡을 수 있는 상태가 아니라면.
                        break;
                    } // 루프 탈출.
                      // 현재 위치에서 슬라이드 위치까지의 거리를 얻는다.
                    float offset = this.grabbed_block.calcDirOffset(mouse_position_xy, this.grabbed_block.slide_dir);
                    if (offset < Block.COLLISION_SIZE / 2.0f)
                    { // 수리 거리가 블록 크기의 절반보다 작다면.
                        break;
                    } // 루프 탈출.
                    this.swapBlock(grabbed_block, grabbed_block.slide_dir, swap_target); // 블록을 교체한다.
                    this.grabbed_block = null; // 지금은 블록을 잡고 있지 않다.
                }

            } while (false);
            //-------------------------------------------------------------------------------------





            // 블록을 잡았을 때.
            if (!Input.GetMouseButton(0))
            { // 마우스 버튼이 눌려져 있지 않으면.
                this.grabbed_block.endGrab(); // 블록을 놨을 때의 처리를 실행.
                this.grabbed_block = null;
            } // grabbed_block을 비우게 설정.
        }

        //EndGame();
        CKKKKKK();
        ChangeUI();
        End();

        if (Input.GetKeyDown(KeyCode.B) && isDie != true)
        {
            GoBack();
        }
    }
    //블록 픽업------------------------------------------------------------
    // ref와 out 둘 다 값을 복사하지 않고, 참조를 통해 매개 변수를 직접 변경함, ref는 초기화가 필요하나, out은 불필요함.
    // 때문에, out으로 받아온 매개 변수는 함수 안에서 대입이 필요함
    public bool unprojectMousePosition(out Vector3 world_position, Vector3 mouse_position)
    {
        bool ret;
        // 판을 작성한다. 이 판은 카메라에 대해서 뒤로 향해서(Vector3.back).
        // 블록의 절반 크기만큼 앞에 둔다.
        Plane plane = new Plane(Vector3.back, new Vector3(0.0f, 0.0f, -Block.COLLISION_SIZE / 2.0f));
        // 카메라와 마우스를 통과하는 빛을 만든다.
        Ray ray = this.main_camera.GetComponent<Camera>().ScreenPointToRay(mouse_position);
        float depth;
        // 광선(ray)이 판(plane)에 닿았다면,
        if (plane.Raycast(ray, out depth))
        { // depth에 정보가 기록된다.
          // 인수 world_position을 마우스 위치로 덮어쓴다.
            world_position = ray.origin + ray.direction * depth;
            ret = true;
        }
        else
        { // 닿지 않았다면.
          // 인수 world_position을 0인 벡터로 덮어쓴다.
            world_position = Vector3.zero;
            ret = false;
        }
        return (ret); // 카메라를 통과하는 광선이 블록에 닿았는지를 반환
    }
    //-------------------------------------------------------------------------------------

    //블록 교체------------------------------------------------------------------------------------
    public BlockControl getNextBlock(BlockControl block, Block.DIR4 dir)
    { // 인수로 지정된 블록과 방향으로 블록이 슬라이드할 곳에 어느 블록이 있는지 반환.
        BlockControl next_block = null; // 슬라이드할 곳의 블록을 여기에 저장.
        switch (dir)
        {
            case Block.DIR4.RIGHT:
                if (block.i_pos.x < Block.BLOCK_NUM_X - 1)
                { // 그리드 안이라면.
                    next_block = this.blocks[block.i_pos.x + 1, block.i_pos.y];
                }
                break;
            case Block.DIR4.LEFT:
                if (block.i_pos.x > 0)
                { // 그리드 안이라면.
                    next_block = this.blocks[block.i_pos.x - 1, block.i_pos.y];
                }
                break;
            case Block.DIR4.UP:
                if (block.i_pos.y < Block.BLOCK_NUM_Y - 1)
                { // 그리드 안이라면.
                    next_block = this.blocks[block.i_pos.x, block.i_pos.y + 1];
                }
                break;
            case Block.DIR4.DOWN:
                if (block.i_pos.y > 0)
                { // 그리드 안이라면.
                    next_block = this.blocks[block.i_pos.x, block.i_pos.y - 1];
                }
                break;
        }
        return (next_block);
    }

    public static Vector3 getDirVector(Block.DIR4 dir)
    { // 인수로 지정된 방향으로 현재 블록에서 지정 방향으로 이동하는 양 반환.
        Vector3 v = Vector3.zero;
        switch (dir)
        {
            case Block.DIR4.RIGHT: v = Vector3.right; break; // 오른쪽으로 1단위 이동.
            case Block.DIR4.LEFT: v = Vector3.left; break; // 왼쪽으로 1단위 이동.
            case Block.DIR4.UP: v = Vector3.up; break; // 위로 1단위 이동.
            case Block.DIR4.DOWN: v = Vector3.down; break; // 아래로 1단위 이동.
        }
        v *= Block.COLLISION_SIZE; // 블록의 크기를 곱한다.
        return (v);
    }

    public static Block.DIR4 getOppositDir(Block.DIR4 dir)
    { // 블록을 서로 교체하기 위해 인수로 지정된 방향의 반대 방향을 반환.
        Block.DIR4 opposit = dir;
        switch (dir)
        {
            case Block.DIR4.RIGHT: opposit = Block.DIR4.LEFT; break;
            case Block.DIR4.LEFT: opposit = Block.DIR4.RIGHT; break;
            case Block.DIR4.UP: opposit = Block.DIR4.DOWN; break;
            case Block.DIR4.DOWN: opposit = Block.DIR4.UP; break;
        }
        return (opposit);
    }

    //이거도 겁나 많이 바꿔야한다.
    //이거도 겁나 많이 바꿔야한다.
    //이거도 겁나 많이 바꿔야한다.
    //이거도 겁나 많이 바꿔야한다.
    public void swapBlock(BlockControl block0, Block.DIR4 dir, BlockControl block1)
    { // 실제로 블록을 교체.


        //각각의 블록 메쉬를 기억해둔다.
        Block.PREFAB mesh0 = block0.prefab;
        Block.PREFAB mesh1 = block1.prefab;

        // 각각의 블록 색을 기억해 둔다.
        Block.COLOR color0 = block0.color;
        Block.COLOR color1 = block1.color;

      

        // 각각의 블록의 확대율을 기억해 둔다.
        Vector3 scale0 = block0.transform.localScale;
        Vector3 scale1 = block1.transform.localScale;
        // 각각의 블록의 '사라지는 시간'을 기억해 둔다.
        float vanish_timer0 = block0.vanish_timer;
        float vanish_timer1 = block1.vanish_timer;
        // 각각의 블록의 이동할 곳을 구한다.
        Vector3 offset0 = BlockRoot.getDirVector(dir);
        Vector3 offset1 = BlockRoot.getDirVector(BlockRoot.getOppositDir(dir));



        //색이 같으면
        if (color1 == color0)
        {
            if (mesh1 == (Block.PREFAB)2)
            {
                block0.setColor(color0, mesh0);
                block1.setColor(color0, mesh0);

                //여기에 오브젝트 상태 변경X
                block0.setPrefab((Block.PREFAB)0);
                block1.setPrefab(mesh0);



                // 확대율을 교체한다.
                block0.transform.localScale = scale1;
                block1.transform.localScale = scale0;
                // '사라지는 시간'을 교체한다.
                block0.vanish_timer = vanish_timer1;
                block1.vanish_timer = vanish_timer0;

                block0.beginSlide(offset0); // 원래 블록 이동을 시작한다.
                block1.beginSlide(offset1); // 이동할 위치의 블록 이동을 시작한다.      

                //이거에서 숫자를 ++하고 end조건에 해당 스테이지의 숫자 많큼이면 게임종료한다.
                //J.GetComponent<Json>().ClearCount--;
                SoundMAnager.instance.CoinSound();



                GoBackData();
            }
            else
            {
                SoundMAnager.instance.DontMoveSound();
                block0.beginSlide(offset0); // 원래 블록 이동을 시작한다.
                block1.beginSlide(offset1); // 이동할 위치의 블록 이동을 시작한다.     
            }  
        }
        //색이 다르면
        else
        {
            // 색을 교체한다. 색이 같으면 이동 불가

            // 조건이 조합하는 방법? 그걸 하나하나 지정해서 조건문 달지말고 하나의 열거형 더 만들어서 색 데이터 관리해야한다.
            //block0.setColor(color1);
            //block1.setColor(color0);

            //색을 채워야하는 블록이 아니면
            if (mesh1 != (Block.PREFAB)2)
            {
                if (color1 == (Block.COLOR)1 && color0 == (Block.COLOR)3 || color1 == (Block.COLOR)3 && color0 == (Block.COLOR)1)
                {
                    block0.setColor((Block.COLOR)4, mesh1);
                    block1.setColor((Block.COLOR)4, mesh0);

                    //여기에 오브젝트 상태도 변경해야한다.
                    block0.setPrefab(mesh1);
                    block1.setPrefab(mesh0);
                    // 확대율을 교체한다.
                    block0.transform.localScale = scale1;
                    block1.transform.localScale = scale0;
                    // '사라지는 시간'을 교체한다.
                    block0.vanish_timer = vanish_timer1;
                    block1.vanish_timer = vanish_timer0;

                    block0.beginSlide(offset0); // 원래 블록 이동을 시작한다.
                    block1.beginSlide(offset1); // 이동할 위치의 블록 이동을 시작한다.
                    SoundMAnager.instance.MoveSound();
                }                //빨강노랑
                else if (color1 == (Block.COLOR)1 && color0 == (Block.COLOR)2 || color1 == (Block.COLOR)2 && color0 == (Block.COLOR)1)
                {
                    block0.setColor((Block.COLOR)5, mesh1);
                    block1.setColor((Block.COLOR)5, mesh0);

                    //여기에 오브젝트 상태도 변경해야한다.
                    block0.setPrefab(mesh1);
                    block1.setPrefab(mesh0);



                    // 확대율을 교체한다.
                    block0.transform.localScale = scale1;
                    block1.transform.localScale = scale0;
                    // '사라지는 시간'을 교체한다.
                    block0.vanish_timer = vanish_timer1;
                    block1.vanish_timer = vanish_timer0;
                    block0.beginSlide(offset0); // 원래 블록 이동을 시작한다.
                    block1.beginSlide(offset1); // 이동할 위치의 블록 이동을 시작한다.
                    SoundMAnager.instance.MoveSound();
                } //빨강파랑
                else if (color1 == (Block.COLOR)2 && color0 == (Block.COLOR)3 || color1 == (Block.COLOR)3 && color0 == (Block.COLOR)2)
                {
                    block0.setColor((Block.COLOR)6, mesh1);
                    block1.setColor((Block.COLOR)6, mesh0);

                    //여기에 오브젝트 상태도 변경해야한다.
                    block0.setPrefab(mesh1);
                    block1.setPrefab(mesh0);



                    // 확대율을 교체한다.
                    block0.transform.localScale = scale1;
                    block1.transform.localScale = scale0;
                    // '사라지는 시간'을 교체한다.
                    block0.vanish_timer = vanish_timer1;
                    block1.vanish_timer = vanish_timer0;
                    block0.beginSlide(offset0); // 원래 블록 이동을 시작한다.
                    block1.beginSlide(offset1); // 이동할 위치의 블록 이동을 시작한다.
                    SoundMAnager.instance.MoveSound();
                } //노랑파랑
                else if (color1 == (Block.COLOR)4 && color0 == (Block.COLOR)3 || color1 == (Block.COLOR)3 && color0 == (Block.COLOR)4)
                {
                    block0.setColor((Block.COLOR)7, mesh1);
                    block1.setColor((Block.COLOR)7, mesh0);

                    //여기에 오브젝트 상태도 변경해야한다.
                    block0.setPrefab(mesh1);
                    block1.setPrefab(mesh0);



                    // 확대율을 교체한다.
                    block0.transform.localScale = scale1;
                    block1.transform.localScale = scale0;
                    // '사라지는 시간'을 교체한다.
                    block0.vanish_timer = vanish_timer1;
                    block1.vanish_timer = vanish_timer0;
                    block0.beginSlide(offset0); // 원래 블록 이동을 시작한다.
                    block1.beginSlide(offset1); // 이동할 위치의 블록 이동을 시작한다.
                    SoundMAnager.instance.MoveSound();
                } //주황노랑
                else if (color1 == (Block.COLOR)4 && color0 == (Block.COLOR)1 || color1 == (Block.COLOR)1 && color0 == (Block.COLOR)4)
                {
                    block0.setColor((Block.COLOR)8, mesh1);
                    block1.setColor((Block.COLOR)8, mesh0);

                    //여기에 오브젝트 상태도 변경해야한다.
                    block0.setPrefab(mesh1);
                    block1.setPrefab(mesh0);



                    // 확대율을 교체한다.
                    block0.transform.localScale = scale1;
                    block1.transform.localScale = scale0;
                    // '사라지는 시간'을 교체한다.
                    block0.vanish_timer = vanish_timer1;
                    block1.vanish_timer = vanish_timer0;
                    block0.beginSlide(offset0); // 원래 블록 이동을 시작한다.
                    block1.beginSlide(offset1); // 이동할 위치의 블록 이동을 시작한다.
                    SoundMAnager.instance.MoveSound();
                } //주황빨강
                else if (color1 == (Block.COLOR)7 && color0 == (Block.COLOR)1 || color1 == (Block.COLOR)1 && color0 == (Block.COLOR)7)
                {
                    block0.setColor((Block.COLOR)4, mesh1);
                    block1.setColor((Block.COLOR)4, mesh0);

                    //여기에 오브젝트 상태도 변경해야한다.
                    block0.setPrefab(mesh1);
                    block1.setPrefab(mesh0);



                    // 확대율을 교체한다.
                    block0.transform.localScale = scale1;
                    block1.transform.localScale = scale0;
                    // '사라지는 시간'을 교체한다.
                    block0.vanish_timer = vanish_timer1;
                    block1.vanish_timer = vanish_timer0;
                    block0.beginSlide(offset0); // 원래 블록 이동을 시작한다.
                    block1.beginSlide(offset1); // 이동할 위치의 블록 이동을 시작한다.
                    SoundMAnager.instance.MoveSound();
                } //복숭아빨강
                else if (color1 == (Block.COLOR)8 && color0 == (Block.COLOR)3 || color1 == (Block.COLOR)3 && color0 == (Block.COLOR)8)
                {
                    block0.setColor((Block.COLOR)4, mesh1);
                    block1.setColor((Block.COLOR)4, mesh0);

                    //여기에 오브젝트 상태도 변경해야한다.
                    block0.setPrefab(mesh1);
                    block1.setPrefab(mesh0);



                    // 확대율을 교체한다.
                    block0.transform.localScale = scale1;
                    block1.transform.localScale = scale0;
                    // '사라지는 시간'을 교체한다.
                    block0.vanish_timer = vanish_timer1;
                    block1.vanish_timer = vanish_timer0;
                    block0.beginSlide(offset0); // 원래 블록 이동을 시작한다.
                    block1.beginSlide(offset1); // 이동할 위치의 블록 이동을 시작한다.
                    SoundMAnager.instance.MoveSound();
                } //다홍노랑
                else if (color1 == (Block.COLOR)5 && color0 == (Block.COLOR)2 || color1 == (Block.COLOR)2 && color0 == (Block.COLOR)5)
                {
                    block0.setColor((Block.COLOR)9, mesh1);
                    block1.setColor((Block.COLOR)9, mesh0);

                    //여기에 오브젝트 상태도 변경해야한다.
                    block0.setPrefab(mesh1);
                    block1.setPrefab(mesh0);



                    // 확대율을 교체한다.
                    block0.transform.localScale = scale1;
                    block1.transform.localScale = scale0;
                    // '사라지는 시간'을 교체한다.
                    block0.vanish_timer = vanish_timer1;
                    block1.vanish_timer = vanish_timer0;
                    block0.beginSlide(offset0); // 원래 블록 이동을 시작한다.
                    block1.beginSlide(offset1); // 이동할 위치의 블록 이동을 시작한다.
                    SoundMAnager.instance.MoveSound();
                } //보라파랑
                else if (color1 == (Block.COLOR)5 && color0 == (Block.COLOR)1 || color1 == (Block.COLOR)1 && color0 == (Block.COLOR)5)
                {
                    block0.setColor((Block.COLOR)10, mesh1);
                    block1.setColor((Block.COLOR)10, mesh0);

                    //여기에 오브젝트 상태도 변경해야한다.
                    block0.setPrefab(mesh1);
                    block1.setPrefab(mesh0);



                    // 확대율을 교체한다.
                    block0.transform.localScale = scale1;
                    block1.transform.localScale = scale0;
                    // '사라지는 시간'을 교체한다.
                    block0.vanish_timer = vanish_timer1;
                    block1.vanish_timer = vanish_timer0;
                    block0.beginSlide(offset0); // 원래 블록 이동을 시작한다.
                    block1.beginSlide(offset1); // 이동할 위치의 블록 이동을 시작한다.
                    SoundMAnager.instance.MoveSound();
                } //보라빨강
                else if (color1 == (Block.COLOR)9 && color0 == (Block.COLOR)1 || color1 == (Block.COLOR)1 && color0 == (Block.COLOR)9)
                {
                    block0.setColor((Block.COLOR)5, mesh1);
                    block1.setColor((Block.COLOR)5, mesh0);

                    //여기에 오브젝트 상태도 변경해야한다.
                    block0.setPrefab(mesh1);
                    block1.setPrefab(mesh0);



                    // 확대율을 교체한다.
                    block0.transform.localScale = scale1;
                    block1.transform.localScale = scale0;
                    // '사라지는 시간'을 교체한다.
                    block0.vanish_timer = vanish_timer1;
                    block1.vanish_timer = vanish_timer0;
                    block0.beginSlide(offset0); // 원래 블록 이동을 시작한다.
                    block1.beginSlide(offset1); // 이동할 위치의 블록 이동을 시작한다.
                    SoundMAnager.instance.MoveSound();
                } //남색빨강
                else if (color1 == (Block.COLOR)10 && color0 == (Block.COLOR)2 || color1 == (Block.COLOR)2 && color0 == (Block.COLOR)10)
                {
                    block0.setColor((Block.COLOR)5, mesh1);
                    block1.setColor((Block.COLOR)5, mesh0);

                    //여기에 오브젝트 상태도 변경해야한다.
                    block0.setPrefab(mesh1);
                    block1.setPrefab(mesh0);



                    // 확대율을 교체한다.
                    block0.transform.localScale = scale1;
                    block1.transform.localScale = scale0;
                    // '사라지는 시간'을 교체한다.
                    block0.vanish_timer = vanish_timer1;
                    block1.vanish_timer = vanish_timer0;
                    block0.beginSlide(offset0); // 원래 블록 이동을 시작한다.
                    block1.beginSlide(offset1); // 이동할 위치의 블록 이동을 시작한다.
                    SoundMAnager.instance.MoveSound();
                } //자주파랑
                else if (color1 == (Block.COLOR)6 && color0 == (Block.COLOR)3 || color1 == (Block.COLOR)3 && color0 == (Block.COLOR)6)
                {
                    block0.setColor((Block.COLOR)11, mesh1);
                    block1.setColor((Block.COLOR)11, mesh0);

                    //여기에 오브젝트 상태도 변경해야한다.
                    block0.setPrefab(mesh1);
                    block1.setPrefab(mesh0);



                    // 확대율을 교체한다.
                    block0.transform.localScale = scale1;
                    block1.transform.localScale = scale0;
                    // '사라지는 시간'을 교체한다.
                    block0.vanish_timer = vanish_timer1;
                    block1.vanish_timer = vanish_timer0;
                    block0.beginSlide(offset0); // 원래 블록 이동을 시작한다.
                    block1.beginSlide(offset1); // 이동할 위치의 블록 이동을 시작한다.
                    SoundMAnager.instance.MoveSound();
                } //초록노랑
                else if (color1 == (Block.COLOR)6 && color0 == (Block.COLOR)2 || color1 == (Block.COLOR)2 && color0 == (Block.COLOR)6)
                {
                    block0.setColor((Block.COLOR)12, mesh1);
                    block1.setColor((Block.COLOR)12, mesh0);

                    //여기에 오브젝트 상태도 변경해야한다.
                    block0.setPrefab(mesh1);
                    block1.setPrefab(mesh0);



                    // 확대율을 교체한다.
                    block0.transform.localScale = scale1;
                    block1.transform.localScale = scale0;
                    // '사라지는 시간'을 교체한다.
                    block0.vanish_timer = vanish_timer1;
                    block1.vanish_timer = vanish_timer0;
                    block0.beginSlide(offset0); // 원래 블록 이동을 시작한다.
                    block1.beginSlide(offset1); // 이동할 위치의 블록 이동을 시작한다.
                    SoundMAnager.instance.MoveSound();
                } //초록파랑
                else if (color1 == (Block.COLOR)11 && color0 == (Block.COLOR)2 || color1 == (Block.COLOR)2 && color0 == (Block.COLOR)11)
                {
                    block0.setColor((Block.COLOR)6, mesh1);
                    block1.setColor((Block.COLOR)6, mesh0);

                    //여기에 오브젝트 상태도 변경해야한다.
                    block0.setPrefab(mesh1);
                    block1.setPrefab(mesh0);



                    // 확대율을 교체한다.
                    block0.transform.localScale = scale1;
                    block1.transform.localScale = scale0;
                    // '사라지는 시간'을 교체한다.
                    block0.vanish_timer = vanish_timer1;
                    block1.vanish_timer = vanish_timer0;
                    block0.beginSlide(offset0); // 원래 블록 이동을 시작한다.
                    block1.beginSlide(offset1); // 이동할 위치의 블록 이동을 시작한다.
                    SoundMAnager.instance.MoveSound();
                } //연두파랑
                else if (color1 == (Block.COLOR)12 && color0 == (Block.COLOR)3 || color1 == (Block.COLOR)3 && color0 == (Block.COLOR)12)
                {
                    block0.setColor((Block.COLOR)6, mesh1);
                    block1.setColor((Block.COLOR)6, mesh0);

                    //여기에 오브젝트 상태도 변경해야한다.
                    block0.setPrefab(mesh1);
                    block1.setPrefab(mesh0);



                    // 확대율을 교체한다.
                    block0.transform.localScale = scale1;
                    block1.transform.localScale = scale0;
                    // '사라지는 시간'을 교체한다.
                    block0.vanish_timer = vanish_timer1;
                    block1.vanish_timer = vanish_timer0;
                    block0.beginSlide(offset0); // 원래 블록 이동을 시작한다.
                    block1.beginSlide(offset1); // 이동할 위치의 블록 이동을 시작한다.
                    SoundMAnager.instance.MoveSound();
                } //청록노랑
                else if (color1 == (Block.COLOR)(0))
                {
                    block0.setColor(color0, mesh1);
                    block1.setColor(color0, mesh0);

                    //여기에 오브젝트 상태도 변경해야한다.
                    block0.setPrefab(mesh1);
                    block1.setPrefab(mesh0);

                    // 확대율을 교체한다.
                    block0.transform.localScale = scale1;
                    block1.transform.localScale = scale0;
                    // '사라지는 시간'을 교체한다.
                    block0.vanish_timer = vanish_timer1;
                    block1.vanish_timer = vanish_timer0;
                    block0.beginSlide(offset0); // 원래 블록 이동을 시작한다.
                    block1.beginSlide(offset1); // 이동할 위치의 블록 이동을 시작한다.
                    SoundMAnager.instance.MoveSound();
                } //바뀌는색이 회색이면
                else if (color1 == (Block.COLOR)(-1))
                {
                    SoundMAnager.instance.DontMoveSound();
                    block0.beginSlide(offset0); // 원래 블록 이동을 시작한다.
                    block1.beginSlide(offset1); // 이동할 위치의 블록 이동을 시작한다.
                }
                else
                {
                    block0.setColor((Block.COLOR)(-1), mesh1);
                    block1.setColor((Block.COLOR)(-1), mesh0);

                    //여기에 오브젝트 상태도 변경해야한다.
                    block0.setPrefab(mesh1);
                    block1.setPrefab(mesh0);



                    // 확대율을 교체한다.
                    block0.transform.localScale = scale1;
                    block1.transform.localScale = scale0;
                    // '사라지는 시간'을 교체한다.
                    block0.vanish_timer = vanish_timer1;
                    block1.vanish_timer = vanish_timer0;
                    block0.beginSlide(offset0); // 원래 블록 이동을 시작한다.
                    block1.beginSlide(offset1); // 이동할 위치의 블록 이동을 시작한다.


                    isDie = true;
                    dietext.text = "색 조합실패";
                    Debug.Log("색 조합실패");
                }

                GoBackData();
            }
            //색을 채워야하는 블록이면
            else
            {
                block0.setColor(color0, mesh0);
                block1.setColor(color1, mesh1);

                //여기에 오브젝트 상태도 변경해야한다.
                block0.setPrefab(mesh0);
                block1.setPrefab(mesh1);



                // 확대율을 교체한다.
                block0.transform.localScale = scale1;
                block1.transform.localScale = scale0;
                // '사라지는 시간'을 교체한다.
                block0.vanish_timer = vanish_timer1;
                block1.vanish_timer = vanish_timer0;
                block0.beginSlide(offset0); // 원래 블록 이동을 시작한다.
                block1.beginSlide(offset1); // 이동할 위치의 블록 이동을 시작한다.
                SoundMAnager.instance.DontMoveSound();
            }
        }

        
    }


    //-------------------------------------------------------------------------------------

    //블록 생성-----------------------
    public void initialSetUp()
    {
        //this.blocks = new BlockControl[Block.BLOCK_NUM_X, Block.BLOCK_NUM_Y]; // 그리드의 크기를 9×9로 한다.
        this.blocks = new BlockControl[Block.BLOCK_NUM_X, Block.BLOCK_NUM_Y]; // 그리드의 크기를 9×9로 한다.
        int color_index = 0; // 블록의 색 번호.


        //------
        int mesh_index = 0;
        //------

        for (int y = 0; y < Block.BLOCK_NUM_X; y++)
        { // 처음~마지막행
            for (int x = 0; x < Block.BLOCK_NUM_X; x++)
            { // 왼쪽~오른쪽

                if (J.GetComponent<Json>().BLOCK_COLOR.map[x].mapX[y] == 0)
                {
                    color_index = 0;
                }
                else if (J.GetComponent<Json>().BLOCK_COLOR.map[x].mapX[y] == 1)
                {
                    color_index = 1;
                }
                else if (J.GetComponent<Json>().BLOCK_COLOR.map[x].mapX[y] == 2)
                {
                    color_index = 2;
                }
                else if (J.GetComponent<Json>().BLOCK_COLOR.map[x].mapX[y] == 3)
                {
                    color_index = 3;
                }
                else if (J.GetComponent<Json>().BLOCK_COLOR.map[x].mapX[y] == 4)
                {
                    color_index = 4;
                }
                else if (J.GetComponent<Json>().BLOCK_COLOR.map[x].mapX[y] == 5)
                {
                    color_index = 5;
                }
                else if (J.GetComponent<Json>().BLOCK_COLOR.map[x].mapX[y] == 6)
                {
                    color_index = 6;
                }
                else if (J.GetComponent<Json>().BLOCK_COLOR.map[x].mapX[y] == 7)
                {
                    color_index = 7;
                }
                else if (J.GetComponent<Json>().BLOCK_COLOR.map[x].mapX[y] == 8)
                {
                    color_index = 8;
                }
                else if (J.GetComponent<Json>().BLOCK_COLOR.map[x].mapX[y] == 9)
                {
                    color_index = 9;
                }
                else if (J.GetComponent<Json>().BLOCK_COLOR.map[x].mapX[y] == 10)
                {
                    color_index = 10;
                }
                else if (J.GetComponent<Json>().BLOCK_COLOR.map[x].mapX[y] == 11)
                {
                    color_index = 11;
                }
                else if (J.GetComponent<Json>().BLOCK_COLOR.map[x].mapX[y] == 12)
                {
                    color_index = 12;
                }
                else
                {
                    color_index = -1;
                }

                if (J.GetComponent<Json>().BLOCK_STATE.map[x].mapX[y] == 0)
                {
                    mesh_index = 0;
                }
                else if (J.GetComponent<Json>().BLOCK_STATE.map[x].mapX[y] == 1)
                {
                    mesh_index = 1;
                }
                else if (J.GetComponent<Json>().BLOCK_STATE.map[x].mapX[y] == 2)
                {
                    mesh_index = 2;
                }


                // BlockPrefab의 인스턴스를 씬에 만든다.
                GameObject game_object = Instantiate(this.BlockPrefab) as GameObject;
                BlockControl block = game_object.GetComponent<BlockControl>(); // 블록의 BlockControl 클래스를 가져온다.
                this.blocks[x, y] = block; // 블록을 그리드에 저장한다.
                block.i_pos.x = x; // 블록의 위치 정보(그리드 좌표)를 설정한다.
                block.i_pos.y = y;
                block.block_root = this; // 각 BlockControl이 연계할 GameRoot는 자신이라고 설정한다.
                Vector3 position = BlockRoot.calcBlockPosition(block.i_pos); // 그리드 좌표를 실제 위치(씬의 좌표)로 변환
                block.transform.position = position; // 씬의 블록 위치를 이동한다.


                block.setPrefab((Block.PREFAB)mesh_index);

                block.setColor((Block.COLOR)color_index, (Block.PREFAB)mesh_index); // 블록의 색을 변경한다.
                                                                                    // 블록의 이름을 설정(후술)한다. 나중에 블록 정보 확인때 필요.
                block.name = "block(" + block.i_pos.x.ToString() + "," + block.i_pos.y.ToString() + ")";
            }
        }
    }

    // 지정된 그리드 좌표로 씬에서의 좌표를 구한다.
    public static Vector3 calcBlockPosition(Block.iPosition i_pos)
    {
        // 배치할 왼쪽 위 구석 위치를 초기값으로 설정한다.
        Vector3 position = new Vector3(-(Block.BLOCK_NUM_X / 2.0f - 0.5f), -(Block.BLOCK_NUM_Y / 2.0f - 0.5f), 0.0f);
        // 초깃값 + 그리드 좌표 × 블록 크기.
        position.x += (float)i_pos.x * Block.COLLISION_SIZE;
        position.y += (float)i_pos.y * Block.COLLISION_SIZE;
        return (position); // 씬에서의 좌표를 반환한다.
    }
    //---------------------


    public int p_C; //핑크블록 카운트
    public int o_C; //오랜지블록 카운트


    public int PPP; //1인 수

    public bool pBool = true; //핑크색 불

    //End 조건
    //---------------------
    public void EndGame()
    {
        //이거는 블록이 교채됬을 때만 실행

        int p = 0;
        int o = 0;

        for (int y = 0; y < Block.BLOCK_NUM_Y; y++)
        {
            for (int x = 0; x < Block.BLOCK_NUM_X; x++)
            {
                if (blocks[x, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                {
                    if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == (Block.COLOR)0)
                    {

                    }
                    else if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == (Block.COLOR)5)
                    {
                        o++;
                    }
                    else if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == (Block.COLOR)6)
                    {
                        p++;
                    }
                }
            }
        }
        p_C = p;
        o_C = o;


        int pink_1 = 0;

        for (int y = 0; y < Block.BLOCK_NUM_Y; y++)
        {
            for (int x = 0; x < Block.BLOCK_NUM_X; x++)
            {
                if (p_C > 2)
                {
                    if (blocks[x, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0 && blocks[x, y].gameObject.GetComponent<BlockControl>().color == (Block.COLOR)6)
                    {
                        //상하좌우 비교해야한다.
                        //상 비교

                        if (x + 1 < Block.BLOCK_NUM_X && y + 1 < Block.BLOCK_NUM_Y && x != 0 && y != 0)
                        {

                            int p1 = 0;

                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x, y + 1].gameObject.GetComponent<BlockControl>().color && blocks[x, y + 1].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                            {
                                //같은게 있다
                                //continue;
                                p1++;
                            }//상비교
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x, y - 1].gameObject.GetComponent<BlockControl>().color && blocks[x, y - 1].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                            {
                                p1++;
                                //continue;
                            }//하비교
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x - 1, y].gameObject.GetComponent<BlockControl>().color && blocks[x - 1, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                            {
                                p1++;
                                //continue;
                            }//좌비교
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x + 1, y].gameObject.GetComponent<BlockControl>().color && blocks[x + 1, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                            {
                                p1++;
                                //continue;
                            }//우비교

                            //p1은 갈수있는 길의 수 이걸 각 노드의 변수에 집어넣어


                            blocks[x, y].gameObject.GetComponent<BlockControl>().loadNum = p1;



                            //위 조건에 해당하는게 없다면
                            //pBool = false;
                            //return;
                        }
                    }
                }
            }
        }

        if (pink_1 > 2)
        {
            pBool = false;
        }

    }
    public void End()
    {
        for (int y = 0; y < Block.BLOCK_NUM_Y; y++)
        {
            for (int x = 0; x < Block.BLOCK_NUM_X; x++)
            {
                if (blocks[x, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)2)
                {
                    return;
                }
            }
        }

        isClear = true;

    }

    //---------------------실패조건(끊어졌을때)
    public void CKKKKKK()
    {
        //이거 색 수많큼 있어야한다.
        int num1 = 0;
        int num2 = 0;
        int num3 = 0;
        int num4 = 0;
        int num5 = 0;
        int num6 = 0;
        int num7 = 0;
        int num8 = 0;
        int num9 = 0;
        int num10 = 0;
        int num11 = 0;
        int num12 = 0;

        for (int y = 0; y < Block.BLOCK_NUM_Y; y++)
        {
            for (int x = 0; x < Block.BLOCK_NUM_X; x++)
            {
                if (blocks[x, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                {
                    if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == (Block.COLOR)0)
                    {

                    }
                    else if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == (Block.COLOR)1)
                    {
                        num1++;
                        blocks[x, y].gameObject.GetComponent<BlockControl>().loadNum = num1;
                        blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = num1;
                    }
                    else if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == (Block.COLOR)2)
                    {
                        num2++;
                        blocks[x, y].gameObject.GetComponent<BlockControl>().loadNum = num2;
                        blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = num2;
                    }
                    else if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == (Block.COLOR)3)
                    {
                        num3++;
                        blocks[x, y].gameObject.GetComponent<BlockControl>().loadNum = num3;
                        blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = num3;
                    }
                    else if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == (Block.COLOR)4)
                    {
                        num4++;
                        blocks[x, y].gameObject.GetComponent<BlockControl>().loadNum = num4;
                        blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = num4;
                    }
                    else if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == (Block.COLOR)5)
                    {
                        num5++;
                        blocks[x, y].gameObject.GetComponent<BlockControl>().loadNum = num5;
                        blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = num5;
                    }
                    else if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == (Block.COLOR)6)
                    {
                        num6++;
                        blocks[x, y].gameObject.GetComponent<BlockControl>().loadNum = num6;
                        blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = num6;
                    }
                    else if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == (Block.COLOR)7)
                    {
                        num7++;
                        blocks[x, y].gameObject.GetComponent<BlockControl>().loadNum = num7;
                        blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = num7;
                    }
                    else if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == (Block.COLOR)8)
                    {
                        num8++;
                        blocks[x, y].gameObject.GetComponent<BlockControl>().loadNum = num8;
                        blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = num8;
                    }
                    else if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == (Block.COLOR)9)
                    {
                        num9++;
                        blocks[x, y].gameObject.GetComponent<BlockControl>().loadNum = num9;
                        blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = num9;
                    }
                    else if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == (Block.COLOR)10)
                    {
                        num10++;
                        blocks[x, y].gameObject.GetComponent<BlockControl>().loadNum = num10;
                        blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = num10;
                    }
                    else if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == (Block.COLOR)11)
                    {
                        num11++;
                        blocks[x, y].gameObject.GetComponent<BlockControl>().loadNum = num11;
                        blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = num11;
                    }
                    else if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == (Block.COLOR)12)
                    {
                        num12++;
                        blocks[x, y].gameObject.GetComponent<BlockControl>().loadNum = num12;
                        blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = num12;
                    }
                }
            }
        }


        for (int y = 0; y < Block.BLOCK_NUM_Y; y++)
        {
            for (int x = 0; x < Block.BLOCK_NUM_X; x++)
            {
                if (blocks[x, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                {
                    //상하좌우 비교해야한다.
                    //상 비교

                    if (x + 1 < Block.BLOCK_NUM_X && y + 1 < Block.BLOCK_NUM_Y && x != 0 && y != 0)
                    {
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x, y + 1].gameObject.GetComponent<BlockControl>().color && blocks[x, y + 1].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            //같은게 있다
                            //작거나 같은지 판별 지금이 더 크면
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x, y + 1].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x, y + 1].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//상비교
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x, y - 1].gameObject.GetComponent<BlockControl>().color && blocks[x, y - 1].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x, y - 1].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x, y - 1].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//하비교
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x - 1, y].gameObject.GetComponent<BlockControl>().color && blocks[x - 1, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x - 1, y].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x - 1, y].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//좌비교
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x + 1, y].gameObject.GetComponent<BlockControl>().color && blocks[x + 1, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x + 1, y].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x + 1, y].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//우비교
                    }
                }
            }
        }
        for (int x = 0; x < Block.BLOCK_NUM_X; x++)
        {
            for (int y = 0; y < Block.BLOCK_NUM_Y; y++)
            {
                if (blocks[x, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                {
                    //상하좌우 비교해야한다.
                    //상 비교

                    if (x + 1 < Block.BLOCK_NUM_X && y + 1 < Block.BLOCK_NUM_Y && x != 0 && y != 0)
                    {
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x, y + 1].gameObject.GetComponent<BlockControl>().color && blocks[x, y + 1].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            //같은게 있다
                            //작거나 같은지 판별 지금이 더 크면
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x, y + 1].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x, y + 1].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//상비교
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x, y - 1].gameObject.GetComponent<BlockControl>().color && blocks[x, y - 1].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x, y - 1].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x, y - 1].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//하비교
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x - 1, y].gameObject.GetComponent<BlockControl>().color && blocks[x - 1, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x - 1, y].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x - 1, y].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//좌비교
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x + 1, y].gameObject.GetComponent<BlockControl>().color && blocks[x + 1, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x + 1, y].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x + 1, y].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//우비교
                    }
                }
            }
        }
        for (int y = Block.BLOCK_NUM_Y - 1; y >= 0; y--)
        {
            for (int x = Block.BLOCK_NUM_X - 1; x >= 0; x--)
            {
                if (blocks[x, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                {
                    //상하좌우 비교해야한다.
                    //상 비교

                    if (x + 1 < Block.BLOCK_NUM_X && y + 1 < Block.BLOCK_NUM_Y && x != 0 && y != 0)
                    {
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x, y + 1].gameObject.GetComponent<BlockControl>().color && blocks[x, y + 1].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            //같은게 있다
                            //작거나 같은지 판별 지금이 더 크면
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x, y + 1].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x, y + 1].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//상비교
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x, y - 1].gameObject.GetComponent<BlockControl>().color && blocks[x, y - 1].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x, y - 1].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x, y - 1].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//하비교
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x - 1, y].gameObject.GetComponent<BlockControl>().color && blocks[x - 1, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x - 1, y].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x - 1, y].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//좌비교
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x + 1, y].gameObject.GetComponent<BlockControl>().color && blocks[x + 1, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x + 1, y].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x + 1, y].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//우비교
                    }
                }
            }
        }
        for (int x = Block.BLOCK_NUM_X - 1; x >= 0; x--)
        {
            for (int y = Block.BLOCK_NUM_Y - 1; y >= 0; y--)
            {
                if (blocks[x, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                {
                    //상하좌우 비교해야한다.
                    //상 비교

                    if (x + 1 < Block.BLOCK_NUM_X && y + 1 < Block.BLOCK_NUM_Y && x != 0 && y != 0)
                    {
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x, y + 1].gameObject.GetComponent<BlockControl>().color && blocks[x, y + 1].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            //같은게 있다
                            //작거나 같은지 판별 지금이 더 크면
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x, y + 1].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x, y + 1].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//상비교
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x, y - 1].gameObject.GetComponent<BlockControl>().color && blocks[x, y - 1].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x, y - 1].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x, y - 1].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//하비교
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x - 1, y].gameObject.GetComponent<BlockControl>().color && blocks[x - 1, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x - 1, y].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x - 1, y].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//좌비교
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x + 1, y].gameObject.GetComponent<BlockControl>().color && blocks[x + 1, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x + 1, y].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x + 1, y].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//우비교
                    }
                }
            }
        }

        for (int y = 0; y < Block.BLOCK_NUM_Y; y++)
        {
            for (int x = 0; x < Block.BLOCK_NUM_X; x++)
            {
                if (blocks[x, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                {
                    //상하좌우 비교해야한다.
                    //상 비교

                    if (x + 1 < Block.BLOCK_NUM_X && y + 1 < Block.BLOCK_NUM_Y && x != 0 && y != 0)
                    {
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x, y + 1].gameObject.GetComponent<BlockControl>().color && blocks[x, y + 1].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            //같은게 있다
                            //작거나 같은지 판별 지금이 더 크면
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x, y + 1].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x, y + 1].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//상비교
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x, y - 1].gameObject.GetComponent<BlockControl>().color && blocks[x, y - 1].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x, y - 1].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x, y - 1].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//하비교
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x - 1, y].gameObject.GetComponent<BlockControl>().color && blocks[x - 1, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x - 1, y].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x - 1, y].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//좌비교
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x + 1, y].gameObject.GetComponent<BlockControl>().color && blocks[x + 1, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x + 1, y].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x + 1, y].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//우비교
                    }
                }
            }
        }
        for (int x = 0; x < Block.BLOCK_NUM_X; x++)
        {
            for (int y = 0; y < Block.BLOCK_NUM_Y; y++)
            {
                if (blocks[x, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                {
                    //상하좌우 비교해야한다.
                    //상 비교

                    if (x + 1 < Block.BLOCK_NUM_X && y + 1 < Block.BLOCK_NUM_Y && x != 0 && y != 0)
                    {
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x, y + 1].gameObject.GetComponent<BlockControl>().color && blocks[x, y + 1].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            //같은게 있다
                            //작거나 같은지 판별 지금이 더 크면
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x, y + 1].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x, y + 1].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//상비교
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x, y - 1].gameObject.GetComponent<BlockControl>().color && blocks[x, y - 1].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x, y - 1].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x, y - 1].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//하비교
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x - 1, y].gameObject.GetComponent<BlockControl>().color && blocks[x - 1, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x - 1, y].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x - 1, y].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//좌비교
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x + 1, y].gameObject.GetComponent<BlockControl>().color && blocks[x + 1, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x + 1, y].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x + 1, y].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//우비교
                    }
                }
            }
        }
        for (int y = Block.BLOCK_NUM_Y - 1; y >= 0; y--)
        {
            for (int x = Block.BLOCK_NUM_X - 1; x >= 0; x--)
            {
                if (blocks[x, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                {
                    //상하좌우 비교해야한다.
                    //상 비교

                    if (x + 1 < Block.BLOCK_NUM_X && y + 1 < Block.BLOCK_NUM_Y && x != 0 && y != 0)
                    {
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x, y + 1].gameObject.GetComponent<BlockControl>().color && blocks[x, y + 1].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            //같은게 있다
                            //작거나 같은지 판별 지금이 더 크면
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x, y + 1].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x, y + 1].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//상비교
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x, y - 1].gameObject.GetComponent<BlockControl>().color && blocks[x, y - 1].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x, y - 1].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x, y - 1].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//하비교
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x - 1, y].gameObject.GetComponent<BlockControl>().color && blocks[x - 1, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x - 1, y].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x - 1, y].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//좌비교
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x + 1, y].gameObject.GetComponent<BlockControl>().color && blocks[x + 1, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x + 1, y].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x + 1, y].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//우비교
                    }
                }
            }
        }
        for (int x = Block.BLOCK_NUM_X - 1; x >= 0; x--)
        {
            for (int y = Block.BLOCK_NUM_Y - 1; y >= 0; y--)
            {
                if (blocks[x, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                {
                    //상하좌우 비교해야한다.
                    //상 비교

                    if (x + 1 < Block.BLOCK_NUM_X && y + 1 < Block.BLOCK_NUM_Y && x != 0 && y != 0)
                    {
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x, y + 1].gameObject.GetComponent<BlockControl>().color && blocks[x, y + 1].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            //같은게 있다
                            //작거나 같은지 판별 지금이 더 크면
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x, y + 1].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x, y + 1].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//상비교
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x, y - 1].gameObject.GetComponent<BlockControl>().color && blocks[x, y - 1].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x, y - 1].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x, y - 1].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//하비교
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x - 1, y].gameObject.GetComponent<BlockControl>().color && blocks[x - 1, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x - 1, y].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x - 1, y].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//좌비교
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x + 1, y].gameObject.GetComponent<BlockControl>().color && blocks[x + 1, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x + 1, y].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x + 1, y].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//우비교
                    }
                }
            }
        }


        for (int y = 0; y < Block.BLOCK_NUM_Y; y++)
        {
            for (int x = 0; x < Block.BLOCK_NUM_X; x++)
            {
                if (blocks[x, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0 && blocks[x, y].gameObject.GetComponent<BlockControl>().color == (Block.COLOR)1)
                {
                    if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum != 1)
                    {
                        isDie = true;
                        dietext.text = "빨강색이 끊어졌습니다.";
                        Debug.Log("1끊어졋습니다.");
                    }
                }
            }
        }
        for (int y = 0; y < Block.BLOCK_NUM_Y; y++)
        {
            for (int x = 0; x < Block.BLOCK_NUM_X; x++)
            {
                if (blocks[x, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0 && blocks[x, y].gameObject.GetComponent<BlockControl>().color == (Block.COLOR)2)
                {
                    if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum != 1)
                    {
                        isDie = true;

                        dietext.text = "파랑색이 끊어졌습니다.";
                        Debug.Log("2끊어졋습니다.");
                    }
                }
            }
        }
        for (int y = 0; y < Block.BLOCK_NUM_Y; y++)
        {
            for (int x = 0; x < Block.BLOCK_NUM_X; x++)
            {
                if (blocks[x, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0 && blocks[x, y].gameObject.GetComponent<BlockControl>().color == (Block.COLOR)3)
                {
                    if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum != 1)
                    {
                        isDie = true;
                        dietext.text = "노랑색이 끊어졌습니다.";
                        Debug.Log("3끊어졋습니다.");
                    }
                }
            }
        }
        for (int y = 0; y < Block.BLOCK_NUM_Y; y++)
        {
            for (int x = 0; x < Block.BLOCK_NUM_X; x++)
            {
                if (blocks[x, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0 && blocks[x, y].gameObject.GetComponent<BlockControl>().color == (Block.COLOR)4)
                {
                    if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum != 1)
                    {
                        isDie = true;
                        dietext.text = "주황색이 끊어졌습니다.";
                        Debug.Log("4끊어졋습니다.");
                    }
                }
            }
        }
        for (int y = 0; y < Block.BLOCK_NUM_Y; y++)
        {
            for (int x = 0; x < Block.BLOCK_NUM_X; x++)
            {
                if (blocks[x, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0 && blocks[x, y].gameObject.GetComponent<BlockControl>().color == (Block.COLOR)5)
                {
                    if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum != 1)
                    {
                        isDie = true;
                        dietext.text = "보라색이 끊어졌습니다.";
                        Debug.Log("5끊어졋습니다.");
                    }
                }
            }
        }
        for (int y = 0; y < Block.BLOCK_NUM_Y; y++)
        {
            for (int x = 0; x < Block.BLOCK_NUM_X; x++)
            {
                if (blocks[x, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0 && blocks[x, y].gameObject.GetComponent<BlockControl>().color == (Block.COLOR)6)
                {
                    if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum != 1)
                    {
                        isDie = true;
                        dietext.text = "초록색이 끊어졌습니다.";
                        Debug.Log("6끊어졋습니다.");
                    }
                }
            }
        }
        for (int y = 0; y < Block.BLOCK_NUM_Y; y++)
        {
            for (int x = 0; x < Block.BLOCK_NUM_X; x++)
            {
                if (blocks[x, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0 && blocks[x, y].gameObject.GetComponent<BlockControl>().color == (Block.COLOR)7)
                {
                    if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum != 1)
                    {
                        isDie = true;
                        dietext.text = "귤색이 끊어졌습니다.";
                        Debug.Log("7끊어졋습니다.");
                    }
                }
            }
        }
        for (int y = 0; y < Block.BLOCK_NUM_Y; y++)
        {
            for (int x = 0; x < Block.BLOCK_NUM_X; x++)
            {
                if (blocks[x, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0 && blocks[x, y].gameObject.GetComponent<BlockControl>().color == (Block.COLOR)8)
                {
                    if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum != 1)
                    {
                        isDie = true;
                        dietext.text = "다홍색이 끊어졌습니다.";
                        Debug.Log("8끊어졋습니다.");
                    }
                }
            }
        }
        for (int y = 0; y < Block.BLOCK_NUM_Y; y++)
        {
            for (int x = 0; x < Block.BLOCK_NUM_X; x++)
            {
                if (blocks[x, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0 && blocks[x, y].gameObject.GetComponent<BlockControl>().color == (Block.COLOR)9)
                {
                    if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum != 1)
                    {
                        isDie = true;
                        dietext.text = "남색이 끊어졌습니다.";
                        Debug.Log("9끊어졋습니다.");
                    }
                }
            }
        }
        for (int y = 0; y < Block.BLOCK_NUM_Y; y++)
        {
            for (int x = 0; x < Block.BLOCK_NUM_X; x++)
            {
                if (blocks[x, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0 && blocks[x, y].gameObject.GetComponent<BlockControl>().color == (Block.COLOR)10)
                {
                    if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum != 1)
                    {
                        isDie = true;
                        dietext.text = "자주색이 끊어졌습니다.";
                        Debug.Log("10끊어졋습니다.");
                    }
                }
            }
        }
        for (int y = 0; y < Block.BLOCK_NUM_Y; y++)
        {
            for (int x = 0; x < Block.BLOCK_NUM_X; x++)
            {
                if (blocks[x, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0 && blocks[x, y].gameObject.GetComponent<BlockControl>().color == (Block.COLOR)11)
                {
                    if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum != 1)
                    {
                        isDie = true;
                        dietext.text = "연두색이 끊어졌습니다.";
                        Debug.Log("11끊어졋습니다.");
                    }
                }
            }
        }
        for (int y = 0; y < Block.BLOCK_NUM_Y; y++)
        {
            for (int x = 0; x < Block.BLOCK_NUM_X; x++)
            {
                if (blocks[x, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0 && blocks[x, y].gameObject.GetComponent<BlockControl>().color == (Block.COLOR)12)
                {
                    if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum != 1)
                    {
                        isDie = true;
                        dietext.text = "청록색이 끊어졌습니다.";
                        Debug.Log("12끊어졋습니다.");
                    }
                }
            }
        }
    }

    //-------------------UI관련
    public void ChangeUI()
    {
        if (isDie == true)
        {
            if (isDieTrue == false)
            {
                SoundMAnager.instance.FailSound();
                isDieTrue = true;
            }


            //시간 멈추기
            Time.timeScale = 0;
            D.gameObject.SetActive(true);
        }

        if (isClear == true)
        {
            if (isClearTrue == false)
            {
                SoundMAnager.instance.ClearSound();
                isClearTrue = true;
            }

           
            Time.timeScale = 0;
            C.gameObject.SetActive(true);
        }
    }



    public List<int[,]> S_L = new List<int[,]>();
    public List<int[,]> C_L = new List<int[,]>();

    //뒤로가기 데이터 저장
    public void GoBackData()
    {
        int[,] num = new int[9, 9];
        int[,] num2 = new int[9, 9];


        for (int x = 0; x < Block.BLOCK_NUM_X; x++)
        {
            for (int y = 0; y < Block.BLOCK_NUM_Y; y++)
            {
                //일반블럭이면
                if (GameObject.Find("block(" + x + "," + y + ")").GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                {
                    num[x, y] = 0;
                }
                else if (GameObject.Find("block(" + x + "," + y + ")").GetComponent<BlockControl>().prefab == (Block.PREFAB)1)
                {
                    num[x, y] = 1;
                }
                else if (GameObject.Find("block(" + x + "," + y + ")").GetComponent<BlockControl>().prefab == (Block.PREFAB)2)
                {
                    num[x, y] = 2;
                }
                
                if (GameObject.Find("block(" + x + "," + y + ")").GetComponent<BlockControl>().color == (Block.COLOR)0)
                {
                    num2[x, y] = 0;
                }
                else if (GameObject.Find("block(" + x + "," + y + ")").GetComponent<BlockControl>().color == (Block.COLOR)1)
                {
                    num2[x, y] = 1;
                }
                else if (GameObject.Find("block(" + x + "," + y + ")").GetComponent<BlockControl>().color == (Block.COLOR)2)
                {
                    num2[x, y] = 2;
                }
                else if (GameObject.Find("block(" + x + "," + y + ")").GetComponent<BlockControl>().color == (Block.COLOR)3)
                {
                    num2[x, y] = 3;
                }
                else if (GameObject.Find("block(" + x + "," + y + ")").GetComponent<BlockControl>().color == (Block.COLOR)4)
                {
                    num2[x, y] = 4;
                }
                else if (GameObject.Find("block(" + x + "," + y + ")").GetComponent<BlockControl>().color == (Block.COLOR)5)
                {
                    num2[x, y] = 5;
                }
                else if (GameObject.Find("block(" + x + "," + y + ")").GetComponent<BlockControl>().color == (Block.COLOR)6)
                {
                    num2[x, y] = 6;
                }
                else if (GameObject.Find("block(" + x + "," + y + ")").GetComponent<BlockControl>().color == (Block.COLOR)7)
                {
                    num2[x, y] = 7;
                }
                else if (GameObject.Find("block(" + x + "," + y + ")").GetComponent<BlockControl>().color == (Block.COLOR)8)
                {
                    num2[x, y] = 8;
                }
                else if (GameObject.Find("block(" + x + "," + y + ")").GetComponent<BlockControl>().color == (Block.COLOR)9)
                {
                    num2[x, y] = 9;
                }
                else if (GameObject.Find("block(" + x + "," + y + ")").GetComponent<BlockControl>().color == (Block.COLOR)10)
                {
                    num2[x, y] = 10;
                }
                else if (GameObject.Find("block(" + x + "," + y + ")").GetComponent<BlockControl>().color == (Block.COLOR)11)
                {
                    num2[x, y] = 11;
                }
                else if (GameObject.Find("block(" + x + "," + y + ")").GetComponent<BlockControl>().color == (Block.COLOR)12)
                {
                    num2[x, y] = 12;
                }
                else if (GameObject.Find("block(" + x + "," + y + ")").GetComponent<BlockControl>().color == (Block.COLOR)(-1))
                {
                    num2[x, y] = -1;
                }
            }
        }


        S_L.Add(num);
        C_L.Add(num2);
    }

    //뒤로가기
    public void GoBack()
    {
        if (S_L.Count >= 2 && isDie != true)
        {
            int[,] num = S_L[S_L.Count - 2];
            int[,] num2 = C_L[C_L.Count - 2];

            for (int y = 0; y < Block.BLOCK_NUM_Y; y++)
            {
                for (int x = 0; x < Block.BLOCK_NUM_X; x++)
                {
                    blocks[x, y].gameObject.GetComponent<BlockControl>().prefab = (Block.PREFAB)num[x,y];
                    blocks[x, y].gameObject.GetComponent<BlockControl>().setPrefab((Block.PREFAB)num[x, y]);


                    blocks[x, y].gameObject.GetComponent<BlockControl>().color = (Block.COLOR)num2[x, y];
                    blocks[x, y].gameObject.GetComponent<BlockControl>().setColor((Block.COLOR)num2[x, y], (Block.PREFAB)num[x, y]);
                }
            }

            S_L.Remove(S_L[S_L.Count - 1]);
            C_L.Remove(C_L[C_L.Count - 1]);
        }
    }
}
