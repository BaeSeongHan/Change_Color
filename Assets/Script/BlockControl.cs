using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{ // ��Ͽ� ���� ������ �ٷ��.
    public static float COLLISION_SIZE = 1.0f; // ����� �浹 ũ��.
    public static float VANISH_TIME = 3.0f; // �� �ٰ� ����� �������� �ð�.
    public struct iPosition
    { // �׸��忡���� ��ǥ�� ��Ÿ���� ����ü.
        public int x; // X ��ǥ.
        public int y; // Y ��ǥ.
    }
    public enum COLOR
    { // ��� ����.
        NONE = -1, // �� ���� ����.
        GRAY = 0, RED, BLUE, YELLOW, ORANGE, PURPLE, GREEN, // 0.ȸ��
        PEACH, CRIMSON, INDIGO, REDPURPLE, LIGHTGREEN, TURQUOISE, // 
        NUM, // �÷��� �� �������� ��Ÿ����(=7).
        FIRST = GRAY, LAST = TURQUOISE, // �ʱ� �÷�(��ȫ��), ���� �÷�(ȸ��).
        NORMAL_COLOR_NUM = GRAY, // ���� �÷�(ȸ�� �̿��� ��)�� ��.
    };
    public enum DIR4
    { // �����¿� �� ����.
        NONE = -1, // �������� ����.
        RIGHT, LEFT, UP, DOWN, // ��. ��, ��, ��.
        NUM, // ������ �� ���� �ִ��� ��Ÿ����(=4).
    };


    //�̰ɷ� �� ������ ����
    public static int BLOCK_NUM_X = 9; // ����� ��ġ�� �� �ִ� X���� �ִ��.
    public static int BLOCK_NUM_Y = 9; // ����� ��ġ�� �� �ִ� Y���� �ִ��.


    public static bool FillColor = false;

    //-----------------------------
    public enum PREFAB
    {
        NONE = -1, //������ ����
        BLOCK = 0, MOVEBLOCK, FILLBLOCK   //0�Ϲݺ� , 1�����̴º�. 2�� ä�����ϴ� ��
    }
    //-----------------------------

    public enum STEP
    {
        // ���� ���� ����, ��� ��, ���� ����, ������ ����, �����̵� ��, �Ҹ� ��, ����� ��, ���� ��, ũ�� �����̵� ��, ���� ����.
        NONE = -1, IDLE = 0, GRABBED, RELEASED, SLIDE, VACANT, RESPAWN, FALL, LONG_SLIDE, NUM,
    };
}

public class BlockControl : MonoBehaviour
{
    //-----
    public Block.PREFAB prefab = (Block.PREFAB)0; //���� ������

    public Mesh block_mesh; //����
    public Mesh moveblock_mesh; //����
    public Mesh fillblock_mesh; //����
    public Material transparent_material; // ������ ��Ƽ����.

    public Material PURPLE;//�����
    public Material PEACH;//�����ƻ�
    public Material ORANGE;//������
    public Material INDIGO;//����
    public Material REDPURPLE;//���ֻ�
    public Material LIGHTGREEN;//���λ�
    public Material TURQUOISE;//û�ϻ�
    public Material CRIMSON;//��ȫ��


    //-----
    public int loadNum = 0;
    public int rootNum = 0;



    public bool loadCK = false;
    //-----

    public bool fillcolor = false;

