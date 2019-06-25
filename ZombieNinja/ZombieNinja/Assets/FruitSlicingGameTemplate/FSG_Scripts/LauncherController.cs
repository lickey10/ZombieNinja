using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// FruitLauncherController Class handles the Launching of the gameObjects.  Fruit,Bombs, and PowerUps.
/// </summary>
public class LauncherController : MonoBehaviour
{
    public static LauncherController LaunchControllerInstance;                  // our static reference to this LaunchController
    [Header("Bottom Fruit Launchers, and how many to Launch")]
    public GameObject[] bottomFruitLaunchers;                                   // an array of the fruit launchers at the bottom of the dojo
    public int bottomLauncherSalvoAmount;                                       // the salvo amt (an int that determines how many fruit are fired
    [Header("Side Fruit Launchers(Frenzy), and how many to Launch")]
    public GameObject[] sideFruitLaunchers;                                     // an array of the fruit launchers at the side of the dojo (for Frenzy PowerUp)
    public int sideLauncherSalvoAmount;                                         // the salvo amt (an int that determines how many fruit are fired during a frenzy
    [Header("How Long To Wait For Fruit to Spawn?(MIN)")]
    public float minWaitTime;                                                   // Min time to wait before fruit are spawned (when requested)
    [Header("How Long To Wait For Fruit to Spawn?(MAX)")]
    public float maxWaitTime;                                                   // Max time to wait before fruit are spawned (when requested)
    [Header("How Long Between Regular Bottom Fruit Launches?")]
    public float timeBetweenLaunches;                                           // the time between salvos (there is a little break between them)
    public float timeBetweenRandomLaunches;                                     // time between random launches (there are some salvos that come at random intervals too(breaks up the interval launches)
    public float timeBetweenSpecialLaunches;                                    // the time between special object's launches (power ups)
    public float timeBetweenBombLaunches;                                       // the time between bomb object's launches
    [Header("What should the Max Number of Fruit Launches be?")]
    public int maximumSimultaneousFruitLaunches;                                // the max number of simultaneous fruit launches (max salvo amt)
    [Tooltip("This is the number that after the 'maxSimultaneousFruitLaunches' is reached, that the launcher will start over at")]
    public int resetFruitLauncherAmount;                                        // if we reach the max number of launches, what number should we drop it back down to?
    [Header("How Many More Fruit In Relax Mode?")]
    public int RelaxModeExtraFruit;                                             // How many extra fruit would we like launched in relax mode?  there are no fruit and bombs, so why not add fruit!?
    [Header("Amount of Objects that are launched")]
    [Header(" **goes up each loop** ")]
    public int BombSalvoAmount;                                                 // the amount of bombs that will be launched during bomb salvos
    public int maxBombSalvoAmt;                                                 // the max number of bombs that can be launched at one time
    public int powerUpSalvoAmount;                                              // the amount of power-ups that will be launched when called for(one usually)
    public int freezeBananaPowerUpCount;                                        // the count of how many Freeze Bananas have launched in the round (ArcadeMode)
    public int frenzyBananaPowerUpCount;                                        // the count of how many Frenzy Bananas have launched in the round (ArcadeMode)
    public int twoTimesScoreBananaPowerUpCount;                                 // the count of how many 2xScore Bananas have launched in the round (ArcadeMode)
    private float initialTimeBetweenLaunches;                                   // the initial time between the fruit launches
    private List<FruitLauncher> bottomLaunchersScriptReference;                 // a List that contains all of the Bottom FruitLaunchers (the gameObject the "FruitLauncher" script is attached to)
    private List<FruitLauncher> sideLaunchersScriptReference;                   // a List that contains all of the Side FruitLaunchers (the gameObject the "FruitLauncher" script is attached to)
    private CountdownTimer timer;                                               // a reference to our "CountdownTimer". GameController,DojoBoundary,LauncherContoller(this), and CountdownTimer are all on GameController gameObject
    private FreezeEffect freezeEffectReference;                                 // a reference to the FreezeEffect GameObject (in the scene.. the one that activates the effect)
    private FrenzyEffect frenzyEffectReference;                                 // a reference to the FreezeEffect GameObject (in the scene.. the one that activates the effect)
    private TwoTimesScoreEffect twoTimesScoreEffectReference;                   // a reference to the 2xScore GameObject (in the scene.. the one that activates the effect)
    private int cycleThruSideLaunchersForFrenzy;                                // the int that is used for looping through the side launchers
    private int numOfTimesFreezeBananaLaunched;                                 // the number of times that the Freeze Banana has been launched
    private int numOfTimesFrenzyBananaLaunched;                                 // the number of times that the Freeze Banana has been launched
    private int numOfTimesScoreTimesTwoLaunched;                                // the number of times that the Freeze Banana has been launched

