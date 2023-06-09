﻿using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Weapon : MonoBehaviour
{
	public GameObject ammoPrefab;
	public int poolSize;
	public float weaponVelocity;
	[HideInInspector]
	public Animator animator;
	static List<GameObject> ammoPool;
	bool isFiring;
	Camera localCamera;
	float positiveSlope;
	float negativeSlope;

	enum Quadrant
	{
		East,
		South,
		West,
		North
	}

	void Start()
	{
		animator = GetComponent<Animator>();
		isFiring = false;
		localCamera = Camera.main;

		// Cztery obiekty reprezentujace punkty w rogach ekranu
		Vector2 lowerLeft = localCamera.ScreenToWorldPoint(new Vector2(0, 0));
		Vector2 upperRight = localCamera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
		Vector2 upperLeft = localCamera.ScreenToWorldPoint(new Vector2(0, Screen.height));
		Vector2 lowerRight = localCamera.ScreenToWorldPoint(new Vector2(Screen.width, 0));

		// Wspolczynniki kierunkowe dwoch prostych:
		// pierwszej, przechodzacej przez lewy dolny i prawy gorny rog
		// drugi, analogicznie na odwrot
		positiveSlope = GetSlope(lowerLeft, upperRight);
		negativeSlope = GetSlope(upperLeft, lowerRight);
	}

	void Awake()
	{
		if (ammoPool == null)
		{
			ammoPool = new List<GameObject>();
		}

		for (int i = 0; i < poolSize; i++)
		{
			GameObject ammoObject = Instantiate(ammoPrefab);
			ammoObject.SetActive(false);
			ammoPool.Add(ammoObject);
		}

	}

	void Update()
	{
		//float shootHorizontal = Input.GetAxis("ShootHorizontal");
		//float shootVertical = Input.GetAxis("ShootVertical");
		if (Input.GetMouseButtonDown(0))
		{
			isFiring = true;
			FireAmmo();
		}

		UpdateState();
	}

	private void UpdateState()
	{
		if (isFiring)
		{
			Vector2 quadrantVector;
			Quadrant quadEnum = GetQuadrant();
			switch (quadEnum)
			{
				case Quadrant.East:
					quadrantVector = new Vector2(1.0f, 0.0f);
					break;
				case Quadrant.South:
					quadrantVector = new Vector2(0.0f, -1.0f);
					break;
				case Quadrant.West:
					quadrantVector = new Vector2(-1.0f, 0.0f);
					break;
				case Quadrant.North:
					quadrantVector = new Vector2(0.0f, 1.0f);
					break;
				default:
					quadrantVector = new Vector2(0.0f, 0.0f);
					break;
			}

			animator.SetBool("isFiring", true);
			animator.SetFloat("fireXDir", quadrantVector.x);
			animator.SetFloat("fireYDir", quadrantVector.y);

			isFiring = false;
		}
		else
		{
			animator.SetBool("isFiring", false);
		}
	}

	GameObject SpawnAmmo(Vector3 location)
	{
		foreach (GameObject ammo in ammoPool)
		{
			if (ammo.activeSelf == false)
			{
				ammo.SetActive(true);
				ammo.transform.position = location;

				return ammo;
			}
		}

		return null;
	}

	void FireAmmo()
	{
		Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		GameObject ammo = SpawnAmmo(transform.position);
		if (ammo != null)
		{
			Arc arcScript = ammo.GetComponent<Arc>();
			float travelDuration = 1.0f / weaponVelocity;

			StartCoroutine(arcScript.TravelArc(mousePosition, travelDuration));
		}
	}

	void OnDestroy()
	{
		ammoPool = null;
	}

	float GetSlope(Vector2 pointOne, Vector2 pointTwo)
	{
		return (pointTwo.y - pointOne.y) / (pointTwo.x - pointOne.x);
	}

	bool HigherThanPositiveSlopeLine(Vector2 inputPoint)
	{
		Vector2 playerPosition = gameObject.transform.position;
		Vector2 mousePosition = localCamera.ScreenToWorldPoint(inputPoint);
		// Rownanie b=y-ax w celu porownania punktow przecieca prostych z osia Y
		float yIntercept = playerPosition.y - (positiveSlope * playerPosition.x);
		float inputIntercept = mousePosition.y - (positiveSlope * mousePosition.x);

		return inputIntercept > yIntercept;
	}

	bool HigherThanNegativeSlopeLine(Vector2 inputPoint)
	{
		Vector2 playerPosition = gameObject.transform.position;
		Vector2 mousePosition = localCamera.ScreenToWorldPoint(inputPoint);
		// Rownanie b=y-ax w celu porownania punktow przecieca prostych z osia Y
		float yIntercept = playerPosition.y - (negativeSlope * playerPosition.x);
		float inputIntercept = mousePosition.y - (negativeSlope * mousePosition.x);

		return inputIntercept > yIntercept;
	}

	// Metoda zwraca wartość typu Quadrant opisujaca kwadrant klikniety przez gracza
	Quadrant GetQuadrant()
	{
		bool higherThanPositiveSlopeLine = HigherThanPositiveSlopeLine(Input.mousePosition);
		bool higherThanNegativeSlopeLine = HigherThanNegativeSlopeLine(Input.mousePosition);
		if (!higherThanPositiveSlopeLine && higherThanNegativeSlopeLine)
		{
			return Quadrant.East;
		}
		else if (!higherThanPositiveSlopeLine && !higherThanNegativeSlopeLine)
		{
			return Quadrant.South;
		}
		else if (higherThanPositiveSlopeLine && !higherThanNegativeSlopeLine)
		{
			return Quadrant.West;
		}
		else 
		{
			return Quadrant.North;
		}
	}
}
