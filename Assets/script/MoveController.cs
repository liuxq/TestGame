using UnityEngine;
using System.Collections;

public class MoveController : MonoBehaviour {

    private Animator animator;
    private SmoothFollow sf;

    private Transform moveDes;
    private bool hasDes = false;
    private float minLen = 0f;
    private int skillId = 1;
    private UnityEngine.GameObject UIGame;
    void Start()
    {
        animator = GetComponent<Animator>();
        sf = Camera.main.GetComponent<SmoothFollow>();
        hasDes = false;

        UIGame = UnityEngine.GameObject.FindGameObjectWithTag("UIGame");
    }

    void OnEnable()
    {
        EasyJoystick.On_JoystickMove += OnJoystickMove;
        EasyJoystick.On_JoystickMoveEnd += OnJoystickMoveEnd;
    }

    void OnDisable()
    {
        EasyJoystick.On_JoystickMove -= OnJoystickMove;
        EasyJoystick.On_JoystickMoveEnd -= OnJoystickMoveEnd;
    }

    void OnDestroy()
    {
        EasyJoystick.On_JoystickMove -= OnJoystickMove;
        EasyJoystick.On_JoystickMoveEnd -= OnJoystickMoveEnd;
    }


    void OnJoystickMoveEnd(MovingJoystick move)
    {
        if (move.joystickName == "MoveJoystick")
        {
            animator.speed = 1.0f;
            animator.SetFloat("Speed", 0.0f);
            //GetComponent<Animation>().CrossFade("idle");
        }
        else if (move.joystickName == "RotJoystick")
        {
            
        }
    }
    void OnJoystickMove(MovingJoystick move)
    {
        hasDes = false;
        float joyPositionX = move.joystickAxis.x;
        float joyPositionY = move.joystickAxis.y;

        if (move.joystickName == "MoveJoystick")
        {
            if (joyPositionY != 0 || joyPositionX != 0)
            {
                //设置角色的朝向（朝向当前坐标+摇杆偏移量）
                Quaternion r = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);

                Vector3 dir = r * new Vector3(joyPositionX, 0, joyPositionY);
                transform.LookAt(new Vector3(transform.position.x + dir.x, transform.position.y, transform.position.z + dir.z));
                
                //移动玩家的位置（按朝向位置移动）
                transform.Translate(Vector3.forward * Time.deltaTime * 5);

                //移动摄像机
                sf.FollowUpdate();
                //播放奔跑动画
                animator.speed = 2.0f;
                animator.SetFloat("Speed", 1.0f);
            }
        }
        //else if (move.joystickName == "RotJoystick")
        //{
        //    if (joyPositionY != 0 || joyPositionX != 0)
        //    {
        //        sf.rotate += joyPositionX*2;
        //        if (sf.rotate < -180.0f)
        //            sf.rotate += 360.0f;
        //        if (sf.rotate > 180.0f)
        //            sf.rotate -= 360.0f;
        //        sf.height -= joyPositionY;
        //        if (sf.height < .5f)
        //            sf.height = .5f;
        //        if (sf.height > 15.0f)
        //            sf.height = 15.0f;
               
        //    }
        //}
    }
    void Update()
    {
        if (hasDes)
        {
            if (Vector3.Distance(transform.position, moveDes.position) < minLen)
            {
                hasDes = false;
                animator.speed = 1.0f;
                animator.SetFloat("Speed", 0.0f);
                UIGame.GetComponent<UI_Game>().AttackSkill(skillId);
            }
            else
            {
                transform.LookAt(moveDes);
                transform.Translate(Vector3.forward * Time.deltaTime * 5);
                //移动摄像机
                sf.FollowUpdate();
                //播放奔跑动画
                animator.speed = 2.0f;
                animator.SetFloat("Speed", 1.0f);
            }
        }
    }
    //移动，直到操作或者到达指定距离，然后释放技能
    public void moveTo(Transform des, float minLen, int skillId)
    {
        hasDes = true;
        moveDes = des;
        this.minLen = minLen;
        this.skillId = skillId;
    }
}