    // Use this for pre-initialization
    void Awake()
    {
        //make sure our static LaunchController Instance is set to THIS gameObject
        LaunchControllerInstance = this;

        //our timer reference by calling GetComponent on THIS gameObject
        timer = GetComponent<CountdownTimer>();

        //Use GameObjects FindObjectWithTag method to setup our references to the freeze,frenzy, and 2xScore objects/classes.  Using our Const Strings in "Tags"
        freezeEffectReference = GameObject.FindGameObjectWithTag(Tags.freezeEffectGameObjectTag).GetComponent<FreezeEffect>();
        frenzyEffectReference = GameObject.FindGameObjectWithTag(Tags.frenzyEffectGameObjectTag).GetComponent<FrenzyEffect>();
        twoTimesScoreEffectReference = GameObject.FindGameObjectWithTag(Tags.twoTimesScoreEffectGameObjectTag).GetComponent<TwoTimesScoreEffect>();

        //Use GameObjects FindObjectWithTag method to setup our references to the bottom and side fruitLaunchers.
        bottomFruitLaunchers = GameObject.FindGameObjectsWithTag(Tags.bottomFruitLaunchers);
        sideFruitLaunchers = GameObject.FindGameObjectsWithTag(Tags.sideFruitLaunchers);

        //Initialize the lists  "bottomLaunchersScriptReference" && "sideLaunchersScriptReference"
        bottomLaunchersScriptReference = new List<FruitLauncher>();
        sideLaunchersScriptReference = new List<FruitLauncher>();

        //loop through all of the bottom fruit launchers
        for (int i = 0; i < bottomFruitLaunchers.Length; i++)
        {
            //now add each bottom fruit launcher to our List ("bottomLaunchersScriptReference")
            bottomLaunchersScriptReference.Add(bottomFruitLaunchers[i].GetComponent<FruitLauncher>());
        }
        //loop through all of the side fruit launchers
        for (int j = 0; j < sideFruitLaunchers.Length; j++)
        {
            //now add each side fruit launcher to our List ("sideLaunchersScriptReference")
            sideLaunchersScriptReference.Add(sideFruitLaunchers[j].GetComponent<FruitLauncher>());
        }
    }


    // Use this for initialization
    void Start()
    {
        //we store our "timeBetweenLaunches" in the InitialTimeBetweenLaunches" variable. (so we have a back up of the original value)
        initialTimeBetweenLaunches = timeBetweenLaunches;
    }


    /// <summary>
    /// This Method calls for the First Fruit Launch.
    /// </summary>
    /// <param name="amt"></param>
    /// <returns></returns>
    private IEnumerator FirstFruitLaunch(int amt)
    {
        //we wait for 1f seconds
        yield return new WaitForSeconds(1f);
        //Then we "RequestFruitSalvoFromBottom(and pass in the "amt" that was passed with the method call)"
        RequestFruitSalvoFromBottom(amt);
    }


    /// <summary>
    /// This method adjusts some of the launcher settings/times when relax is the gameMode.
    /// </summary>
    private void ChangeToRelaxSettings()
    {
        //we assign timeBetweenLaunches 4.
        timeBetweenLaunches = 4;
        //we assign initialTimeBetweenLaunches 4 as well.
        initialTimeBetweenLaunches = 4 ;
        //then we set the "maximumSimultaneousFruitLaunches" to 11;
        maximumSimultaneousFruitLaunches = 11;

    }