    public Block.COLOR color = (Block.COLOR)0; // ��� ��.
    public BlockRoot block_root = null; // ����� ��.
    public Block.iPosition i_pos; // ��� ��ǥ.
    //���콺�� ��� �Ⱦ�-----------------------------------------------------------------------------
    public Block.STEP step = Block.STEP.NONE; // ���� ����.
    public Block.STEP next_step = Block.STEP.NONE; // ���� ����.
    private Vector3 position_offset_initial = Vector3.zero; // ��ü �� ��ġ.
    public Vector3 position_offset = Vector3.zero; // ��ü �� ��ġ.
    //-----------------------------------------------------------------------------------------------
    //��� ��ü------------------------------------------------------------------------------------
    public float vanish_timer = -1.0f; // ����� ����� �������� �ð�.
    public Block.DIR4 slide_dir = Block.DIR4.NONE; // �����̵�� ����.
    public float step_timer = 0.0f; // ����� ��ü�� ���� �̵��ð� ��.
    public Block.DIR4 calcSlideDir(Vector2 mouse_position) { // ���콺 ��ġ�� �������� �����̵�� ������ ���Ѵ�.
        Block.DIR4 dir = Block.DIR4.NONE;
        Vector2 v = mouse_position - new Vector2(this.transform.position.x, this.transform.position.y); // ������ mouse_positio�� ���� ��ġ�� ���� ��Ÿ��.
        if (v.magnitude > 0.1f)
        { // ������ ũ�Ⱑ 0.1���� ũ��. �׺��� ������ �����̵����� ���� �ɷ� �����Ѵ�.
            if (v.y > v.x)
            {
                if (v.y > -v.x)
                {
                    dir = Block.DIR4.UP;
                }
                else { dir = Block.DIR4.LEFT; }
            }
            else
            {
                if (v.y > -v.x)
                {
                    dir = Block.DIR4.RIGHT;
                }
                else { dir = Block.DIR4.DOWN; }
            }
        }
        return (dir);
    }
    public float calcDirOffset(Vector2 position, Block.DIR4 dir)
    { // ���� ��ġ�� �����̵��� ���� �Ÿ��� ��� �����ΰ� ��ȯ�Ѵ�.
        float offset = 0.0f;
        Vector2 v = position - new Vector2(this.transform.position.x, this.transform.position.y); // ������ ��ġ�� ����� ���� ��ġ�� ���� ��Ÿ���� ����.
        switch (dir)
        { // ������ ���⿡ ���� ��������.
            case Block.DIR4.RIGHT: offset = v.x; break;
            case Block.DIR4.LEFT: offset = -v.x; break;
            case Block.DIR4.UP: offset = v.y; break;
            case Block.DIR4.DOWN: offset = -v.y; break;
        }
        return (offset);
    }
    public void beginSlide(Vector3 offset)
    { // �̵� ������ �˸��� �޼���
        this.position_offset_initial = offset;
        this.position_offset = this.position_offset_initial;
        this.next_step = Block.STEP.SLIDE; // ���¸� SLIDE�� ����.
    }

    //-----------------------------------------------------------------------------------------------
    void Start()
    {
        this.setColor(this.color, this.prefab);
        this.next_step = Block.STEP.IDLE;

        //�޽�����-------
        this.setPrefab(this.prefab);
        //-------
    } // ���� ����� ���������.
    void Update()
    { // ����� ������ ���� ����� ũ�� �ϰ� �׷��� ���� ���� ������� ���ư�
        Vector3 mouse_position; // ���콺 ��ġ.
        this.block_root.unprojectMousePosition(out mouse_position, Input.mousePosition); // ���콺 ��ġ ȹ��.
        Vector2 mouse_position_xy = new Vector2(mouse_position.x, mouse_position.y); // ȹ���� ���콺 ��ġ�� X�� Y������ �Ѵ�.
        
        //��� ��ü------------------------------------------------------------------------------------
        this.step_timer += Time.deltaTime;
        float slide_time = 0.2f;
        if (this.next_step == Block.STEP.NONE)
        { // '���� ���� ����'�� ���.
            switch (this.step)
            {
                case Block.STEP.SLIDE:
                    if (this.step_timer >= slide_time)
                    { // �����̵� ���� ����� �Ҹ�Ǹ� VACANT(�����) ���·� ����.
                        if (this.vanish_timer == 0.0f)
                        {
                            this.next_step = Block.STEP.VACANT;
                        }
                        else
                        { // vanish_timer�� 0�� �ƴϸ� IDLE(���) ���·� ����.
                            this.next_step = Block.STEP.IDLE;
                        }
                    }
                    break;
            }
        }
        //-----------------------------------------------------------------------------------------------

        while (this.next_step != Block.STEP.NONE)
        { // '���� ���' ���°� '���� ����' �̿��� ����. ��> '���� ���' ���°� ����� ���.
            this.step = this.next_step;
            this.next_step = Block.STEP.NONE;
            switch (this.step)
            {
                case Block.STEP.IDLE: // '���' ����.
                    this.position_offset = Vector3.zero;
                    this.transform.localScale = Vector3.one * 1.0f; break; // ��� ǥ�� ũ�⸦ ���� ũ��� �Ѵ�.
                case Block.STEP.GRABBED: // '����' ����.
                    this.transform.localScale = Vector3.one * 1.2f; break; // ��� ǥ�� ũ�⸦ ũ�� �Ѵ�.
                case Block.STEP.RELEASED: // '������ �ִ�' ����.
                    this.position_offset = Vector3.zero;
                    this.transform.localScale = Vector3.one * 1.0f; break;
                case Block.STEP.VACANT:
                    this.position_offset = Vector3.zero; break;
            } // ��� ǥ�� ũ�⸦ ���� ������� �Ѵ�.
            this.step_timer = 0.0f;
        }
        //��� ��ü------------------------------------------------------------------------------------
        switch (this.step)
        {
            case Block.STEP.GRABBED: // ���� ����.
                this.slide_dir = this.calcSlideDir(mouse_position_xy); break; // ���� ������ ���� �׻� �����̵� ������ üũ.
            case Block.STEP.SLIDE: // �����̵�(��ü) ��.
                float rate = this.step_timer / slide_time; // ����� ������ �̵��ϴ� ó��.
                rate = Mathf.Min(rate, 1.0f);
                rate = Mathf.Sin(rate * Mathf.PI / 2.0f);
                this.position_offset = Vector3.Lerp(this.position_offset_initial, Vector3.zero, rate); break;
        }
        //-----------------------------------------------------------------------------------------------
        
        // �׸��� ��ǥ�� ���� ��ǥ(���� ��ǥ)�� ��ȯ�ϰ�.
        Vector3 position = BlockRoot.calcBlockPosition(this.i_pos) + this.position_offset; // position_offset�� �߰��Ѵ�.
        this.transform.position = position; // ���� ��ġ�� ���ο� ��ġ�� �����Ѵ�.
    }




