using UnityEngine;
using System.Collections;

/// <summary>
/// The TwoTimesScoreEffect Class handles the activation of the SpriteRenderer that uses the "2xScore" text from
/// the UI Atlas.  It is responsible for activating the effect, reporting that "twoTimesScoreEffectIs On", and
/// stopping the effect when the countdown has completed.
/// </summary>
public class TwoTimesScoreEffect : MonoBehaviour
{
    public bool twoTimesScoreIsOn;                      // the boolean that returns true if the effect is active.
    public GameObject twoTimesScoreText;                // the GameObject that contains child sprite-renderers that contain the ui text.


    // Use this for pre-initialization
    void Awake()
    {
        //set the effect text inactive.
        twoTimesScoreText.SetActive(false);
    }

    /// <summary>
    /// This method changes the boolean "twoTimesScoreIsOn" to true, activates the text, and Starts a coroutine that
    /// will disable the boolean, and the text in 10 seconds time.
    /// </summary>
    public void StartEffect()
    {
        //change the "twoTimesScoreIsOn" boolean to true;
        twoTimesScoreIsOn = true;
        //activate the gameobject with the child sprite renderers;
        twoTimesScoreText.SetActive(true);
        //start coroutine that disables we just enabled... but in 10 seconds.
        StartCoroutine(TwoTimesScore(10f));

    }

    /// <summary>
    /// This Method is called to disable the TwoTimesScoreEffect in (waitTime)Seconds.
    /// </summary>
    /// <param name="waitTime"></param>
    /// <returns></returns>
    IEnumerator TwoTimesScore(float waitTime)
    {
        //waitForSeconds(wait time) before starting.
        yield return new WaitForSeconds(waitTime);
        //gameObject set active false.
        twoTimesScoreText.SetActive(false);
        //twoTimesScoreIsOn boolean gets changed back to false;
        twoTimesScoreIsOn = false;
        //yield return null... we are done.
        yield return null;
    }
}
