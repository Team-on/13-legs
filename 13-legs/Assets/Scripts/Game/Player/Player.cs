﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour {
	public Action OnLose;

	[Header("HP")]
	[SerializeField] float hpMax;
	float hpCurr;
	[SerializeField] float hpLoss;
	[SerializeField] float hpLossTime;
	[SerializeField] Transform hpCircle;

	[Header("Moving")]
	[SerializeField] float moveForce;

	[Header("Score")]
	public TextMeshProUGUI scoreText;

	[Header("Refs")]
	[SerializeField] Rigidbody2D rb;

	Vector3 moveVect;
	float currHpLoseTimer;
	bool isPlaying;

	float currScore;

	void Update() {
		if (!isPlaying)
			return;

		moveVect = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

		currScore += Time.deltaTime;
		scoreText.text = currScore.ToString("0.00");

		currHpLoseTimer += Time.deltaTime;
		while (currHpLoseTimer >= hpLossTime) {
			currHpLoseTimer -= hpLossTime;
			if ((hpCurr -= hpLoss) <= 0.0f)
				hpCurr = 0;

			if (hpCurr == 0)
				Lose();

			hpCircle.localScale = Vector3.one * (hpCurr / hpMax);
		}

	}

	void FixedUpdate() {
		if (!isPlaying)
			return;

		rb.AddForce(moveVect * moveForce * Time.fixedDeltaTime);
	}

	void OnTriggerEnter2D(Collider2D collision) {
		if (collision.tag == "Food") {
			Food food = collision.GetComponent<Food>();
			if((hpCurr += food.hpRegen) > hpMax) 
				hpCurr = hpMax;
			food.OnEat();
			Destroy(collision.gameObject);
		}
	}

	public void StartGame() {
		isPlaying = true;
		currHpLoseTimer = 0;
		currScore = 0;
		hpCurr = hpMax;

		StopAllCoroutines();
		hpCircle.localScale = Vector3.one;
	}

	public void Lose() {
		isPlaying = false;
		StartCoroutine(FillCoroutine());

		PlayerPrefs.SetFloat("CurrScore", currScore);
		OnLose.Invoke();
		
		float maxScore = PlayerPrefs.GetFloat("MaxScore", 0.0f);
		if (currScore > maxScore)
			PlayerPrefs.SetFloat("MaxScore", currScore);
	}

	IEnumerator FillCoroutine() {
		while (true) {
			hpCurr += 1.0f;
			if(hpCurr > hpMax) 
				hpCurr = hpMax;

			hpCircle.localScale = Vector3.one * (hpCurr / hpMax);

			if (hpCurr == hpMax)
				break;

			yield return null;
		}
	}
}
