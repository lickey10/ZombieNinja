using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// FruitDestroyCombo Class handles the Visibility,Tween, and Count of the Destroyed Fruit Combo Display.
/// </summary>
public class FruitDestroyCombo : MonoBehaviour
{

    //public float timeSinceFruitContact;                                       // how long its been since we hit a fruit
    public float comboAmountOfTime = 0.5f;                                      // the amount of time you have to swipe at least 3 fruit in to get a combo
    public Text fruitComboNum;                                                  //the text component we would be feeding the combo number if we don't use sprites
    private CanvasGroup comboTweenCanvasGroup;                                  // the canvas group reference so we can use it to fade alpha
    private Animator comboAnim;                                                 // animator reference for the "combo text tween"
    private int tweenHash = Animator.StringToHash(Tags.comboAnimStringToHash);  // the value to activate the Tween of the Combo Text
    public int fruitDestroyedInTime;                                            // the number of fruit we destroyed in this half second block.
    private AnimatorStateInfo baseStateInfo;                                    // reference to the animator state
    public RectTransform comboRect;                                             // rect of the combo texts/sprites
    public RectTransform comboRectParent;                                       // parent rect with this script on it.
    public List<Transform> comboTextLocations;                                  // list of transforms from empty gameObjects placed in a sort of grid around the scene.
    public Transform selectedComboTextPoint;                                    // the transform that was closest to the last cut fruit. and where we will place to combo text.

    public bool useImagesForComboNum;                                           // boolean that determines whether sprites or a text component are used to display the numeric portion of the Combo Text
    //Inspector Formatting Region...
    #region
    [Space(15, order = 0)]
    [Header("Below is only Used With Non-Text Component Combo Numbers.", order = 1)]
    [Space(15, order = 1)]
    [Header("If not using the 'Text' UI Component for the Fruit", order = 2)]
    [Space(-5, order = 2)]
    [Header("'#', then this is an array of the likely ''Combo", order = 3)]
    [Space(-5, order = 3)]
    [Header("Number Sprites'' from our Atlas ie.'(Number 3-9)'",order = 4)]
    [Space(-5, order = 4)]
    [Space(15, order = 5)]

    #endregion

    public Sprite[] comboNumberSpritesFromAtlas;                                // Numbers 3 - 9 of the UI Atlas
    public Image fruitComboNumImageReference;                                   // reference to the image component that we will feed the desired "Number Sprite" to
    private float _internalComboTimerStartAmount;                               // the value we first put on the timer.

    // Use this for pre-initialization
    void Awake()
    {

        //initialize the list of Transform we declared above.
        comboTextLocations = new List<Transform>();
        
        //our ComboRect element is the First Child of the script canvas, and so we get the RectTransform component.
        comboRect = transform.GetChild(0).GetComponent<RectTransform>();

        //because we will need to do some simple math to get the ComboText RectTransform aligned we will also
        //get the ComboRects Parent(the object this script is attached to).
        comboRectParent = GetComponent<RectTransform>();

        //Get a reference to the UI text element that will display the number of destroyed fruit
        //in the half second combo stretch periods
        fruitComboNum = GameObject.FindGameObjectWithTag(Tags.comboNumTag).GetComponent<Text>();
        //Get a CanvasGroup reference.  the "Canvas Group" is the component who's alpha we fade.  Easier to fade the parent
        //of the ComboText object(s) ( because we may uses meshes/textures/UI texts... etc)
        comboTweenCanvasGroup = GetComponent<CanvasGroup>();
        //Get a reference to the Animator component so that we can play the ComboText tween animation.
        comboAnim = GetComponentInChildren<Animator>();

        //if we intend to use atlas images for numbers 3,4,5,6,7,8,9 Combos, then get a reference to our Image Component
        //and disable the Text Component because we don't need both.
        if (useImagesForComboNum)
        {
            // lol... this feels really, really bad, BUT I am trying to keep Find/Tag usage to a minimum... if you use any
            //part of this for a release title, please change this, or note that the Combo Canvas ordering CANNOT be changed
            //at least the top of the hierarchy... we may change this before template submission.
            fruitComboNumImageReference = transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Image>();

            //set the initial 'source' image to the first sprite in our ComboNumberSpritesFromAtlas Array...
            fruitComboNumImageReference.sprite = comboNumberSpritesFromAtlas[0];

            //lets go ahead and just deactivate the text component on our original "Text.text" version of the "# Hit Combo" system.
            //We don't want it showing through our Sprite Numbers.  If the above boolean is checked then you must have it setup to use the images,
            //or you will be stuck without a working combo system.  Refer to the working copy of the game scene and look at what's happening.
            fruitComboNum.enabled = false;

            //Below we will write a Method Named "ChooseCorrectNumberSprite()" and then we will use the same boolean from above so that both
            //systems will work together... just requiring a boolean to be tick in the inspector.  I am only doing this because the atlas' "numbers" 
            //"style" matches that of the "Combo" and "Hit" text sprites.  By the way... TextMeshPro is an incredible asset.  You should give it a look.
            //I am doing this because i wanted more control over the text visuals, and so I am doing this the quick and dirty way.  BUT.. with the above
            //mentioned asset you get a lot of control/ability you cannot get with the built-in color/styles of text components.
        }



    }

