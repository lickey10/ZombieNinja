using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// FreezePanelEffect Class handles all of the...
/// </summary>
public class FreezeEffect : MonoBehaviour
{
    public GameObject freezeEffectGameObject;           // this is the object in the scene that has the "Freeze" spriteRenderer and Ice Border.
    public GameObject snowParticleSystemGameObject;     // this is the reference to the particle system (child object of the freezeEffectGameObject
    public SpriteRenderer[] freezeScreenSprites;        // the freeze effects sprites...
    [Header("Text Fade In/Out && Duration")]        //Label
    public float textFadeInSpeed;                       // the speed at which the "Freeze" word from the UI atlas will fade in.
    public float textFadeOutSpeed;                      // the speed at which the "Freeze" word from the UI atlas will fade out.
    public float textFreezeDuration;                    // the length of time "Freeze" word will be on screen.
    [Header("Border Fade In/Out && Duration")]      //Label
    public float borderFadeInSpeed;                     // the ice borders fade in speed
    public float borderFadeOutSpeed;                    // the ice borders fade out speed
    public float borderFreezeDuration;                  // the amount of time the ice border will be on screen
    public bool freezeEffectIsOn;                       // a bool that lets the gameController or launcher controller know that the effect is on already
    public static FreezeEffect instance;                // "Reference" to this instance, so that when we ()

    // Use this for pre-initialization
    void Awake()
    {
        //set the "instance" to this
        instance = this;
        //this script is on the freeze effect gameobject in the scene... so freezeEffectGameObject is this.gameObject.
        freezeEffectGameObject = this.gameObject;
    }

    /// <summary>
    /// This method starts the freeze effect.
    /// </summary>
    public void StartEffect()
    {
        //change the boolean to true...
        freezeEffectIsOn = true;
        //start the coroutine that fades in the first sprite(FreezeText), no delay, alpha to 1, and at the speed of "textFadeInSpeed" variable
        StartCoroutine(FadeImages(freezeScreenSprites[0], 0f, 1f, textFadeInSpeed));//text
        //Invoke the method that starts the snow particle system in "textFadeInSpeed" seconds.
        Invoke("StartSnowEffect", textFadeInSpeed);
        //invoke the method that slows down time in "textFadeInSpeed" seconds.
        Invoke("StartTimeSlowdownEffect", textFadeInSpeed);
        //Start the Coroutine that fades the Second sprite(IceBorder), no delay, alpha to 1, and at the speed of "borderFadeInSpeed" variable
        StartCoroutine(FadeImages(freezeScreenSprites[1], 0f, 1f, borderFadeInSpeed));//border
        //call endEffect method.
        EndEffect();
    }

    /// <summary>
    /// This method starts the snow particle system
    /// </summary>
    public void StartSnowEffect()
    {
        //the particle system resides on a child gameobject that is inactive so we activate it.
        snowParticleSystemGameObject.SetActive(true);
        //we invoke the method that ends the snow effect in "borderFreezeDuration" seconds plus 1 extra second...
        Invoke("EndSnowEffect", borderFreezeDuration + 1f);
        //we invoke the method that ends the slow down time effect in "borderFreezeDuration" seconds plus 1 extra second...
        Invoke("EndTimeSlowDownEffect", borderFreezeDuration + 1f);
    }

    /// <summary>
    /// This is the method that slows down time for the freeze effect.
    /// </summary>
    public void StartTimeSlowdownEffect()
    {
        //we Set Time.timeScale to 0.5f so we are at half speed.
        Time.timeScale = 0.5f;
    }

    
    /// <summary>
    /// This is the method that ends the Freeze Effect.  It starts coroutines to fade the sprites back to an alpha of zero.
    /// </summary>
    public void EndEffect()
    {
        //this time the coroutines are started for sprite 0, and 1 simultaneously, but with a delay before starting the fade.
        //after the delay is over..  we fade them to 0f, at the rate of their individual fadeOutTimes.
        StartCoroutine(FadeImages(freezeScreenSprites[0], textFreezeDuration, 0f, textFadeOutSpeed));//text
        StartCoroutine(FadeImages(freezeScreenSprites[1], borderFreezeDuration, 0f, borderFadeOutSpeed));//border
    }


    /// <summary>
    /// Method to end snow effect.
    /// </summary>
    public void EndSnowEffect()
    {
        //we change the gameObject the particle system is attached to back to inactive.
        snowParticleSystemGameObject.SetActive(false);
    }


    /// <summary>
    /// This method ends the time slow down.
    /// </summary>
    public void EndTimeSlowDownEffect()
    {
        //we set the Time.timeScale back to 1f
        Time.timeScale = 1.0f;
        //and we change the boolean freezeEffectIsOn to false.
        freezeEffectIsOn = false;
    }


    /// <summary>
    /// The Fade Images Method takes a SpriteRenderer, a delay, a value to fade to (0 to 1), and a time to do
    /// it in.  This is just like the fade method in several other Classes.  Just slightly modified.
    /// </summary>
    /// <param name="img"></param>
    /// <param name="initFadeDelay"></param>
    /// <param name="aValue"></param>
    /// <param name="aTime"></param>
    /// <returns></returns>
    IEnumerator FadeImages(SpriteRenderer img, float initFadeDelay, float aValue, float aTime)
    {
        //we wait the amount of the initFadeDelay
        yield return new WaitForSeconds(initFadeDelay);
        //we create a new Color named tempColor and assign it the passed img's color.
        Color tempColor = img.color;
        //we create a float and name it alpha we assign it the value of the tempColors alpha.
        float alpha = tempColor.a;
        //we loop from 0 to 1 at the rate of Time.deltaTime divided by aTime...
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            //every iteration we Mathf.Lerp the alpha and aValue(we called with), by t which is increasing to 1.  Then we assign that value to tempColor alpha
            tempColor.a = Mathf.Lerp(alpha, aValue, t);
            //img.color get assigned or new tempColor
            img.color = tempColor;
            //as a precaution (due to some experiences on lesser mobile devices) once the alpha is 1/10 of the way down we just set it to 0.. Ive seen some
            //phones that had a slight remainder after the lerp was finished... I could not deduce why, but something faltered towards the end of the loop.
            //Just setting it manually to 0, then that is the plan.  I also had to do this a Method that was fading the alpha on a canvas group.
            if(img.color.a <= 0.1f)
            {
                //tempColors alpha gets assigned 0f
                tempColor.a = 0f;
                //img.color is assigned tempColor (for the few that may be learning... the "Color" includes the alpha in this case. R,G,B,A
                img.color = tempColor;
            }
            //yield return null... we are done.
            yield return null;
        }
    }

}