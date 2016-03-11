
using UnityEngine;
using UnityEngine.UI;
using KBEngine;
using System.Collections;
using System;
using System.Xml;
using System.Collections.Generic;

public class GameEntity : MonoBehaviour
{
    public bool isPlayer = false;
    public bool isTestOffLine = false;

    private Vector3 _position = Vector3.zero;
    private Vector3 _eulerAngles = Vector3.zero;
    private Vector3 _scale = Vector3.zero;

    public Vector3 destPosition = Vector3.zero;
    public Vector3 destDirection = Vector3.zero;

    private float _speed = 50f;
    private byte jumpState = 0;
    private float currY = 1.0f;

    private Camera playerCamera = null;

    public string entity_name;

    public Int32 hp = 0;
    public Int32 hpMax = 0;

    float npcHeight = 3.0f;

    public CharacterController characterController;

    public bool isOnGround = true;

    public bool entityEnabled = true;

    public Text headName;

    private Animator animator;
    private CharacterController controller;

    private float last_angleY;
    private Vector3 last_position;

    void Awake()
    {
    }

    void Start()
    {
        characterController = ((UnityEngine.GameObject)gameObject).GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }

    public Vector3 position
    {
        get
        {
            return _position;
        }

        set
        {
            _position = value;

            if (gameObject != null)
                gameObject.transform.position = _position;
        }
    }

    public Vector3 eulerAngles
    {
        get
        {
            return _eulerAngles;
        }

        set
        {
            _eulerAngles = value;

            if (gameObject != null)
            {
                gameObject.transform.eulerAngles = _eulerAngles;
            }
        }
    }

    public Quaternion rotation
    {
        get
        {
            return Quaternion.Euler(_eulerAngles);
        }

        set
        {
            eulerAngles = value.eulerAngles;
        }
    }

    public Vector3 scale
    {
        get
        {
            return _scale;
        }

        set
        {
            _scale = value;

            if (gameObject != null)
                gameObject.transform.localScale = _scale;
        }
    }

    public float speed
    {
        get
        {
            return _speed;
        }

        set
        {
            _speed = value;
        }
    }

    public void entityEnable()
    {
        entityEnabled = true;
    }

    public void entityDisable()
    {
        entityEnabled = false;
    }


    void FixedUpdate()
    {
        if (!entityEnabled)
            return;

        if (isPlayer == false && KBEngineApp.app != null || isTestOffLine)
            return;

        KBEngine.Event.fireIn("updatePlayer", gameObject.transform.position.x,
            gameObject.transform.position.y, gameObject.transform.position.z, gameObject.transform.rotation.eulerAngles.y);
        
    }

    void Update()
    {
        //绘制头顶文字
        if (headName != null)
            headName.text = entity_name;

        if (!entityEnabled)
        {
            position = destPosition;
            return;
        }

        float deltaSpeed = (speed * Time.deltaTime);

        if (isPlayer == true)
        {
            //characterController.stepOffset = deltaSpeed;

            if (isOnGround != characterController.isGrounded)
            {
                KBEngine.Entity player = KBEngineApp.app.player();
                player.isOnGround = characterController.isGrounded;
                isOnGround = characterController.isGrounded;
            }
            return;
        }

        //配角的动作
        if (Quaternion.Angle(rotation, Quaternion.Euler(destDirection)) > 1.0f || Vector3.Distance(destPosition, position) > 0.01f)
        {
            animator.speed = 2.0f;
            animator.SetFloat("Speed", 1.0f);
        }
        else
        {
            animator.speed = 1.0f;
            animator.SetFloat("Speed", 0.0f);
        }

        if (Vector3.Distance(eulerAngles, destDirection) > 0.0004f)
        {
            rotation = Quaternion.Slerp(rotation, Quaternion.Euler(destDirection), 8f * Time.deltaTime);
        }

        float dist = 0.0f;

        if (isOnGround)
        {
            dist = Vector3.Distance(new Vector3(destPosition.x, 0f, destPosition.z),
                new Vector3(position.x, 0f, position.z));
        }
        else
        {
            dist = Vector3.Distance(destPosition, position);
        }

        if (jumpState > 0)
        {
            if (jumpState == 1)
            {
                currY += 0.05f;

                if (currY > 2.0f)
                    jumpState = 2;
            }
            else
            {
                currY -= 0.05f;
                if (currY < 1.0f)
                {
                    jumpState = 0;
                    currY = 1.0f;
                }
            }

            Vector3 pos = position;
            pos.y = currY;
            position = pos;
        }

        if (dist > 0.01f)
        {
            Vector3 pos = position;

            Vector3 movement = destPosition - pos;
            movement.y = 0f;
            movement.Normalize();

            movement *= deltaSpeed;

            if (dist > deltaSpeed || movement.magnitude > deltaSpeed)
                pos += movement;
            else
                pos = destPosition;

            if (isOnGround)
                pos.y = currY;

            position = pos;
            //test 不用什么速度的
            position = destPosition;
        }
        else
        {
        }
    }

    public void OnJump()
    {
        Debug.Log("jumpState: " + jumpState);

        if (jumpState != 0)
            return;

        jumpState = 1;
    }
}