    // Use this for initialization
    void Start()
    {
        _internalComboTimerStartAmount = comboAmountOfTime;
        //add all the "Empty" Gameobjects that are our "9 potential ComboText Anchors" to a GameObject array
        GameObject[] itemsForList = GameObject.FindGameObjectsWithTag(Tags.comboTextLocations);
        //loop through all of those "anchors" in the gameobject array.
        for (int i = 0; i < itemsForList.Length; i++)
        {
            //create a Transform var for each element in the GO array.
            Transform trans = itemsForList[i].GetComponent<Transform>();
            //then we add the transform that we create for every element, and add it to the List.
            comboTextLocations.Add(trans);
        }

        //Because our ComboText start enabled, and visible... the first thing we need to do is to fade it out...
        StartCoroutine(FadeComboText(0f, 0f, 0.5f));
    }


    /// <summary>
    /// This method is responsible for taking the last fruit we chopped in our fruit combo "stretch"... It sorts through
    /// the list of ComboTextAnchors we have in the list<> and fines the one closest to our fruit.  This is done because the
    /// Official FN has a similar effect.  When slice a number of fruit the combo text appears close to your combo area.
    /// So we set 9 points in our game area 3x3, so that we can have a similar effect, and make sure that the location
    /// of the ComboText is predictable, and never off screen.
    /// </summary>
    /// <param name="posOfFruit"></param>
    public void SortDistanceToComboTextAnchorLocation(Vector3 posOfFruit)
    {

        ///Original Form of the Sort Method within region...  This is just another in-line way to do the sort.
        #region
        //comboTextLocations.Sort(delegate (Transform t1, Transform t2) {
        //    return Vector3.Distance(t1.transform.position, transform.position).CompareTo(Vector3.Distance(t2.transform.position, transform.position));
        //});
        #endregion

        //This uses the basic List<T> Sort Method... It compares 2 values.  when this method is called it compares the distance between our first anchor
        //position and our fruits position, and it stores it in "distanceTo1".  Then it does the same with "distanceTo2" but with another list entry, then 
        //it uses .CompareTo to compare distanceTo1 and distanceTo2... See next comment to see what the CompareTo returns.
        comboTextLocations.Sort (delegate (Transform t1, Transform t2)
        {
            //if the list entries are not null...
            if (t1 != null && t2 != null)
            {
                //new floats that represent the distance between point one and the last fruit chopped in the combo stretch..(described above)
                float distanceTo1 = Vector3.Distance(t1.position, posOfFruit);
                float distanceTo2 = Vector3.Distance(t2.position, posOfFruit);

                //return an int(which is how the list of anchors are sorted/sifted.  All of the points are checked(two at a time), and moved around in the list.
                // if t1 is closer to the fruit, than t2, then -1 is returned to the caller.  the rest of the return possibilities are commented below, but this
                //is how this is sorted.  There are so many ways that this can be done, and you can learn a lot more by looking up List(T).Sort Method 
                //(IComparer(T)) on the MSDN.

                return distanceTo1.CompareTo(distanceTo2);// t1 is closer = returns -1 // if t1 is same dist = returns 0 // if t1 is further = returns 1 //
            }
            else
            // if there are any missing elements or problems... return 0
            {
                return 0;
            }
        });//end of delegate sort Method

        //SortDistanceToComboTextAnchorLocation END
    }

    /// <summary>
    /// This Method Changes the XHitCombo's Number image to the correct Number image from out sprite array.
    /// </summary>
    public void ChooseCorrectNumberSprite(int numOfDestroyedFruit)
    {
        if(fruitComboNumImageReference != null)
        {
            if(comboNumberSpritesFromAtlas.Length == 7)
            {
                //Debug.Log("Yep, everything is set up properly in the inspector...");
                //Debug.Log("there are 7 entries in the array, so because or combo Sprite function /n is only for 3-9 fruit we should be good...");
                //Debug.Log("go ahead and change sprite to" + "=" + " " + numOfDestroyedFruit.ToString());
                fruitComboNumImageReference.sprite = comboNumberSpritesFromAtlas[numOfDestroyedFruit - 3];

            }

            //Debug.Log("image ref wasn't null");

        }

    }


