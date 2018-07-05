namespace KBEngine
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class Avatar : AvatarBase
    {
        public static SkillBox skillbox = new SkillBox();
        public Dictionary<UInt64, ITEM_INFO> itemDict = new Dictionary<UInt64, ITEM_INFO>();
        public Dictionary<UInt64, ITEM_INFO> equipItemDict = new Dictionary<UInt64, ITEM_INFO>();

        private UInt64[] itemIndex2Uids = new UInt64[12];
        private UInt64[] equipIndex2Uids = new UInt64[4];

        public Avatar()
            : base()
        {
        }

        // 由于任何玩家被同步到该客户端都会使用这个模块创建，因此客户端可能存在很多这样的实体
        // 但只有一个是自己的玩家实体，所以需要判断一下
        public override void __init__()
        {
            if (isPlayer())
            {
                Event.registerIn("relive", this, "relive");
                Event.registerIn("updatePlayer", this, "updatePlayer");
                Event.registerIn("sendChatMessage", this, "sendChatMessage");
            }	
        }

        public override void onDestroy()
        {
            if (isPlayer())
            {
                KBEngine.Event.deregisterIn(this);
            }
        }

        public virtual void updatePlayer(float x, float y, float z, float yaw)
        {
            position.x = x;
            position.y = y;
            position.z = z;

            direction.z = yaw;
        }
        public override void onEnterWorld()
        {
            base.onEnterWorld();

            if (isPlayer())
            {
                Event.fireOut("onAvatarEnterWorld", new object[] { KBEngineApp.app.entity_uuid, id, this });
                SkillBox.inst.pull();
            }
        }
        public void sendChatMessage(string msg)
        {
            baseCall("sendChatMessage", name + ": " + msg);
        }
        public override void ReceiveChatMessage(string msg)
        {
            Event.fireOut("ReceiveChatMessage", msg);
        }
        public void relive(Byte type)
        {
            cellCall("relive", type);
        }

        //技能
        public int useTargetSkill(Int32 skillID, Int32 targetID)
        {
            Skill skill = SkillBox.inst.get(skillID);
            if (skill == null)
                return 4;

            SCEntityObject scobject = new SCEntityObject(targetID);
            int errorCode = skill.validCast(this, scobject);
            if (errorCode == 0)
            {
                skill.use(this, scobject);
                return errorCode;
            }

            //Dbg.DEBUG_MSG(className + "::useTargetSkill: skillID=" + skillID + ", targetID=" + targetID + ". is failed!");
            return errorCode;
        }

        public override void onAddSkill(Int32 skillID)
        {
            Dbg.DEBUG_MSG(className + "::onAddSkill(" + skillID + ")");
            //Event.fireOut("onAddSkill", new object[] { this });

            Skill skill = new Skill();
            skill.id = skillID;
            skill.name = skillID + " ";
            switch (skillID)
            {
                case 1:
                    skill.displayType = Skill_DisplayType.SkillDisplay_Event_Bullet;
                    skill.canUseDistMax = 30f;
                    skill.skillEffect = "Ice1";
                    skill.name = "魔法球";
                    break;
                case 2:
                    skill.displayType = Skill_DisplayType.SkillDisplay_Event_Bullet;
                    skill.canUseDistMax = 30f;
                    skill.skillEffect = "Fire1";
                    skill.name = "火球";
                    break;
                case 3:
                    skill.displayType = Skill_DisplayType.SkillDisplay_Event_Bullet;
                    skill.canUseDistMax = 20f;
                    skill.skillEffect = "Medical1";
                    skill.name = "治疗";
                    break;
                case 4:
                    skill.displayType = Skill_DisplayType.SkillDisplay_Event_Effect;
                    skill.canUseDistMax = 5f;
                    skill.skillEffect = "MagicEffect1";
                    skill.name = "斩击";
                    break;
                case 5:
                    skill.displayType = Skill_DisplayType.SkillDisplay_Event_Effect;
                    skill.canUseDistMax = 5f;
                    skill.skillEffect = "LightningEffect1";
                    skill.name = "挥击";
                    break;
                case 6:
                    skill.displayType = Skill_DisplayType.SkillDisplay_Event_Effect;
                    skill.canUseDistMax = 5f;
                    skill.skillEffect = "BloodEffect1";
                    skill.name = "吸血";
                    break;
                case 6000101:
                    skill.canUseDistMax = 20f;
                    skill.skillEffect = "skill3";
                    break;
                default:
                    break;
            };

            SkillBox.inst.add(skill);

            if (SkillBox.inst.skills.Count == 3)
            {
                Event.fireOut("setSkillButton");
            }
        }

        public override void onRemoveSkill(Int32 skillID)
        {
            Dbg.DEBUG_MSG(className + "::onRemoveSkill(" + skillID + ")");
            Event.fireOut("onRemoveSkill", new object[] { this });
            SkillBox.inst.remove(skillID);
        }

        
        public void reqItemList()
        {
            baseCall("reqItemList");
        }
        public void dropRequest(UInt64 itemUUID)
        {
            baseCall("dropRequest", itemUUID);
        }
        public void swapItemRequest(Int32 srcIndex, Int32 dstIndex)
        {
            UInt64 srcUid = itemIndex2Uids[srcIndex];
            UInt64 dstUid = itemIndex2Uids[dstIndex];
		
		    itemIndex2Uids[srcIndex] = dstUid;
		    if (dstUid != 0)
			    itemDict[dstUid].itemIndex = srcIndex;
		    itemIndex2Uids[dstIndex] = srcUid;
            if (srcUid != 0)
                itemDict[srcUid].itemIndex = dstIndex;

            baseCall("swapItemRequest", new object[] { srcIndex, dstIndex });
        }
        public void equipItemRequest(Int32 itemIndex, Int32 equipIndex)
        {
            baseCall("equipItemRequest", new object[] { itemIndex, equipIndex });
        }
        public void useItemRequest(Int32 itemIndex)
        {
            if (!ConsumeLimitCD.instance.isWaiting())
            {
                baseCall("useItemRequest", new object[] { itemIndex });
                ConsumeLimitCD.instance.Start(2);
            }
            else
            {
                UI_ErrorHint._instance.errorShow("物品使用冷却中");
            }
        }

        //OWN_CLIENT
        public override void onHPChanged(Int32 oldValue)
        {
            // Dbg.DEBUG_MSG(className + "::set_HP: " + old + " => " + v); 
            Event.fireOut("set_HP", new object[] { this, HP, HP_Max });
        }

        public override void onMPChanged(Int32 oldValue)
        {
            // Dbg.DEBUG_MSG(className + "::set_MP: " + old + " => " + v); 
            Event.fireOut("set_MP", new object[] { this, MP, MP_Max });
        }

        public override void onHP_MaxChanged(Int32 oldValue)
        {
            // Dbg.DEBUG_MSG(className + "::set_HP_Max: " + old + " => " + v); 
            Event.fireOut("set_HP_Max", new object[] { this, HP_Max, HP });
        }

        public override void onMP_MaxChanged(Int32 oldValue)
        {
            // Dbg.DEBUG_MSG(className + "::set_MP_Max: " + old + " => " + v); 
            Event.fireOut("set_MP_Max", new object[] { this, MP_Max, MP });
        }

        public override void onStateChanged(SByte oldValue)
        {
            Event.fireOut("set_state", new object[] { this, state });
        }


        public override void onAttack_MaxChanged(Int32 old)
        { 
            Event.fireOut("set_attack_Max", new object[] { attack_Max });
        }

        public override void onAttack_MinChanged(Int32 old)
        {
            Event.fireOut("set_attack_Min", new object[] { attack_Min });
        }
        public override void onDefenceChanged(Int32 old)
        {
            Event.fireOut("set_defence", new object[] { defence });
        }
        public override void onRatingChanged(Int32 old)
        {
            Event.fireOut("set_rating", new object[] { rating });
        }
        public override void onDodgeChanged(Int32 old)
        {
            Event.fireOut("set_dodge", new object[] { dodge });
        }
        public override void onStrengthChanged(Int32 old)
        {
            Event.fireOut("set_strength", new object[] { strength });
        }
        public override void onDexterityChanged(Int32 old)
        {
            Event.fireOut("set_dexterity", new object[] { dexterity });
        }
        public override void onExpChanged(UInt64 old)
        {
            Event.fireOut("set_exp", new object[] { exp });
        }
        public override void onLevelChanged(UInt16 old)
        {
            Event.fireOut("set_level", new object[] { level });
        }
        public override void onStaminaChanged(Int32 old)
        {
            Event.fireOut("set_stamina", new object[] { stamina });
        }

        public override void onEquipWeaponChanged(Int32 old)
        {
            Event.fireOut("set_equipWeapon", new object[] { this, equipWeapon });
        }


        //dialog
        public void dialog(Int32 targetID, UInt32 dialogID)
        {
            cellCall("dialog", new object[] { targetID, dialogID });
        }

        //-----------------------response-------------------------
        public override void dropItem_re(Int32 itemId, UInt64 itemUUId)
        {
            Int32 itemIndex = itemDict[itemUUId].itemIndex;
            itemDict.Remove(itemUUId);
            itemIndex2Uids[itemIndex] = 0;
            Event.fireOut("dropItem_re", new object[] { itemIndex });
        }
        public override void pickUp_re(ITEM_INFO itemInfo)
        {
            Event.fireOut("pickUp_re", new object[] { itemInfo });
            itemDict[itemInfo.UUID] = itemInfo;
        }
        public override void equipItemRequest_re(ITEM_INFO itemInfo, ITEM_INFO equipItemInfo)
        {
            Event.fireOut("equipItemRequest_re", new object[] { itemInfo, equipItemInfo });
            UInt64 itemUUid = itemInfo.UUID;
            UInt64 equipItemUUid = equipItemInfo.UUID;
            if (itemUUid == 0 && equipItemUUid != 0)//带上装备
            {
                equipItemDict[equipItemUUid] = equipItemInfo;
                itemDict.Remove(equipItemUUid);
            }
            else if (itemUUid != 0 && equipItemUUid != 0)//替换装备
            {
                itemDict.Remove(equipItemUUid);
                equipItemDict[equipItemUUid] = equipItemInfo;
                equipItemDict.Remove(itemUUid);
                itemDict[itemUUid] = itemInfo;
            }
            else if (itemUUid != 0 && equipItemUUid == 0)//脱下装备
            {
                equipItemDict.Remove(itemUUid);
                itemDict[itemUUid] = itemInfo;
            }
        }
        public override void onReqItemList(ITEM_INFO_LIST infos, ITEM_INFO_LIST equipInfos)
        {
            itemDict.Clear();
            List<ITEM_INFO> listinfos = infos.values;
            for (int i = 0; i < listinfos.Count; i++)
            {
                ITEM_INFO info = listinfos[i];
                itemDict.Add(info.UUID, info);
                itemIndex2Uids[info.itemIndex] = info.UUID;
            }
            equipItemDict.Clear();
            List<ITEM_INFO> elistinfos = equipInfos.values;
            for (int i = 0; i < elistinfos.Count; i++)
            {
                ITEM_INFO info = elistinfos[i];
                equipItemDict.Add(info.UUID, info);
                equipIndex2Uids[info.itemIndex] = info.UUID;
            }
            // ui event
            Event.fireOut("onReqItemList", new object[] { itemDict, equipItemDict });
        }
        public override void errorInfo(Int32 errorCode)
        {
            Dbg.DEBUG_MSG("errorInfo(" + errorCode + ")");
        }

        

        //dialog
        public override void dialog_setContent(Int32 talkerId, List<UInt32> dialogs, List<string> dialogsTitles, string title, string body, string sayname)
        {
            Event.fireOut("dialog_setContent", new object[] { talkerId, dialogs, dialogsTitles, title, body, sayname });
        }
        public override void dialog_close()
        {
            Event.fireOut("dialog_close");
        }

        public override void recvDamage(Int32 attackerID, Int32 skillID, Int32 damageType, Int32 damage)
        {
            //Dbg.DEBUG_MSG(className + "::recvDamage: attackerID=" + attackerID + ", skillID=" + skillID + ", damageType=" + damageType + ", damage=" + damage);
            Entity entity = KBEngineApp.app.findEntity(attackerID);
            Event.fireOut("recvDamage", new object[] { this, entity, skillID, damageType, damage });
        }
    }
}
