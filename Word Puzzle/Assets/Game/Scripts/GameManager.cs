using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	private GameManager() {

	}

	private static GameManager instance = null;
	public static GameManager Instance {
		get {
			return instance;
		}
	}

	public Canvas canvas;
	public List<GameObject> gridPoints;
	public GameObject letterPlaceholder;
	[SerializeField] private GameObject holder;
	[SerializeField] private GameObject letterPrefab;
	[SerializeField] private GameObject completedWordPrefab;
	[SerializeField] private GameObject comboTxtPrefab;

	[HideInInspector] public string alphabets = "abcdefghijklmnopqrstuvwxyz";
	[HideInInspector] public string vowels = "aeiou";
	[SerializeField] private TextAsset wordsData;
	private string wordsString;
	private List<string> wordsList;

	// Words along each line of grid
	[HideInInspector] public Word[] words = new Word[20];

	public int LetterSpawnCount {
		get;
		set;
	}

	public string CurrentLetterFeed {
		get;
		set;
	}

	public string NextLetter {
		get {
			string nextLetter = "";
			int idx = LetterSpawnCount % 4;
			if (idx < 3) {
				nextLetter = CurrentLetterFeed [idx + 1].ToString ();
			}
			else {
				nextLetter = nextLetterFeed [0].ToString ();
			}

			return nextLetter;
		}
	}

	private string nextLetterFeed = "";
	private bool firstFeed = true;

	public GameObject CurrentActiveLetter {
		get;
		set;
	}
	private GameObject nextLetter;
	private bool firstSpawn = true;

	private GameObject completedWord;
	private GameObject comboTxt;

	void Awake() {
		if (instance == null) {
			instance = this;
		}
		else if (instance != this) {
			Destroy(gameObject);
		}
	}

	void Start() {
		wordsString = wordsData.text;
		wordsList = new List<string> (wordsString.Split (" ".ToCharArray (), StringSplitOptions.RemoveEmptyEntries));

		AdsManager.Instance.ShowBanner ();
	}

	public void StartGame() {
		Reset ();

		SpawnLetter ();
	}

	private void Reset() {
		DestroyLetters ();
		FreeGridPoints ();
		LetterSpawnCount = 0;
		firstFeed = true;
		firstSpawn = true;

		PowerupManager.Instance.Reset ();
		ScoreManager.Instance.Reset ();
	}

	private void DestroyLetters() {
		foreach (GameObject point in gridPoints) {
			GameObject letter = point.GetComponent<GridPoint> ().Letter;
			if (letter != null) {
				Destroy (letter);
			}
		}

		if (CurrentActiveLetter != null) {
			Destroy (CurrentActiveLetter);
		}

		if (nextLetter != null) {
			Destroy (nextLetter);
		}
	}

	private void FreeGridPoints() {
		foreach(GameObject point in gridPoints) {
			point.GetComponent<GridPoint> ().IsOccupied = false;
		}
	}

	public void SpawnLetter() {
		if (firstSpawn) {
			CurrentActiveLetter = Instantiate (letterPrefab, letterPlaceholder.transform.position,
				Quaternion.identity, holder.transform);
			CurrentActiveLetter.GetComponent<Letter> ().SetLetter ();

			nextLetter = Instantiate (letterPrefab, holder.transform);
			nextLetter.GetComponent<Letter> ().SetLetter ();
			nextLetter.GetComponent<LetterMovement> ().enabled = false;

			firstSpawn = false;
		}
		else {
			CurrentActiveLetter = nextLetter;
			StartCoroutine (LoadLetterCo (CurrentActiveLetter.transform, letterPlaceholder.transform));
		}
	}

	public void SummonLetter(string l) {
		CurrentActiveLetter = Instantiate (letterPrefab, letterPlaceholder.transform.position,
			Quaternion.identity, holder.transform);
		CurrentActiveLetter.GetComponent<Letter> ().SetLetter (l);
	}

	public void SetLetterFeed() {
		if (firstFeed) {
			CurrentLetterFeed = wordsList [UnityEngine.Random.Range (0, wordsList.Count)];
			CurrentLetterFeed = Utility.Shuffle (CurrentLetterFeed);

			firstFeed = false;
		}
		else {
			CurrentLetterFeed = nextLetterFeed;
		}

		nextLetterFeed = wordsList [UnityEngine.Random.Range (0, wordsList.Count)];
		nextLetterFeed = Utility.Shuffle (nextLetterFeed);
	}

	public void CheckForWords() {
		string[] letters = new string[gridPoints.Count];

		for (int i = 0; i < letters.Length; i++) {
			GameObject letter = gridPoints [i].GetComponent<GridPoint> ().Letter;
			if (letter == null) {
				letters [i] = "@";
			}
			else {
				letters [i] = letter.GetComponent<Letter> ().Name;
			}
		}

		words [0] = new Word (0, 1, 2, 3);
		words [1] = new Word (4, 5, 6, 7);
		words [2] = new Word (8, 9, 10, 11);
		words [3] = new Word (12, 13, 14, 15);

		words [4] = new Word (0, 4, 8, 12);
		words [5] = new Word (1, 5, 9, 13);
		words [6] = new Word (2, 6, 10, 14);
		words [7] = new Word (3, 7, 11, 15);

		words [8] = new Word (12, 9, 6, 3);
		words [9] = new Word (0, 5, 10, 15);

		words [10] = new Word (3, 2, 1, 0);
		words [11] = new Word (7, 6, 5, 4);
		words [12] = new Word (11, 10, 9, 8);
		words [13] = new Word (15, 14, 13, 12);

		words [14] = new Word (12, 8, 4, 0);
		words [15] = new Word (13, 9, 5, 1);
		words [16] = new Word (14, 10, 6, 2);
		words [17] = new Word (15, 11, 7, 3);

		words [18] = new Word (3, 6, 9, 12);
		words [19] = new Word (15, 10, 5, 0);

		List<string> foundWords = new List<string> ();
		for (int i = 0; i < words.Length; i++) {
			for (int k = 0; k < 4; k++) {
				words [i].word += letters [words [i].indexes [k]];
			}

			string wordToSearch = words [i].word.ToUpper ();

			// Check if a word has four valid letters
			if (!wordToSearch.Contains ("@")) {
				// Search the word and its reverse in words list
				int wordIdx = wordsList.BinarySearch (wordToSearch);
				if (wordIdx >= 0) {
					foundWords.Add (words [i].word);
					GameObject[] completedWordLetters = new GameObject[4];
					for (int k = 0; k < 4; k++) {
						completedWordLetters[k] = gridPoints [words [i].indexes[k]].GetComponent<GridPoint> ().Letter;
						StartCoroutine (DestroyLetterCo (completedWordLetters [k], gridPoints [words [i].indexes [k]]));
					}
				}
			}
		}

		// Display all the correct words
		if (foundWords.Count > 0) {
			string txt = "";
			foreach (string w in foundWords) {
				txt += (w + " ");
			}
			if (completedWord != null) {
				Destroy (completedWord);
			}
			completedWord = Instantiate (completedWordPrefab, holder.transform);
			completedWord.GetComponent<Text> ().text = txt;
			completedWord.GetComponent<Animator> ().Play ("CompletedWordAppear");
			StartCoroutine (FadeOutCo (completedWord, "CompletedWordHide", foundWords.Count + 1));

			int combo = foundWords.Count;
			int scoreEarned = ((combo * combo) * 100);
			int bonusScore = scoreEarned - (foundWords.Count * 100);
			ScoreManager.Instance.Score += scoreEarned;
			UIManager.Instance.UpdateScore (ScoreManager.Instance.Score);

			if (combo > 1) {
				if (comboTxt != null) {
					Destroy (comboTxt);
				}
				comboTxt = Instantiate (comboTxtPrefab, holder.transform);
				comboTxt.GetComponent<Text> ().text = combo + "x combo - " + bonusScore + " bonus score";
				comboTxt.GetComponent<Animator> ().Play ("ComboTxtShow");
				StartCoroutine (FadeOutCo (comboTxt, "ComboTxtHide", foundWords.Count + 1));
				AudioManager.Instance.PlayComboSound ();

				UIManager.Instance.HidePauseBtnFor (foundWords.Count + 1);
			}
			else {
				AudioManager.Instance.PlayLineClearSound ();
			}
		}

		CheckForGameOver ();
	}

	private IEnumerator LoadLetterCo(Transform letter, Transform dest) {
		float distance = Vector3.Distance (letter.position, dest.position);
		float speed = distance * 4;
		float time = distance / speed;
		float timer = 0;

		while (timer < time) {
			float step = speed * Time.deltaTime;
			letter.position = Vector3.MoveTowards (letter.position, dest.position, step);

			timer += Time.deltaTime;

			yield return new WaitForEndOfFrame ();
		}

		letter.position = dest.position;
		letter.GetComponent<LetterMovement> ().enabled = true;

		nextLetter = Instantiate (letterPrefab, holder.transform);
		nextLetter.GetComponent<Letter> ().SetLetter ();
		nextLetter.GetComponent<LetterMovement> ().enabled = false;
	}

	public IEnumerator DestroyLetterCo(GameObject letter, GameObject gridPoint = null) {
		if (gridPoint != null) {
			gridPoint.GetComponent<GridPoint> ().IsOccupied = false;
		}

		letter.GetComponent<Animator> ().Play ("LetterHide");
		yield return new WaitForSeconds (0.5f);

		if (letter != null) {
			Destroy (letter);
		}
	}

	private IEnumerator FadeOutCo(GameObject obj, string animName, float delay) {
		yield return new WaitForSeconds (delay);

		if (obj != null) {
			obj.GetComponent<Animator> ().Play (animName);
			Destroy (obj, 0.5f);
		}
	}

	private void CheckForGameOver() {
		bool gameOver = true;
		foreach (GameObject point in gridPoints) {
			if (!point.GetComponent<GridPoint> ().IsOccupied) {
				gameOver = false;
				break;
			}
		}

		if (gameOver) {
			AudioManager.Instance.PlayGameOverSound ();
			UIManager.Instance.LoadScreen (UIManager.Instance.gameplay, UIManager.Instance.gameOverMenu);

			if (ScoreManager.Instance.Score > PrefsManager.Instance.BestScore) {
				PrefsManager.Instance.BestScore = ScoreManager.Instance.Score;
			}

			UIManager.Instance.SetGameOverScoreTxt ("Score: " + ScoreManager.Instance.Score);
			UIManager.Instance.UpdateBestScoreTxts ();

			AdsManager.Instance.ShowInterstitial ();
		}
	}

}
