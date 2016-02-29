using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI_Target : MonoBehaviour {
    public bool bShowDetail = false;
    public Slider slider_hp;
    public Text text_targetName;
    public GameObject GO_targetUI;
    public GameEntity GE_target;

    public Text text_hpDetail;

	// Use this for initialization
	void Start () {
	}
    public void setHPMax(int v)
    {
        slider_hp.maxValue = v;
        if (bShowDetail)
            text_hpDetail.text = slider_hp.value + "/" + slider_hp.maxValue;
    }
    public void setHP(int v)
    {
        slider_hp.value = v;
        if (bShowDetail)
            text_hpDetail.text = slider_hp.value + "/" + slider_hp.maxValue;
    }
    public void setName(string v)
    {
        text_targetName.text = v;
    }
    public void deactivate()
    {
        GO_targetUI.SetActive(false);
    }

    public void activate()
    {
        GO_targetUI.SetActive(true);
    }

    public void UpdateTargetUI()
    {
        activate();
        setHP(GE_target.hp);
        setHPMax(GE_target.hpMax);
        setName(GE_target.entity_name);
    }
}
