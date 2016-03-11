using KBEngine;
using UnityEngine;
using System.Collections;
using System.Linq;
using System;
using System.Collections.Generic;

public class World : MonoBehaviour {
    //单例模式
    public static World instance;

    private UnityEngine.GameObject terrain = null;
    public UnityEngine.GameObject terrainPerfab;

    private UnityEngine.GameObject player = null;
    //private UnityEngine.GameObject snowBalls = new Dictionary;
    public UnityEngine.GameObject otherPlayerPerfab;
    public UnityEngine.GameObject gatePerfab;
    public UnityEngine.GameObject avatarPerfab;
    public UnityEngine.GameObject snowBallPerfab;
    public UnityEngine.GameObject droppedItemPerfab;

    private bool isFirstPos = true;

    private UI_Target ui_target = null;
    private UI_Target ui_targetPlayer = null;
    
    static World()
    {
        UnityEngine.GameObject go = new UnityEngine.GameObject("World");
        DontDestroyOnLoad(go);
        instance = go.AddComponent<World>();
        instance.terrainPerfab = (UnityEngine.GameObject)Resources.Load("Terrain");
        instance.otherPlayerPerfab = (UnityEngine.GameObject)Resources.Load("entity");
        instance.gatePerfab = (UnityEngine.GameObject)Resources.Load("Gate");
        instance.avatarPerfab = (UnityEngine.GameObject)Resources.Load("player");
        instance.snowBallPerfab = (UnityEngine.GameObject)Resources.Load("snowBall");
        instance.droppedItemPerfab = (UnityEngine.GameObject)Resources.Load("droppedItem");

        
    }
    //激活单例
    public void init()
    {
    }
	// Use this for initialization
	void Start () {

        UnityEngine.GameObject target = UnityEngine.GameObject.FindGameObjectWithTag("Target");
        if (target)
        {
            ui_target = target.GetComponent<UI_Target>();
        }

        KBEngine.Event.registerOut("addSpaceGeometryMapping", this, "addSpaceGeometryMapping");
        KBEngine.Event.registerOut("onAvatarEnterWorld", this, "onAvatarEnterWorld");
        KBEngine.Event.registerOut("onEnterWorld", this, "onEnterWorld");
        KBEngine.Event.registerOut("onLeaveWorld", this, "onLeaveWorld");
        KBEngine.Event.registerOut("set_position", this, "set_position");
        KBEngine.Event.registerOut("set_direction", this, "set_direction");
        KBEngine.Event.registerOut("updatePosition", this, "updatePosition");
        KBEngine.Event.registerOut("set_name", this, "set_entityName");
        KBEngine.Event.registerOut("set_state", this, "set_state");
        KBEngine.Event.registerOut("set_HP", this, "set_HP");
        KBEngine.Event.registerOut("set_HP_Max", this, "set_HP_Max");
        KBEngine.Event.registerOut("recvDamage", this, "recvDamage");
        KBEngine.Event.registerOut("onReqItemList", this, "onReqItemList");
        KBEngine.Event.registerOut("set_equipWeapon", this, "set_equipWeapon");
        KBEngine.Event.registerOut("setSkillButton", this, "setSkillButton");
	}
    void OnDestroy()
    {
        KBEngine.Event.deregisterOut(this);
    }
	// Update is called once per frame
	void Update () {
        createPlayer();
	}