    // �μ� color�� ������ ����� ĥ�Ѵ�.
    public void setColor(Block.COLOR color, Block.PREFAB prefab)
    {
        if (prefab == (Block.PREFAB)2)
        {
            this.color = color; // �̹��� ������ ���� ��� ������ �����Ѵ�.
            Color color_value; // Color Ŭ������ ���� ��Ÿ����.
            switch (this.color)
            { // ĥ�� ���� ���� ��������.
                default:

                case Block.COLOR.NONE:
                    color_value = Color.black; break;
                case Block.COLOR.GRAY:
                    color_value = Color.gray; break;
                case Block.COLOR.RED:
                    color_value = Color.red; break;
                case Block.COLOR.BLUE:
                    color_value = Color.blue; break;
                case Block.COLOR.YELLOW:
                    color_value = Color.yellow; break;
                case Block.COLOR.ORANGE:
                    color_value = ORANGE.color; break;
                case Block.COLOR.PURPLE:
                    color_value = PURPLE.color; break;
                case Block.COLOR.GREEN:
                    color_value = Color.green; break;
                case Block.COLOR.PEACH:
                    color_value = PEACH.color; break;
                case Block.COLOR.CRIMSON:
                    //color_value = Color.magenta; break;
                    color_value = CRIMSON.color; break;
                case Block.COLOR.INDIGO:
                    color_value = INDIGO.color; break;
                case Block.COLOR.REDPURPLE:
                    color_value = REDPURPLE.color; break;
                case Block.COLOR.LIGHTGREEN:
                    color_value = LIGHTGREEN.color; break;
                case Block.COLOR.TURQUOISE:
                    color_value = TURQUOISE.color; break;
                    /*
                case Block.COLOR.NONE:
                    color_value = Color.Lerp(Color.black, Color.white, 0.5f); break;
                case Block.COLOR.GRAY:
                    color_value = Color.Lerp(Color.gray, Color.white, 0.5f); break;
                case Block.COLOR.RED:
                    color_value = Color.Lerp(Color.red, Color.white, 0.5f); break;
                case Block.COLOR.BLUE:
                    color_value = Color.Lerp(Color.blue, Color.white, 0.5f); break;
                case Block.COLOR.YELLOW:
                    color_value = Color.Lerp(Color.yellow, Color.white, 0.5f); break;
                case Block.COLOR.ORANGE:
                    color_value = Color.Lerp(ORANGE.color, Color.white, 0.5f); break;
                case Block.COLOR.PURPLE:
                    color_value = Color.Lerp(PURPLE.color, Color.white, 0.5f); break;
                case Block.COLOR.GREEN:
                    color_value = Color.Lerp(Color.green, Color.white, 0.5f); break;
                case Block.COLOR.PEACH:
                    color_value = Color.Lerp(PEACH.color, Color.white, 0.5f); break;
                case Block.COLOR.CRIMSON:
                    color_value = Color.Lerp(Color.magenta, Color.white, 0.5f); break;
                case Block.COLOR.INDIGO:
                    color_value = Color.Lerp(INDIGO.color, Color.white, 0.5f); break;
                case Block.COLOR.REDPURPLE:
                    color_value = Color.Lerp(REDPURPLE.color, Color.white, 0.5f); break;
                case Block.COLOR.LIGHTGREEN:
                    color_value = Color.Lerp(LIGHTGREEN.color, Color.white, 0.5f); break;
                case Block.COLOR.TURQUOISE:
                    color_value = Color.Lerp(TURQUOISE.color, Color.white, 0.5f); break;
                    */

            }
            this.GetComponent<Renderer>().material.color = color_value; // �� ���� ������Ʈ�� ������ �����Ѵ�.

        }
        else
        {
            this.color = color; // �̹��� ������ ���� ��� ������ �����Ѵ�.
            Color color_value; // Color Ŭ������ ���� ��Ÿ����.
            switch (this.color)
            { // ĥ�� ���� ���� ��������.
                default:

                case Block.COLOR.NONE:
                    color_value = Color.black; break;
                case Block.COLOR.GRAY:
                    color_value = Color.gray; break;
                case Block.COLOR.RED:
                    color_value = Color.red; break;
                case Block.COLOR.BLUE:
                    color_value = Color.blue; break;
                case Block.COLOR.YELLOW:
                    color_value = Color.yellow; break;
                case Block.COLOR.ORANGE:
                    color_value = ORANGE.color; break;
                case Block.COLOR.PURPLE:
                    color_value = PURPLE.color; break;
                case Block.COLOR.GREEN:
                    color_value = Color.green; break;
                case Block.COLOR.PEACH:
                    color_value = PEACH.color; break;
                case Block.COLOR.CRIMSON:
                    //color_value = Color.magenta; break;
                    color_value = CRIMSON.color; break;
                case Block.COLOR.INDIGO:
                    color_value = INDIGO.color; break;
                case Block.COLOR.REDPURPLE:
                    color_value = REDPURPLE.color; break;
                case Block.COLOR.LIGHTGREEN:
                    color_value = LIGHTGREEN.color; break;
                case Block.COLOR.TURQUOISE:
                    color_value = TURQUOISE.color; break;
              
            }
            this.GetComponent<Renderer>().material.color = color_value; // �� ���� ������Ʈ�� ������ �����Ѵ�.

        }

    }