    // Update is called once per frame
    void Update()
    {
        //call this method every frame to reduce the combo timer(and reset it to zero
        // when necessary,  It also increments the number of fruit destroyed within that
        //time.  See method for more details.
        CheckTheCountdownTimer();

    }


    /// <summary>
    /// This method is responsible for checking if the time on the combo clock is below zero.  If it is below
    /// zero then it sets it back to half a second.  If the timer has run out; it zeros out all fruit recorded 
    /// to the "fruitDestoyedInTime" variable.
    /// </summary>
    public void CheckTheCountdownTimer()
    {
        //is the timer still running and above 0? or is it less than or equal to 0?
        if (comboAmountOfTime <= 0f)
        {
            //call this small method to give a bit of a delay before resetting comboTimer/fruit amount
            //a little padding so that as crazy fruit numbers spawn their isn't a constant combo ui element
            //obstructing the view.  I feel like one second is a good, wait.. maybe two...

            //commented invoke... game flow takes care of timing issue
            //Invoke("ResetComboTimer", 0.15f);
            ResetComboTimer();

        }
        else
        {
            //else, clearly the timer is still about 0... reduce time since last frame,
            //and lets do this all again.
            comboAmountOfTime -= Time.deltaTime;
        }
    }

    /// <summary>
    /// This is a super short Method that just resets the combo timer, and zero's out the "fruitDestroyedInTime" Count.
    /// This is in a separate method from above so that we can invoke this method after a second or so passes, that way it doesn't
    /// have a chance to achieve combos back to back to back... etc.  If you want it that way just add this to the Above reset method.
    /// </summary>
    public void ResetComboTimer()
    {
        //if the time has run out we zero out the number of destroyed fruit...
        //this round is over... better luck next time ;-P
        fruitDestroyedInTime = 0;
        // the timer is reset to half a second.
        comboAmountOfTime = _internalComboTimerStartAmount;
    }

    /// <summary>
    /// This method is responsible for counting the fruit that are destroyed within a half second of each other.  Once it
    /// does that it, if the number is 3 or above it displays our "ComboText", and puts it at 1 of the 9 anchors that we placed
    /// in the scene.  The anchor that is closest to the last chopped fruit(in this combo time-frame).
    /// </summary>
    /// <param name="fruitPosition"></param>
    public void CheckTimeAndRecordFruit(Vector3 fruitPosition)
    {
        // if the timer is above 0
        if (comboAmountOfTime > 0f)
        {
            //we must still be counting down so increase the number of destroyed fruit.
            fruitDestroyedInTime++;

            // if we have destroyed at least 3 other fruits and less than
            if(fruitDestroyedInTime >= 3)
            {
                //call the sort Method and give it the eligible fruit pos((Method Parameter)the last fruit that achieved the "combo")
                SortDistanceToComboTextAnchorLocation(fruitPosition);
                
                //after the sort method has been called the ComboText anchor point that is closest to the eligible fruit
                // should be at position 0 in the list.

                //so now... selectedComboTextPoint = comboTextLocations[0]
                selectedComboTextPoint = comboTextLocations[0];

                //call the method that turns the world space coord(the comboText anchor) into a screen-space
                //coord.  (so that we can display the UI Images/Text at an appropriate location) The Location
                //that is relative to the anchor location, from out camera view-port.
                SetComboTextToFruitPosition(selectedComboTextPoint.position);
            }
        }
        //call the method that fades the comboText elements into view
        ActivateAndTweenComboText();
    }


    /// <summary>
    /// This Method is responsible for changing the coords of a ComboText anchor point into screen-space coords
    /// </summary>
    /// <param name="aPos"></param>
    public void SetComboTextToFruitPosition(Vector3 aPos)
    {
        //new vector2 that stores the value of the aPos that we sent to this Method.
        Vector2 pos = aPos;

        //new vector2 that stores the conversion of WorldToViewportPoint passed our parameter position("pos")
        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(pos);

        //new vector2 then you use the ComboTextRects Parent RectTransform to deduce the necessary shift to
        //place the ComboTextRect in the right location.  For the Canvas 0,0 is center, but the function
        // WorldToViewportPoint uses "view-port"(i.e. 0,0 is bottom left).  So we need to subtract the height /
        // width of the canvas * 0.5 to get the proper coord.
        Vector2 WorldObject_ScreenPosition = new Vector2(
            ((ViewportPosition.x * comboRectParent.sizeDelta.x) - (comboRectParent.sizeDelta.x * 0.5f)),
            ((ViewportPosition.y * comboRectParent.sizeDelta.y) - (comboRectParent.sizeDelta.y * 0.5f)));

        //so now we give the ComboTextRext the anchor location that we adjusted above.
        comboRect.anchoredPosition = WorldObject_ScreenPosition;

        // so now the ComboText Element(s) are in the correct point, and from a vantage point down the 
        // z-axis it will appear that the ComboText is at one of our 9 "anchor" locations. :)
    }


