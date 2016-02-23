using UnityEngine;
using System.Collections;
using KBEngine;
using UnityEngine.UI;
using System;

public class UI_Game : MonoBehaviour {
    public InputField input_content;
    public Transform tran_text;
    public Scrollbar sb_vertical;
    public Text text_pos;
    public Transform tran_relive;
    public Text text_error;

    private Text text_content;

    public UnityEngine.GameObject inventory;
    public UnityEngine.GameObject characterSystem;
    public UnityEngine.GameObject statePanel;
    public UnityEngine.GameObject craftSystem;
    private Inventory craftSystemInventory;
    private Inventory mainInventory;
    private Inventory characterSystemInventory;
    private UI_AvatarState avatarState;
    private Tooltip toolTip;

    //event delegates for consuming, gearing
    public delegate void PickDelegate();
    public static event PickDelegate ItemPick;

	// Use this for initialization
	void Start () {
        text_content = tran_text.GetComponent<Text>();
      
        KBEngine.Event.registerOut("ReceiveChatMessage", this, "ReceiveChatMessage");

        //inventory
        UnityEngine.GameObject canvas = UnityEngine.GameObject.FindGameObjectWithTag("Canvas");
        if (canvas.transform.Find("Panel - Inventory(Clone)") != null)
            inventory = canvas.transform.Find("Panel - Inventory(Clone)").gameObject;
        if (canvas.transform.Find("Panel - EquipmentSystem(Clone)") != null)
            characterSystem = canvas.transform.Find("Panel - EquipmentSystem(Clone)").gameObject;
        if (canvas.transform.Find("Panel - State") != null)
            statePanel = canvas.transform.Find("Panel - State").gameObject;

        if (UnityEngine.GameObject.FindGameObjectWithTag("Tooltip") != null)
            toolTip = UnityEngine.GameObject.FindGameObjectWithTag("Tooltip").GetComponent<Tooltip>();
        if (inventory != null)
            mainInventory = inventory.GetComponent<Inventory>();
        if (characterSystem != null)
            characterSystemInventory = characterSystem.GetComponent<Inventory>();
        if (craftSystem != null)
            craftSystemInventory = craftSystem.GetComponent<Inventory>();
        if (statePanel != null)
            avatarState = statePanel.GetComponent<UI_AvatarState>();
	}
	
	// Update is called once per frame
	void Update () {
        KBEngine.Entity entity = KBEngineApp.app.player();
        KBEngine.Avatar avatar = null;
        if (entity != null && entity.className == "Avatar")
            avatar = (KBEngine.Avatar)entity;
        
        if (avatar != null)
        {
            text_pos.text = "位置：" + avatar.position.x + "," + avatar.position.z;
            Skill sk = SkillBox.inst.get(1);
            if( sk != null )
            {
                sk.updateTimer(Time.deltaTime);//更新一号技能的冷却时间
            }
        }
        //选中对象
        if(Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100.0f, 1 << LayerMask.NameToLayer("CanAttack")))
            {
                UnityEngine.GameObject target = UnityEngine.GameObject.FindGameObjectWithTag("Target");
                hit.collider.GetComponent<GameEntity>().UI_target = target;
                string name = hit.collider.GetComponent<GameEntity>().name;
                Int32 entityId = Utility.getPostInt(name);

                if (avatar != null)
                {
                    avatar.useTargetSkill(1, entityId);
                }
            }
        }
        
	}
    public void ReceiveChatMessage(string msg)
    {
        if (text_content.text.Length > 0)
        {
            text_content.text += "\n" + msg;
        }
        else
        {
            text_content.text += msg;
        }
        //;
        if (text_content.preferredHeight + 30 > 67)
            tran_text.GetComponent<RectTransform>().sizeDelta = new Vector2(0, text_content.preferredHeight);

        sb_vertical.value = 0;
        input_content.text = "";
       
    }
    public void OnSendMessage()
    {
        if (input_content.text.Length > 0)
            KBEngine.Event.fireIn("sendChatMessage", input_content.text);
    }
    public void OnRelive()
    {
        KBEngine.Event.fireIn("relive", (Byte)1);
    }
    public void OnCloseGame()
    {
        Application.Quit();
    }
    public void OnInventory()
    {
        if (!inventory.activeSelf)
        {
            mainInventory.openInventory();
        }
        else
        {
            if (toolTip != null)
                toolTip.deactivateTooltip();
            mainInventory.closeInventory();
        }
    }
    public void OnEquipInventory()
    {
        if (!characterSystem.activeSelf)
        {
            characterSystemInventory.openInventory();
        }
        else
        {
            if (toolTip != null)
                toolTip.deactivateTooltip();
            characterSystemInventory.closeInventory();
        }
    }
    public void OnState()
    {
        if (!statePanel.activeSelf)
        {
            avatarState.openInventory();
        }
        else
        {
            avatarState.closeInventory();
        }
    }
    public void OnPick()
    {
        if (ItemPick != null)
            ItemPick();
    }

    public void OnResetView()
    {
        Camera.main.GetComponent<SmoothFollow>().ResetView();
    }
    public UnityEngine.GameObject TabSelected()
    { 
        KBEngine.Entity entity = KBEngineApp.app.player();
        KBEngine.Avatar avatar = null;
        if (entity != null && entity.className == "Avatar")
            avatar = (KBEngine.Avatar)entity;
        if (avatar == null)
            return null;

        UnityEngine.GameObject[] objs = UnityEngine.GameObject.FindObjectsOfType<UnityEngine.GameObject>();
        float mindis = 10000;
        UnityEngine.GameObject minObj = null;
        foreach (UnityEngine.GameObject obj in objs)
        {
            if (obj.layer != LayerMask.NameToLayer("CanAttack") || obj.GetComponent<GameEntity>() == null)
                continue;
            float dis = Vector3.Distance(avatar.position, obj.transform.position);
            if (mindis > dis)
            {
                mindis = dis;
                minObj = obj;
            }
        }
        return minObj;

        
    }
    public void OnAttackSkill1()
    {
        KBEngine.Entity entity = KBEngineApp.app.player();
        KBEngine.Avatar avatar = null;
        if (entity != null && entity.className == "Avatar")
            avatar = (KBEngine.Avatar)entity;
        if (avatar == null)
            return;

        UnityEngine.GameObject selectedObj = TabSelected();
        if (selectedObj != null)
        {
            string name = selectedObj.GetComponent<GameEntity>().name;
            Int32 entityId = Utility.getPostInt(name);

            if (avatar != null)
            {
                avatar.useTargetSkill(1, entityId);
            }
        }
    }
}