    //������Ʈ ��� ����-------------------------------------
    public void setPrefab(Block.PREFAB prefab)
    {
        this.prefab = prefab; // �̹��� ������ �������� ��� ������ �����Ѵ�.
        Mesh prefab_mesh;
        switch (this.prefab)
        {
            default:
            case Block.PREFAB.BLOCK:
                prefab_mesh = block_mesh;
                break;
            case Block.PREFAB.MOVEBLOCK:
                prefab_mesh = moveblock_mesh;
                break;
            case Block.PREFAB.FILLBLOCK:
                prefab_mesh = fillblock_mesh;
                break;
                
        }
        this.GetComponent<MeshFilter>().mesh = prefab_mesh;
    }
    //-------------------------------------------------------



    //���콺�� ��� �Ⱦ�-----------------------------------------------------------------------------
    public void beginGrab()
    { // ������ �� ȣ���Ѵ�.
        this.next_step = Block.STEP.GRABBED;
    }
    public void endGrab()
    { // ������ �� ȣ���Ѵ�.
        this.next_step = Block.STEP.IDLE;
    }
    public bool isGrabbable()
    { // ���� �� �ִ� ���� ���� �Ǵ��Ѵ�.
        bool is_grabbable = false;
        switch (this.step)
        {
            case Block.STEP.IDLE: // '���' ������ ����.
                is_grabbable = true; // true(���� �� �ִ�)�� ��ȯ�Ѵ�.
                break;
        }
        return (is_grabbable);
    }
    public bool isContainedPosition(Vector2 position)
    { // ������ ���콺 ��ǥ�� �ڽŰ� ��ġ���� ��ȯ�Ѵ�.
        bool ret = false;
        Vector3 center = this.transform.position;
        float h = Block.COLLISION_SIZE / 2.0f;
        do
        {
            if (position.x < center.x - h || center.x + h < position.x) { break; } // X ��ǥ�� �ڽŰ� ��ġ�� ������ ������ ���� ������.
            if (position.y < center.y - h || center.y + h < position.y) { break; } // Y ��ǥ�� �ڽŰ� ��ġ�� ������ ������ ���� ������.
            ret = true; // X ��ǥ, Y ��ǥ ��� ���� ������ true(���� �ִ�)�� ��ȯ�Ѵ�.
        } while (false);
        return (ret);
    }
    //-----------------------------------------------------------------------------------------------


}