    public void onAvatarEnterWorld(UInt64 rndUUID, Int32 eid, KBEngine.Avatar avatar)
    {
        if (!avatar.isPlayer())
        {
            return;
        }
        createPlayer();
        //UI.inst.info("loading scene...(加载场景中...)");
        Debug.Log("loading scene...");

        //object speed = avatar.getDefinedProperty("moveSpeed");
        //if (speed != null)
        //    set_moveSpeed(avatar, speed);

        object state = avatar.getDefinedProperty("state");
        if (state != null)
            set_state(avatar, state);

        //object modelScale = avatar.getDefinedProperty("modelScale");
        //if (modelScale != null)
        //    set_modelScale(avatar, modelScale);

        object name = avatar.getDefinedProperty("name");
        if (name != null)
            set_entityName(avatar, (string)name);

        object hp = avatar.getDefinedProperty("HP");
        if (hp != null)
            set_HP(avatar, hp);

        object hpMax = avatar.getDefinedProperty("HP_Max");
        if (hpMax != null)
            set_HP_Max(avatar, hpMax);

        //设置头像属性
        UnityEngine.GameObject ptarget = UnityEngine.GameObject.FindGameObjectWithTag("TargetPlayer");
        
        if (ptarget)
        {
            ui_targetPlayer = ptarget.GetComponent<UI_Target>();
        }
        if (ui_targetPlayer)
        {
            ui_targetPlayer.GE_target = player.GetComponent<GameEntity>();
            ui_targetPlayer.UpdateTargetUI();
        }
        //end
        UnityEngine.GameObject canvas = UnityEngine.GameObject.FindGameObjectWithTag("Canvas");
        UnityEngine.GameObject panel_state = null;
        if (canvas.transform.Find("Panel - State") != null)
            panel_state = canvas.transform.Find("Panel - State").gameObject;
        if (panel_state != null)
        {
            UI_AvatarState avaterState = panel_state.GetComponent<UI_AvatarState>();
            object attack_max = avatar.getDefinedProperty("attack_Max");
            if (attack_max != null)
                avaterState.setAttackMax((Int32)attack_max);

            object attack_min = avatar.getDefinedProperty("attack_Min");
            if (attack_min != null)
                avaterState.setAttackMin((Int32)attack_min);

            object defence = avatar.getDefinedProperty("defence");
            if (defence != null)
                avaterState.setDefence((Int32)defence);

            object rating = avatar.getDefinedProperty("rating");
            if (rating != null)
                avaterState.setRating((Int32)rating);

            object dodge = avatar.getDefinedProperty("dodge");
            if (dodge != null)
                avaterState.setDodge((Int32)dodge);

            object strength = avatar.getDefinedProperty("strength");
            if (strength != null)
                avaterState.setStrength((Int32)strength);

            object dexterity = avatar.getDefinedProperty("dexterity");
            if (dexterity != null)
                avaterState.setDexterity((Int32)dexterity);

            object stamina = avatar.getDefinedProperty("stamina");
            if (stamina != null)
                avaterState.setStamina((Int32)stamina);

            object exp = avatar.getDefinedProperty("exp");
            if (exp != null)
                avaterState.setExp((UInt64)exp);

            object level = avatar.getDefinedProperty("level");
            if (level != null)
                avaterState.setLevel((UInt16)level);
        }

        object equipWeapon = avatar.getDefinedProperty("equipWeapon");
        if (equipWeapon != null)
            set_equipWeapon(avatar, (Int32)equipWeapon);

        SkillBox.inst.initSkillDisplay();

    }

