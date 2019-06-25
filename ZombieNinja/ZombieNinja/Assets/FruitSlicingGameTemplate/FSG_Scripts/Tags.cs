using UnityEngine;

/// <summary>
/// Tag Class handles all of the Tags in the scene... There were quite a few entries made to the ProjectSettings Tags/Layers, and
/// I got tired of writing all of the strings, and was worried about some of them being misspelled.  This way I can create a static
/// constant read-only string once, make sure it IS right, and then I can have code completion.  So now if I use GameObject.FindGameObjectWithTag
/// I can pass Tags.Player instead of "Player".  Code completion is your friend!
/// 
/// </summary>
public static class Tags
{
    ///////////////////////////////////////////////////////////
    ///////// Scene, Player, & GameObject References //////////
    ///////////////////////////////////////////////////////////

    public static readonly string playerTag = "Player";                                                 // Tags.playerTag is "Player" ()
    public static readonly string fruitTag = "Fruit";                                                   // Tags.fruitTag is "Fruit" (all the fruit are tagged in the scene)

    public static readonly string comboNumTag = "ComboNumberText";                                      // Tags.comboNumTag is "ComboNumberText" ()
    public static readonly string comboCanvasTag = "ComboCanvas";                                       // Tags.comboCanvasTag is "ComboCanvas" ()
    public static readonly string comboTextLocations = "PossibleComboTextLocations";                    // Tags.comboTextLocations is "PossibleComboTextLocations" (anchors for "ComboTextPopUp")

    public static readonly string bottomFruitLaunchers = "BottomFruitLaunchers";                        // Tags.bottomFruitLaunchers is "BottomFruitLaunchers" (the launchers at the bottom of the dojo)

    public static readonly string sideFruitLaunchers = "SideFruitLaunchers";                            // Tags.sideFruitLaunchers is "SideFruitLaunchers" (the launchers at the sides of the dojo(frenzy))

    public static readonly string cameraTag = "MainCamera";                                             // Tags.cameraTag is "MainCamera" (reference to the main camera)
    public static readonly string gameControllerTag = "GameController";                                 // Tags.GameController is "GameController" (ref to the Game Controller in the scene)

    public static readonly string freezeEffectGameObjectTag = "FreezeEffectGameObject";                 // Tags.freezeEffectGameObjectTag is "FreezeEffectGameObject" (Scene Object - Freeze PowerUp)

    public static readonly string frenzyEffectGameObjectTag = "FrenzyEffectGameObject";                 // Tags.frenzyEffectGameObjectTag is "FrenzyEffectGameObject" (Scene Object - Frenzy PowerUp)

    public static readonly string twoTimesScoreEffectGameObjectTag = "TwoTimesScoreEffectGameObject";   // Tags.twoTimesScoreEffectGameObjectTag is "TwoTimesScoreEffectGameObject" (Scene Object - 2XScore)

    public static readonly string playerMenuScoreText = "MenuScoreText";                                // Tags.playerMenuScoreText is "MenuScoreText" (the ref to the Menu Canvas Score Text Component)
    public static readonly string playerMenuLevelText = "MenuLevelText";                                // Tags.playerMenuLevelText is "MenuLevelText" (the ref to the Menu Canvas Level Text Component)


    public static readonly string PineappleTweenIcon = "PineappleTweenIcon";                            // Tags.PineappleTweenIcon is "PineappleTweenIcon" (tag used to get the pineapple Animator)
    public static readonly string cutFruitAnimationTrigger = "CutFruit";                                // Tags.cutFruitAnimationTrigger is "CutFruit" (tag used to trigger the pineapple Animators tween clip)

    public static readonly string GameMusicAudio = "GameMusic";                                         // Tags.GameMusicAudio is "GameMusic" (reference to the scene's Music AudioSource)
    public static readonly string GameSfxAudio = "GameSfx";                                             // Tags.GameSfxAudio is "GameSfx" (reference to the scene's Sfx AudioSource)

    public static readonly string FruitPools = "FruitPools";                                            // Tags.FruitPools is the "FruitPools" (reference to the pools that contain the Fruits for launching)
    public static readonly string OtherPools = "OtherPools";                                            // Tags.OtherPools is the "OtherPools" (reference to the pools that contain the Bombs and PowerUps for launching)

    public static readonly string DojoChnager = "DojoChanger";                                          // Tags.DojoChanger is the "DojoChanger" (reference to the DojoChange GO in each Scene)

    public static readonly string comboAnimStringToHash = "PlayComboTween";                         // the string the animator calls to trigger the combo text tween.

    ///////////////////////////////////////////////////////////
    ////////////// PlayerPrefs Strings & Keys /////////////////
    ///////////////////////////////////////////////////////////

    public static readonly string experience = "playerExperience";                                      // Tags.experience is "playerExperience" (reference to the "experience" text component)
    public static readonly string arcadeScore = "arcadeScore";                                          // Tags.arcadeScore is "arcadeScore" (reference to the GameVariables.ArcadeScore)
    public static readonly string classicScore = "classicScore";                                        // Tags.classicScore is "classicScore" (reference to the GameVariables.ClassicScore)
    public static readonly string relaxScore = "relaxScore";                                            // Tags.relaxScore is "relaxScore" (reference to the GameVariables.RelaxScore)

    public static readonly string highestArcadeScore = "highestArcadeModeScore";                        // Tags.highestArcadeScore is "highestArcadeModeScore" (reference to the GameVariables.highestArcadeScore)
    public static readonly string highestClassicScore = "highestClassicModeScore";                      // Tags.highestClassicScore is "highestClassicModeScore" (reference to the GameVariables.highestClassicScore)
    public static readonly string highestRelaxScore = "highestRelaxModeScore";                          // Tags.highestRelaxScore is "highestRelaxModeScore" (reference to the GameVariables.highestRelaxScore)


}

