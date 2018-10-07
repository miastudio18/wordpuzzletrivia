using UnityEngine;

public class RateManager : SingletonMonoBehavior<RateManager> {

    [Tooltip("Enter iTunes ID of this app (for iOS). No need to enter anything for android.")]
	public string iTunesID = "";

    protected override void Awake() {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
	}

	public void OpenRatingPage() {
		string url = "";
		#if UNITY_ANDROID
		url = "https://play.google.com/store/apps/details?id=" + Application.identifier;
		#elif UNITY_IOS
		url = "itms://itunes.apple.com/us/app/apple-store/id" + iTunesID + "?mt=8";
		#endif

		Application.OpenURL(url);
	}
}
