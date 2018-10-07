using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	private UIManager() {

	}

	private static UIManager instance = null;
	public static UIManager Instance {
		get {
			return instance;
		}
	}

	public GameObject menu;
	public GameObject gameplay;
	public GameObject pauseMenu;
	public GameObject gameOverMenu;
	public GameObject tutorial;
	[SerializeField] private Animator faderAnim;
	[SerializeField] private GameObject pauseBtn;
	[SerializeField] private GameObject[] noSoundLines;
	public Color[] letterColors;
	public Color[] letterShadowColors;
	public Color selectedColor;
	public Color selectedShadowColor;
	public Text gameScoreTxt;
	[SerializeField] private Text gameOverScoreTxt;
	[SerializeField] private Text[] bestScoreTxts;
	[SerializeField] private GameObject powerupsPopup;
    [SerializeField] private GameObject earnPowerupsPopup;
	[SerializeField] private Animator powerupsBtnAnim;
	[SerializeField] private GameObject summonableLetters;
	[SerializeField] private GameObject[] destroyLineTools;
	[SerializeField] private Button destroyLetterBtn;
	[SerializeField] private Button destroyLineBtn;
	[SerializeField] private Button summonLetterBtn;
	[SerializeField] private Text destroyLetterUsesTxt;
	[SerializeField] private Text destroyLineUsesTxt;
	[SerializeField] private Text summonLetterUsesTxt;
    [SerializeField] private Button earnPowerupsVideoBtn;

	void Awake() {
		if (instance == null) {
			instance = this;
		}
		else if (instance != this) {
			Destroy(gameObject);
		}
	}

	void Start() {
		UpdateSoundBtns ();
		UpdateBestScoreTxts ();

		ShowTutorialIfNeeded ();
	}

	public void PlayBtn_OnClick() {
		AudioManager.Instance.PlayClickSound ();
		LoadGameplay (menu);
	}

	public void TutorialBtn_OnClick() {
		AudioManager.Instance.PlayClickSound ();
		LoadScreen (menu, tutorial);
	}

	public void SoundBtn_OnClick() {
		AudioManager.Instance.PlayClickSound ();
		PrefsManager.Instance.IsSoundOn = !PrefsManager.Instance.IsSoundOn;
		UpdateSoundBtns ();
	}

    public void RateBtn_OnClick() {
        AudioManager.Instance.PlayClickSound();
        RateManager.Instance.OpenRatingPage();
    }

    public void UpdateScore(int newScore) {
		StartCoroutine (UpdateScoreCo (newScore));
	}

	public void SetScoreTxt(string txt) {
		gameScoreTxt.text = txt;
	}

	public void SetGameOverScoreTxt(string txt) {
		gameOverScoreTxt.text = txt;
	}

	public void UpdateBestScoreTxts() {
		foreach (Text t in bestScoreTxts) {
			t.text = "Best Score: " + PrefsManager.Instance.BestScore;
		}
	}

	public void PowerupsBtn_OnClick() {
		AudioManager.Instance.PlayClickSound ();

        if (PowerupManager.Instance.destroyLetterUses > 0 || PowerupManager.Instance.destroyLineUses > 0 ||
            PowerupManager.Instance.summonLetterUses > 0) {

            powerupsPopup.SetActive(true);
            powerupsPopup.GetComponent<Animator>().Play("PowerupsPopupShow");
            UpdatePowerupUses();
        }
        else {
            earnPowerupsPopup.SetActive(true);
            earnPowerupsPopup.GetComponent<Animator>().Play("PowerupsPopupShow");
        }
	}

	public void PowerupsCloseBtn_OnClick() {
		AudioManager.Instance.PlayClickSound ();
		SetPowerupInteraction (false);
		StartCoroutine (HideCo (powerupsPopup, "PowerupsPopupHide", 0.5f));
	}

    public void EarnPowerupsCloseBtn_OnClick() {
        AudioManager.Instance.PlayClickSound();
        StartCoroutine(HideCo(earnPowerupsPopup, "PowerupsPopupHide", 0.5f));
    }

    public void EarnPowerupsVideoBtn_OnClick() {
        AudioManager.Instance.PlayClickSound();

        AdsManager.Instance.ShowRewardedAd(() => {

            StartCoroutine(HideCo(earnPowerupsPopup, "PowerupsPopupHide", 0.5f));

            powerupsPopup.SetActive(true);
            powerupsPopup.GetComponent<Animator>().Play("PowerupsPopupShow");
            UpdatePowerupUses();

        });
    }

    public void EarnPowerupsRestartBtn_OnClick() {
        AudioManager.Instance.PlayClickSound();

        earnPowerupsPopup.SetActive(false);
        LoadGameplay(gameplay);
    }

    public void PauseBtn_OnClick() {
		AudioManager.Instance.PlayClickSound ();
		LoadScreen (gameplay, pauseMenu);
	}

	public void ResumeBtn_OnClick() {
		AudioManager.Instance.PlayClickSound ();
		LoadScreen (pauseMenu, gameplay);
	}

	public void RestartBtn_OnClick() {
		AudioManager.Instance.PlayClickSound ();
		LoadGameplay (pauseMenu);
	}

	public void PauseHomeBtn_OnClick() {
		AudioManager.Instance.PlayClickSound ();
		LoadScreen (pauseMenu, menu);
	}

	public void GORestartBtn_OnClick() {
		AudioManager.Instance.PlayClickSound ();
		LoadGameplay (gameOverMenu);
	}

	public void GOHomeBtn_OnClick() {
		AudioManager.Instance.PlayClickSound ();
		LoadScreen (gameOverMenu, menu);
	}

	public void ShowSummonableLetters() {
		summonableLetters.SetActive (true);
		summonableLetters.GetComponent<Animator> ().Play ("SummonableLettersShow");
	}

	public void DestroyLetterBtn_OnClick() {
		AudioManager.Instance.PlayClickSound ();
		StartCoroutine (DestroyLetterCo ());
	}

	private IEnumerator DestroyLetterCo() {
		SetPowerupInteraction (false);
		yield return StartCoroutine (HideCo (powerupsPopup, "PowerupsPopupHide", 0.5f));
		PowerupManager.Instance.PowerupDestroyLetter ();
	}

	public void DestroyLineBtn_OnClick() {
		if (PowerupManager.Instance.destroyLineUses <= 0) {
			return;
		}

		AudioManager.Instance.PlayClickSound ();
		StartCoroutine (DestroyLineCo ());
	}

	private IEnumerator DestroyLineCo() {
		SetPowerupInteraction (false);
		yield return StartCoroutine (HideCo (powerupsPopup, "PowerupsPopupHide", 0.5f));
		PowerupManager.Instance.IsLineDestroyMode = true;
		SetActiveDestroyLineTools (true);
	}

	public void SummonLetterBtn_OnClick() {
		AudioManager.Instance.PlayClickSound ();
		summonableLetters.SetActive (true);
	}

	public void SummonLetter(string l) {
		AudioManager.Instance.PlayClickSound ();
		StartCoroutine (SummonLetterCo (l));
	}

	private IEnumerator SummonLetterCo(string l) {
		summonableLetters.SetActive (false);
		SetPowerupInteraction (false);
		yield return StartCoroutine (HideCo (powerupsPopup, "PowerupsPopupHide", 0.5f));
		PowerupManager.Instance.SummonLetter (l);
	}

	public void SummonLettersCloseBtn_OnClick() {
		AudioManager.Instance.PlayClickSound ();
		summonableLetters.SetActive (false);
	}

	public void DestroyLineCloseBtn_OnClick() {
		AudioManager.Instance.PlayClickSound ();
		PowerupManager.Instance.IsLineDestroyMode = false;
		SetActiveDestroyLineTools (false);
	}

	private IEnumerator UpdateScoreCo(int newScore) {
		int oldScore = int.Parse (gameScoreTxt.text);
		int currentScore = oldScore;
		int steps = 10;
		float time = 0.5f;

		while (currentScore < newScore) {
			currentScore += ((newScore - oldScore) / 10);
			gameScoreTxt.text = currentScore.ToString ();

			yield return new WaitForSeconds (time / steps);
		}
	}

	private IEnumerator HideCo(GameObject obj, string animName, float delay) {
		obj.GetComponent<Animator> ().Play (animName);
		yield return new WaitForSeconds (delay);

		obj.SetActive (false);
	}

	public void SetActiveDestroyLineTools(bool active) {
		foreach (GameObject obj in destroyLineTools) {
			obj.SetActive (active);
		}
	}

	public void SetPowerupInteraction(bool state) {
		destroyLetterBtn.interactable = state;
		destroyLineBtn.interactable = state;
		summonLetterBtn.interactable = state;
	}

	public void UpdatePowerupUses() {
		destroyLetterUsesTxt.text = "Uses: " + PowerupManager.Instance.destroyLetterUses.ToString ();
		destroyLineUsesTxt.text = "Uses: " + PowerupManager.Instance.destroyLineUses.ToString ();
		summonLetterUsesTxt.text = "Uses: " + PowerupManager.Instance.summonLetterUses.ToString ();

		destroyLetterBtn.interactable = PowerupManager.Instance.destroyLetterUses > 0;
		destroyLineBtn.interactable = PowerupManager.Instance.destroyLineUses > 0;
		summonLetterBtn.interactable = PowerupManager.Instance.summonLetterUses > 0;
	}

	public void UpdatePowerupsBtnAnim(bool def = false) {
		if (def) {
			powerupsBtnAnim.Play ("PowerupsBtn");

			return;
		}

		bool active = PowerupManager.Instance.destroyLetterUses > 0 ||
		              PowerupManager.Instance.destroyLineUses > 0 ||
		              PowerupManager.Instance.summonLetterUses > 0;

		if (!active) {
			powerupsBtnAnim.Play ("PowerupsBtnReset");
		}
	}

	public void HidePauseBtnFor(float duration) {
		StartCoroutine (HidePauseBtnCo (duration));
	}

	private IEnumerator HidePauseBtnCo(float duration) {
		pauseBtn.SetActive (false);
		yield return new WaitForSeconds (duration);

		pauseBtn.SetActive (true);
	}

	private void UpdateSoundBtns() {
		foreach (GameObject line in noSoundLines) {
			line.SetActive (!PrefsManager.Instance.IsSoundOn);
		}
	}

	private void ShowTutorialIfNeeded() {
		if (!PrefsManager.Instance.IsTutorialShown) {
			menu.SetActive (false);
			tutorial.SetActive (true);

			PrefsManager.Instance.IsTutorialShown = true;
		}
	}

	public void LoadGameplay(GameObject currentScreen) {
		StartCoroutine (LoadGameplayCo (currentScreen));
	}

	private IEnumerator LoadGameplayCo(GameObject currentScreen) {
		faderAnim.GetComponent<Image> ().raycastTarget = true;
		faderAnim.Play ("FaderIn");
		yield return new WaitForSeconds (0.5f);

		currentScreen.SetActive (false);
		gameplay.SetActive (true);
		GameManager.Instance.StartGame ();

		faderAnim.Play ("FaderOut");
		yield return new WaitForSeconds (0.5f);

		faderAnim.GetComponent<Image> ().raycastTarget = false;
	}

	public void LoadScreen(GameObject fromScreen, GameObject toScreen) {
		StartCoroutine (LoadScreenCo (fromScreen, toScreen));
	}

	private IEnumerator LoadScreenCo(GameObject fromScreen, GameObject toScreen) {
		faderAnim.GetComponent<Image> ().raycastTarget = true;
		faderAnim.Play ("FaderIn");
		yield return new WaitForSeconds (0.5f);

		fromScreen.SetActive (false);
		toScreen.SetActive (true);

		faderAnim.Play ("FaderOut");
		yield return new WaitForSeconds (0.5f);

		faderAnim.GetComponent<Image> ().raycastTarget = false;
	}

}
