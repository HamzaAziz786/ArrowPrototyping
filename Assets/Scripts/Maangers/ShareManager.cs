using UnityEngine;
using UnityEngine.SceneManagement;

public class ShareManager : MonoBehaviour
{
    [Header("Share Content")]
    [TextArea(2, 5)]
    public string shareMessage = "🎮 Check out this awesome game I’m playing! Download now: ";
    public string gameLink = "https://play.google.com/store/apps/dev?id=6508553873090680206"; // your Play Store link

    public void ShareGame()
    {
        SoundManager.Instance.PlayButton();
#if UNITY_ANDROID
        string fullMessage = shareMessage + "\n" + gameLink;

        AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
        AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");

        intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
        intentObject.Call<AndroidJavaObject>("setType", "text/plain");
        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), "My Game");
        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), fullMessage);

        AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");

        AndroidJavaObject chooser = intentClass.CallStatic<AndroidJavaObject>(
            "createChooser", intentObject, "Share via"
        );

        currentActivity.Call("startActivity", chooser);
#elif UNITY_IOS
        // You can use native iOS sharing plugin or Unity's ShareSheet wrapper
        Debug.Log("iOS share feature coming soon...");
#else
        Debug.Log("Sharing not supported on this platform.");
#endif
    }

    public void Gameplay()
    {
        SceneManager.LoadScene(1);
    }
}
