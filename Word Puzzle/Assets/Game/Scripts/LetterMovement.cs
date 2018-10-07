using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LetterMovement : MonoBehaviour {

	private float radius = 0;
	private bool canMove = false;

	void Start () {
		radius = (GetComponent<RectTransform> ().rect.width * GameManager.Instance.canvas.scaleFactor) / 2.0f;
	}

	void Update () {
		// Movement of letter (devices with touch screen)
		for (int i = 0; i < Input.touchCount; i++) {
			if (canMove) {
				transform.position = new Vector3 (Input.GetTouch (i).position.x, Input.GetTouch (i).position.y, transform.position.z);
			}

			if (Input.GetTouch (i).phase == TouchPhase.Ended) {
				canMove = false;
				StartCoroutine (PlaceCo ());
			}
		}

		// Movement of letter (devices with mouse)
		#if UNITY_EDITOR
		if (Input.GetMouseButton (0)) {
			if (canMove) {
				transform.position = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, transform.position.z);
			}
		}
		if (Input.GetMouseButtonUp (0)) {
			canMove = false;
			StartCoroutine (PlaceCo ());
		}
		#endif
		
		// Keep the letter within screen
		if (transform.position.x <= 0 + radius) {
			transform.position = new Vector3 (0 + radius, transform.position.y, transform.position.z);
		}
		else if(transform.position.x >= Screen.width - radius) {
			transform.position = new Vector3 (Screen.width - radius, transform.position.y, transform.position.z);
		}
		if (transform.position.y <= 0 + radius) {
			transform.position = new Vector3 (transform.position.x, 0 + radius, transform.position.z);
		}
		else if(transform.position.y >= Screen.height - radius) {
			transform.position = new Vector3 (transform.position.x, Screen.height - radius, transform.position.z);
		}
	}

	public void Letter_OnPointerDown() {
		if (this.enabled && !PowerupManager.Instance.IsLineDestroyMode) {
			canMove = true;
		}
	}

	private GameObject GetNearestPoint(GameObject point) {
		GameObject nearestPoint = GameManager.Instance.gridPoints [0];
		float minDistance = Vector3.Distance (point.transform.position, GameManager.Instance.gridPoints [0].transform.position);
		for (int i = 1; i < GameManager.Instance.gridPoints.Count; i++) {
			if (Vector3.Distance (point.transform.position, GameManager.Instance.gridPoints [i].transform.position) < minDistance) {
				nearestPoint = GameManager.Instance.gridPoints [i];
				minDistance = Vector3.Distance (point.transform.position, nearestPoint.transform.position);
			}
		}

		return nearestPoint;
	}

	private IEnumerator PlaceCo() {
		GameObject nearestPoint = GetNearestPoint (this.gameObject);
		GridPoint gridPoint = nearestPoint.GetComponent<GridPoint> ();

		// Placed on a grid point
		if (Vector3.Distance (transform.position, nearestPoint.transform.position) <= radius && !gridPoint.IsOccupied) {
			AudioManager.Instance.PlayClickSound ();
			yield return StartCoroutine (MoveCo (transform, nearestPoint.transform));

			gridPoint.IsOccupied = true;
			gridPoint.Letter = this.gameObject;
			GetComponent<Letter> ().IsPlaced = true;
			this.enabled = false;

			GameManager.Instance.CheckForWords ();

			GameManager.Instance.SpawnLetter ();
		}
		// Reverted back to placeholder
		else {
			yield return StartCoroutine (MoveCo (transform, GameManager.Instance.letterPlaceholder.transform));
		}
	}

	private IEnumerator MoveCo(Transform current, Transform target) {
		float distance = Vector3.Distance (current.position, target.position);
		float speed = distance * 10;
		float time = distance / speed;
		float timer = 0;

		while (timer < time) {
			float step = speed * Time.deltaTime;
			current.position = Vector3.MoveTowards (current.position, target.position, step);

			timer += Time.deltaTime;

			yield return new WaitForEndOfFrame ();
		}

		current.position = target.position;
	}

}
