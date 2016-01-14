namespace KBEngine
{
    using UnityEngine;
    using System.Collections;
    using System;
    using System.Collections.Generic;

    public class Account : KBEngine.GameObject
    {
        public Dictionary<UInt64, Dictionary<string, object>> avatars = new Dictionary<UInt64, Dictionary<string, object>>();
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

        //public void selectAvatarGame(UInt64 dbid)
        //{
        //    Dbg.DEBUG_MSG("Account::selectAvatarGame: dbid=" + dbid);
        //    baseCall("selectAvatarGame", dbid);
        //}

        public void onReqAvatarList(Dictionary<string, object> infos)
        {
            avatars.Clear();

            List<object> listinfos = (List<object>)infos["values"];

            Dbg.DEBUG_MSG("Account::onReqAvatarList: avatarsize=" + listinfos.Count);
            for (int i = 0; i < listinfos.Count; i++)
            {
                Dictionary<string, object> info = (Dictionary<string, object>)listinfos[i];
                Dbg.DEBUG_MSG("Account::onReqAvatarList: name" + i + "=" + (string)info["name"]);
                avatars.Add((UInt64)info["dbid"], info);
            }

            // ui event
            Dictionary<UInt64, Dictionary<string, object>> avatarList = new Dictionary<ulong, Dictionary<string, object>>(avatars);
            Event.fireOut("onReqAvatarList", new object[] { avatarList });

            // selectAvatarGame(avatars.Keys.ToList()[0]);
        }
        public void onCreateAvatarResult(byte retcode, object info)
        {
            if (retcode == 0)
            {
                avatars.Add((UInt64)((Dictionary<string, object>)info)["dbid"], (Dictionary<string, object>)info);
                Dbg.DEBUG_MSG("Account::onCreateAvatarResult: name=" + (string)((Dictionary<string, object>)info)["name"]);
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
        public void onRemoveAvatar(UInt64 dbid)
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