    /// <summary>
    /// This Method is where all of the "Launching" happens.  This method is pretty long compared to the others, so sit down, and strap in.
    /// We monitor the "GameMode" we are in based on a Switch that compares our gameModes var to the GameModes Enum.
    /// </summary>
    public void ReduceLaunchTimersAndLaunchObjects()
    {

        //before we get into gameMode specific code... timeBetweenLaunches gets decremented by Time.deltaTime;
        timeBetweenLaunches -= Time.deltaTime;
        //if "timeBetweenLaunches" is less than or equal to 0f, then we...
        if (timeBetweenLaunches <= 0f)
        {
            //create a new int named "r" and give it a random value between 0 and 3 (inclusive, exclusive... so values can be 0,1,2)
            int r = Random.Range(0, 3);
            //now for the gameMode that we are in.
            switch (GameController.GameControllerInstance.gameModes)
            {


                /////////////////////////////////////
                ////////______RELAX-MODE_____////////
                /////////////////////////////////////


                //if we are in GameMode Classic
                case GameModes.Classic:

                    
                    //we check to see what random value was generated...

                    //if r equals 0, we....
                    if (r == 0)
                    {

                        //if our FruitMissed var is less than 3(then the game must still be running)
                        if (GameVariables.FruitMissed/*GameController.GameControllerInstance.fruitMissed*/ < 3 )
                        {
                            //we request a Fruit Salve From Bottom Launchers and we pass the " bottomLauncherSalvoAmount "
                            RequestFruitSalvoFromBottom(bottomLauncherSalvoAmount);

                            //increment launch amount
                            bottomLauncherSalvoAmount++;

                            //reset timer
                            timeBetweenLaunches = initialTimeBetweenLaunches;
                        }
                    }


                    //if r equals 1, we...
                    if (r == 1)
                    {

                        //we request a Fruit Salve From Bottom Launchers and we pass the "bottomLauncherSalvoAmount"
                        RequestFruitSalvoFromBottom(BombSalvoAmount + 2);

                        //we also request a bomb salvo from the bottom launchers and we pass the "BombSalvoAmount - 1, and we pass int(4) to ask for a Bomb"
                        RequestOtherObjectSalvoFromBottomLauncher(BombSalvoAmount - 1, 4);

                        //bottomLauncherSalvoAmount++;
                        BombSalvoAmount++;

                        //timeBetweenLaunchers gets assigned the value of "initialTimeBetweenLaunches"
                        timeBetweenLaunches = initialTimeBetweenLaunches;
                        //if the bombSalvoAmount is greater than maxBombSalvoAmt, then we...
                        if (BombSalvoAmount > maxBombSalvoAmt)
                        {
                            //we reset the bomb salvo amount to some other value between 1 and the maxBombSalvoAmt
                            BombSalvoAmount = Random.Range(1, maxBombSalvoAmt);
                        }
                    }


                    //if r equals 2, we...
                    if (r == 2)
                    {
                        ///////   NOTE - If 2 is the random value AND we are in Classic Mode... this version will only Instantiate one Bomb(for the cases, where
                        /////       Bombs don't spawn for a while... we will spawn fruit and One Bomb.  This is kind of a lazy copy and paste from Int (1) above, but
                        ////        it will cover some of the gaps.
                        ////

                        //we request a Fruit Salve From Bottom Launchers and we pass the "bottomLauncherSalvoAmount"
                        RequestFruitSalvoFromBottom(BombSalvoAmount + 2);

                        //we also request a bomb salvo from the bottom launchers and we pass the "BombSalvoAmount - 1, and we pass int(4) to ask for a Bomb"
                        RequestOtherObjectSalvoFromBottomLauncher(1, 4);

                        //bottomLauncherSalvoAmount++;
                        BombSalvoAmount++;

                        //timeBetweenLaunchers gets assigned the value of "initialTimeBetweenLaunches"
                        timeBetweenLaunches = initialTimeBetweenLaunches;
                        //if the bombSalvoAmount is greater than maxBombSalvoAmt, then we...
                        if (BombSalvoAmount > maxBombSalvoAmt)
                        {
                            //we reset the bomb salvo amount to some other value between 1 and the maxBombSalvoAmt
                            BombSalvoAmount = Random.Range(1, maxBombSalvoAmt);
                        }

                    }


                    // if the ( bottomLauncherSalvoAmount is equal= to maximumSimultaneousFruitLaunches ) then...
                    if (bottomLauncherSalvoAmount == maximumSimultaneousFruitLaunches)
                    {
                        //we need to assign the "resetFruitLauncherAmount" to our "bottomLauncherSalvoAmount" variable.
                        bottomLauncherSalvoAmount = resetFruitLauncherAmount;
                    }



                    break;


                /////////////////////////////////////
                ////////_____ARCADE-MODE_____////////
                /////////////////////////////////////


                //if we are in GameMode Classic
                case GameModes.Arcade:


                    //we check to see what random value was generated...

                    //if r equals 0, we....  ( we just launch fruit )
                    if (r == 0)
                    {

                        //if the time is above 1.5 second(changed from zero to prevent a launch right after GameOver Screen)
                        if (timer.timeLeft > 1.5f)
                        {
                            //launch fruit
                            RequestFruitSalvoFromBottom(bottomLauncherSalvoAmount);
                            //increment salvo amount
                            bottomLauncherSalvoAmount++;
                            //time between launches gets initialTimeBetweenLaunches value
                            timeBetweenLaunches = initialTimeBetweenLaunches;
                        }

                    }


                    //if r equals 1, we...  ( we launch fruit and bombs )
                    if (r == 1)
                    {

                        //if the time is above 1.5 second(changed from zero to prevent a launch right after GameOver Screen)
                        if (timer.timeLeft > 1.5f)
                        {
                            //launch fruit (lesser amount because we are also launching bombs...)
                            RequestFruitSalvoFromBottom(BombSalvoAmount + 2);
                            //launch bombs
                            RequestOtherObjectSalvoFromBottomLauncher(BombSalvoAmount, 3);
                            //increment bomb salvo amount
                            BombSalvoAmount++;
                            //time between launches gets initialTimeBetweenLaunches value
                            timeBetweenLaunches = initialTimeBetweenLaunches;
                            //if the BombSalvoAmount is greater than maxBombSalveAmt then....
                            if (BombSalvoAmount > maxBombSalvoAmt)
                            {
                                //we reset the bomb salvo amount to some other value between 1 and the maxBombSalvoAmt
                                BombSalvoAmount = Random.Range(1, maxBombSalvoAmt);
                            }
                        }

                    }


                    //if r equals 2, we...  ( we launch fruit and a power-up )
                    if (r == 2)
                    {

                        //if the time is above 1.5 second(changed from zero to prevent a launch right after GameOver Screen)
                        if (timer.timeLeft > 1.5f && timer.timeLeft < 40f)
                        {
                            //launch fruit (lesser amount because we are also launching a power-up...)
                            RequestFruitSalvoFromBottom(powerUpSalvoAmount + 4);
                            //create a random value to select a power-up type
                            int powerUpTypeRandom = Random.Range(0, 3);



                            //if the powerUpTypeRandom is 0...  [(  spawn a freeze effect banana  )]
                            if(powerUpTypeRandom == 0)
                            {

                                //if freeze effect is not on, and the number of times a freeze banana has been launched is less than 2
                                if (!freezeEffectReference.freezeEffectIsOn && numOfTimesFreezeBananaLaunched < 2)
                                {
                                    //request a freeze banana from bottom launchers (freeze amount var, 2(freeze obj type) )
                                    RequestOtherObjectSalvoFromBottomLauncher(freezeBananaPowerUpCount, 2);
                                    //increment the number of times we spawned freeze banana
                                    numOfTimesFreezeBananaLaunched++;
                                }
                                else
                                {
                                    //launch fruit
                                    RequestFruitSalvoFromBottom(bottomLauncherSalvoAmount);
                                    //increment launch amount
                                    bottomLauncherSalvoAmount++;
                                }

                            }



                            //if the powerUpTypeRandom is 1...  [(  spawn a frenzy effect banana  )]
                            if (powerUpTypeRandom == 1)
                            {
                                //if frenzy effect is not on, and the number of times a frenzy banana has been launched is less than 2
                                if (!frenzyEffectReference.frenzyEffectIsOn && numOfTimesFrenzyBananaLaunched < 2)
                                {
                                    //request a frenzy banana from bottom launchers (frenzy amount var, 2(Frenzy obj type) )
                                    RequestOtherObjectSalvoFromBottomLauncher(frenzyBananaPowerUpCount, 1);

                                    //increment the number of times we spawned frenzy banana
                                    numOfTimesFrenzyBananaLaunched++;
                                }
                                else
                                {
                                    //launch fruit
                                    RequestFruitSalvoFromBottom(bottomLauncherSalvoAmount);
                                    //increment launch amount
                                    bottomLauncherSalvoAmount++;
                                }

                            }



                            //if the powerUpTypeRandom is 2...  [(  spawn a 2xScore effect banana  )]
                            if (powerUpTypeRandom == 2)
                            {
                                //if 2xScore effect is not on, and the number of times a 2xScore banana has been launched is less than 2
                                if (!twoTimesScoreEffectReference.twoTimesScoreIsOn && numOfTimesScoreTimesTwoLaunched < 2)
                                {
                                    //request a 2xScore banana from bottom launchers (2xScore amount var, 2(2xScore obj type) )
                                    RequestOtherObjectSalvoFromBottomLauncher(twoTimesScoreBananaPowerUpCount, 0);

                                    //increment the number of times we spawned 2xScore banana
                                    numOfTimesScoreTimesTwoLaunched++;
                                }
                                else
                                {
                                    //launch fruit
                                    RequestFruitSalvoFromBottom(bottomLauncherSalvoAmount);
                                    //increment launch amount
                                    bottomLauncherSalvoAmount++;
                                }
                            }

                            //timeBetweenLaunchers gets assigned initialTimeBewteenLaunches
                            timeBetweenLaunches = initialTimeBetweenLaunches;
                        }

                    }

                    //if the "bottomLauncherSalvoAmount" is at the "maximumSimultaneousFruitLaunches"
                    if (bottomLauncherSalvoAmount == maximumSimultaneousFruitLaunches)
                    {
                        //then we assign "resetFruitLauncherAmount" to "bottomLauncherSalvoAmount"
                        bottomLauncherSalvoAmount = resetFruitLauncherAmount;
                    }



                    break;


                /////////////////////////////////////
                ////////______RELAX-MODE_____////////
                /////////////////////////////////////


                //if we are in GameMode Relax
                case GameModes.Relax:

                    //RelaxMode is for fruit only... no matter what r is assigned
                    if ( r == 0 || r == 1 || r == 2 )
                    {
                        //if the time is above 1.5 second(changed from zero to prevent a launch right after GameOver Screen
                        if (timer.timeLeft > 1.5f)
                        {
                            //launch fruit
                            RequestFruitSalvoFromBottom(bottomLauncherSalvoAmount);
                            //increment the salvo amount.
                            bottomLauncherSalvoAmount++;
                            //we reset the time between launches...
                            timeBetweenLaunches = initialTimeBetweenLaunches;
                        }

                    }

                    //if the "bottomLauncherSalvoAmount" is at the "maximumSimultaneousFruitLaunches"
                    if (bottomLauncherSalvoAmount == maximumSimultaneousFruitLaunches)
                    {
                        //then we assign "resetFruitLauncherAmount" to "bottomLauncherSalvoAmount"
                        bottomLauncherSalvoAmount = resetFruitLauncherAmount;
                    }


                    break;
                default:

                    break;
            }

        }

    }


