using UnityEngine;

/// <summary>
/// This is an Enum that contains the different possible types of Bombs/Power-ups
/// </summary>
public enum ObjectType
{
    TwoTimesScoreBanana,                    // our 2xScore Banana PowerUp
    FrenzyBanana,                           // our Frenzy Banana PowerUp
    FreezeBanana,                           // our Freeze Banana PowerUp
    MinusTenBomb,                           // our "-10" Bomb
    GameOverBomb                            // our "Game Over" Bomb
};
/// <summary>
/// The BombDestroy Class handles all of the destruction, destruction effects, and the game effects of the different
/// Bombs and PowerUps.  
/// </summary>
public class DestroyBombOrPowerUp : MonoBehaviour
{
    [Header("Choose what Type of Bomb/PowerUp Object this is")]

    public ObjectType oType;                                            // the selected ObjectType (oType) that is selected on this prefab, in inspector"
    private GameObject thisGameObject;                                  // reference to the current gameObject.
    private FreezeEffect freezeEffectGameObject;                        // a reference to the FreezeEffect gameObject in our scene.
    private FrenzyEffect frenzyEffectGameObject;                        // a reference to the FrenzyEffect gameObject in our scene.
    private TwoTimesScoreEffect twoTimesScoreEffectGameObject;          // a reference to the twoTimesScoreEffect gameObject in our scene.

    [Header("Minus10BombAnimationGO")]

    public GameObject minusTenBombGibs;                                 // our prefab that has the -10 bomb explode animation; throwing gibs

    [Header("GameOverBombAnimationGO")]

    public GameObject gameOverBombGibs;                                 // our prefab that has the game over bomb explode animation; throwing gibs.

    [Header("ParticleSystemsToInstantiate")]

    public GameObject twoTimesScoreParticleEffect;                      // a reference to the particle effect for this power-ups destruction.
    public GameObject frenzyParticleEffect;                             // a reference to the particle effect for this power-ups destruction.
    public GameObject freezeParticleEffect;                             // a reference to the particle effect for this power-ups destruction.
    public GameObject minusTenParticleEffect;                           // a reference to the particle effect for this power-ups destruction.
    public GameObject gameOverBombParticleEffect;                       // a reference to the particle effect for this power-ups destruction.

    [Header("These References Set themselves up")]

    public ChromaticAberration chroma;                                  // the reference to our chromatic aberration script on the MainCam
    public SimpleCameraShake shake;                                     // the reference to our camera shake script on the MainCam

    void Awake()
    {
        //I usually use awake for setting up references, so we will do that first.
        //the next three lines will get a reference to the 3 gameObjects in the game scenes that control the powerUp
        //effects.  These gameObjects each have a spriteRenderer that is used to display the powerUp labels and some have
        //borders (like Freeze Effect).

        //all of the tags in the game are stored in [const readonly strings], in our Tags Class.  There was quite a few
        //different tags, and since I would prefer the scripts to autoComplete, I created read only strings for them.
        freezeEffectGameObject = GameObject.FindGameObjectWithTag(Tags.freezeEffectGameObjectTag).GetComponent<FreezeEffect>();
        frenzyEffectGameObject = GameObject.FindGameObjectWithTag(Tags.frenzyEffectGameObjectTag).GetComponent<FrenzyEffect>();
        twoTimesScoreEffectGameObject = GameObject.FindGameObjectWithTag(Tags.twoTimesScoreEffectGameObjectTag).GetComponent<TwoTimesScoreEffect>();

        //cache a reference to this gameObject.
        thisGameObject = this.gameObject;

        //Setup Shake and Chroma Script References... both scripts are on the main camera
        shake = Camera.main.GetComponent<SimpleCameraShake>();
        chroma = Camera.main.GetComponent<ChromaticAberration>();
    }



    /// <summary>
    /// This Method is called when one of our Bomb/Power-up objects is destroyed.  It's called from our
    /// FNCTouchSlicer when they collider or ray-cast comes into contact with the object(PowerUp or Bomb).
    /// The method is called, and then whatever type of object the prefab is setup as, determines which
    /// case is used, and subsequently which Methods are called(i.e. what explosion types/or image effects
    /// are used.)
    /// </summary>
    public void ActivateDestructionPerObjecType()
    {
        switch (oType)
        {
            case ObjectType.TwoTimesScoreBanana:
                TwoTimesScoreBananaDestroy();
                //2xScore Method Below

                break;
            case ObjectType.FrenzyBanana:
                FenzyBananaDestroy();
                //2xScore Method Below

                break;
            case ObjectType.FreezeBanana:
                FreezeBananaDestroy();
                //2xScore Method Below

                break;
            case ObjectType.MinusTenBomb:
                MinusTenPointsBombDestroy();
                //2xScore Method Below

                break;
            case ObjectType.GameOverBomb:
                GameOverBombDestroy();
                //2xScore Method Below

                break;
            default:
                //NONE
                break;
        }
    }


