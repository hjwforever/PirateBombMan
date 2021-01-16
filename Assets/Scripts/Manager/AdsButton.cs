using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

[RequireComponent (typeof (Button))]
public class AdsButton : MonoBehaviour, IUnityAdsListener
{
    #if UNITY_IOS
        private string gameId = "3969290";
    #elif UNITY_ANDROID
    private string gameId = "3969291";
#endif

    Button AdButton;
    public string myPlacementId = "rewardedVideo";

    void Start()
    {
        AdButton = GetComponent<Button>();

        // Set interactivity to be dependent on the Placement’s status:
       // AdsButton.interactable = Advertisement.IsReady(myPlacementId);

        // Map the ShowRewardedVideo function to the button’s click listener:
        if (AdButton) AdButton.onClick.AddListener(ShowRewardedVideo);

        // Initialize the Ads listener and service:
        Advertisement.AddListener(this);
        Advertisement.Initialize(gameId, true);
    }

    // Implement a function for showing a rewarded video ad:
    void ShowRewardedVideo()
    {
        Advertisement.Show(myPlacementId);
    }

    public void OnUnityAdsReady(string placementId)
    {
        // Debug.Log("广告就绪");
        if (placementId == myPlacementId)
        {
            AdButton.interactable = true;
        }
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        // Debug.Log("广告开始播放");
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        switch (showResult)
        {
            case ShowResult.Failed:
                // Debug.LogWarning("The ad did not finish due to an error.");
                break;
            case ShowResult.Skipped:
                // Debug.Log("广告被跳过，无奖励");
                break;
            case ShowResult.Finished:
                // Debug.Log("广告播放完成，给奖励");
                {
                    FindObjectOfType<PlayerController>().Revive(); // 玩家复活
                    break;
                }
            default:
                break;
        }
    }

    public void OnUnityAdsDidError(string message)
    {
        // Debug.Log("播放广告出错");
    }
}
