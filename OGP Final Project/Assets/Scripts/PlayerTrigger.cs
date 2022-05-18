using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CMF;

public class PlayerTrigger : MonoBehaviour
{
    [Tooltip("How long the speed boost is on")]
    [SerializeField] private float speedBoostLength = 3f;
    [Tooltip("Speed boost multiplier")]
    [SerializeField] private float speedBoostMultiplier = 2f;
    [Tooltip("Jump multiplier")]
    [SerializeField] private float jumpMultiplier = 2f;
    [Tooltip("Initial player state (must be 'NotBoosted')")]
    [SerializeField] private float jumpLength = 0.2f;
    [SerializeField] SpeedBoostState playerSpeedBoostState = SpeedBoostState.NotBoosted;
    [SerializeField] JumptState playerJumpState = JumptState.NotJumped;
    private Rigidbody rigidBody;
    private AdvancedWalkerController advancedWalkerController;
    private float previousForwardSpeed;
    private float previousJumpSpeed;
    private float previousJumpDuration;
    public AudioSource Source;
    public AudioClip BoostClip;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        advancedWalkerController = GetComponent<AdvancedWalkerController>(); // Getting the player movement component
        previousJumpDuration = advancedWalkerController.jumpDuration;
        previousJumpSpeed = advancedWalkerController.jumpSpeed;
    }

    public enum SpeedBoostState
    {
        Boosted,
        NotBoosted
    }

    public enum JumptState
    {
        Jumped,
        NotJumped
    }

    private void OnTriggerEnter(Collider other)
    {
        // Boosts the player's speed using the parameters defined in the editor and then restores the initial speed
        if (playerSpeedBoostState == SpeedBoostState.NotBoosted && other.CompareTag("SpeedBoostTrigger"))
        {
            //Debug.Log("SpeedBoostTrigger");
            previousForwardSpeed = advancedWalkerController.movementSpeed;
            advancedWalkerController.movementSpeed *= speedBoostMultiplier;
            playerSpeedBoostState = SpeedBoostState.Boosted;
            StartCoroutine(SpeedBoostTimer());
            if (Source != null)
                Source.PlayOneShot(BoostClip);
        }
        // Makes the player to do a reinforced jump using the multiplicator defined in the editor
        // Speed boost is disabled after trigger
        else if (playerJumpState == JumptState.NotJumped && other.CompareTag("JumpTrigger"))
        {
            //Debug.Log("JumpTrigger");
            if (playerSpeedBoostState == SpeedBoostState.Boosted)
                advancedWalkerController.movementSpeed = previousForwardSpeed;
            playerJumpState = JumptState.Jumped;
            advancedWalkerController.jumpKeyWasPressed = true;
            advancedWalkerController.jumpKeyIsPressed = true;
            advancedWalkerController.jumpDuration = jumpLength;
            advancedWalkerController.jumpSpeed *= jumpMultiplier;
            StartCoroutine(JumpTimer());
            //rigidBody.AddForce(other.transform.transform.up.normalized * advancedWalkerController.jumpSpeed * jumpMultiplier, ForceMode.Impulse);
        }
    }

    // Restores the initial speed after speedBoostLength seconds
    IEnumerator SpeedBoostTimer()
    {
        yield return new WaitForSeconds(speedBoostLength);
        advancedWalkerController.movementSpeed = previousForwardSpeed;
        playerSpeedBoostState = SpeedBoostState.NotBoosted;
    }

    IEnumerator JumpTimer()
    {
        yield return new WaitForSeconds(jumpLength);
        advancedWalkerController.jumpDuration = previousJumpDuration;
        advancedWalkerController.jumpSpeed = previousJumpSpeed;
        playerJumpState = JumptState.NotJumped;
    }
}
