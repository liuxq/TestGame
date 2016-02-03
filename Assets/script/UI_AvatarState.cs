using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class UI_AvatarState : MonoBehaviour
{
    public Text text_attack;
    public Text text_defence;
    public Text text_rating;
    public Text text_dodge;

    private int attack_max;
    private int attack_min;

    void Start()
    {
        KBEngine.Event.registerOut("set_attack_Max", this, "setAttackMax");
        KBEngine.Event.registerOut("set_attack_Min", this, "setAttackMin");
        KBEngine.Event.registerOut("set_defence", this, "setDefence");
        KBEngine.Event.registerOut("set_rating", this, "setRating");
        KBEngine.Event.registerOut("set_dodge", this, "setDodge");
    }

    public void closeInventory()
    {
        this.gameObject.SetActive(false);
    }

    public void openInventory()
    {
        this.gameObject.SetActive(true);
    }

    public void setAttackMax(int v)
    {
        attack_max = v;
        text_attack.text = attack_min + " - " + attack_max;
    }
    public void setAttackMin(int v)
    {
        attack_min = v;
        text_attack.text = attack_min + " - " + attack_max;
    }
    public void setDefence(int v)
    {
        text_defence.text = v + "";
    }
    public void setRating(int v)
    {
        text_rating.text = v + "";
    }
    public void setDodge(int v)
    {
        text_dodge.text = v + "";
    }
}

