namespace KBEngine
{
    using UnityEngine;
    using System.Collections;
    using System;
    using System.Collections.Generic;

    public class Account : AccountBase
    {
        public Dictionary<UInt64, AVATAR_INFO> avatars = new Dictionary<UInt64, AVATAR_INFO>();

        public Account(): base()
        {
        }
        public override void __init__()
        {
            Event.fireOut("onLoginSuccessfully", new object[] { KBEngineApp.app.entity_uuid, id, this });
            baseCall("reqAvatarList");
        }
        public void reqCreateAvatar(string name, byte roleType)
        {
            baseCall("reqCreateAvatar", new object[]{name, roleType});
        }

        public void reqRemoveAvatar(string name)
        {
            Dbg.DEBUG_MSG("Account::reqRemoveAvatar: name=" + name);
            baseCall("reqRemoveAvatar", name);
        }

        public override void onReqAvatarList(AVATAR_INFO_LIST infos)
        {
            avatars.Clear();

            Dbg.DEBUG_MSG("Account::onReqAvatarList: avatarsize=" + infos.values.Count);
            for (int i = 0; i < infos.values.Count; i++)
            {
                AVATAR_INFO info = infos.values[i];
                Dbg.DEBUG_MSG("Account::onReqAvatarList: name" + i + "=" + info.name);
                avatars.Add(info.dbid, info);
            }

            // ui event
            Dictionary<UInt64, AVATAR_INFO> avatarList = new Dictionary<ulong, AVATAR_INFO>(avatars);
            Event.fireOut("onReqAvatarList", new object[] { avatarList });

            // selectAvatarGame(avatars.Keys.ToList()[0]);
        }
        public override void onCreateAvatarResult(byte retcode, AVATAR_INFO info)
        {
            if (retcode == 0)
            {
                avatars.Add(info.dbid, info);
                Dbg.DEBUG_MSG("Account::onCreateAvatarResult: name=" + info.name);
            }
            else
            {
                Dbg.ERROR_MSG("Account::onCreateAvatarResult: retcode=" + retcode);
                if (retcode == 3)
                {
                    Dbg.ERROR_MSG("角色数量不能超过三个！");
                }
            }

            // ui event
            
            Event.fireOut("onCreateAvatarResult", new object[] { retcode, info, avatars });
        }
        public override void onRemoveAvatar(UInt64 dbid)
        {
            Dbg.DEBUG_MSG("Account::onRemoveAvatar: dbid=" + dbid);

            avatars.Remove(dbid);

            // ui event
            Event.fireOut("onRemoveAvatar", new object[] { dbid, avatars });
        }

        public void selectAvatarGame(UInt64 dbid)
        {
            Dbg.DEBUG_MSG("Account::selectAvatarGame: dbid=" + dbid);
            baseCall("selectAvatarGame", dbid);
        }
    }
}