    public void onEnterWorld(KBEngine.Entity entity)
    {
        if (entity.isPlayer())
            return;

        float y = entity.position.y;
        if (entity.isOnGround)
            y = 0.0f;

        if (entity.className == "Gate")
        {
            entity.renderObj = Instantiate(gatePerfab, new Vector3(entity.position.x, y, entity.position.z),
            Quaternion.Euler(new Vector3(entity.direction.y, entity.direction.z, entity.direction.x))) as UnityEngine.GameObject;
            ((UnityEngine.GameObject)(entity.renderObj)).GetComponent<GameEntity>().entityDisable();
        }
        else if (entity.className == "Monster")
        {
            entity.renderObj = Instantiate(otherPlayerPerfab, new Vector3(entity.position.x, y, entity.position.z),
            Quaternion.Euler(new Vector3(entity.direction.y, entity.direction.z, entity.direction.x))) as UnityEngine.GameObject;

        }
        else if (entity.className == "DroppedItem")
        {
            entity.renderObj = Instantiate(droppedItemPerfab, new Vector3(entity.position.x, y, entity.position.z),
            Quaternion.Euler(new Vector3(entity.direction.y, entity.direction.z, entity.direction.x))) as UnityEngine.GameObject;
        }
        else if (entity.className == "Avatar")
        {
            entity.renderObj = Instantiate(otherPlayerPerfab, new Vector3(entity.position.x, y, entity.position.z),
            Quaternion.Euler(new Vector3(entity.direction.y, entity.direction.z, entity.direction.x))) as UnityEngine.GameObject;
        }
        else if (entity.className == "NPC")
        {
            entity.renderObj = Instantiate(otherPlayerPerfab, new Vector3(entity.position.x, y, entity.position.z),
            Quaternion.Euler(new Vector3(entity.direction.y, entity.direction.z, entity.direction.x))) as UnityEngine.GameObject;
        }

        ((UnityEngine.GameObject)entity.renderObj).name = entity.className + "_" + entity.id;

        set_position(entity);
        set_direction(entity);

        //object speed = entity.getDefinedProperty("moveSpeed");
        //if (speed != null)
        //    set_moveSpeed(entity, speed);

        object state = entity.getDefinedProperty("state");
        if (state != null)
            set_state(entity, state);

        //object modelScale = entity.getDefinedProperty("modelScale");
        //if (modelScale != null)
        //    set_modelScale(entity, modelScale);

        object name = entity.getDefinedProperty("name");
        if (name != null)
            set_entityName(entity, (string)name);

        object hp = entity.getDefinedProperty("HP");
        if (hp != null)
            set_HP(entity, hp);

        object hpMax = entity.getDefinedProperty("HP_Max");
        if (hpMax != null)
            set_HP_Max(entity, hpMax);

        object equipWeapon = entity.getDefinedProperty("equipWeapon");
        if (equipWeapon != null)
            set_equipWeapon(entity, (Int32)equipWeapon);

    }

    public void onLeaveWorld(KBEngine.Entity entity)
    {
        if (entity.renderObj == null)
            return;

        //选择窗口消失
        GameEntity ge = ((UnityEngine.GameObject)entity.renderObj).GetComponent<GameEntity>();
        if (getUITarget().GE_target == ge)
            getUITarget().deactivate();

        UnityEngine.GameObject.Destroy((UnityEngine.GameObject)entity.renderObj);
        entity.renderObj = null;
    }
    public void set_entityName(KBEngine.Entity entity, object v)
    {
        if (entity.renderObj != null)
        {
            ((UnityEngine.GameObject)entity.renderObj).GetComponent<GameEntity>().entity_name = (string)v;
        }
    }
    public void set_position(KBEngine.Entity entity)
    {
        if (entity.renderObj == null)
            return;

        Vector3 v = (Vector3)entity.getDefinedProperty("position");
        ((UnityEngine.GameObject)entity.renderObj).GetComponent<GameEntity>().destPosition = v;
        ((UnityEngine.GameObject)entity.renderObj).GetComponent<GameEntity>().position = v;

        if (entity.isPlayer())//第一次主角获取位置时，调整摄像头
        {
            Camera.main.GetComponent<SmoothFollow>().FollowUpdate();
            if (isFirstPos)
            {
                Camera.main.GetComponent<SmoothFollow>().ResetView();
                isFirstPos = false;
            }
        }
    }
    public void set_direction(KBEngine.Entity entity)
    {
        if (entity.renderObj == null)
            return;

        ((UnityEngine.GameObject)entity.renderObj).GetComponent<GameEntity>().destDirection =
            new Vector3(entity.direction.y, entity.direction.z, entity.direction.x);
    }
    public UI_Target getUITarget()
    {
        if (ui_target == null)
        {
            UnityEngine.GameObject target = UnityEngine.GameObject.FindGameObjectWithTag("Target");
            if (target)
            {
                ui_target = target.GetComponent<UI_Target>();
            }
        }
        
        return ui_target;
        
    }
    public void set_HP(KBEngine.Entity entity, object v)
    {
        if (entity.renderObj != null)
        {
            GameEntity ge = ((UnityEngine.GameObject)entity.renderObj).GetComponent<GameEntity>();
            ge.hp = (Int32)v;
            ge.hpMax = (Int32)entity.getDefinedProperty("HP_Max");

            if (getUITarget() && getUITarget().GE_target == ge)
                getUITarget().UpdateTargetUI();
            else if (ui_targetPlayer && ui_targetPlayer.GE_target == ge)
                ui_targetPlayer.UpdateTargetUI();
            
        }
    }
    public void set_HP_Max(KBEngine.Entity entity, object v)
    {
        if (entity.renderObj != null)
        {
            GameEntity ge = ((UnityEngine.GameObject)entity.renderObj).GetComponent<GameEntity>();
            ge.hpMax = (Int32)v;
            ge.hp = (Int32)entity.getDefinedProperty("HP");

            if (getUITarget() && getUITarget().GE_target == ge)
                getUITarget().UpdateTargetUI();
            else if (ui_targetPlayer && ui_targetPlayer.GE_target == ge)
                ui_targetPlayer.UpdateTargetUI();
        }
    }
    
