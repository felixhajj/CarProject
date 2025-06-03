using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingManager : MonoBehaviour
{
	private CarObjects carobjects;

	int active = 0;
	bool isHoldingL = false;
	float timeLPressed = 0f;
	public Rigidbody car;

	
	public float minSpeed = 40f;
	public float fadeDuration = 1f;

	private float startTrailTime; // initial value of trail time
	private float elapsedTime;
	private bool abovemin = false;



	

	private void Start()
	{
		carobjects = GetComponent<CarObjects>();

		startTrailTime = carobjects.backtrail.time;
		elapsedTime = 0f;

		
		
	}

	void Update()
	{
		if (car.velocity.magnitude > minSpeed)
		{
            carobjects.trailleft.SetActive(true);
            carobjects.trailright.SetActive(true);
			abovemin = true;
		}
		else
		{
			if (abovemin)
			{
				FadeOut();
			}
		}


		if (Input.GetKeyDown(KeyCode.L))
		{
			timeLPressed = Time.time;
			isHoldingL = true;
		}

		if (Input.GetKeyUp(KeyCode.L))
		{
			float timeLReleased = Time.time;
			float pressDuration = timeLReleased - timeLPressed;

			if (active == 0 && pressDuration < 0.5f) // none is turned on and clicked
			{
                carobjects.frontleftlow.SetActive(true);
                carobjects.frontrightlow.SetActive(true);
				active = 1;
			}
			else if (active == 0 && pressDuration >= 0.5f) // none is turned on and held
			{
                carobjects.frontleftlow.SetActive(true);
                carobjects.frontrightlow.SetActive(true);
                carobjects.highbeamleft.SetActive(true);
                carobjects.highbeamright.SetActive(true);
				active = 2;
			}
			else if (active == 1 && pressDuration < 0.5f) // low beam is on and clicked
			{
                carobjects.frontleftlow.SetActive(false);
                carobjects.frontrightlow.SetActive(false);
				active = 0;
			}
			else if (active == 1 && pressDuration >= 0.5f) // low beam is on and held
			{
                carobjects.highbeamleft.SetActive(true);
                carobjects.highbeamright.SetActive(true);
				active = 2;
			}
			else if (active == 2 && pressDuration < 0.5f) // high beam is on and clicked
			{
                carobjects.frontleftlow.SetActive(false);
                carobjects.frontrightlow.SetActive(false);
                carobjects.highbeamleft.SetActive(false);
                carobjects.highbeamright.SetActive(false);
				active = 0;
			}
			else if (active == 2 && pressDuration >= 0.5f) // high beam is on and held
			{
                carobjects.highbeamleft.SetActive(false);
                carobjects.highbeamright.SetActive(false);
                carobjects.frontleftlow.SetActive(true);
                carobjects.frontrightlow.SetActive(true);
				active = 1;
			}

			isHoldingL = false;
		}

		if (isHoldingL)
		{
			if (active == 2) // both high and low are on
			{
				if (Input.GetKeyUp(KeyCode.L))
				{
                    carobjects.highbeamleft.SetActive(false);
                    carobjects.highbeamright.SetActive(false);
					active = 1;
				}
			}
			else if (active == 1) // low beams are on
			{
				if (Input.GetKeyUp(KeyCode.L))
				{
                    carobjects.frontleftlow.SetActive(true);
                    carobjects.frontrightlow.SetActive(true);
                    carobjects.highbeamleft.SetActive(true);
                    carobjects.highbeamright.SetActive(true);
					active = 2;
				}
			}
		}
	}
    private void FadeOut()
    {
        if (carobjects.trailLeftRenderer != null && carobjects.trailLeftRenderer.time > 0f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;
            float targetTrailTime = Mathf.Lerp(startTrailTime, 0f, t);

            carobjects.trailLeftRenderer.time = targetTrailTime;
            carobjects.trailRightRenderer.time = targetTrailTime;

            if (elapsedTime >= fadeDuration)
            {
                carobjects.trailLeftRenderer.time = startTrailTime;
                carobjects.trailRightRenderer.time = startTrailTime;

                carobjects.trailleft.SetActive(false);
                carobjects.trailright.SetActive(false);
                abovemin = false;
                elapsedTime = 0f;
            }
        }
    }

}
