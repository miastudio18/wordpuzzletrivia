using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefsManager : MonoBehaviour {

	private PrefsManager() {

	}

	private static PrefsManager instance = null;
	public static PrefsManager Instance {
		get {
			return instance;
		}
	}

	string k_sound = "IsSoundOn";
	string k_bestScore = "BestScore";
	string k_tutorialShown = "IsTutorialShown";

	void Awake() {
		if (instance == null) {
			instance = this;
		}
		else if (instance != this) {
			Destroy(gameObject);
		}
	}

	public bool IsSoundOn {
		get {
			return PlayerPrefs.GetInt(k_sound, 1) == 1;
		}

		set {
			PlayerPrefs.SetInt(k_sound, value == true ? 1 : 0);
		}
	}

	public int BestScore {
		get {
			return PlayerPrefs.GetInt(k_bestScore, 0);
		}

		set {
			PlayerPrefs.SetInt(k_bestScore, value);
		}
	}

	public bool IsTutorialShown {
		get {
			return PlayerPrefs.GetInt(k_tutorialShown, 0) == 1;
		}

		set {
			PlayerPrefs.SetInt(k_tutorialShown, value == true ? 1 : 0);
		}
	}

}
