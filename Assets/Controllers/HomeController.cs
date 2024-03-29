﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeController : ScreenController
{
	public Text nameField, plantsField, leavesField;
	public RawImage profilePic;

	private User currentUser;
	private Texture2D photoTexture;

	public void Start ()
	{
		currentTab = "Home";
		photoTexture = UserService.user.profilePicture;

		TutorialService.CheckTutorial("Welcome");

		UpdateAndShowUser ();
	}

	public void UpdateAndShowUser()
	{
		FillInfoFields();

		if (PlantsService.plants != null)
			plantsField.text = PlantsService.plants.Length.ToString();
		else
			StartCoroutine(_RequestUserPlants());

		StartCoroutine(_UpdateUserInfo());
	}

	private void FillInfoFields ()
	{
		currentUser = UserService.user;
		string savedPlants = "CompGirls:Plants:" + currentUser._id;

		if (PlayerPrefs.HasKey(savedPlants))
			plantsField.text = PlayerPrefs.GetString(savedPlants);

		if (currentUser.name != null)
			nameField.text = currentUser.name;

		if (currentUser.points.ToString() != null)
			leavesField.text = currentUser.points.ToString();

		UserService.user.profilePicture = photoTexture;
		profilePic.texture = photoTexture;
	}

	private IEnumerator _RequestUserPlants ()
	{
		WWW plantsRequest = PlantsService.GetUserPlants(UserService.user._id);

		while (!plantsRequest.isDone)
			yield return new WaitForSeconds(0.1f);

		Debug.Log("Header: " + plantsRequest.responseHeaders["STATUS"]);
		Debug.Log("Text: " + plantsRequest.text);

		if (plantsRequest.responseHeaders["STATUS"] == HTML.HTTP_200)
		{
			PlantsService.UpdateLocalPlants(plantsRequest.text);
			SavePlantsCount(PlantsService.plants.Length);
		}

		yield return null;
	}

	private IEnumerator _UpdateUserInfo ()
	{
		WWW userRequest = UserService.GetUser(currentUser._id);

		while (!userRequest.isDone)
			yield return new WaitForSeconds(0.1f);

		Debug.Log("Header: " + userRequest.responseHeaders["STATUS"]);
		Debug.Log("Text: " + userRequest.text);

		if (userRequest.responseHeaders["STATUS"] == HTML.HTTP_200)
		{
			UserService.UpdateLocalUser(userRequest.text);
			FillInfoFields();
		}

		yield return null;
	}

	private void SavePlantsCount (int count)
	{
		string pathString = "CompGirls:Plants:" + currentUser._id;

		PlayerPrefs.SetString(pathString, count.ToString());
		plantsField.text = PlayerPrefs.GetString(pathString);
	}
}
