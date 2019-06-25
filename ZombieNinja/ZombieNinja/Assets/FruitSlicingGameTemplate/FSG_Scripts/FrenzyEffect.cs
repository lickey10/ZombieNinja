using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The FrenzyEffect Class (just like the FreezeEffect, and TwoTimesScoreEffect Classes) Handles the Frenzy Effect.  It is responsible
/// for changing the background colors, starting a particle system, and showing the "Frenzy" Text from the UI Atlas.
/// </summary>
public class FrenzyEffect : MonoBehaviour
{
    public GameObject frenzyEffectGameObject;                   // this is the reference to our FrenzyEffectGameObject(this object.. the scene object)
    public GameObject frenzyParticleSystem;                     // the particle system attached to a child of this gameobject.

    public Color[] randomBgColorsForFrenzyMode;                 // an array of random colors to change the BackGround color while the effect is on.
    public Color startAndEndColor;                              // This is the color that will hold the start and end colors.  So that we can return after all the color changes
    public SpriteRenderer[] frenzyScreenSprites;                // the sprites that are activate as soon as the effect is activated.  The "Frenzy" word on the UI Atlas

    public SpriteRenderer changingColorSprite;                  // the sprite renderer that will be changing colors

    [Header("Text Fade In/Out && Duration")]
    public float textFadeInSpeed;                               //the fade in speed of the word "Frenzy"
    public float textFadeOutSpeed;                              //the fade out speed of the word "Frenzy"
    public float textFreezeDuration;                            //the duration of the word "Frenzy"

    [Header("Border Fade In/Out && Duration")]
    public float psDuration;                                    //the duration of the particle system(the stars flowing from the edges)
    public bool frenzyEffectIsOn;                               // boolean that is true when the Frenzy effect is on.

    private int backgroundColorCount;                           // integer we use to cycle through the random background colors
    public int loopThruColorAmt;                                // the number of times to loop through colors (for loop)


    // Use this for pre-initialization
    void Awake()
    {
        //frenzyEffectGameObject reference is THIS.gameObject
        frenzyEffectGameObject = this.gameObject;
    }


    /// <summary>
    /// The StartEffect() method starts the Frenzy Effect.
    /// </summary>
    public void StartEffect()
    {
        //set boolean to true
        frenzyEffectIsOn = true;
        //assign the startAndEndColor to changingColorSprite.color
        changingColorSprite.color = startAndEndColor;
        //Start Coroutine( TweenSpriteColor( the amount of time to wait before starting... .05f ))
        StartCoroutine(TweenSpriteColor(.05f));
        //Start Coroutine FadeImages  (the image to fade, delay amt, what alpha to fade to, and what speed to fade at.)
        StartCoroutine(FadeImages(frenzyScreenSprites[0], 0f, 1f, textFadeInSpeed));//text
        //Invoke StartFrenzyParticleEffect in "textFadeInSpeed"
        Invoke("StartFrenzyParticleEffect", textFadeInSpeed);
        //call the EndEffect() method
        EndEffect();
    }


    /// <summary>
    /// The StartFrenzyParticleEffect method starts the star particle system.
    /// </summary>
    public void StartFrenzyParticleEffect()
    {
        //set the gameObject that has the particle system on it active
        frenzyParticleSystem.SetActive(true);
        //Invoke EndFrenzyParticleEffect in "psDuration" + "textFreezeDuration" + 1f seconds.
        Invoke("EndFrenzyParticleEffect", psDuration + textFreezeDuration + 1f);
    }


    /// <summary>
    /// the EndEffect method will.  This method is called to end the effect, and do this method
    /// calls start coroutine (FadeImages), and fades the "Frenzy" text back out..
    /// </summary>
    public void EndEffect()
    {
        //Start Coroutine FadeImages  (the image to fade, delay amt, what alpha to fade to, and what speed to fade at.)
        StartCoroutine(FadeImages(frenzyScreenSprites[0], textFreezeDuration, 0f, textFadeOutSpeed));//text
    }


    /// <summary>
    /// The EndFrenzyParticleEffect method turns the star particle system off.
    /// </summary>
    public void EndFrenzyParticleEffect()
    {
        //turn frenzyEffectIsOn boolean back to false
        frenzyEffectIsOn = false;
        //lets turn the gameobject that the particle system is attached to back to inactive.
        frenzyParticleSystem.SetActive(false);
    }


    /// <summary>
    /// This method rapidly changes the color of the background.
    /// </summary>
    /// <param name="waitTime"></param>
    /// <returns>null...</returns>
    IEnumerator TweenSpriteColor(float waitTime)
    {
        //we loop through "loopThruColorAmt" (about 130) and we change the color every iteration, and because of the
        //number of "loopThruColorAmt" each color will be used about 10 times.
        for (int i = 0; i < loopThruColorAmt; i++)
        {
            //we wait to begin.  "waitTime" in seconds.
            yield return new WaitForSeconds(waitTime);

            //If backgroundColorCount is greater than randomBgColorForFrenzyMode.length -1
            if(backgroundColorCount > randomBgColorsForFrenzyMode.Length - 1)
            {
                //if it is, then we need to set the backgroundColorCount back to zero
                backgroundColorCount = 0;
            }

            //changingColorSprite.color is assigned the color of the "backgroundColorCount"# element in the randomBgColorsForFrenzyMode array.
            changingColorSprite.color = randomBgColorsForFrenzyMode[backgroundColorCount];
            //backgroundColorCount is incremented each iteration.
            backgroundColorCount++;
            //yield return new... 
            yield return null;
        }
        //finally after the loop the "changingColorSprite"'s "color" is assigned the startAndEndColor... (which is what we started with).
        changingColorSprite.color = startAndEndColor;


    }



    /// <summary>
    /// This is another Fade Method... 
    /// </summary>
    /// <remarks>
    /// Gosh I should have made a real verbose semi-generic, or at least multi-Type Static Extension Method.  I came back 
    /// through to fulfill about 65% of the comments(after the project was finished, and I am only now realizing how many versions
    /// of the same Method there are. Sorry peeps.)
    /// </remarks>>
    /// <param name="img"></param>
    /// <param name="initFadeDelay"></param>
    /// <param name="aValue"></param>
    /// <param name="aTime"></param>
    /// <returns>null...</returns>
    IEnumerator FadeImages(SpriteRenderer img, float initFadeDelay, float aValue, float aTime)
    {
        //wait for this amount of time... initFadeDelay
        yield return new WaitForSeconds(initFadeDelay);

        //this makes the frenzy fruit come pouring out of the sides of the level.  The link below access the LaunchController Instance and requests
        //a salvo from the side launchers. 
        LauncherController.LaunchControllerInstance.RequestFruitSalvoFromSide(LauncherController.LaunchControllerInstance.sideLauncherSalvoAmount);

        //we create a new Color, and Float as usual...
        Color tempColor = img.color;
        float alpha = tempColor.a;
        //we for loop from 0 to 1 at the rate of Time.deltaTime divided by aTime.
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            //we lerp from start no alpha, to the alpha we need, by t.
            tempColor.a = Mathf.Lerp(alpha, aValue, t);

            //img.color get assigned or new tempColor
            img.color = tempColor;

            //as a precaution (due to some experiences on lesser mobile devices) once the alpha is 1/10 of the way down we just set it to 0
            if (img.color.a <= 0.1f)
            {
                //tempColors alpha gets assigned 0f
                tempColor.a = 0f;
                //img.color is assigned tempColor
                img.color = tempColor;
            }
            //yield return null.
            yield return null;
        }
    }

}