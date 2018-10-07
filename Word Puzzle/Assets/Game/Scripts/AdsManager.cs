using System;
using UnityEngine;

using GoogleMobileAds.Api;

public class AdsManager : SingletonMonoBehavior<AdsManager> {

    [SerializeField] private string androidAppId;
    [SerializeField] private string androidBannerId;
    [SerializeField] private string androidInterstitialId;
    [SerializeField] private string androidRewardedAdIdDump;
    [SerializeField] private string androidRewardedAdIdDestroy;
    [SerializeField] private string androidRewardedAdIdSwap;

    [SerializeField] private string iosAppId;
    [SerializeField] private string iosBannerId;
    [SerializeField] private string iosInterstitialId;
    [SerializeField] private string iosRewardedAdIdDump;
    [SerializeField] private string iosRewardedAdIdDestroy;
    [SerializeField] private string iosRewardedAdIdSwap;

    [SerializeField] private uint showInterstitialAfter = 1;

    [SerializeField] private string dumpRewardType;
    [SerializeField] private string destroyRewardType;
    [SerializeField] private string swapRewardType;

    private BannerView bannerView;
    private InterstitialAd interstitial;
    private RewardBasedVideoAd rewardedAd;

    private Action RewardedAdCompleted;

    private uint interstitialCounter = 0;

    protected override void Awake() {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);

        Initialize();