    /// <summary>
    /// This is the Method to call when destroying a Two Times Score Banana.  It will call a
    /// method on a the GameObject in the scene that holds/activates the "2xScore" sprite renderer,
    /// starts 2x points for about 5-8 seconds, and dims the BackGround. Then the method instantiates
    /// a particle system at the hit/death location, and then SetActive(false)'s the object.
    /// </summary>
    void TwoTimesScoreBananaDestroy()
    {
        //The start effect gets called on the scene 2xScoreEffect object (starts the "2xScore" sprite
        //renderer, and dims the BG.
        twoTimesScoreEffectGameObject.StartEffect();
        //We instantiate the star and blast ring effect that is used for all of the Banana PowerUps
        //it simple, and there aren't any gibs used... would look better with some banana gibs, maybe we
        //could make some simple shapes that use the UV space of the respective objects texture.  Left
        //out for now.
        Instantiate(twoTimesScoreParticleEffect, transform.position - new Vector3(0, 0, 20), Quaternion.identity);
        //set this object inactive so that it can be called from the pool again.
        thisGameObject.SetActive(false);
    }

    /// <summary>
    /// This is the Method to call when destroying a Frenzy Banana.  It will call a method on a the GameObject
    /// in the scene that holds/activates the "Frenzy" sprite renderer, starts the Frenzy Fruit Spawner's for 5-8 
    /// seconds, and makes the BG change color rapidly. Then the method instantiates a particle system at the hit or
    /// death location, and then SetActive(false)'s the object.
    /// </summary>
    void FenzyBananaDestroy()
    {
        //The start effect gets called on the scene FrenzyEffect object (starts the "Frenzy" Sprite Renderer,
        //and changes BG color.
        frenzyEffectGameObject.StartEffect();
        //We instantiate the star and blast ring effect that is used for all of the Banana PowerUps
        //it simple, and there aren't any gibs used... would look better with some banana gibs, maybe we
        //could make some simple shapes that use the UV space of the respective objects texture.  Left
        //out for now.
        Instantiate(frenzyParticleEffect, transform.position - new Vector3(0, 0, 20), Quaternion.identity);
        //set this object inactive so that it can be called from the pool again.
        thisGameObject.SetActive(false);
    }


    /// <summary>
    /// This is the Method to call when destroying a Freeze Banana.  It will call a method on the GameObject
    /// in the scene that holds/activates the "Freeze" sprite renderer, the ice border, and snow particle system. It 
    /// also slows time for 5-8 seconds.  Then the method instantiates a particle system at the hit or death location,
    /// and then SetActive(false)'s the object.
    /// </summary>
    void FreezeBananaDestroy()
    {
        //The start effect gets called on the scene FreezeEffect object (starts the "Freeze" and Ice Border
        //Sprite Renderers
        freezeEffectGameObject.StartEffect();
        //We instantiate the star and blast ring effect that is used for all of the Banana PowerUps
        //it simple, and there aren't any gibs used... would look better with some banana gibs, maybe we
        //could make some simple shapes that use the UV space of the respective objects texture.  Left
        //out for now.
        Instantiate(freezeParticleEffect, transform.position - new Vector3(0, 0, 20), Quaternion.identity);
        //set this object inactive so that it can be called from the pool again.
        thisGameObject.SetActive(false);
    }

    /// <summary>
    /// This is the Method to call when destroying a GameOver Bomb in classic mode.  Then it calls a method
    /// that ends the game in the game controller.  We instantiate an animated bomb explosion(gibs), and a explosion
    /// effect particle system.  Next we call subtle camera shake, and chromatic aberration methods.  Lastly
    /// it calls a Method that creates a blast wave that pushes fruit away.  Then we SetActive(false) the object
    /// and we are done.
    /// </summary>
    /// This Method takes one parameter(the bomb destroys do, so that we can send the correct rotation for the
    /// animated Gameobject instantiation.
    /// <param name="objRot"></param>
    void GameOverBombDestroy()
    {
        //Call this method on the GameController Static Reference so that the game ends
        //(the game over bomb is only in classic mode and if its hit, it ends the game)
        GameController.GameControllerInstance.TakeFruitAndEndGame();

        //Instantiate the minusTenBombGibs (Which is a prefab that is set to play a unity animation
        //that does not loop, and that makes it look like pieces of the bomb are flying outward.
        Instantiate(gameOverBombGibs, transform.position, Quaternion.identity);
        //Instantiate our Bomb Explosion Particle System.  
        Instantiate(gameOverBombParticleEffect, transform.position - new Vector3(0, 0, 20), Quaternion.identity);

        //call our Start Camera Shake Method;
        shake.StartShake();
        //call our Start Chromatic Aberration Method;
        chroma.StartAberration();

        //Call the Method at the bottom of this class, which adds an outward force from the 
        //explosion which only effects Fruit Tagged Objects
        ExplosionBlastForce();
        //Set this object inactive so we can use it again.
        thisGameObject.SetActive(false);
    }


