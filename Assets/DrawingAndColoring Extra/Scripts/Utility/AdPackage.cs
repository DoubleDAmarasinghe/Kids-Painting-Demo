using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

///Developed by Indie Studio
///https://assetstore.unity.com/publishers/9268
///www.indiestd.com
///info@indiestd.com

namespace IndieStudio.DrawingAndColoring.Utility
{
    [Serializable]
    public class AdPackage
    {
        public bool isEnabled = true;
        public List<AdEvent> adEvents = new List<AdEvent>();
        public Package package;

        [Serializable]
        public class AdEvent
        {
            public Event evt;
            public Type type = Type.BANNER;
            public BannerPosition adPostion = BannerPosition.Bottom;

            public bool isEnabled = false;

            public enum Event
            {
                ON_SHOW_TRASH_DIALOG,
                ON_LOAD_ALBUM_SCENE,
                ON_LOAD_GAME_SCENE,
            }

            public enum Type
            {
                BANNER,
                INTERSTITIAL,
                RewardBasedVideo,
            }

            public enum BannerPosition
            {
                Top = 0,
                Bottom = 1,
                TopLeft = 2,
                TopRight = 3,
                BottomLeft = 4,
                BottomRight = 5,
                Center = 6,
            }
        }

        public enum Package
        {
            ADMOB,
            CHARTBOOST,
            UNITY
        }

        /// <summary>
        /// Build the ad events.
        /// </summary>
        public void BuildAdEvents()
        {
            Array eventsEnum = Enum.GetValues(typeof(AdEvent.Event));

            if (NeedsToResetAdEventsList(eventsEnum, adEvents))
            {
                adEvents.Clear();
            }

            foreach (AdEvent.Event e in eventsEnum)
            {
                if (!InAdEventsList(adEvents, e))
                {
                    adEvents.Add(new AdEvent() { evt = e });
                }
            }
        }

        /// <summary>
        /// Whether the given event in the adEvents list.
        /// </summary>
        /// <returns><c>true</c>, if evt was found, <c>false</c> otherwise.</returns>
        /// <param name="adEvents">Ad events.</param>
        /// <param name="evt">Evt.</param>
        public bool InAdEventsList(List<AdEvent> adEvents, AdEvent.Event evt)
        {
            if (adEvents == null)
            {
                return false;
            }

            foreach (AdEvent adEvent in adEvents)
            {
                if (adEvent.evt == evt)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Whether to reset ad events list or not.
        /// </summary>
        /// <returns><c>true</c>, if reset ad events list was needed, <c>false</c> otherwise.</returns>
        /// <param name="eventsEnum">Events enum.</param>
        /// <param name="adEvents">Ad events.</param>
        public bool NeedsToResetAdEventsList(Array eventsEnum, List<AdEvent> adEvents)
        {
            if (eventsEnum.Length != adEvents.Count)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Convert Banner Position to Admob Position.
        /// </summary>
        /// <returns>The banner position of Admob.</returns>
        /// <param name="bannerPosition">Banner position.</param>
#if GOOGLE_MOBILE_ADS
        public static GoogleMobileAds.Api.AdPosition BannerPositionToAdmobPosition(AdEvent.BannerPosition bannerPosition)
        {
            if (bannerPosition == AdEvent.BannerPosition.Top)
            {
                return GoogleMobileAds.Api.AdPosition.Top;
            }
            else if (bannerPosition == AdEvent.BannerPosition.TopLeft)
            {
                return GoogleMobileAds.Api.AdPosition.TopLeft;
            }
            else if (bannerPosition == AdEvent.BannerPosition.TopRight)
            {
                return GoogleMobileAds.Api.AdPosition.TopRight;
            }
            else if (bannerPosition == AdEvent.BannerPosition.Bottom)
            {
                return GoogleMobileAds.Api.AdPosition.Bottom;
            }
            else if (bannerPosition == AdEvent.BannerPosition.BottomLeft)
            {
                return GoogleMobileAds.Api.AdPosition.BottomLeft;
            }
            else if (bannerPosition == AdEvent.BannerPosition.BottomRight)
            {
                return GoogleMobileAds.Api.AdPosition.BottomRight;
            }
            else if (bannerPosition == AdEvent.BannerPosition.Center)
            {
                return GoogleMobileAds.Api.AdPosition.Center;
            }

            return default;

        }
#endif


#if UNITY_ADS
        public static UnityEngine.Advertisements.BannerPosition BannerPositionToUnityPosition(AdEvent.BannerPosition bannerPosition)
        {
            if (bannerPosition == AdEvent.BannerPosition.Top)
            {
                return UnityEngine.Advertisements.BannerPosition.TOP_CENTER;
            }
            else if (bannerPosition == AdEvent.BannerPosition.TopLeft)
            {
                return UnityEngine.Advertisements.BannerPosition.TOP_LEFT;
            }
            else if (bannerPosition == AdEvent.BannerPosition.TopRight)
            {
                return UnityEngine.Advertisements.BannerPosition.TOP_RIGHT;
            }
            else if (bannerPosition == AdEvent.BannerPosition.Bottom)
            {
                return UnityEngine.Advertisements.BannerPosition.BOTTOM_CENTER;
            }
            else if (bannerPosition == AdEvent.BannerPosition.BottomLeft)
            {
                return UnityEngine.Advertisements.BannerPosition.BOTTOM_LEFT;
            }
            else if (bannerPosition == AdEvent.BannerPosition.BottomRight)
            {
                return UnityEngine.Advertisements.BannerPosition.BOTTOM_RIGHT;
            }
            else if (bannerPosition == AdEvent.BannerPosition.Center)
            {
                return UnityEngine.Advertisements.BannerPosition.CENTER;
            }

            return default;
        }
#endif
    }
}

