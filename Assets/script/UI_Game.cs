using UnityEngine;
using System.Collections;
using KBEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class UI_Game : MonoBehaviour
{
    public InputField input_content;
    public Transform tran_text;
    public Scrollbar sb_vertical;
    public Text text_pos;
    public Transform tran_relive;
    public UnityEngine.GameObject text_error;

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
            text_pos.text = avatar.position.x.ToString("#.0") + "," + avatar.position.z.ToString("#.0");
            
            foreach (Skill sk in SkillBox.inst.skills)
            {
                sk.updateTimer(Time.deltaTime);//更新技能的冷却时间
            }
            
            
        }
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100.0f, 1 << LayerMask.NameToLayer("CanAttack")))
            {
                UI_Target ui_target = World.instance.getUITarget();
                ui_target.GE_target = hit.collider.GetComponent<GameEntity>();
                ui_target.UpdateTargetUI();

                string name = Utility.getPreString(ui_target.GE_target.name);
                if (name == "NPC" && !MenuBox.hasMenu())
                {
                    Int32 id = Utility.getPostInt(ui_target.GE_target.name);
                    NPC _npc = (NPC)KBEngineApp.app.findEntity(id);

                    if (_npc != null)
                    {
                        UInt32 dialogID = (UInt32)_npc.getDefinedProperty("dialogID");
                        avatar.dialog(id, dialogID);
                    }
                }
            }
        }
        //更新消耗品计时器
        ConsumeLimitCD.instance.Update(Time.deltaTime);
        
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
    public void OnTabSelected()
    {
        UI_Target ui_target = World.instance.getUITarget();
        GameEntity ge = null;
        if (ui_target != null && ui_target.GE_target != null)
            ge = ui_target.GE_target;

        KBEngine.Entity entity = KBEngineApp.app.player();
        KBEngine.Avatar avatar = null;
        if (entity != null && entity.className == "Avatar")
            avatar = (KBEngine.Avatar)entity;
        if (avatar == null)
            return;

        UnityEngine.GameObject[] objs = UnityEngine.GameObject.FindObjectsOfType<UnityEngine.GameObject>();
        float mindis = 10000;
        UnityEngine.GameObject minObj = null;
        foreach (UnityEngine.GameObject obj in objs)
        {
            if (obj.layer != LayerMask.NameToLayer("CanAttack") || obj.GetComponent<GameEntity>() == null || obj.GetComponent<GameEntity>().hp == 0)
                continue;
            float dis = Vector3.Distance(avatar.position, obj.transform.position);
            if (mindis > dis && (ge == null || ge != null && ge != obj.GetComponent<GameEntity>()))
            {
                mindis = dis;
                minObj = obj;
            }
        }
        if (minObj != null)
        {
            ui_target.GE_target = minObj.GetComponent<GameEntity>();
            ui_target.UpdateTargetUI();
        }
    }
    public void AttackSkill(int skillId)
    { 
        KBEngine.Entity entity = KBEngineApp.app.player();
        KBEngine.Avatar avatar = null;
        if (entity != null && entity.className == "Avatar")
            avatar = (KBEngine.Avatar)entity;
        if (avatar == null)
            return;

        UI_Target ui_target = World.instance.getUITarget();
        if (ui_target != null && ui_target.GE_target != null)
        {
            string name = ui_target.GE_target.name;
            Int32 entityId = Utility.getPostInt(name);

            if (avatar != null)
            {
                int errorCode = avatar.useTargetSkill(skillId, entityId);
                if (errorCode == 1)
                {
                    UI_ErrorHint._instance.errorShow("目标太远");
                    //逼近目标
                    UnityEngine.GameObject renderEntity = (UnityEngine.GameObject)entity.renderObj;
                    renderEntity.GetComponent<MoveController>().moveTo(ui_target.GE_target.transform, SkillBox.inst.get(skillId).canUseDistMax-1, skillId);
                }
                if (errorCode == 2)
                {
                    UI_ErrorHint._instance.errorShow("技能冷却");
                }
                if (errorCode == 3)
                {
                    UI_ErrorHint._instance.errorShow("目标已死亡");
                }
            }
        }
        else 
        {
            if (skillId == 3)//治疗自己
            {
                if (avatar != null)
                {
                    int errorCode = avatar.useTargetSkill(skillId, avatar.id);
                    if (errorCode == 2)
                    {
                        UI_ErrorHint._instance.errorShow("技能冷却");
                    }
                    if (errorCode == 3)
                    {
                        UI_ErrorHint._instance.errorShow("目标已死亡");
                    }
                }
            }
            else
                UI_ErrorHint._instance.errorShow("未选择目标");
        }
     
    }
    public void OnAttackSkill1()
    {
        AttackSkill(SkillBox.inst.skills[0].id);
    }
    public void OnAttackSkill2()
    {
        AttackSkill(SkillBox.inst.skills[1].id);
    }
    public void OnAttackSkill3()
    {
        AttackSkill(SkillBox.inst.skills[2].id);
    }
}