    /// <summary>
    /// This is the Method to call when destroying a Minus Ten Points Bomb in arcade mode.  Then we make sure
    /// there are 10 points to remove from the score.  If not we set the score to 0.  We instantiate an animated 
    /// bomb explosion(gibs), and a explosion effect particle system.  Next we call subtle camera shake, and 
    /// chromatic aberration methods.  Lastly it calls a Method that creates a blast wave that pushes fruit away.
    /// Then we SetActive(false) the object, and we are done.
    /// </summary>
    void MinusTenPointsBombDestroy()
    {
        // if the score is greater than 10 then...
        if (GameVariables.ArcadeModeScore > 10)
        {
            //we know we can remove 10 points;
            GameVariables.ArcadeModeScore -= 10;
        }
        //if the player score is less than 10 points...
        else
        {
            //lets just make the score 0;
            GameVariables.ArcadeModeScore = 0;
        }

        //Instantiate the minusTenBombGibs (Which is a prefab that is set to play a unity animation
        //that does not loop, and that makes it look like pieces of the bomb are flying outward.
        Instantiate(minusTenBombGibs, transform.position, Quaternion.identity);
        //Instantiate our Bomb Explosion Particle System.  
        Instantiate(minusTenParticleEffect, transform.position - new Vector3(0, 0, 20), Quaternion.identity);

        //call our Start Camera Shake Method;
        shake.StartShake();
        //call our Start Chromatic Aberration Method;
        chroma.StartAberration();

        //Call the Method at the bottom of this class, which adds an outward force from the 
        //explosion which only effects Fruit Tagged Objects
        ExplosionBlastForce();
        //Set this object inactive so we can use it again.
        thisGameObject.SetActive(false);
    }

    /// <summary>
    /// This method is responsible for adding force to nearby fruit.  It is called when either
    /// type of bomb explodes.
    /// </summary>
    public void ExplosionBlastForce()
    {
        //create 2 new floats.  
        //We will use this for the radius of the explosion.
        float radius = 48f;
        //we will use this for the force of the explosion
        float explosionForce = 3f;
        //create a new vector3 and zero it out.
        Vector3 explosionOrigin = new Vector3(0, 0, 0);
        //assign the transform.position of the bomb(when this method is called) to explosionOrigin.
        explosionOrigin = transform.position;
        //lets create an array of colliders and then check Physics.OverlapSphere and pass that function
        //our explosionOrigin, and our desired radius from earlier.  
        Collider[] colliders = Physics.OverlapSphere(explosionOrigin, radius);

        //now that we have our array of colliders within our radius and at our position we will start
        //by looping through them.
        for (int i = 0; i < colliders.Length; i++)
        {
            //we check to make sure a few conditions are met when looping through our array of "hit"
            //colliders.  We make sure that the colliders we hit are tagged "Fruit", and we make sure
            //that they do have a rigidbody(because we are going to attempt to addForce() to the hit
            //colliders.
            if (colliders[i].CompareTag("Fruit") && colliders[i].GetComponent<Rigidbody>() != null)
            {

                //now that we know we are only talking about our "Fruit" tagged Rigidbodies we...

                //create a new vector3 and we subtract our transform.position from the hit colliders
                //position(this vector3 basically now stores the direction we are going to "Push"
                //the fruit in with addForce()
                Vector3 pushDir = colliders[i].transform.position - transform.position;

                //we setup a quick final reference to the hit colliders Rigidbody so that we can apply
                //force and torque to the fruits
                Rigidbody hitRB = colliders[i].GetComponent<Rigidbody>();

                //commented out debug info for release time
                //Debug.Log(colliders[i].name.ToString());

                //here is where we addForce to our hit fruit collider.  The vector3 that we pass addForce()
                //is our pushDir from earlier multiplied by our explosionForce float.  We use ForceMode.Impulse 
                // so that its kind of a "Punch" of force pushing the objects away from the explosion position.
                //Then we add a little bit of spin to the fruit using addTorque and feeding Random.insideUnitSphere,
                //while again using ForceMode.Impulse;

                hitRB.AddForce(pushDir * explosionForce, ForceMode.Impulse);
                hitRB.AddTorque(Random.insideUnitSphere, ForceMode.Impulse);

            }

            //we do these steps for all of the fruit with colliders/rigidbodies we find within our radius.

        }
    }



}
