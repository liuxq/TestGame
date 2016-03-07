using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using KBEngine;

public class DialogProcess : MonoBehaviour
{
    void Start()
    {
        KBEngine.Event.registerOut("dialog_close", this, "dialog_close");
        KBEngine.Event.registerOut("dialog_setContent", this, "dialog_setContent");
    }

    public void dialog_setContent(Int32 talkerId, List<object> dialogIds, List<object> dialogsTitles, string title, string body, string sayname)
    {
        KBEngine.Entity entity = KBEngineApp.app.player();
        KBEngine.Avatar avatar = null;
        if (entity != null && entity.className == "Avatar")
            avatar = (KBEngine.Avatar)entity;

        List<string> titles = new List<string>();
        List<UnityAction> actions = new List<UnityAction>();
        for (int i = 0; i < dialogsTitles.Count; i++ )
        {
            titles.Add((string)dialogsTitles[i]);
            UInt32 dialogId = (UInt32)dialogIds[i];
            actions.Add(() => avatar.dialog(talkerId, dialogId));
        }
        if (titles.Count > 0)
        {
            MenuBox.Show
            (
                titles,
                actions,
                sayname + ": " + body
            );
        }
    }

    public void dialog_close()
    {
        //MenuBox.Destroy();
    }
}