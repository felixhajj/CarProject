using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class CameraBehavior : MonoBehaviour
{
	public GameObject Player;
	public float speed = 1;
	public float minDistance = 20f;

	
	private void Awake()
	{
		Player = GameObject.FindGameObjectWithTag("Player");
	}

	private void FixedUpdate()
	{
		follow();
	}

	private void follow()
	{
		
		if (Vector3.Distance(transform.position, Player.transform.position) > minDistance)
		{
			gameObject.transform.position = Vector3.Lerp(transform.position, Player.transform.position + new Vector3(0,4,0), Time.deltaTime * speed);
		}
		gameObject.transform.LookAt(Player.gameObject.transform.position);
	}
}
