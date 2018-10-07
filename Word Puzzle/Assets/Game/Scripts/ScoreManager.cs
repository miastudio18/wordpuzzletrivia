using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour {

	private ScoreManager() {

	}

	private static ScoreManager instance = null;
	public static ScoreManager Instance {
		get {
			return instance;
		}
	}

	private int score = 0;
	public int Score {
		get {
			return score;
		}

		set {
			score = value;
		}
	}

	void Awake() {
		if (instance == null) {
			instance = this;
		}
		else if (instance != this) {
			Destroy(gameObject);
		}
	}

	public void Reset() {
		Score = 0;
		UIManager.Instance.SetScoreTxt (Score.ToString());
	}

}
