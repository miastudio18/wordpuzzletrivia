using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialLetter : MonoBehaviour {

	private float radius = 0;
	private bool canMove = false;

	void Start () {
		radius = (GetComponent<RectTransform> ().rect.width * TutorialManager.Instance.canvas.scaleFactor) / 2.0f;
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
		if (this.enabled) {
			canMove = true;
		}
	}

	private GameObject GetTargetPoint() {
		if (TutorialManager.Instance.InstructionCounter == 1) {
			return TutorialManager.Instance.sample1TargetPoints [TutorialManager.Instance.StepCounter];
		}
		else if (TutorialManager.Instance.InstructionCounter == 3) {
			return TutorialManager.Instance.sample2TargetPoint;
		}

		return null;
	}

	private IEnumerator PlaceCo() {
		GameObject targetPoint = GetTargetPoint ();

		// Placed on the target grid point
		if (Vector3.Distance (transform.position, targetPoint.transform.position) <= radius) {
			AudioManager.Instance.PlayClickSound ();
			yield return StartCoroutine (MoveCo (transform, targetPoint.transform));

			this.enabled = false;
			TutorialManager.Instance.StepCounter++;

			if (TutorialManager.Instance.StepCounter < 4) {
				StartCoroutine (TutorialManager.Instance.LoadLetterCo ());
			}
			else if (TutorialManager.Instance.StepCounter == 4) {
				foreach (GameObject letter in TutorialManager.Instance.sample1SourceLetters) {
					StartCoroutine (TutorialManager.Instance.DestroyLetterCo (letter));
				}

				TutorialManager.Instance.ClearHighlight ();
				TutorialManager.Instance.InstructionCounter++;
				TutorialManager.Instance.UpdateInstructionTxt ();
				TutorialManager.Instance.SetNextBtnInteraction (true);
				AudioManager.Instance.PlayLineClearSound ();
			}
			else if (TutorialManager.Instance.StepCounter == 5) {
				foreach (GameObject letter in TutorialManager.Instance.sample2SourceLetters) {
					StartCoroutine (TutorialManager.Instance.DestroyLetterCo (letter));
				}

				TutorialManager.Instance.ClearHighlight ();
				TutorialManager.Instance.InstructionCounter++;
				TutorialManager.Instance.UpdateInstructionTxt ();
				TutorialManager.Instance.SetNextBtnInteraction (true);
				TutorialManager.Instance.SetNextBtnTxt ("PLAY");
				AudioManager.Instance.PlayComboSound ();
			}
		}
		// Reverted back to placeholder
		else {
			yield return StartCoroutine (MoveCo (transform, TutorialManager.Instance.letterPlaceholder.transform));
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
