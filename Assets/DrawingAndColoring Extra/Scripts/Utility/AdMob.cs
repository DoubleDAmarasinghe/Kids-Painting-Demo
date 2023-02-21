using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
#if GOOGLE_MOBILE_ADS
using GoogleMobileAds.Common;
using GoogleMobileAds.Api;
#endif
using UnityEngine.Events;

namespace IndieStudio.DrawingAndColoring.Utility
{
    [DisallowMultipleComponent]
    public class AdMob : MonoBehaviour
    {
#if GOOGLE_MOBILE_ADS
        private BannerView bannerView;
        private InterstitialAd interstitialAd;
        private RewardedAd rewardBasedVideoAd;
        public string androidBannerAdUnitID;
        public string IOSBannerAdUnitID;
        public string androidInterstitialAdUnitID;
        public string IOSInterstitialAdUnitID;
        public string androidRewardBasedVideoAdUnitID;
        public string IOSRewardBasedVideoAdUnitID;
        public TagForChildDirectedTreatment tagForChildDirectedTreatment = TagForChildDirectedTreatment.False;
        public TagForUnderAgeOfConsent tagForUnderAgeOfConsent = TagForUnderAgeOfConsent.False;
        public List<string> testDeviceIDS;

        void Start()
        {
            MobileAds.SetiOSAppPauseOnBackground(true);

            //testDeviceIDS.Add(AdRequest.TestDeviceSimulator);

            // Configure TagForChildDirectedTreatment and test device IDs.
            var requestConfiguration =
                new RequestConfiguration.Builder()
                //.SetTagForChildDirectedTreatment(tagForChildDirectedTreatment)
                //.SetTagForUnderAgeOfConsent(tagForUnderAgeOfConsent)
                .SetTestDeviceIds(testDeviceIDS).build();

            MobileAds.SetRequestConfiguration(requestConfiguration);

            // Initialize the Google Mobile Ads SDK.
            MobileAds.Initialize(HandleInitCompleteAction);
        }

        private void HandleInitCompleteAction(InitializationStatus initstatus)
        {
            // Callbacks from GoogleMobileAds are not guaranteed to be called on
            // main thread.
            // In this example we use MobileAdsEventExecutor to schedule these calls on
            // the next Update() loop.
            MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {
                Debug.Log("Initialization complete");

                RequestInterstitialAd();
                RequestRewardBasedVideoAd();
            });
        }

        public void RequestBannerAd(AdPosition adPostion)
        {
#if UNITY_ANDROID
                string adUnitId = androidBannerAdUnitID;
#elif UNITY_IPHONE
				string adUnitId = IOSBannerAdUnitID;
#else
				string adUnitId = "unexpected_platform";
#endif

            adUnitId = adUnitId.Trim();

            if (string.IsNullOrEmpty(adUnitId))
            {
                return;
            }

            //Destroy current banner ad, if exists
            DestroyBannerAd();

            // Create a banner
            this.bannerView = new BannerView(adUnitId, AdSize.Banner, adPostion);

            // Register for ad events.
            this.bannerView.OnAdLoaded += this.HandleBannerLoaded;
            this.bannerView.OnAdFailedToLoad += this.HandleBannerFailedToLoad;
            this.bannerView.OnAdOpening += this.HandleBannerOpened;
            this.bannerView.OnAdClosed += this.HandleBannerClosed;

            // Create an ad request.
            AdRequest request = CreateAdRequest();

            // Load the banner with the request.
            this.bannerView.LoadAd(request);

            Debug.Log("Banner Requested");
        }

        private void RequestInterstitialAd()
        {
#if UNITY_ANDROID
                        string adUnitId = androidInterstitialAdUnitID;
#elif UNITY_IPHONE
			            string adUnitId = IOSInterstitialAdUnitID;
#else
			            string adUnitId = "unexpected_platform";
#endif

            adUnitId = adUnitId.Trim();

            if (string.IsNullOrEmpty(adUnitId))
            {
                return;
            }

            //Destroy current Interstitial ad, if exists
            DestroyInterstitialAd();

            // Initialize an InterstitialAd.
            this.interstitialAd = new InterstitialAd(adUnitId);

            // Register for ad events.
            this.interstitialAd.OnAdLoaded += this.HandleInterstitialLoaded;
            this.interstitialAd.OnAdFailedToLoad += this.HandleInterstitialFailedToLoad;
            this.interstitialAd.OnAdOpening += this.HandleInterstitialOpened;
            this.interstitialAd.OnAdClosed += this.HandleInterstitialClosed;

            // Create an ad request.
            AdRequest request = CreateAdRequest();

            // Load the interstitial with the request.
            this.interstitialAd.LoadAd(request);

            Debug.Log("interstitialAd Requested");

        }

