using UnityEngine;
using Cinemachine;

public class CinemachineCameras : MonoBehaviour
{
	private CarObjects carobjects;
    private PowerToWheels powertowheels;

    public float rotatespeed = 5f;
	
	private bool worldupcurr;
	private CinemachineVirtualCamera currcamera;
	private int currview;


	private void Start()
	{
        carobjects = GetComponent<CarObjects>();
        powertowheels = GetComponent<PowerToWheels>();

        currcamera = carobjects.thirdpersonrear;
		worldupcurr = false;
		SwitchToCamera(currcamera, carobjects.thirdpersonrear);
		currview = 0;
	}



	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.V))
		{
			currview++;
			if(currview%3==0)
			{
				SwitchToCamera(currcamera, carobjects.thirdpersonrear);
				currview = 0;
			}
			else if(currview%3==1)
			{
                carobjects.Brain.m_WorldUpOverride = carobjects.worldup.transform;
				SwitchToCamera(currcamera, carobjects.worldupcamera);
				worldupcurr = true;
			}
			else if(currview%3==2)
			{
                carobjects.Brain.m_WorldUpOverride = null;
				SwitchToCamera(currcamera, carobjects.firstperson);
				worldupcurr = false;
			}
		}

		if (currview==0)
		{
			if (powertowheels.gasinput == -1)
			{
				SwitchToCamera(currcamera, carobjects.thirdpersonfront);
			}
			else
			{
				if (currcamera != carobjects.thirdpersonrear)
				{
					SwitchToCamera(currcamera, carobjects.thirdpersonrear);
					Debug.Log(powertowheels.gasinput);
				}
			}
		}
			
	}

	private void SwitchToCamera(CinemachineVirtualCamera current, CinemachineVirtualCamera cameraToSwitchTo)
	{
		cameraToSwitchTo.Priority = 10;
		current.Priority = 5;
		currcamera = cameraToSwitchTo;

	}
}
