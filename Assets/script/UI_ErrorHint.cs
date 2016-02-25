using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_ErrorHint : MonoBehaviour {
    public static UI_ErrorHint _instance;
	// Use this for initialization
    private Text text_content;
    private Animator ani_control;
	void Awake () {
        _instance = this;

        text_content = this.gameObject.GetComponent<Text>();
        ani_control = this.gameObject.GetComponent<Animator>();
	}
	public void errorShow(string str)
    {
        text_content.text = str;
        ani_control.SetTrigger("Error");
    }
}
