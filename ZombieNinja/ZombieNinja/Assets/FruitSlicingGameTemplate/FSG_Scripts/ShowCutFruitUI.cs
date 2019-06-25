using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The ShowCutFruitUI Class just handles what Current and Highest Score is displayed at the top left-hand corner of the screen.  The values
/// that are shown are determined by the GameController, but WHICH ones to show are handled by this script.  It checks the Game Controllers current
/// selected game mode and activates that score/highest score ui element.
/// </summary>
public class ShowCutFruitUI : MonoBehaviour
{
    public Text[] arcadeModeCutFruitText;           // the Text elements for arcade mode
    public Text[] classicModeCutFruitText;          // the Text elements for classic mode
    public Text[] relaxModeCutFruitText;            // the Text elements for relax mode


    // Use this for initialization
    void Start()
    {
        //In start we call the Method that deactivates all the text, and turns on one of them based on GameController.
        DeactivateAllAndSelectCutTextPerMode();

    }

    /// <summary>
    /// This Method starts by looping through all of the elements in the Text Array and disables them, then it sets the proper text elements active.
    /// </summary>
    private void DeactivateAllAndSelectCutTextPerMode()
    {

        //first loop through all arcadeMode Text elements in the array
        for (int i = 0; i < arcadeModeCutFruitText.Length; i++)
        {
            //then each element [i] gets set disabled.
            arcadeModeCutFruitText[i].enabled = false;
        }


        //first loop through all classicMode Text elements in the array
        for (int j = 0; j < classicModeCutFruitText.Length; j++)
        {
            //then each element [i] gets set disabled.
            classicModeCutFruitText[j].enabled = false;
        }


        //first loop through all relaxMode Text elements in the array
        for (int k = 0; k < relaxModeCutFruitText.Length; k++)
        {
            //then each element [i] gets set disabled.
            relaxModeCutFruitText[k].enabled = false;
        }



        //now for turning on one of the sets we just disabled...
        switch (GameController.GameControllerInstance.gameModes)
        {
            //if GameController's gameMode equals GameModes.Classis then...
            case GameModes.Classic:

                //we loop back through an array of the text elements...
                for (int i = 0; i < classicModeCutFruitText.Length; i++)
                {
                    // and set all of the elements to enabled!
                    classicModeCutFruitText[i].enabled = true;
                }
                break;
            //if GameController's gameMode equals GameModes.Arcade then...
            case GameModes.Arcade:

                //we loop back through an array of the text elements...
                for (int i = 0; i < arcadeModeCutFruitText.Length; i++)
                {
                    // and set all of the elements to enabled!
                    arcadeModeCutFruitText[i].enabled = true;
                }
                break;
            //if GameController's gameMode equals GameModes.Relax then...
            case GameModes.Relax:

                //we loop back through an array of the text elements...
                for (int i = 0; i < relaxModeCutFruitText.Length; i++)
                {
                    // and set all of the elements to enabled!
                    relaxModeCutFruitText[i].enabled = true;
                }

                break;
            default:
                //no action if some other action happens... We are all done here :)
                break;
        }
    }

}