    /// <summary>
    /// This Method Stops all of the Object Launchers. 
    /// </summary>
    private void CancelAllObjectLaunchers()
    {
        //set the timer to 0
        timer.timeLeft = 0;
        //call StopAllCoroutines.
        StopAllCoroutines();
    }


    /// <summary>
    /// This Coroutine is called by "WaitToLaunchBottomFruit".  It fulfills the launching of the fruit.  The Coroutine does the "actual firing".
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitToLaunchBottomFruit()
    {
        //we wait a random interval between minWaitTime and maxWaitTime
        yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));

        //create a random variable from 0 to the length of the bottomeFruitLaunchers array.
        int r = Random.Range(0, bottomFruitLaunchers.Length);
        //the we call LoadAndFireRandomFruit on the bottomLauncherScriptReference element at position r.
        bottomLaunchersScriptReference[r].LoadAndFireRandomFruit();
    }





    /// <summary>
    /// This Coroutine is called by "RequestOtherObjectSalvoFromBottomLauncher".  It fulfills the launching of the fruit.  The Coroutine does the "actual firing".
    /// </summary>
    /// <param name="objectType"></param>
    /// <returns></returns>
    private IEnumerator WaitToLaunchBottomOtherObject(int objectType)
    {
        //we wait a random interval between minWaitTime and maxWaitTime
        yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
        //create a random variable from 0 to the length of the bottomeFruitLaunchers array.
        int r = Random.Range(0, bottomFruitLaunchers.Length);
        //the we call LoadAndFireOtherObject on the bottomLauncherScriptReference element at position r, and pass the objectType.
        bottomLaunchersScriptReference[r].LoadAndFireOtherObject(objectType);
    }




    /// <summary>
    /// This Coroutine is called by "RequestFruitSalvoFromSide".  It fulfills the launching of the fruit from the side(frenzy).  The Coroutine does the "actual firing".
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitToLaunchSideFruit()
    {
        //we wait a random interval between minWaitTime and maxWaitTime
        yield return new WaitForSeconds(Random.Range( 0f , 5f ));
        //if the variable "cycleThruSideLaunchersForFrenzy" (the number of the side launchers we are on), is greater than the number of sideFruitLaunchers then we set
        //the cycleThruSideLaunchersForFrenzy back to zero so we can start at the first side launcher again.
        if (cycleThruSideLaunchersForFrenzy > sideFruitLaunchers.Length - 1)
        {
            //reset the cycleThruSideLaunchersForFrenzy to zero
            cycleThruSideLaunchersForFrenzy = 0;
        }
        //the we call LoadAndFireRandomFruit on the sideLaunchersScriptReference element at position ("cycleThruSideLaunchersForFrenzy")
        sideLaunchersScriptReference[cycleThruSideLaunchersForFrenzy].LoadAndFireRandomFruit();
        //we increment cycleThruSideLaunchersForFrenzy
        cycleThruSideLaunchersForFrenzy++;
    }



    /// <summary>
    /// This Method is called When fruit should be launched from the bottom.  We pass the number of fruit that we want launched.  This Method start the actual coroutines that call the 
    /// fire method on the "Fruit Launcher"
    /// </summary>
    /// <param name="numOfFruit"></param>
    public void RequestFruitSalvoFromBottom(int numOfFruit)
    {
        //loop through  "numOfFruit"
        for (int i = 0; i < numOfFruit; i++)
        {
            //and call startCoroutine for the number iterations in the loop
            StartCoroutine(WaitToLaunchBottomFruit());
        }
    }



    /// <summary>
    /// This Method is called When other objects should be launched from the bottom.  We pass "numOfOtherProjectiles" for the AMT of other objects we want.  Then We pass the number for the TYPE we 
    /// want launched. This Method starts the actual coroutines that call the fire method on the "Fruit Launcher" 0 = 2X Score , 1 = Frenzy , 2 = Freeze , 3 = Minus10Bomb , and 4 = GameOverBomb.
    /// </summary>
    /// <param name="numOfOtherProjectiles"></param>
    /// <param name="objectType"></param>
    public void RequestOtherObjectSalvoFromBottomLauncher(int numOfOtherProjectiles, int objectType)
    {
        //loop through  "numOfFruit"
        for (int i = 0; i < numOfOtherProjectiles; i++)
        {
            //and call startCoroutine (passing the objectType that we want launched) as a parameter, and we do that for the number iterations in the loop
            StartCoroutine(WaitToLaunchBottomOtherObject(objectType));
        }
    }



    /// <summary>
    /// This Method is called When fruit should be launched from the side.  We pass the number of fruit that we want launched.  This Method starts the actual coroutines that call the 
    /// fire method on the "Fruit Launcher"
    /// </summary>
    /// <param name="numOfFruit"></param>
    public void RequestFruitSalvoFromSide(int numOfFruit)
    {
        //loop through  "numOfFruit"
        for (int i = 0; i < numOfFruit; i++)
        {
            //and call startCoroutine for the number iterations in the loop
            StartCoroutine(WaitToLaunchSideFruit());
        }
    }


}
