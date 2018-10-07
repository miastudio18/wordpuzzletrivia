using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Letter : MonoBehaviour {

	public string Name {
		get;
		set;
	}

	public bool IsSelected {
		get;
		set;
	}

	public bool IsPlaced {
		get;
		set;
	}

	[SerializeField] private Text nameTxt;
	private Color color;
	private Color shadowColor;

	void Start() {
		RandomizeColor ();
	}

	private void RandomizeColor() {
		int randomIdx = Random.Range (0, UIManager.Instance.letterColors.Length);
		color = UIManager.Instance.letterColors [randomIdx];
		shadowColor = UIManager.Instance.letterShadowColors [randomIdx];
		GetComponent<Image> ().color = color;
		GetComponent<Shadow> ().effectColor = shadowColor;
	}

	public void SetLetter(string l = "") {
		int idx = 0;
		string name = "";

		if (l != "") {
			name = l.ToUpper ();
		}
		else {
			if (GameManager.Instance.LetterSpawnCount % 4 == 0) {
				GameManager.Instance.SetLetterFeed ();
			}

			idx = GameManager.Instance.LetterSpawnCount % 4;
			name = GameManager.Instance.CurrentLetterFeed [idx].ToString ();

			GameManager.Instance.LetterSpawnCount++;
		}

		Name = name;
		nameTxt.text = Name.ToString ().ToUpper ();
	}

	public void ResetColor() {
		GetComponent<Image> ().color = color;
		GetComponent<Shadow> ().effectColor = shadowColor;
	}

	public void Letter_OnPointerEnter() {
		if (PowerupManager.Instance.IsLineDestroyMode && IsPlaced) {
			IsSelected = true;
			PowerupManager.Instance.SelectLetter (this.gameObject);
		}
	}

}
