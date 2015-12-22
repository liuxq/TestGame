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

    private Text text_content;
	// Use this for initialization
	void Start () {
        text_content = tran_text.GetComponent<Text>();
        

        KBEngine.Event.registerOut("ReceiveChatMessage", this, "ReceiveChatMessage");
	}
	
	// Update is called once per frame
	void Update () {
        KBEngine.Avatar avatar = (KBEngine.Avatar)KBEngineApp.app.player();
        if (avatar != null)
        {
            text_pos.text = "位置：" + avatar.position.x + "," + avatar.position.z;
            SkillBox.inst.get(1).updateTimer(Time.deltaTime);//更新一号技能的冷却时间
        }
        if(Input.GetMouseButton (0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100.0f, 1 << LayerMask.NameToLayer("CanAttack"))) 
            {
                string name = hit.collider.GetComponent<GameEntity>().name;
                Int32 entityId = Int32.Parse(name.Substring(name.IndexOf('_')+1));
                
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
}
