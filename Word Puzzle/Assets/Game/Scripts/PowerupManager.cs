using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerupManager : MonoBehaviour {

	private PowerupManager() {

	}

	private static PowerupManager instance = null;
	public static PowerupManager Instance {
		get {
			return instance;
		}
	}

	public bool IsLineDestroyMode {
		get;
		set;
	}

	[SerializeField] private int destroyLetterTotal;
	[SerializeField] private int destroyLineTotal;
	[SerializeField] private int summonLetterTotal;

	[HideInInspector] public int destroyLetterUses;
	[HideInInspector] public int destroyLineUses;
	[HideInInspector] public int summonLetterUses;

	void Awake() {
		if (instance == null) {
			instance = this;
		}
		else if (instance != this) {
			Destroy(gameObject);
		}
	}

	public void Reset() {
		IsLineDestroyMode = false;
		destroyLetterUses = destroyLetterTotal;
		destroyLineUses = destroyLineTotal;
		summonLetterUses = summonLetterTotal;
		UIManager.Instance.UpdatePowerupsBtnAnim (true);
	}

	void Update() {
		if (IsLineDestroyMode) {
			for (int i = 0; i < Input.touchCount; i++) {
				if (Input.GetTouch (i).phase == TouchPhase.Ended) {
					CheckAndDestroyLine ();
					ClearSelection ();
				}
			}

			#if UNITY_EDITOR
			if (Input.GetMouseButtonUp (0)) {
				CheckAndDestroyLine ();
				ClearSelection ();
			}
			#endif
		}
	}

	public void PowerupDestroyLetter() {
		if (destroyLetterUses <= 0) {
			return;
		}

		destroyLetterUses--;
		UIManager.Instance.UpdatePowerupUses ();
		UIManager.Instance.UpdatePowerupsBtnAnim ();
		StartCoroutine (PowerupDestroyLetterCo ());
	}

	private IEnumerator PowerupDestroyLetterCo() {
		yield return StartCoroutine (GameManager.Instance.DestroyLetterCo (GameManager.Instance.CurrentActiveLetter));
		GameManager.Instance.SpawnLetter ();
	}

	private void CheckAndDestroyLine() {
		GameObject[] gridPoints = GameManager.Instance.gridPoints.ToArray ();
		List<int> selectedIndexes = new List<int> ();

		for (int i = 0; i < gridPoints.Length; i++) {
			if (gridPoints [i].GetComponent<GridPoint> ().Letter != null) {
				if (gridPoints [i].GetComponent<GridPoint> ().Letter.GetComponent<Letter> ().IsSelected) {
					selectedIndexes.Add (i);
				}
			}
		}

		if (selectedIndexes.Count == 4) {
			Word[] words = GameManager.Instance.words;
			for (int i = 0; i < words.Length; i++) {
				int lettersMatched = 0;
				for (int k = 0; k < selectedIndexes.Count; k++) {
					if (selectedIndexes.Contains (words [i].indexes [k])) {
						lettersMatched++;
					}
				}

				if (lettersMatched == 4) {
					destroyLineUses--;
					UIManager.Instance.UpdatePowerupUses ();
					UIManager.Instance.UpdatePowerupsBtnAnim ();

					GameObject[] letters = new GameObject[4];
					for (int k = 0; k < letters.Length; k++) {
						if (gridPoints [words [i].indexes [k]].GetComponent<GridPoint> ().Letter != null) {
							letters [k] = gridPoints [words [i].indexes [k]].GetComponent<GridPoint> ().Letter;
							StartCoroutine (GameManager.Instance.DestroyLetterCo (letters [k], gridPoints [words [i].indexes [k]]));
						}
					}

					AudioManager.Instance.PlayLineClearSound ();
					PowerupManager.instance.IsLineDestroyMode = false;
					UIManager.Instance.SetActiveDestroyLineTools (false);

					return;
				}
			}
		}
	}

	private void ClearSelection() {
		GameObject[] gridPoints = GameManager.Instance.gridPoints.ToArray ();
		for (int i = 0; i < gridPoints.Length; i++) {
			if (gridPoints [i].GetComponent<GridPoint> ().Letter != null) {
				gridPoints [i].GetComponent<GridPoint> ().Letter.GetComponent<Letter> ().IsSelected = false;
				DeselectLetter (gridPoints [i].GetComponent<GridPoint> ().Letter);
			}
		}
	}

	public void SummonLetter(string l) {
		if (summonLetterUses <= 0) {
			return;
		}

		summonLetterUses--;
		UIManager.Instance.UpdatePowerupUses ();
		UIManager.Instance.UpdatePowerupsBtnAnim ();
		StartCoroutine (SummonLetterCo (l));
	}

	private IEnumerator SummonLetterCo(string l) {
		yield return StartCoroutine (GameManager.Instance.DestroyLetterCo (GameManager.Instance.CurrentActiveLetter));
		GameManager.Instance.SummonLetter (l);
	}

	public void SelectLetter(GameObject letter) {
		letter.GetComponent<Image> ().color = UIManager.Instance.selectedColor;
		letter.GetComponent<Shadow> ().effectColor = UIManager.Instance.selectedShadowColor;
	}

	public void DeselectLetter(GameObject letter) {
		letter.GetComponent<Letter> ().ResetColor ();
	}

}