        private void RequestRewardBasedVideoAd()
        {
#if UNITY_ANDROID
                        string adUnitId = androidRewardBasedVideoAdUnitID;
#elif UNITY_IPHONE
			            string adUnitId = IOSRewardBasedVideoAdUnitID;
#else
			            string adUnitId = "unexpected_platform";
#endif

            adUnitId = adUnitId.Trim();

            if (string.IsNullOrEmpty(adUnitId))
            {
                return;
            }


            // Get singleton reward based video ad reference.
            rewardBasedVideoAd = new RewardedAd(adUnitId);

            this.rewardBasedVideoAd.OnAdLoaded += this.HandleRewardBasedVideoLoaded;
            this.rewardBasedVideoAd.OnAdFailedToLoad += this.HandleRewardBasedVideoFailedToLoad;
            this.rewardBasedVideoAd.OnAdOpening += this.HandleRewardBasedVideoOpened;
            this.rewardBasedVideoAd.OnAdFailedToShow += this.HandleRewardBasedVideoAdFailedToShow;
            this.rewardBasedVideoAd.OnAdClosed += this.HandleRewardBasedVideoClosed;
            this.rewardBasedVideoAd.OnUserEarnedReward += this.HandleRewardBasedUserEarnedReward;

            // Create an ad request.
            AdRequest request = CreateAdRequest();

            this.rewardBasedVideoAd.LoadAd(request);
        }

        // Returns an empty ad request.
        private AdRequest CreateAdRequest()
        {
            return new AdRequest.Builder()
            //.AddKeyword("unity-admob-sample")
            .Build();
        }

        private void ShowBannerAd()
        {
            if (this.bannerView == null)
            {
                return;
            }

            Debug.Log("Show bannerView");

            this.bannerView.Show();
        }

        public void ShowInterstitialAd(UnityEvent onShowAdsEvent)
        {
            if (this.interstitialAd == null)
            {
                return;
            }

            if (this.interstitialAd.IsLoaded())
            {
                if (onShowAdsEvent != null)
                    onShowAdsEvent.Invoke();
                this.interstitialAd.Show();
            }
        }

        public void ShowRewardBasedVideoAd(UnityEvent onShowAdsEvent)
        {
            if (this.rewardBasedVideoAd == null)
            {
                return;
            }

            if (rewardBasedVideoAd.IsLoaded())
            {
                if (onShowAdsEvent != null)
                    onShowAdsEvent.Invoke();
                this.rewardBasedVideoAd.Show();
            }
        }

        public void DestroyBannerAd()
        {
            if (this.bannerView == null)
            {
                return;
            }
            this.bannerView.Destroy();
        }

        private void DestroyInterstitialAd()
        {
            if (this.interstitialAd == null)
            {
                return;
            }
            this.interstitialAd.Destroy();
        }

        #region Banner callback handlers

        private void HandleBannerLoaded(object sender, EventArgs args)
        {
            ShowBannerAd();
            MonoBehaviour.print("HandleBannerLoaded event received");
        }

        private void HandleBannerFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            Debug.LogError("GetDomain=>"+args.LoadAdError.GetDomain().ToString());
            Debug.LogError("GetCode=>"+args.LoadAdError.GetCode().ToString());
            Debug.LogError("GetMessage=>"+args.LoadAdError.GetMessage().ToString());
            Debug.LogError("GetResponseInfo=>"+args.LoadAdError.GetResponseInfo().ToString());

            //(Optional)Try to request new one
            MonoBehaviour.print("HandleBannerFailedToLoad event received");
        }

        private void HandleBannerOpened(object sender, EventArgs args)
        {
            MonoBehaviour.print("HandleBannerOpened event received");
        }

        private void HandleBannerClosed(object sender, EventArgs args)
        {
            MonoBehaviour.print("HandleBannerClosed event received");
        }

        #endregion

        #region Interstitial callback handlers

        private void HandleInterstitialLoaded(object sender, EventArgs args)
        {
            MonoBehaviour.print("HandleInterstitialLoaded event received");
        }

        private void HandleInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            //(Optional)Try to request new one
            MonoBehaviour.print("HandleInterstitialFailedToLoad event received");
        }

        private void HandleInterstitialOpened(object sender, EventArgs args)
        {
            MonoBehaviour.print("HandleInterstitialOpened event received");
        }

        private void HandleInterstitialClosed(object sender, EventArgs args)
        {
            MonoBehaviour.print("HandleInterstitialClosed event received");
            RequestInterstitialAd();
        }

        private void HandleInterstitialLeftApplication(object sender, EventArgs args)
        {
            MonoBehaviour.print("HandleInterstitialLeftApplication event received");
        }

        #endregion

        #region RewardBasedVideo callback handlers

        private void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
        {
            MonoBehaviour.print("HandleRewardBasedVideoLoaded event received");
        }

        private void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            //(Optional)Try to request new one
            MonoBehaviour.print("HandleRewardBasedVideoFailedToLoad event received");
        }

        private void HandleRewardBasedVideoOpened(object sender, EventArgs args)
        {
            MonoBehaviour.print("HandleRewardBasedVideoOpened event received");
        }

        private void HandleRewardBasedVideoAdFailedToShow(object sender, EventArgs args)
        {
            MonoBehaviour.print("HandleRewardBasedVideoStarted event received");
        }

        private void HandleRewardBasedVideoClosed(object sender, EventArgs args)
        {
            RequestRewardBasedVideoAd();
            MonoBehaviour.print("HandleRewardBasedVideoClosed event received");
        }

        private void HandleRewardBasedVideoRewarded(object sender, Reward args)
        {
            string type = args.Type;
            double amount = args.Amount;
            MonoBehaviour.print("HandleRewardBasedVideoRewarded event received for " + amount.ToString() + " " + type);
        }

        private void HandleRewardBasedUserEarnedReward(object sender, EventArgs args)
        {
            MonoBehaviour.print("HandleRewardBasedVideoLeftApplication event received");
        }

        #endregion

#endif
    }
}