    public void set_state(KBEngine.Entity entity, object v)
    {
        if (entity.renderObj != null)
        {
            if (((SByte)v) == 1)//死亡
            {
                ((UnityEngine.GameObject)(entity.renderObj)).GetComponent<Animator>().Play("Dead");
            }
            else
            {
                ((UnityEngine.GameObject)(entity.renderObj)).GetComponent<Animator>().Play("Idle");
            }
        }

        if (entity.isPlayer())
        {
            Debug.Log("player->set_state: " + v);

            UnityEngine.GameObject UIGame = UnityEngine.GameObject.FindGameObjectWithTag("UIGame");


            if (((SByte)v) == 1)
            {
                UIGame.GetComponent<UI_Game>().tran_relive.gameObject.SetActive(true);

            }
            else
                UIGame.GetComponent<UI_Game>().tran_relive.gameObject.SetActive(false);

            return;
        }
    }
    public void createPlayer()
    {
        if (player != null)
            return;

        if (KBEngineApp.app.entity_type != "Avatar")
        {
            return;
        }

        KBEngine.Avatar avatar = (KBEngine.Avatar)KBEngineApp.app.player();
        if (avatar == null)
        {
            Debug.Log("wait create(palyer)!");
            return;
        }

        float y = avatar.position.y;
        if (avatar.isOnGround)
            y = 1.3f;

        player = Instantiate(avatarPerfab, new Vector3(avatar.position.x, y, avatar.position.z),
                             Quaternion.Euler(new Vector3(avatar.direction.y, avatar.direction.z, avatar.direction.x))) as UnityEngine.GameObject;
        
        initPlayer(player);
        //player.GetComponent<GameEntity>().entityDisable();
        avatar.renderObj = player;
        Camera.main.GetComponent<SmoothFollow>().target = player.transform;
        
        
        Camera.allCameras[1].GetComponent<MapFollow>().target = player.transform;
        ((UnityEngine.GameObject)avatar.renderObj).GetComponent<GameEntity>().isPlayer = true;
    }
    private void initPlayer(UnityEngine.GameObject player)
    {
        if (player == null)
            return;

        KBEngine.Avatar avatar = (KBEngine.Avatar)KBEngineApp.app.player();
        if (avatar == null)
        {
            Debug.Log("wait create(palyer)!");
            return;
        }

        //初始化物品栏
        avatar.reqItemList();
    }
    public void addSpaceGeometryMapping(string respath)
    {
        Debug.Log("loading scene(" + respath + ")...");
        print("scene(" + respath + "), spaceID=" + KBEngineApp.app.spaceID);
        if (terrain == null)
            terrain = Instantiate(terrainPerfab) as UnityEngine.GameObject;

        //player.GetComponent<GameEntity>().entityEnable();
    }

