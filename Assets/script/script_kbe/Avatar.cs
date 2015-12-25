namespace KBEngine
{
    using UnityEngine;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class Avatar : KBEngine.GameObject
    {
        public Combat combat = null;
        public static SkillBox skillbox = new SkillBox();

        public override void __init__()
        {
            combat = new Combat(this);
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
            object name = getDefinedPropterty("name");
            baseCall("sendChatMessage", (string)name + ": " + msg);
        }
        public void ReceiveChatMessage(string msg)
        {
            Event.fireOut("ReceiveChatMessage", msg);
        }
        public void relive(Byte type)
        {
            cellCall("relive", type);
        }

        //技能
        public bool useTargetSkill(Int32 skillID, Int32 targetID)
        {
            Skill skill = SkillBox.inst.get(skillID);
            if (skill == null)
                return false;

            SCEntityObject scobject = new SCEntityObject(targetID);
            if (skill.validCast(this, scobject))
            {
                skill.use(this, scobject);
                return true;
            }

            //Dbg.DEBUG_MSG(className + "::useTargetSkill: skillID=" + skillID + ", targetID=" + targetID + ". is failed!");
            return false;
        }

        public virtual void onAddSkill(Int32 skillID)
        {
            Dbg.DEBUG_MSG(className + "::onAddSkill(" + skillID + ")");
            //Event.fireOut("onAddSkill", new object[] { this });

            Skill skill = new Skill();
            skill.id = skillID;
            skill.name = skillID + " ";
            switch (skillID)
            {
                case 1:
                    skill.canUseDistMax = 50f;
                    break;
                case 1000101:
                    skill.canUseDistMax = 20f;
                    break;
                case 2000101:
                    skill.canUseDistMax = 20f;
                    break;
                case 3000101:
                    skill.canUseDistMax = 20f;
                    break;
                case 4000101:
                    skill.canUseDistMax = 20f;
                    break;
                case 5000101:
                    skill.canUseDistMax = 20f;
                    break;
                case 6000101:
                    skill.canUseDistMax = 20f;
                    break;
                default:
                    break;
            };

            SkillBox.inst.add(skill);
        }

        public virtual void onRemoveSkill(Int32 skillID)
        {
            Dbg.DEBUG_MSG(className + "::onRemoveSkill(" + skillID + ")");
            Event.fireOut("onRemoveSkill", new object[] { this });
            SkillBox.inst.remove(skillID);
        }
    }
}
