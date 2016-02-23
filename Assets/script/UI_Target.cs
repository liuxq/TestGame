using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI_Target : MonoBehaviour {

    public Image imget_hpContent;
    public Text text_targetName;
    public GameObject GO_targetUI;

    private Vector2 hpBar;
    private float width;
    private int hp;
    private int hpmax;
	// Use this for initialization
	void Start () {
        hpBar = new Vector2(imget_hpContent.rectTransform.sizeDelta.x, imget_hpContent.rectTransform.sizeDelta.y);
        width = imget_hpContent.rectTransform.sizeDelta.x;
	}
    public void setHPMax(int v)
    {
        hpmax = v;
        updateHP();
    }
    public void setHP(int v)
    {
        hp = v;
        updateHP();
    }
    private void updateHP()
    {
        hpBar.x = (float)hp / hpmax * width;
        imget_hpContent.rectTransform.sizeDelta = hpBar;
    }
    public void deactivate()
    {
        GO_targetUI.SetActive(false);
    }

    public void activate()
    {
        GO_targetUI.SetActive(true);
    }
}