    public void updatePosition(KBEngine.Entity entity)
    {
        if (entity.renderObj == null)
            return;

        GameEntity gameEntity = ((UnityEngine.GameObject)entity.renderObj).GetComponent<GameEntity>();
        gameEntity.destPosition = entity.position;
        gameEntity.isOnGround = entity.isOnGround;
    }

    public void recvDamage(KBEngine.Entity entity, KBEngine.Entity attacker, Int32 skillID, Int32 damageType, Int32 damage)
    {
        Skill sk = SkillBox.inst.get(skillID);
        if (sk != null)
        {
            Vector3 dir = entity.position - attacker.position;

            UnityEngine.GameObject renderEntity = (UnityEngine.GameObject)attacker.renderObj;
            renderEntity.GetComponent<Animator>().Play("Punch");
            //显示技能效果
            sk.displaySkill(attacker, entity);

            if (attacker.isPlayer())
            {
                renderEntity.transform.LookAt(new Vector3(renderEntity.transform.position.x + dir.x, renderEntity.transform.position.y, renderEntity.transform.position.z + dir.z));
            }
        }
    }

    public void onReqItemList(Dictionary<UInt64, Dictionary<string, object>> itemList, Dictionary<UInt64, Dictionary<string, object>> equipItemDict)
    {
        UnityEngine.GameObject _player = UnityEngine.GameObject.FindGameObjectWithTag("Player");
        Inventory _inventory = null;
        Inventory _equipInventory = null;
        if (_player != null)
        {
            _inventory = _player.GetComponent<PlayerInventory>().inventory.GetComponent<Inventory>();
            _equipInventory = _player.GetComponent<PlayerInventory>().characterSystem.GetComponent<Inventory>();
        }
        if (_inventory != null)
        {
            foreach (UInt64 dbid in itemList.Keys)
            {
                Dictionary<string, object> info = itemList[dbid];
                Int32 id = (Int32)info["itemId"];
                UInt64 uid = (UInt64)info["UUID"];
                Int32 index = (Int32)info["itemIndex"];
                UInt32 count = (UInt32)info["itemCount"];
                _inventory.addItemToInventory(id, uid, (int)count, index);
                _inventory.updateItemList();
                _inventory.stackableSettings();
            }
        }
        if (_equipInventory != null)
        {
            foreach (UInt64 dbid in equipItemDict.Keys)
            {
                Dictionary<string, object> info = equipItemDict[dbid];
                Int32 id = (Int32)info["itemId"];
                UInt64 uid = (UInt64)info["UUID"];
                Int32 index = (Int32)info["itemIndex"];
                _equipInventory.addItemToInventory(id, uid, 1, index);
                _equipInventory.updateItemList();
                _equipInventory.stackableSettings();
            }
        }
    }

    public void set_equipWeapon(KBEngine.Entity dst, Int32 itemId)
    {
        if (dst.renderObj == null)
            return;

        if (itemId == -1)
        {
            ((UnityEngine.GameObject)dst.renderObj).GetComponent<EquipWeapon>().clearWeapon();
        }
        else 
        {
            ((UnityEngine.GameObject)dst.renderObj).GetComponent<EquipWeapon>().equipWeapon(itemId);
        }
    }
    public void setSkillButton()
    {
        UI_MainUI.inst.setSkill1(SkillBox.inst.skills[0].name);
        UI_MainUI.inst.setSkill2(SkillBox.inst.skills[1].name);
        UI_MainUI.inst.setSkill3(SkillBox.inst.skills[2].name);
    }
}
