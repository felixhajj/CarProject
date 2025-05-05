using UnityEngine;
using Cinemachine;

public class CinemachineCameras : MonoBehaviour
{

	public float rotatespeed = 5f;
	[SerializeField] private CinemachineVirtualCamera thirdpersonrear;
	[SerializeField] private CinemachineVirtualCamera thirdpersonfront;
	[SerializeField] private CinemachineVirtualCamera worldupcamera;
	[SerializeField] private CinemachineVirtualCamera firstperson;

	[SerializeField] private CinemachineBrain Brain;
	[SerializeField] private GameObject worldup;

	private bool worldupcurr;
	private PowerToWheels power;
	private CinemachineVirtualCamera currcamera;

	private int currview;

	private void Start()
	{
		currcamera = thirdpersonrear;
		worldupcurr = false;
		SwitchToCamera(currcamera, thirdpersonrear);
		currview = 0;

		power = GetComponent<PowerToWheels>();

	}



	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.V))
		{
			currview++;
			if(currview%3==0)
			{
				SwitchToCamera(currcamera,thirdpersonrear);
				currview = 0;
			}
			else if(currview%3==1)
			{
				Brain.m_WorldUpOverride = worldup.transform;
				SwitchToCamera(currcamera, worldupcamera);
				worldupcurr = true;
			}
			else if(currview%3==2)
			{
				Brain.m_WorldUpOverride = null;
				SwitchToCamera(currcamera, firstperson);
				worldupcurr = false;
			}
		}

		if (currview==0)
		{
			if (power.gasinput == -1)
			{
				SwitchToCamera(currcamera, thirdpersonfront);
			}
			else
			{
				if (currcamera != thirdpersonrear)
				{
					SwitchToCamera(currcamera, thirdpersonrear);
					Debug.Log(power.gasinput);
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