    /// <summary>
    /// This method is responsible for activating, and tweening the "ComboText".  If in the half a second the comboTimer has 
    /// we destroyed 3 or more fruit, then we will activate the tween animation, and fade in the ComboText UI element.  Using
    /// our Fade coroutine.
    /// </summary>
    public void ActivateAndTweenComboText()
    {
        //have we destroyed 3 or more fruit?
        if(fruitDestroyedInTime > 2  && fruitDestroyedInTime < 10)
        {
            //if so then we check the current animator state(which should be idle)
            baseStateInfo = comboAnim.GetCurrentAnimatorStateInfo(0);

            //if that animator state does not equal the "tweenHash"(which is the StringToHash "PlayComboTween"
            // which we store in our Tags class.
            if (baseStateInfo.fullPathHash != tweenHash)
            {
                //if the state isn't "playTween".. then we will play the tween
                comboAnim.SetTrigger(tweenHash);
            }
            //give bones points(amount of combo)
            switch (GameController.GameControllerInstance.gameModes)
            {
                case GameModes.Classic:
                    GameVariables.ClassicModeScore += fruitDestroyedInTime;
                    break;
                case GameModes.Arcade:
                    GameVariables.ClassicModeScore += fruitDestroyedInTime;
                    break;
                case GameModes.Relax:
                    GameVariables.RelaxModeScore += fruitDestroyedInTime;
                    break;
                default:
                    break;
            }
            //stop all coroutines for good measure
            StopAllCoroutines();
            //start our fade coroutine immediately, and change alpha to 1, in 0.5 seconds...
            // which sets our ComboText to visible.
            StartCoroutine(FadeComboText(0f, 1f, 0.5f));


            if (!useImagesForComboNum)
            {
                // make the fruitComboNum.text Text UI Element to the var amount "fruitDestroyedInTime"... .ToString because we need a string.
                fruitComboNum.text = fruitDestroyedInTime.ToString();
            }
            else
            {
                ChooseCorrectNumberSprite(fruitDestroyedInTime);
            }


            //Invoke our DeactivateComboText Method in 4 seconds... which will stop all coroutines(after they should have already finished anyway)
            // and it starts the fade out coroutine.
            Invoke("DeactivateComboText", 4);
        }

    }


    /// <summary>
    /// This method starts a coroutine that fades the "ComboText" to Transparent
    /// </summary>
    public void DeactivateComboText()
    {
        //stop all coroutines for good measure ;)
        StopAllCoroutines();

        //Start Coroutine to fade the ComboText Alpha to Zero
        StartCoroutine(FadeComboText(0f, 0f, 0.5f));
        //Debug.Log("done fading combo text!");
    }


    /// <summary>
    /// This Method is responsible for fading the ComboText in and out.  First parameter is how long to wait before 
    /// starting the fade.  Second parameter is WHAT value to fade to(ie.. 1 opaque, 2 transparent).  The Third
    /// parameter is how long the fade should take.  This method is used to both fade in, and out.
    /// </summary>
    /// <param name="initFadeDelay"></param>
    /// <param name="aValue"></param>
    /// <param name="aTime"></param>
    /// <returns></returns>
    IEnumerator FadeComboText(float initFadeDelay, float aValue, float aTime)
    {
        //wait if there is a wait time...
        yield return new WaitForSeconds(initFadeDelay);
        //record the current alpha value of our ComboText Canvas
        float alpha = comboTweenCanvasGroup.alpha;

        //for loop the length of Time.  we increment by time.delta / parameter time every frame
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            //every pass we Lerp the alpha close to the desired value by the increment amount(t)
            comboTweenCanvasGroup.alpha = Mathf.Lerp(alpha, aValue, t);
            //return null - loop again until t = value.

            if(comboTweenCanvasGroup.alpha <= 0.1f)
            {
                comboTweenCanvasGroup.alpha = 0f;
            }
            yield return null;

        }
    }


}