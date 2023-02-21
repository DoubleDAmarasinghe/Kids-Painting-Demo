using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_ADS
using UnityEngine.Advertisements;
#endif

///Developed by Indie Studio
///https://assetstore.unity.com/publishers/9268
///www.indiestd.com
///info@indiestd.com

namespace IndieStudio.DrawingAndColoring.Utility
{

#if UNITY_ADS
    [DisallowMultipleComponent]
    public class UnityAd : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener, IUnityAdsInitializationListener
    {
        [SerializeField] string _androidGameId;
        [SerializeField] string _iOSGameId;
        private string _gameId;


        [SerializeField] string _androidBannerAdUnitId = "Android_Banner";
        [SerializeField] string _iOSBannerAdUnitId = "iOS_Banner";
        string _bannerAdUnitId = null; // This will remain null for unsupported platforms.

        [SerializeField] string _androidInterstitialAdUnitId = "Android_Interstitial";
        [SerializeField] string _iOSInterstitialAdUnitId = "iOS_Interstitial";
        string _interstitialAdUnitId;

        [SerializeField] string _androidRewardedAdUnitId = "Android_Rewarded";
        [SerializeField] string _iOSRewardedAdUnitId = "iOS_Rewarded";
        string _rewardedAdUnitId = null; // This will remain null for unsupported platforms.


        /// <summary>
        /// Enable test mode or not.
        /// </summary>
        public bool testMode;


        private UnityEvent onShowInterstitialAdsCompletedEvent;
        private UnityEvent onShowRewardedAdsCompletedEvent;



        void Awake()
        {
            InitializeAds();
        }

        public void InitializeAds()
        {
            // Get the Ad Unit ID for the current platform:
            _gameId = (Application.platform == RuntimePlatform.IPhonePlayer)
               ? _iOSGameId
               : _androidGameId;

            _gameId = _gameId?.Trim();


            _bannerAdUnitId = (Application.platform == RuntimePlatform.IPhonePlayer)
                ? _iOSBannerAdUnitId
                : _androidBannerAdUnitId;

            _bannerAdUnitId = _bannerAdUnitId?.Trim();

            _interstitialAdUnitId = (Application.platform == RuntimePlatform.IPhonePlayer)
                ? _iOSInterstitialAdUnitId
                : _androidInterstitialAdUnitId;

            _interstitialAdUnitId = _interstitialAdUnitId?.Trim();

            _rewardedAdUnitId = (Application.platform == RuntimePlatform.IPhonePlayer)
            ? _iOSRewardedAdUnitId
            : _androidRewardedAdUnitId;

            _rewardedAdUnitId = _rewardedAdUnitId?.Trim();

            Advertisement.Initialize(_gameId, testMode, this);
        }


        // Implement a method to call when the Load Banner button is clicked:
        public void LoadBanner(BannerPosition _bannerPosition)
        {
            StartCoroutine(LoadBannerCoroutine(_bannerPosition));
        }

        private IEnumerator LoadBannerCoroutine(BannerPosition _bannerPosition)
        {

            while (!Advertisement.isInitialized)
            {
                yield return new WaitForSeconds(0.1f);
            }

            // Set up options to notify the SDK of load events:
            BannerLoadOptions options = new BannerLoadOptions
            {
                loadCallback = OnBannerLoaded,
                errorCallback = OnBannerError
            };

            Advertisement.Banner.SetPosition(_bannerPosition);

            // Load the Ad Unit with banner content:
            Advertisement.Banner.Load(_bannerAdUnitId, options);


        }

        // Implement a method to call when the Show Banner button is clicked:
        private void ShowBannerAd()
        {


            // Set up options to notify the SDK of show events:
            BannerOptions options = new BannerOptions
            {
                clickCallback = OnBannerClicked,
                hideCallback = OnBannerHidden,
                showCallback = OnBannerShown
            };

            // Show the loaded Banner Ad Unit:
            Advertisement.Banner.Show(_bannerAdUnitId, options);


        }

        // Implement a method to call when the Hide Banner button is clicked:
        public void HideBannerAd()
        {
            if (!Advertisement.isInitialized)
                return;

            // Hide the banner:
            Advertisement.Banner.Hide();
        }

        public void LoadInterstitialAd(UnityEvent onShowInterstitialAdsCompletedEvent = null)
        {
            this.onShowInterstitialAdsCompletedEvent = onShowInterstitialAdsCompletedEvent;

            StartCoroutine(LoadInterstitialAdCoroutine());
        }

        private IEnumerator LoadInterstitialAdCoroutine()
        {

            while (!Advertisement.isInitialized)
            {
                yield return new WaitForSeconds(0.1f);
            }

            Advertisement.Load(_interstitialAdUnitId, this);
        }

        public void LoadRewardedAd(UnityEvent onShowRewardedAdsCompletedEvent)
        {
            if (onShowRewardedAdsCompletedEvent == null)
            {
                //Rewarded Ad must be shown with an Event
                Debug.LogWarning("RewardedAd must be called with UnityEvent");

                return;
            }

            this.onShowRewardedAdsCompletedEvent = onShowRewardedAdsCompletedEvent;

            StartCoroutine(LoadRewardedAdCoroutine());
        }


        private IEnumerator LoadRewardedAdCoroutine()
        {

            while (!Advertisement.isInitialized)
            {
                yield return new WaitForSeconds(0.1f);
            }

            Advertisement.Load(_rewardedAdUnitId, this);
        }

        public void OnInitializationComplete()
        {
            Debug.Log("Unity Ads initialization complete.");
        }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
        }

        public void OnBannerClicked()
        {

        }

        public void OnBannerShown()
        {

        }

        public void OnBannerHidden()
        {

        }

        // Implement code to execute when the loadCallback event triggers:
        public void OnBannerLoaded()
        {
            Debug.Log("Banner loaded");

            ShowBannerAd();
        }

        // Implement code to execute when the load errorCallback event triggers:
        public void OnBannerError(string message)
        {
            Debug.Log($"Banner Error: {message}");
            // Optionally execute additional code, such as attempting to load another ad.
        }


        // Implement Load Listener and Show Listener interface methods: 
        public void OnUnityAdsAdLoaded(string adUnitId)
        {
            Debug.Log(" Ad Loaded : " + adUnitId);

            Advertisement.Show(adUnitId, this);
        }

        public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
        {
            Debug.Log($"Error loading Ad Unit: {adUnitId} - {error.ToString()} - {message}");
            // Optionally execute code if the Ad Unit fails to load, such as attempting to try again.
        }

        public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
        {
            Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
            // Optionally execute code if the Ad Unit fails to show, such as loading another ad.
        }

        public void OnUnityAdsShowStart(string adUnitId)
        {

        }

        public void OnUnityAdsShowClick(string adUnitId)
        {

        }

        public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
        {

            if (adUnitId == _rewardedAdUnitId && showCompletionState == UnityAdsShowCompletionState.COMPLETED)
            {
                Debug.Log("Unity Ads Rewarded Ad Completed");

                onShowRewardedAdsCompletedEvent?.Invoke();
            }
            else if (adUnitId == _interstitialAdUnitId & (showCompletionState == UnityAdsShowCompletionState.COMPLETED || showCompletionState == UnityAdsShowCompletionState.SKIPPED))
            {
                Debug.Log("Unity Ads Interstitial Ad Completed");

                onShowInterstitialAdsCompletedEvent?.Invoke();
            }
        }

    }
#else
    [DisallowMultipleComponent]
    public class UnityAd : MonoBehaviour
    {

    }
#endif

}