        RequestBanner();
    }

    void Start() {
        RequestInterstitial();

        rewardedAd = RewardBasedVideoAd.Instance;
        RegisterRewardedAdEvents();
        RequestRewardedAd();
    }

    private void Initialize() {
        #if UNITY_ANDROID
        string appId = androidAppId;
        #elif UNITY_IPHONE
		string appId = iosAppId;
        #else
		string appId = "unexpected_platform";
        #endif

        MobileAds.Initialize(appId);
    }

    private void RequestBanner() {
        #if UNITY_ANDROID
        string adUnitId = androidBannerId;
        #elif UNITY_IPHONE
		string adUnitId = iosBannerId;
        #else
		string adUnitId = "unexpected_platform";
        #endif

        if (bannerView != null) {
            DestroyBanner();
        }

        bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        bannerView.LoadAd(request);

        HideBanner();
    }

    private void RequestInterstitial() {
        #if UNITY_ANDROID
        string adUnitId = androidInterstitialId;
        #elif UNITY_IPHONE
		string adUnitId = iosInterstitialId;
        #else
		string adUnitId = "unexpected_platform";
        #endif

        if (interstitial != null) {
            DestroyInterstitial();
        }

        interstitial = new InterstitialAd(adUnitId);
        RegisterInterstitialEvents();

        AdRequest request = new AdRequest.Builder().Build();

        interstitial.LoadAd(request);
    }

    private void RequestRewardedAd() {
        string adUnitId = "unexpected_platform";

        float randomValue = UnityEngine.Random.value;

        if(randomValue <= 0.6f) {
            #if UNITY_ANDROID
            adUnitId = androidRewardedAdIdDump;
            #elif UNITY_IPHONE
		    adUnitId = iosRewardedAdIdDump;

            #endif
        }
        else if(randomValue <= 0.8f) {
            #if UNITY_ANDROID
            adUnitId = androidRewardedAdIdDestroy;
            #elif UNITY_IPHONE
		    adUnitId = iosRewardedAdIdDestroy;

            #endif
        }
        else {
            #if UNITY_ANDROID
            adUnitId = androidRewardedAdIdSwap;
            #elif UNITY_IPHONE
		    adUnitId = iosRewardedAdIdSwap;

            #endif
        }

        AdRequest request = new AdRequest.Builder().Build();
        rewardedAd.LoadAd(request, adUnitId);
    }

    public void ShowBanner() {
        bannerView.Show();
    }

    public void HideBanner() {
        bannerView.Hide();
    }

    public bool IsInterstitialAvailable() {
        return interstitial.IsLoaded();
    }

    public void ShowInterstitial() {
        interstitialCounter++;

        if(interstitialCounter < showInterstitialAfter) {
            return;
        }

        if (interstitial.IsLoaded()) {
            interstitial.Show();

            interstitialCounter = 0;
        }
    }

    public bool IsRewardedAdAvailable() {
        return rewardedAd.IsLoaded();
    }

    public void ShowRewardedAd(Action completedCallback = null) {
        if (rewardedAd.IsLoaded()) {
            this.RewardedAdCompleted = completedCallback;

            rewardedAd.Show();
        }
        else {
            string heading = "Video Unavailable";
            string msg = "No video available at the moment." +
                "\nPlease try later.";

            PopupManager.Instance.ShowPopup(heading, msg, "OK", "",
                () => {

                    AudioManager.Instance.PlayClickSound();
                    PopupManager.Instance.HidePopup();

                },
                null, null);
        }
    }

    private void DestroyBanner() {
        if (bannerView != null) {
            bannerView.Destroy();
        }
    }

    private void DestroyInterstitial() {
        if (interstitial != null) {
            interstitial.Destroy();
        }
    }

    private void RegisterInterstitialEvents() {
        // Called when an ad request has successfully loaded.
        interstitial.OnAdLoaded += HandleOnAdLoaded;
        // Called when an ad request failed to load.
        interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is shown.
        interstitial.OnAdOpening += HandleOnAdOpened;
        // Called when the ad is closed.
        interstitial.OnAdClosed += HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplication;
    }

    private void RegisterRewardedAdEvents() {
        // Called when an ad request has successfully loaded.
        rewardedAd.OnAdLoaded += HandleRewardBasedVideoLoaded;
        // Called when an ad request failed to load.
        rewardedAd.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoad;
        // Called when an ad is shown.
        rewardedAd.OnAdOpening += HandleRewardBasedVideoOpened;
        // Called when the ad starts to play.
        rewardedAd.OnAdStarted += HandleRewardBasedVideoStarted;
        // Called when the user should be rewarded for watching a video.
        rewardedAd.OnAdRewarded += HandleRewardBasedVideoRewarded;
        // Called when the ad is closed.
        rewardedAd.OnAdClosed += HandleRewardBasedVideoClosed;
        // Called when the ad click caused the user to leave the application.
        rewardedAd.OnAdLeavingApplication += HandleRewardBasedVideoLeftApplication;
    }

    // Interstitial Ad Events
    public void HandleOnAdLoaded(object sender, EventArgs args) {

    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args) {
        RequestInterstitial();
    }

    public void HandleOnAdOpened(object sender, EventArgs args) {

    }

    public void HandleOnAdClosed(object sender, EventArgs args) {
        RequestInterstitial();
    }

    public void HandleOnAdLeavingApplication(object sender, EventArgs args) {

    }

    // Rewarded Ad Events
    public void HandleRewardBasedVideoLoaded(object sender, EventArgs args) {

    }

    public void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args) {
        RequestRewardedAd();
    }

    public void HandleRewardBasedVideoOpened(object sender, EventArgs args) {

    }

    public void HandleRewardBasedVideoStarted(object sender, EventArgs args) {

    }

    public void HandleRewardBasedVideoClosed(object sender, EventArgs args) {
        RequestRewardedAd();
    }

    public void HandleRewardBasedVideoRewarded(object sender, Reward args) {
        string rewardType = args.Type;
        int rewardAmount = (int)args.Amount;

        string heading = "Congratulations";
        string msg = "";

        if (rewardType.Equals(dumpRewardType)) {
            PowerupManager.Instance.destroyLetterUses += rewardAmount;
            msg = "You have earned " + rewardAmount + " Dump power-up(s).";
        }
        else if(rewardType.Equals(destroyRewardType)) {
            PowerupManager.Instance.destroyLineUses += rewardAmount;
            msg = "You have earned " + rewardAmount + " Destroy power-up(s).";
        }
        else if(rewardType.Equals(swapRewardType)) {
            PowerupManager.Instance.summonLetterUses += rewardAmount;
            msg = "You have earned " + rewardAmount + " Swap power-up(s).";
        }

        PopupManager.Instance.ShowPopup(heading, msg, "OK", "",
            () => {

                AudioManager.Instance.PlayClickSound();
                PopupManager.Instance.HidePopup();

            }
            , null, null);

        if(RewardedAdCompleted != null) {
            RewardedAdCompleted();
        }
    }

    public void HandleRewardBasedVideoLeftApplication(object sender, EventArgs args) {

    }

}
