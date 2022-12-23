using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockRoot : MonoBehaviour
{
    public bool isDie = false; //�׾����� üũ
    public bool isClear = false; //Ŭ���� �ߴ��� üũ
    public bool isDieTrue = false; 
    public bool isClearTrue = false;

    public Text dietext;


    public GameObject C; //Ŭ���� UI
    public GameObject D; //���� UI

    public GameObject J; //���̽� ���� �����ð�

    public GameObject BlockPrefab = null; // ���� ����� ������.

    public BlockControl[,] blocks; // �׸���.
                                   // ����� ����� ���� ���� 9ĭ, ���� 9ĭ�� ��ġ�Ѵ�.

    ///��� �Ⱦ�--------------
    private GameObject main_camera = null; // ���� ī�޶�.
    private BlockControl grabbed_block = null; // ���� ���.
    //---------------------

    void Start()
    {
        this.main_camera = GameObject.FindGameObjectWithTag("MainCamera");
    } // ī�޶�κ��� ���콺 Ŀ���� ����ϴ� ������ ��� ���ؼ� �ʿ�

    void Update()
    { // ���콺 ��ǥ�� ��ġ���� üũ�Ѵ�. ���� �� �ִ� ������ ����� ��´�.
        Vector3 mouse_position; // ���콺 ��ġ.
        this.unprojectMousePosition(out mouse_position, Input.mousePosition); // ���콺 ��ġ�� �����´�.
        Vector2 mouse_position_xy = new Vector2(mouse_position.x, mouse_position.y); // ������ ���콺 ��ġ�� �ϳ��� Vector2�� ������.
        if (this.grabbed_block == null)
        { // ���� ����� �������.
          // if(!this.is_has_falling_block()) { // ���߿� �ּ� ����
            if (Input.GetMouseButtonDown(0))
            { // ���콺 ��ư�� ��������
                foreach (BlockControl block in this.blocks)
                { // blocks �迭�� ��� ��Ҹ� ���ʷ� ó���Ѵ�.
                    if (!block.isGrabbable())
                    { // ����� ���� �� ���ٸ�.
                        continue;
                    } // ������ ó������ �����Ѵ�.
                    if (!block.isContainedPosition(mouse_position_xy))
                    { // ���콺 ��ġ�� ��� ���� ���� �ƴϸ�.
                        continue;
                    } // ������ ó������ �����Ѵ�.
                    this.grabbed_block = block; // ó�� ���� ����� grabbed_block�� ����Ѵ�.
                    this.grabbed_block.beginGrab(); break;
                } // ����� ���� ó���� �����Ѵ�.
            } // }
        }
        else
        {

            //��� ��ü------------------------------------------------------------------------------------
            do
            {
                if (grabbed_block.GetComponent<BlockControl>().prefab == (Block.PREFAB)1)
                {
                    BlockControl swap_target = this.getNextBlock(grabbed_block, grabbed_block.slide_dir); // �����̵��� ���� ����� �����´�.
                    if (swap_target == null)
                    { // �����̵��� �� ����� ��� ������.
                        break;
                    } // ���� Ż��.
                    if (!swap_target.isGrabbable())
                    { // �����̵��� ���� ����� ���� �� �ִ� ���°� �ƴ϶��.
                        break;
                    } // ���� Ż��.
                      // ���� ��ġ���� �����̵� ��ġ������ �Ÿ��� ��´�.
                    float offset = this.grabbed_block.calcDirOffset(mouse_position_xy, this.grabbed_block.slide_dir);
                    if (offset < Block.COLLISION_SIZE / 2.0f)
                    { // ���� �Ÿ��� ��� ũ���� ���ݺ��� �۴ٸ�.
                        break;
                    } // ���� Ż��.
                    this.swapBlock(grabbed_block, grabbed_block.slide_dir, swap_target); // ����� ��ü�Ѵ�.
                    this.grabbed_block = null; // ������ ����� ��� ���� �ʴ�.
                }

            } while (false);
            //-------------------------------------------------------------------------------------





            // ����� ����� ��.
            if (!Input.GetMouseButton(0))
            { // ���콺 ��ư�� ������ ���� ������.
                this.grabbed_block.endGrab(); // ����� ���� ���� ó���� ����.
                this.grabbed_block = null;
            } // grabbed_block�� ���� ����.
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
    //��� �Ⱦ�------------------------------------------------------------
    // ref�� out �� �� ���� �������� �ʰ�, ������ ���� �Ű� ������ ���� ������, ref�� �ʱ�ȭ�� �ʿ��ϳ�, out�� ���ʿ���.
    // ������, out���� �޾ƿ� �Ű� ������ �Լ� �ȿ��� ������ �ʿ���
    public bool unprojectMousePosition(out Vector3 world_position, Vector3 mouse_position)
    {
        bool ret;
        // ���� �ۼ��Ѵ�. �� ���� ī�޶� ���ؼ� �ڷ� ���ؼ�(Vector3.back).
        // ����� ���� ũ�⸸ŭ �տ� �д�.
        Plane plane = new Plane(Vector3.back, new Vector3(0.0f, 0.0f, -Block.COLLISION_SIZE / 2.0f));
        // ī�޶�� ���콺�� ����ϴ� ���� �����.
        Ray ray = this.main_camera.GetComponent<Camera>().ScreenPointToRay(mouse_position);
        float depth;
        // ����(ray)�� ��(plane)�� ��Ҵٸ�,
        if (plane.Raycast(ray, out depth))
        { // depth�� ������ ��ϵȴ�.
          // �μ� world_position�� ���콺 ��ġ�� �����.
            world_position = ray.origin + ray.direction * depth;
            ret = true;
        }
        else
        { // ���� �ʾҴٸ�.
          // �μ� world_position�� 0�� ���ͷ� �����.
            world_position = Vector3.zero;
            ret = false;
        }
        return (ret); // ī�޶� ����ϴ� ������ ��Ͽ� ��Ҵ����� ��ȯ
    }
    //-------------------------------------------------------------------------------------

    //��� ��ü------------------------------------------------------------------------------------
    public BlockControl getNextBlock(BlockControl block, Block.DIR4 dir)
    { // �μ��� ������ ��ϰ� �������� ����� �����̵��� ���� ��� ����� �ִ��� ��ȯ.
        BlockControl next_block = null; // �����̵��� ���� ����� ���⿡ ����.
        switch (dir)
        {
            case Block.DIR4.RIGHT:
                if (block.i_pos.x < Block.BLOCK_NUM_X - 1)
                { // �׸��� ���̶��.
                    next_block = this.blocks[block.i_pos.x + 1, block.i_pos.y];
                }
                break;
            case Block.DIR4.LEFT:
                if (block.i_pos.x > 0)
                { // �׸��� ���̶��.
                    next_block = this.blocks[block.i_pos.x - 1, block.i_pos.y];
                }
                break;
            case Block.DIR4.UP:
                if (block.i_pos.y < Block.BLOCK_NUM_Y - 1)
                { // �׸��� ���̶��.
                    next_block = this.blocks[block.i_pos.x, block.i_pos.y + 1];
                }
                break;
            case Block.DIR4.DOWN:
                if (block.i_pos.y > 0)
                { // �׸��� ���̶��.
                    next_block = this.blocks[block.i_pos.x, block.i_pos.y - 1];
                }
                break;
        }
        return (next_block);
    }

    public static Vector3 getDirVector(Block.DIR4 dir)
    { // �μ��� ������ �������� ���� ��Ͽ��� ���� �������� �̵��ϴ� �� ��ȯ.
        Vector3 v = Vector3.zero;
        switch (dir)
        {
            case Block.DIR4.RIGHT: v = Vector3.right; break; // ���������� 1���� �̵�.
            case Block.DIR4.LEFT: v = Vector3.left; break; // �������� 1���� �̵�.
            case Block.DIR4.UP: v = Vector3.up; break; // ���� 1���� �̵�.
            case Block.DIR4.DOWN: v = Vector3.down; break; // �Ʒ��� 1���� �̵�.
        }
        v *= Block.COLLISION_SIZE; // ����� ũ�⸦ ���Ѵ�.
        return (v);
    }

    public static Block.DIR4 getOppositDir(Block.DIR4 dir)
    { // ����� ���� ��ü�ϱ� ���� �μ��� ������ ������ �ݴ� ������ ��ȯ.
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

    //�̰ŵ� �̳� ���� �ٲ���Ѵ�.
    //�̰ŵ� �̳� ���� �ٲ���Ѵ�.
    //�̰ŵ� �̳� ���� �ٲ���Ѵ�.
    //�̰ŵ� �̳� ���� �ٲ���Ѵ�.
    public void swapBlock(BlockControl block0, Block.DIR4 dir, BlockControl block1)
    { // ������ ����� ��ü.


        //������ ��� �޽��� ����صд�.
        Block.PREFAB mesh0 = block0.prefab;
        Block.PREFAB mesh1 = block1.prefab;

        // ������ ��� ���� ����� �д�.
        Block.COLOR color0 = block0.color;
        Block.COLOR color1 = block1.color;

      

        // ������ ����� Ȯ������ ����� �д�.
        Vector3 scale0 = block0.transform.localScale;
        Vector3 scale1 = block1.transform.localScale;
        // ������ ����� '������� �ð�'�� ����� �д�.
        float vanish_timer0 = block0.vanish_timer;
        float vanish_timer1 = block1.vanish_timer;
        // ������ ����� �̵��� ���� ���Ѵ�.
        Vector3 offset0 = BlockRoot.getDirVector(dir);
        Vector3 offset1 = BlockRoot.getDirVector(BlockRoot.getOppositDir(dir));



        //���� ������
        if (color1 == color0)
        {
            if (mesh1 == (Block.PREFAB)2)
            {
                block0.setColor(color0, mesh0);
                block1.setColor(color0, mesh0);

                //���⿡ ������Ʈ ���� ����X
                block0.setPrefab((Block.PREFAB)0);
                block1.setPrefab(mesh0);



                // Ȯ������ ��ü�Ѵ�.
                block0.transform.localScale = scale1;
                block1.transform.localScale = scale0;
                // '������� �ð�'�� ��ü�Ѵ�.
                block0.vanish_timer = vanish_timer1;
                block1.vanish_timer = vanish_timer0;

                block0.beginSlide(offset0); // ���� ��� �̵��� �����Ѵ�.
                block1.beginSlide(offset1); // �̵��� ��ġ�� ��� �̵��� �����Ѵ�.      

                //�̰ſ��� ���ڸ� ++�ϰ� end���ǿ� �ش� ���������� ���� ��ŭ�̸� ���������Ѵ�.
                //J.GetComponent<Json>().ClearCount--;
                SoundMAnager.instance.CoinSound();



                GoBackData();
            }
            else
            {
                SoundMAnager.instance.DontMoveSound();
                block0.beginSlide(offset0); // ���� ��� �̵��� �����Ѵ�.
                block1.beginSlide(offset1); // �̵��� ��ġ�� ��� �̵��� �����Ѵ�.     
            }  
        }
        //���� �ٸ���
        else
        {
            // ���� ��ü�Ѵ�. ���� ������ �̵� �Ұ�

            // ������ �����ϴ� ���? �װ� �ϳ��ϳ� �����ؼ� ���ǹ� �������� �ϳ��� ������ �� ���� �� ������ �����ؾ��Ѵ�.
            //block0.setColor(color1);
            //block1.setColor(color0);

            //���� ä�����ϴ� ����� �ƴϸ�
            if (mesh1 != (Block.PREFAB)2)
            {
                if (color1 == (Block.COLOR)1 && color0 == (Block.COLOR)3 || color1 == (Block.COLOR)3 && color0 == (Block.COLOR)1)
                {
                    block0.setColor((Block.COLOR)4, mesh1);
                    block1.setColor((Block.COLOR)4, mesh0);

                    //���⿡ ������Ʈ ���µ� �����ؾ��Ѵ�.
                    block0.setPrefab(mesh1);
                    block1.setPrefab(mesh0);
                    // Ȯ������ ��ü�Ѵ�.
                    block0.transform.localScale = scale1;
                    block1.transform.localScale = scale0;
                    // '������� �ð�'�� ��ü�Ѵ�.
                    block0.vanish_timer = vanish_timer1;
                    block1.vanish_timer = vanish_timer0;

                    block0.beginSlide(offset0); // ���� ��� �̵��� �����Ѵ�.
                    block1.beginSlide(offset1); // �̵��� ��ġ�� ��� �̵��� �����Ѵ�.
                    SoundMAnager.instance.MoveSound();
                }                //�������
                else if (color1 == (Block.COLOR)1 && color0 == (Block.COLOR)2 || color1 == (Block.COLOR)2 && color0 == (Block.COLOR)1)
                {
                    block0.setColor((Block.COLOR)5, mesh1);
                    block1.setColor((Block.COLOR)5, mesh0);

                    //���⿡ ������Ʈ ���µ� �����ؾ��Ѵ�.
                    block0.setPrefab(mesh1);
                    block1.setPrefab(mesh0);



                    // Ȯ������ ��ü�Ѵ�.
                    block0.transform.localScale = scale1;
                    block1.transform.localScale = scale0;
                    // '������� �ð�'�� ��ü�Ѵ�.
                    block0.vanish_timer = vanish_timer1;
                    block1.vanish_timer = vanish_timer0;
                    block0.beginSlide(offset0); // ���� ��� �̵��� �����Ѵ�.
                    block1.beginSlide(offset1); // �̵��� ��ġ�� ��� �̵��� �����Ѵ�.
                    SoundMAnager.instance.MoveSound();
                } //�����Ķ�
                else if (color1 == (Block.COLOR)2 && color0 == (Block.COLOR)3 || color1 == (Block.COLOR)3 && color0 == (Block.COLOR)2)
                {
                    block0.setColor((Block.COLOR)6, mesh1);
                    block1.setColor((Block.COLOR)6, mesh0);

                    //���⿡ ������Ʈ ���µ� �����ؾ��Ѵ�.
                    block0.setPrefab(mesh1);
                    block1.setPrefab(mesh0);



                    // Ȯ������ ��ü�Ѵ�.
                    block0.transform.localScale = scale1;
                    block1.transform.localScale = scale0;
                    // '������� �ð�'�� ��ü�Ѵ�.
                    block0.vanish_timer = vanish_timer1;
                    block1.vanish_timer = vanish_timer0;
                    block0.beginSlide(offset0); // ���� ��� �̵��� �����Ѵ�.
                    block1.beginSlide(offset1); // �̵��� ��ġ�� ��� �̵��� �����Ѵ�.
                    SoundMAnager.instance.MoveSound();
                } //����Ķ�
                else if (color1 == (Block.COLOR)4 && color0 == (Block.COLOR)3 || color1 == (Block.COLOR)3 && color0 == (Block.COLOR)4)
                {
                    block0.setColor((Block.COLOR)7, mesh1);
                    block1.setColor((Block.COLOR)7, mesh0);

                    //���⿡ ������Ʈ ���µ� �����ؾ��Ѵ�.
                    block0.setPrefab(mesh1);
                    block1.setPrefab(mesh0);



                    // Ȯ������ ��ü�Ѵ�.
                    block0.transform.localScale = scale1;
                    block1.transform.localScale = scale0;
                    // '������� �ð�'�� ��ü�Ѵ�.
                    block0.vanish_timer = vanish_timer1;
                    block1.vanish_timer = vanish_timer0;
                    block0.beginSlide(offset0); // ���� ��� �̵��� �����Ѵ�.
                    block1.beginSlide(offset1); // �̵��� ��ġ�� ��� �̵��� �����Ѵ�.
                    SoundMAnager.instance.MoveSound();
                } //��Ȳ���
                else if (color1 == (Block.COLOR)4 && color0 == (Block.COLOR)1 || color1 == (Block.COLOR)1 && color0 == (Block.COLOR)4)
                {
                    block0.setColor((Block.COLOR)8, mesh1);
                    block1.setColor((Block.COLOR)8, mesh0);

                    //���⿡ ������Ʈ ���µ� �����ؾ��Ѵ�.
                    block0.setPrefab(mesh1);
                    block1.setPrefab(mesh0);



                    // Ȯ������ ��ü�Ѵ�.
                    block0.transform.localScale = scale1;
                    block1.transform.localScale = scale0;
                    // '������� �ð�'�� ��ü�Ѵ�.
                    block0.vanish_timer = vanish_timer1;
                    block1.vanish_timer = vanish_timer0;
                    block0.beginSlide(offset0); // ���� ��� �̵��� �����Ѵ�.
                    block1.beginSlide(offset1); // �̵��� ��ġ�� ��� �̵��� �����Ѵ�.
                    SoundMAnager.instance.MoveSound();
                } //��Ȳ����
                else if (color1 == (Block.COLOR)7 && color0 == (Block.COLOR)1 || color1 == (Block.COLOR)1 && color0 == (Block.COLOR)7)
                {
                    block0.setColor((Block.COLOR)4, mesh1);
                    block1.setColor((Block.COLOR)4, mesh0);

                    //���⿡ ������Ʈ ���µ� �����ؾ��Ѵ�.
                    block0.setPrefab(mesh1);
                    block1.setPrefab(mesh0);



                    // Ȯ������ ��ü�Ѵ�.
                    block0.transform.localScale = scale1;
                    block1.transform.localScale = scale0;
                    // '������� �ð�'�� ��ü�Ѵ�.
                    block0.vanish_timer = vanish_timer1;
                    block1.vanish_timer = vanish_timer0;
                    block0.beginSlide(offset0); // ���� ��� �̵��� �����Ѵ�.
                    block1.beginSlide(offset1); // �̵��� ��ġ�� ��� �̵��� �����Ѵ�.
                    SoundMAnager.instance.MoveSound();
                } //�����ƻ���
                else if (color1 == (Block.COLOR)8 && color0 == (Block.COLOR)3 || color1 == (Block.COLOR)3 && color0 == (Block.COLOR)8)
                {
                    block0.setColor((Block.COLOR)4, mesh1);
                    block1.setColor((Block.COLOR)4, mesh0);

                    //���⿡ ������Ʈ ���µ� �����ؾ��Ѵ�.
                    block0.setPrefab(mesh1);
                    block1.setPrefab(mesh0);



                    // Ȯ������ ��ü�Ѵ�.
                    block0.transform.localScale = scale1;
                    block1.transform.localScale = scale0;
                    // '������� �ð�'�� ��ü�Ѵ�.
                    block0.vanish_timer = vanish_timer1;
                    block1.vanish_timer = vanish_timer0;
                    block0.beginSlide(offset0); // ���� ��� �̵��� �����Ѵ�.
                    block1.beginSlide(offset1); // �̵��� ��ġ�� ��� �̵��� �����Ѵ�.
                    SoundMAnager.instance.MoveSound();
                } //��ȫ���
                else if (color1 == (Block.COLOR)5 && color0 == (Block.COLOR)2 || color1 == (Block.COLOR)2 && color0 == (Block.COLOR)5)
                {
                    block0.setColor((Block.COLOR)9, mesh1);
                    block1.setColor((Block.COLOR)9, mesh0);

                    //���⿡ ������Ʈ ���µ� �����ؾ��Ѵ�.
                    block0.setPrefab(mesh1);
                    block1.setPrefab(mesh0);



                    // Ȯ������ ��ü�Ѵ�.
                    block0.transform.localScale = scale1;
                    block1.transform.localScale = scale0;
                    // '������� �ð�'�� ��ü�Ѵ�.
                    block0.vanish_timer = vanish_timer1;
                    block1.vanish_timer = vanish_timer0;
                    block0.beginSlide(offset0); // ���� ��� �̵��� �����Ѵ�.
                    block1.beginSlide(offset1); // �̵��� ��ġ�� ��� �̵��� �����Ѵ�.
                    SoundMAnager.instance.MoveSound();
                } //�����Ķ�
                else if (color1 == (Block.COLOR)5 && color0 == (Block.COLOR)1 || color1 == (Block.COLOR)1 && color0 == (Block.COLOR)5)
                {
                    block0.setColor((Block.COLOR)10, mesh1);
                    block1.setColor((Block.COLOR)10, mesh0);

                    //���⿡ ������Ʈ ���µ� �����ؾ��Ѵ�.
                    block0.setPrefab(mesh1);
                    block1.setPrefab(mesh0);



                    // Ȯ������ ��ü�Ѵ�.
                    block0.transform.localScale = scale1;
                    block1.transform.localScale = scale0;
                    // '������� �ð�'�� ��ü�Ѵ�.
                    block0.vanish_timer = vanish_timer1;
                    block1.vanish_timer = vanish_timer0;
                    block0.beginSlide(offset0); // ���� ��� �̵��� �����Ѵ�.
                    block1.beginSlide(offset1); // �̵��� ��ġ�� ��� �̵��� �����Ѵ�.
                    SoundMAnager.instance.MoveSound();
                } //���󻡰�
                else if (color1 == (Block.COLOR)9 && color0 == (Block.COLOR)1 || color1 == (Block.COLOR)1 && color0 == (Block.COLOR)9)
                {
                    block0.setColor((Block.COLOR)5, mesh1);
                    block1.setColor((Block.COLOR)5, mesh0);

                    //���⿡ ������Ʈ ���µ� �����ؾ��Ѵ�.
                    block0.setPrefab(mesh1);
                    block1.setPrefab(mesh0);



                    // Ȯ������ ��ü�Ѵ�.
                    block0.transform.localScale = scale1;
                    block1.transform.localScale = scale0;
                    // '������� �ð�'�� ��ü�Ѵ�.
                    block0.vanish_timer = vanish_timer1;
                    block1.vanish_timer = vanish_timer0;
                    block0.beginSlide(offset0); // ���� ��� �̵��� �����Ѵ�.
                    block1.beginSlide(offset1); // �̵��� ��ġ�� ��� �̵��� �����Ѵ�.
                    SoundMAnager.instance.MoveSound();
                } //��������
                else if (color1 == (Block.COLOR)10 && color0 == (Block.COLOR)2 || color1 == (Block.COLOR)2 && color0 == (Block.COLOR)10)
                {
                    block0.setColor((Block.COLOR)5, mesh1);
                    block1.setColor((Block.COLOR)5, mesh0);

                    //���⿡ ������Ʈ ���µ� �����ؾ��Ѵ�.
                    block0.setPrefab(mesh1);
                    block1.setPrefab(mesh0);



                    // Ȯ������ ��ü�Ѵ�.
                    block0.transform.localScale = scale1;
                    block1.transform.localScale = scale0;
                    // '������� �ð�'�� ��ü�Ѵ�.
                    block0.vanish_timer = vanish_timer1;
                    block1.vanish_timer = vanish_timer0;
                    block0.beginSlide(offset0); // ���� ��� �̵��� �����Ѵ�.
                    block1.beginSlide(offset1); // �̵��� ��ġ�� ��� �̵��� �����Ѵ�.
                    SoundMAnager.instance.MoveSound();
                } //�����Ķ�
                else if (color1 == (Block.COLOR)6 && color0 == (Block.COLOR)3 || color1 == (Block.COLOR)3 && color0 == (Block.COLOR)6)
                {
                    block0.setColor((Block.COLOR)11, mesh1);
                    block1.setColor((Block.COLOR)11, mesh0);

                    //���⿡ ������Ʈ ���µ� �����ؾ��Ѵ�.
                    block0.setPrefab(mesh1);
                    block1.setPrefab(mesh0);



                    // Ȯ������ ��ü�Ѵ�.
                    block0.transform.localScale = scale1;
                    block1.transform.localScale = scale0;
                    // '������� �ð�'�� ��ü�Ѵ�.
                    block0.vanish_timer = vanish_timer1;
                    block1.vanish_timer = vanish_timer0;
                    block0.beginSlide(offset0); // ���� ��� �̵��� �����Ѵ�.
                    block1.beginSlide(offset1); // �̵��� ��ġ�� ��� �̵��� �����Ѵ�.
                    SoundMAnager.instance.MoveSound();
                } //�ʷϳ��
                else if (color1 == (Block.COLOR)6 && color0 == (Block.COLOR)2 || color1 == (Block.COLOR)2 && color0 == (Block.COLOR)6)
                {
                    block0.setColor((Block.COLOR)12, mesh1);
                    block1.setColor((Block.COLOR)12, mesh0);

                    //���⿡ ������Ʈ ���µ� �����ؾ��Ѵ�.
                    block0.setPrefab(mesh1);
                    block1.setPrefab(mesh0);



                    // Ȯ������ ��ü�Ѵ�.
                    block0.transform.localScale = scale1;
                    block1.transform.localScale = scale0;
                    // '������� �ð�'�� ��ü�Ѵ�.
                    block0.vanish_timer = vanish_timer1;
                    block1.vanish_timer = vanish_timer0;
                    block0.beginSlide(offset0); // ���� ��� �̵��� �����Ѵ�.
                    block1.beginSlide(offset1); // �̵��� ��ġ�� ��� �̵��� �����Ѵ�.
                    SoundMAnager.instance.MoveSound();
                } //�ʷ��Ķ�
                else if (color1 == (Block.COLOR)11 && color0 == (Block.COLOR)2 || color1 == (Block.COLOR)2 && color0 == (Block.COLOR)11)
                {
                    block0.setColor((Block.COLOR)6, mesh1);
                    block1.setColor((Block.COLOR)6, mesh0);

                    //���⿡ ������Ʈ ���µ� �����ؾ��Ѵ�.
                    block0.setPrefab(mesh1);
                    block1.setPrefab(mesh0);



                    // Ȯ������ ��ü�Ѵ�.
                    block0.transform.localScale = scale1;
                    block1.transform.localScale = scale0;
                    // '������� �ð�'�� ��ü�Ѵ�.
                    block0.vanish_timer = vanish_timer1;
                    block1.vanish_timer = vanish_timer0;
                    block0.beginSlide(offset0); // ���� ��� �̵��� �����Ѵ�.
                    block1.beginSlide(offset1); // �̵��� ��ġ�� ��� �̵��� �����Ѵ�.
                    SoundMAnager.instance.MoveSound();
                } //�����Ķ�
                else if (color1 == (Block.COLOR)12 && color0 == (Block.COLOR)3 || color1 == (Block.COLOR)3 && color0 == (Block.COLOR)12)
                {
                    block0.setColor((Block.COLOR)6, mesh1);
                    block1.setColor((Block.COLOR)6, mesh0);

                    //���⿡ ������Ʈ ���µ� �����ؾ��Ѵ�.
                    block0.setPrefab(mesh1);
                    block1.setPrefab(mesh0);



                    // Ȯ������ ��ü�Ѵ�.
                    block0.transform.localScale = scale1;
                    block1.transform.localScale = scale0;
                    // '������� �ð�'�� ��ü�Ѵ�.
                    block0.vanish_timer = vanish_timer1;
                    block1.vanish_timer = vanish_timer0;
                    block0.beginSlide(offset0); // ���� ��� �̵��� �����Ѵ�.
                    block1.beginSlide(offset1); // �̵��� ��ġ�� ��� �̵��� �����Ѵ�.
                    SoundMAnager.instance.MoveSound();
                } //û�ϳ��
                else if (color1 == (Block.COLOR)(0))
                {
                    block0.setColor(color0, mesh1);
                    block1.setColor(color0, mesh0);

                    //���⿡ ������Ʈ ���µ� �����ؾ��Ѵ�.
                    block0.setPrefab(mesh1);
                    block1.setPrefab(mesh0);

                    // Ȯ������ ��ü�Ѵ�.
                    block0.transform.localScale = scale1;
                    block1.transform.localScale = scale0;
                    // '������� �ð�'�� ��ü�Ѵ�.
                    block0.vanish_timer = vanish_timer1;
                    block1.vanish_timer = vanish_timer0;
                    block0.beginSlide(offset0); // ���� ��� �̵��� �����Ѵ�.
                    block1.beginSlide(offset1); // �̵��� ��ġ�� ��� �̵��� �����Ѵ�.
                    SoundMAnager.instance.MoveSound();
                } //�ٲ�»��� ȸ���̸�
                else if (color1 == (Block.COLOR)(-1))
                {
                    SoundMAnager.instance.DontMoveSound();
                    block0.beginSlide(offset0); // ���� ��� �̵��� �����Ѵ�.
                    block1.beginSlide(offset1); // �̵��� ��ġ�� ��� �̵��� �����Ѵ�.
                }
                else
                {
                    block0.setColor((Block.COLOR)(-1), mesh1);
                    block1.setColor((Block.COLOR)(-1), mesh0);

                    //���⿡ ������Ʈ ���µ� �����ؾ��Ѵ�.
                    block0.setPrefab(mesh1);
                    block1.setPrefab(mesh0);



                    // Ȯ������ ��ü�Ѵ�.
                    block0.transform.localScale = scale1;
                    block1.transform.localScale = scale0;
                    // '������� �ð�'�� ��ü�Ѵ�.
                    block0.vanish_timer = vanish_timer1;
                    block1.vanish_timer = vanish_timer0;
                    block0.beginSlide(offset0); // ���� ��� �̵��� �����Ѵ�.
                    block1.beginSlide(offset1); // �̵��� ��ġ�� ��� �̵��� �����Ѵ�.


                    isDie = true;
                    dietext.text = "�� ���ս���";
                    Debug.Log("�� ���ս���");
                }

                GoBackData();
            }
            //���� ä�����ϴ� ����̸�
            else
            {
                block0.setColor(color0, mesh0);
                block1.setColor(color1, mesh1);

                //���⿡ ������Ʈ ���µ� �����ؾ��Ѵ�.
                block0.setPrefab(mesh0);
                block1.setPrefab(mesh1);



                // Ȯ������ ��ü�Ѵ�.
                block0.transform.localScale = scale1;
                block1.transform.localScale = scale0;
                // '������� �ð�'�� ��ü�Ѵ�.
                block0.vanish_timer = vanish_timer1;
                block1.vanish_timer = vanish_timer0;
                block0.beginSlide(offset0); // ���� ��� �̵��� �����Ѵ�.
                block1.beginSlide(offset1); // �̵��� ��ġ�� ��� �̵��� �����Ѵ�.
                SoundMAnager.instance.DontMoveSound();
            }
        }

        
    }


    //-------------------------------------------------------------------------------------

    //��� ����-----------------------
    public void initialSetUp()
    {
        //this.blocks = new BlockControl[Block.BLOCK_NUM_X, Block.BLOCK_NUM_Y]; // �׸����� ũ�⸦ 9��9�� �Ѵ�.
        this.blocks = new BlockControl[Block.BLOCK_NUM_X, Block.BLOCK_NUM_Y]; // �׸����� ũ�⸦ 9��9�� �Ѵ�.
        int color_index = 0; // ����� �� ��ȣ.


        //------
        int mesh_index = 0;
        //------

        for (int y = 0; y < Block.BLOCK_NUM_X; y++)
        { // ó��~��������
            for (int x = 0; x < Block.BLOCK_NUM_X; x++)
            { // ����~������

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


                // BlockPrefab�� �ν��Ͻ��� ���� �����.
                GameObject game_object = Instantiate(this.BlockPrefab) as GameObject;
                BlockControl block = game_object.GetComponent<BlockControl>(); // ����� BlockControl Ŭ������ �����´�.
                this.blocks[x, y] = block; // ����� �׸��忡 �����Ѵ�.
                block.i_pos.x = x; // ����� ��ġ ����(�׸��� ��ǥ)�� �����Ѵ�.
                block.i_pos.y = y;
                block.block_root = this; // �� BlockControl�� ������ GameRoot�� �ڽ��̶�� �����Ѵ�.
                Vector3 position = BlockRoot.calcBlockPosition(block.i_pos); // �׸��� ��ǥ�� ���� ��ġ(���� ��ǥ)�� ��ȯ
                block.transform.position = position; // ���� ��� ��ġ�� �̵��Ѵ�.


                block.setPrefab((Block.PREFAB)mesh_index);

                block.setColor((Block.COLOR)color_index, (Block.PREFAB)mesh_index); // ����� ���� �����Ѵ�.
                                                                                    // ����� �̸��� ����(�ļ�)�Ѵ�. ���߿� ��� ���� Ȯ�ζ� �ʿ�.
                block.name = "block(" + block.i_pos.x.ToString() + "," + block.i_pos.y.ToString() + ")";
            }
        }
    }

    // ������ �׸��� ��ǥ�� �������� ��ǥ�� ���Ѵ�.
    public static Vector3 calcBlockPosition(Block.iPosition i_pos)
    {
        // ��ġ�� ���� �� ���� ��ġ�� �ʱⰪ���� �����Ѵ�.
        Vector3 position = new Vector3(-(Block.BLOCK_NUM_X / 2.0f - 0.5f), -(Block.BLOCK_NUM_Y / 2.0f - 0.5f), 0.0f);
        // �ʱ갪 + �׸��� ��ǥ �� ��� ũ��.
        position.x += (float)i_pos.x * Block.COLLISION_SIZE;
        position.y += (float)i_pos.y * Block.COLLISION_SIZE;
        return (position); // �������� ��ǥ�� ��ȯ�Ѵ�.
    }
    //---------------------


    public int p_C; //��ũ��� ī��Ʈ
    public int o_C; //��������� ī��Ʈ


    public int PPP; //1�� ��

    public bool pBool = true; //��ũ�� ��

    //End ����
    //---------------------
    public void EndGame()
    {
        //�̰Ŵ� ����� ��ä���� ���� ����

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
                        //�����¿� ���ؾ��Ѵ�.
                        //�� ��

                        if (x + 1 < Block.BLOCK_NUM_X && y + 1 < Block.BLOCK_NUM_Y && x != 0 && y != 0)
                        {

                            int p1 = 0;

                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x, y + 1].gameObject.GetComponent<BlockControl>().color && blocks[x, y + 1].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                            {
                                //������ �ִ�
                                //continue;
                                p1++;
                            }//���
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x, y - 1].gameObject.GetComponent<BlockControl>().color && blocks[x, y - 1].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                            {
                                p1++;
                                //continue;
                            }//�Ϻ�
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x - 1, y].gameObject.GetComponent<BlockControl>().color && blocks[x - 1, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                            {
                                p1++;
                                //continue;
                            }//�º�
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x + 1, y].gameObject.GetComponent<BlockControl>().color && blocks[x + 1, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                            {
                                p1++;
                                //continue;
                            }//���

                            //p1�� �����ִ� ���� �� �̰� �� ����� ������ ����־�


                            blocks[x, y].gameObject.GetComponent<BlockControl>().loadNum = p1;



                            //�� ���ǿ� �ش��ϴ°� ���ٸ�
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

    //---------------------��������(����������)
    public void CKKKKKK()
    {
        //�̰� �� ����ŭ �־���Ѵ�.
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
                    //�����¿� ���ؾ��Ѵ�.
                    //�� ��

                    if (x + 1 < Block.BLOCK_NUM_X && y + 1 < Block.BLOCK_NUM_Y && x != 0 && y != 0)
                    {
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x, y + 1].gameObject.GetComponent<BlockControl>().color && blocks[x, y + 1].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            //������ �ִ�
                            //�۰ų� ������ �Ǻ� ������ �� ũ��
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x, y + 1].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x, y + 1].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//���
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x, y - 1].gameObject.GetComponent<BlockControl>().color && blocks[x, y - 1].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x, y - 1].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x, y - 1].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//�Ϻ�
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x - 1, y].gameObject.GetComponent<BlockControl>().color && blocks[x - 1, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x - 1, y].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x - 1, y].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//�º�
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x + 1, y].gameObject.GetComponent<BlockControl>().color && blocks[x + 1, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x + 1, y].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x + 1, y].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//���
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
                    //�����¿� ���ؾ��Ѵ�.
                    //�� ��

                    if (x + 1 < Block.BLOCK_NUM_X && y + 1 < Block.BLOCK_NUM_Y && x != 0 && y != 0)
                    {
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x, y + 1].gameObject.GetComponent<BlockControl>().color && blocks[x, y + 1].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            //������ �ִ�
                            //�۰ų� ������ �Ǻ� ������ �� ũ��
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x, y + 1].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x, y + 1].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//���
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x, y - 1].gameObject.GetComponent<BlockControl>().color && blocks[x, y - 1].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x, y - 1].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x, y - 1].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//�Ϻ�
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x - 1, y].gameObject.GetComponent<BlockControl>().color && blocks[x - 1, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x - 1, y].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x - 1, y].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//�º�
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x + 1, y].gameObject.GetComponent<BlockControl>().color && blocks[x + 1, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x + 1, y].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x + 1, y].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//���
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
                    //�����¿� ���ؾ��Ѵ�.
                    //�� ��

                    if (x + 1 < Block.BLOCK_NUM_X && y + 1 < Block.BLOCK_NUM_Y && x != 0 && y != 0)
                    {
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x, y + 1].gameObject.GetComponent<BlockControl>().color && blocks[x, y + 1].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            //������ �ִ�
                            //�۰ų� ������ �Ǻ� ������ �� ũ��
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x, y + 1].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x, y + 1].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//���
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x, y - 1].gameObject.GetComponent<BlockControl>().color && blocks[x, y - 1].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x, y - 1].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x, y - 1].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//�Ϻ�
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x - 1, y].gameObject.GetComponent<BlockControl>().color && blocks[x - 1, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x - 1, y].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x - 1, y].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//�º�
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x + 1, y].gameObject.GetComponent<BlockControl>().color && blocks[x + 1, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x + 1, y].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x + 1, y].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//���
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
                    //�����¿� ���ؾ��Ѵ�.
                    //�� ��

                    if (x + 1 < Block.BLOCK_NUM_X && y + 1 < Block.BLOCK_NUM_Y && x != 0 && y != 0)
                    {
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x, y + 1].gameObject.GetComponent<BlockControl>().color && blocks[x, y + 1].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            //������ �ִ�
                            //�۰ų� ������ �Ǻ� ������ �� ũ��
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x, y + 1].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x, y + 1].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//���
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x, y - 1].gameObject.GetComponent<BlockControl>().color && blocks[x, y - 1].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x, y - 1].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x, y - 1].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//�Ϻ�
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x - 1, y].gameObject.GetComponent<BlockControl>().color && blocks[x - 1, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x - 1, y].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x - 1, y].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//�º�
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x + 1, y].gameObject.GetComponent<BlockControl>().color && blocks[x + 1, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x + 1, y].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x + 1, y].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//���
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
                    //�����¿� ���ؾ��Ѵ�.
                    //�� ��

                    if (x + 1 < Block.BLOCK_NUM_X && y + 1 < Block.BLOCK_NUM_Y && x != 0 && y != 0)
                    {
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x, y + 1].gameObject.GetComponent<BlockControl>().color && blocks[x, y + 1].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            //������ �ִ�
                            //�۰ų� ������ �Ǻ� ������ �� ũ��
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x, y + 1].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x, y + 1].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//���
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x, y - 1].gameObject.GetComponent<BlockControl>().color && blocks[x, y - 1].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x, y - 1].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x, y - 1].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//�Ϻ�
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x - 1, y].gameObject.GetComponent<BlockControl>().color && blocks[x - 1, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x - 1, y].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x - 1, y].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//�º�
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x + 1, y].gameObject.GetComponent<BlockControl>().color && blocks[x + 1, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x + 1, y].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x + 1, y].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//���
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
                    //�����¿� ���ؾ��Ѵ�.
                    //�� ��

                    if (x + 1 < Block.BLOCK_NUM_X && y + 1 < Block.BLOCK_NUM_Y && x != 0 && y != 0)
                    {
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x, y + 1].gameObject.GetComponent<BlockControl>().color && blocks[x, y + 1].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            //������ �ִ�
                            //�۰ų� ������ �Ǻ� ������ �� ũ��
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x, y + 1].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x, y + 1].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//���
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x, y - 1].gameObject.GetComponent<BlockControl>().color && blocks[x, y - 1].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x, y - 1].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x, y - 1].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//�Ϻ�
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x - 1, y].gameObject.GetComponent<BlockControl>().color && blocks[x - 1, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x - 1, y].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x - 1, y].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//�º�
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x + 1, y].gameObject.GetComponent<BlockControl>().color && blocks[x + 1, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x + 1, y].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x + 1, y].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//���
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
                    //�����¿� ���ؾ��Ѵ�.
                    //�� ��

                    if (x + 1 < Block.BLOCK_NUM_X && y + 1 < Block.BLOCK_NUM_Y && x != 0 && y != 0)
                    {
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x, y + 1].gameObject.GetComponent<BlockControl>().color && blocks[x, y + 1].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            //������ �ִ�
                            //�۰ų� ������ �Ǻ� ������ �� ũ��
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x, y + 1].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x, y + 1].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//���
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x, y - 1].gameObject.GetComponent<BlockControl>().color && blocks[x, y - 1].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x, y - 1].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x, y - 1].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//�Ϻ�
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x - 1, y].gameObject.GetComponent<BlockControl>().color && blocks[x - 1, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x - 1, y].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x - 1, y].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//�º�
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x + 1, y].gameObject.GetComponent<BlockControl>().color && blocks[x + 1, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x + 1, y].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x + 1, y].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//���
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
                    //�����¿� ���ؾ��Ѵ�.
                    //�� ��

                    if (x + 1 < Block.BLOCK_NUM_X && y + 1 < Block.BLOCK_NUM_Y && x != 0 && y != 0)
                    {
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x, y + 1].gameObject.GetComponent<BlockControl>().color && blocks[x, y + 1].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            //������ �ִ�
                            //�۰ų� ������ �Ǻ� ������ �� ũ��
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x, y + 1].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x, y + 1].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//���
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x, y - 1].gameObject.GetComponent<BlockControl>().color && blocks[x, y - 1].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x, y - 1].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x, y - 1].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//�Ϻ�
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x - 1, y].gameObject.GetComponent<BlockControl>().color && blocks[x - 1, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x - 1, y].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x - 1, y].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//�º�
                        if (blocks[x, y].gameObject.GetComponent<BlockControl>().color == blocks[x + 1, y].gameObject.GetComponent<BlockControl>().color && blocks[x + 1, y].gameObject.GetComponent<BlockControl>().prefab == (Block.PREFAB)0)
                        {
                            if (blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum >= blocks[x + 1, y].gameObject.GetComponent<BlockControl>().rootNum)
                            {
                                blocks[x, y].gameObject.GetComponent<BlockControl>().rootNum = blocks[x + 1, y].gameObject.GetComponent<BlockControl>().rootNum;
                            }
                        }//���
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
                        dietext.text = "�������� ���������ϴ�.";
                        Debug.Log("1������ϴ�.");
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

                        dietext.text = "�Ķ����� ���������ϴ�.";
                        Debug.Log("2������ϴ�.");
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
                        dietext.text = "������� ���������ϴ�.";
                        Debug.Log("3������ϴ�.");
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
                        dietext.text = "��Ȳ���� ���������ϴ�.";
                        Debug.Log("4������ϴ�.");
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
                        dietext.text = "������� ���������ϴ�.";
                        Debug.Log("5������ϴ�.");
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
                        dietext.text = "�ʷϻ��� ���������ϴ�.";
                        Debug.Log("6������ϴ�.");
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
                        dietext.text = "�ֻ��� ���������ϴ�.";
                        Debug.Log("7������ϴ�.");
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
                        dietext.text = "��ȫ���� ���������ϴ�.";
                        Debug.Log("8������ϴ�.");
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
                        dietext.text = "������ ���������ϴ�.";
                        Debug.Log("9������ϴ�.");
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
                        dietext.text = "���ֻ��� ���������ϴ�.";
                        Debug.Log("10������ϴ�.");
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
                        dietext.text = "���λ��� ���������ϴ�.";
                        Debug.Log("11������ϴ�.");
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
                        dietext.text = "û�ϻ��� ���������ϴ�.";
                        Debug.Log("12������ϴ�.");
                    }
                }
            }
        }
    }

    //-------------------UI����
    public void ChangeUI()
    {
        if (isDie == true)
        {
            if (isDieTrue == false)
            {
                SoundMAnager.instance.FailSound();
                isDieTrue = true;
            }


            //�ð� ���߱�
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

    //�ڷΰ��� ������ ����
    public void GoBackData()
    {
        int[,] num = new int[9, 9];
        int[,] num2 = new int[9, 9];


        for (int x = 0; x < Block.BLOCK_NUM_X; x++)
        {
            for (int y = 0; y < Block.BLOCK_NUM_Y; y++)
            {
                //�Ϲݺ��̸�
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

    //�ڷΰ���
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
