using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingManager : MonoBehaviour
{
	int active = 0;
	bool isHoldingL = false;
	float timeLPressed = 0f;
	public Rigidbody car;
	public GameObject frontleftlow;
	public GameObject frontrightlow;
	public GameObject highbeamleft;
	public GameObject highbeamright;

	public GameObject trailleft;
	public GameObject trailright;

	public TrailRenderer backtrail;
	public float minSpeed = 40f;
	public float fadeDuration = 1f;
	//float trailtime;
	//float temptime;
	private float startTrailTime; // initial value of trail time
	private float elapsedTime;
	private bool abovemin = false;



	

	private void Start()
	{
		startTrailTime = backtrail.time;
		elapsedTime = 0f;

		
		
	}

	void Update()
	{
		if (car.velocity.magnitude > minSpeed)
		{
			trailleft.SetActive(true);
			trailright.SetActive(true);
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
				frontleftlow.SetActive(true);
				frontrightlow.SetActive(true);
				active = 1;
			}
			else if (active == 0 && pressDuration >= 0.5f) // none is turned on and held
			{
				frontleftlow.SetActive(true);
				frontrightlow.SetActive(true);
				highbeamleft.SetActive(true);
				highbeamright.SetActive(true);
				active = 2;
			}
			else if (active == 1 && pressDuration < 0.5f) // low beam is on and clicked
			{
				frontleftlow.SetActive(false);
				frontrightlow.SetActive(false);
				active = 0;
			}
			else if (active == 1 && pressDuration >= 0.5f) // low beam is on and held
			{
				highbeamleft.SetActive(true);
				highbeamright.SetActive(true);
				active = 2;
			}
			else if (active == 2 && pressDuration < 0.5f) // high beam is on and clicked
			{
				frontleftlow.SetActive(false);
				frontrightlow.SetActive(false);
				highbeamleft.SetActive(false);
				highbeamright.SetActive(false);
				active = 0;
			}
			else if (active == 2 && pressDuration >= 0.5f) // high beam is on and held
			{
				highbeamleft.SetActive(false);
				highbeamright.SetActive(false);
				frontleftlow.SetActive(true);
				frontrightlow.SetActive(true);
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
					highbeamleft.SetActive(false);
					highbeamright.SetActive(false);
					active = 1;
				}
			}
			else if (active == 1) // low beams are on
			{
				if (Input.GetKeyUp(KeyCode.L))
				{
					frontleftlow.SetActive(true);
					frontrightlow.SetActive(true);
					highbeamleft.SetActive(true);
					highbeamright.SetActive(true);
					active = 2;
				}
			}
		}
	}
	private void FadeOut()
	{

		if (backtrail != null && backtrail.time > 0f)
		{

			elapsedTime += Time.deltaTime;
			float t = elapsedTime / fadeDuration;
			float targetTrailTime = Mathf.Lerp(startTrailTime, 0f, t);
			backtrail.time = targetTrailTime;

			if (elapsedTime >= fadeDuration)
			{
				backtrail.time = startTrailTime;
				trailleft.SetActive(false);
				trailright.SetActive(false);
				abovemin = false;
				elapsedTime = 0f;

			}
		}
	}
}
