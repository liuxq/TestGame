// ----------------------------------------------------------------------------------
//
//
// ----------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

public class MyDrawFpsText : MonoBehaviour 
{
	// -------------------------------------------------------------------------------------------
	public  float updateInterval = 0.5F;
	 
	private float accum   = 0;
	private int   frames  = 0;
	private float timeleft;

    private Text fpsText;
	 
	// -------------------------------------------------------------------------------------------
	void Start()
	{
		timeleft = updateInterval;
        fpsText = GetComponent<Text>();
	}

	void Update()
	{
		timeleft -= Time.deltaTime;
		accum += Time.timeScale/Time.deltaTime;
		++frames;
	    
		// Interval ended - update GUI text and start new interval
		if (timeleft <= 0.0)
		{
			// display two fractional digits (f2 format)
			float fps = accum/frames;
			string format = System.String.Format("{0:F2} FPS",fps);
            fpsText.text = format;

            //if (fps < 30)
            //    fpsText.material.color = Color.yellow;
            //else {
            //    if (fps < 10)
            //         fpsText.material.color = Color.red;
            //    else fpsText.material.color = Color.green;
            //}
			timeleft = updateInterval;
			accum = 0.0F;
			frames = 0;
		}
	}
}
