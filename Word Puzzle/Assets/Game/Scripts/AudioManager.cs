using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

	private AudioManager() {

	}

	private static AudioManager instance = null;
	public static AudioManager Instance {
		get {
			return instance;
		}
	}

	private AudioSource audioSource;

	[SerializeField] private AudioClip clickSound;
	[SerializeField] private AudioClip lineClearSound;
	[SerializeField] private AudioClip comboSound;
	[SerializeField] private AudioClip gameOverSound;

	void Awake() {
		if (instance == null) {
			instance = this;
		}
		else if (instance != this) {
			Destroy(gameObject);
		}
	}

	void Start() {
		audioSource = GetComponent<AudioSource> ();
	}

	public void PlayClickSound() {
		if (PrefsManager.Instance.IsSoundOn) {
			audioSource.clip = clickSound;
			audioSource.Play ();
		}
	}

	public void PlayLineClearSound() {
		if (PrefsManager.Instance.IsSoundOn) {
			audioSource.clip = lineClearSound;
			audioSource.Play ();
		}
	}

	public void PlayComboSound() {
		if (PrefsManager.Instance.IsSoundOn) {
			audioSource.clip = comboSound;
			audioSource.Play ();
		}
	}

	public void PlayGameOverSound() {
		if (PrefsManager.Instance.IsSoundOn) {
			audioSource.clip = gameOverSound;
			audioSource.Play ();
		}
	}

}
























