using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour {

	private TutorialManager() {

	}

	private static TutorialManager instance = null;
	public static TutorialManager Instance {
		get {
			return instance;
		}
	}

	public Canvas canvas;
	public List<GameObject> sample1SourceLetters;
	public List<GameObject> sample1TargetPoints;
	public List<GameObject> sample2SourceLetters;
	public GameObject sample2TargetPoint;
	public GameObject letterPlaceholder;
	public Color highlightColor;
	[TextArea] public string[] instructions;
	[SerializeField] private Button nextBtn;
	[SerializeField] private Text descTxt;
	[SerializeField] private GameObject sample1;
	[SerializeField] private GameObject sample2;
	[SerializeField] private GameObject[] sample1Tools;

	public int InstructionCounter {
		get;
		set;
	}

	public int StepCounter {
		get;
		set;
	}

	void Awake() {
		if (instance == null) {
			instance = this;
		}
		else if (instance != this) {
			Destroy(gameObject);
		}
	}

	void Start() {
		
	}

	public void HighlightTarget() {
		ClearHighlight ();

		if (InstructionCounter == 1) {
			sample1TargetPoints [StepCounter].GetComponent<Image> ().color = highlightColor;
		}
		else if (InstructionCounter == 3) {
			sample2TargetPoint.GetComponent<Image> ().color = highlightColor;
		}
	}

	public void ClearHighlight() {
		if (InstructionCounter == 1) {
			foreach (GameObject target in sample1TargetPoints) {
				target.GetComponent<Image> ().color = Color.white;
			}
		}
		else if (InstructionCounter == 3) {
			sample2TargetPoint.GetComponent<Image> ().color = Color.white;
		}
	}

	public IEnumerator LoadLetterCo() {
		if (StepCounter == 1) {
			foreach (GameObject obj in sample1Tools) {
				obj.SetActive (false);
			}
		}

		Transform source = sample1SourceLetters [StepCounter].transform;
		Transform target = letterPlaceholder.transform;

		float distance = Vector3.Distance (source.position, target.position);
		float speed = distance * 4;
		float time = distance / speed;
		float timer = 0;

		while (timer < time) {
			float step = speed * Time.deltaTime;
			source.position = Vector3.MoveTowards (source.position, target.position, step);

			timer += Time.deltaTime;

			yield return new WaitForEndOfFrame ();
		}

		source.position = target.position;
		source.GetComponent<TutorialLetter> ().enabled = true;

		if (StepCounter < sample1SourceLetters.Count - 1) {
			sample1SourceLetters [StepCounter + 1].SetActive (true);
		}

		HighlightTarget ();
	}

	public IEnumerator DestroyLetterCo(GameObject letter) {
		letter.GetComponent<Animator> ().Play ("LetterHide");
		yield return new WaitForSeconds (0.5f);

		if (letter != null) {
			Destroy (letter);
		}
	}

	public void NextBtn_OnClick() {
		AudioManager.Instance.PlayClickSound ();

		InstructionCounter++;

		UpdateInstructionTxt ();

		if (InstructionCounter == 1) {
			sample1.SetActive (true);
			HighlightTarget ();
			SetNextBtnInteraction (false);
		}

		if (InstructionCounter == 3) {
			sample2.SetActive (true);
			HighlightTarget ();
			SetNextBtnInteraction (false);
		}

		if (InstructionCounter == 5) {
			SceneManager.LoadScene ("Main");
		}
	}

	public void UpdateInstructionTxt() {
		if (InstructionCounter < instructions.Length) {
			descTxt.text = instructions [InstructionCounter];
		}
	}

	public void SetNextBtnInteraction(bool interaction) {
		nextBtn.interactable = interaction;
	}

	public void SetNextBtnTxt(string txt) {
		nextBtn.transform.GetChild (0).GetComponent<Text> ().text = txt;
	}

}
