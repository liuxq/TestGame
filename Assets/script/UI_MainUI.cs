using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI_MainUI : MonoBehaviour {
    public static UI_MainUI inst;
    public Transform but_skill1;
    public Transform but_skill2;
    public Transform but_skill3;

    private Text skill1Text;
    private Text skill2Text;
    private Text skill3Text;
	// Use this for initialization
	void Start () {
        inst = this;

        skill1Text = but_skill1.GetChild(0).GetComponent<Text>();
        skill2Text = but_skill2.GetChild(0).GetComponent<Text>();
        skill3Text = but_skill3.GetChild(0).GetComponent<Text>();

        but_skill1.gameObject.SetActive(false);
        but_skill2.gameObject.SetActive(false);
        but_skill3.gameObject.SetActive(false);
	}
    public void setSkill1(string str)
    {
        skill1Text.text = str;
        but_skill1.gameObject.SetActive(true);
    }
    public void setSkill2(string str)
    {
        skill2Text.text = str;
        but_skill2.gameObject.SetActive(true);
    }
    public void setSkill3(string str)
    {
        skill3Text.text = str;
        but_skill3.gameObject.SetActive(true);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
