using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundsManager : MonoBehaviour
{
    public Animator playerAnimator;

    private AudioSource audioSource;

    public AudioClip[] normalGroundWalkingSounds;
    public AudioClip[] waterWalkingSounds;
    public AudioClip[] normalGroundRunningSounds;
    public AudioClip[] waterRunningSounds;

    private ThirdPersonCharController playerController;

    private void Awake()
    {
        playerController = GetComponentInParent<ThirdPersonCharController>();

        audioSource = GetComponent<AudioSource>();
    }

    public void FootOnGround()
    {
        if (playerController. isWalking)
        {
            if (playerController.isWalkingOnWater)
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.clip = waterWalkingSounds[Random.Range(0, waterWalkingSounds.Length)];
                    audioSource.Play();
                }
                else
                {
                    for (int i = 0; i < normalGroundWalkingSounds.Length; i++)
                    {
                        if (normalGroundWalkingSounds[i].name == audioSource.clip.name)
                        {
                            audioSource.Stop();
                            audioSource.clip = waterWalkingSounds[Random.Range(0, waterWalkingSounds.Length)];
                            audioSource.Play();
                        }
                    }

                }
            }
            else
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.clip = normalGroundWalkingSounds[Random.Range(0, normalGroundWalkingSounds.Length)];
                    audioSource.Play();
                }
                else
                {
                    for (int i = 0; i < waterWalkingSounds.Length; i++)
                    {
                        if (waterWalkingSounds[i].name == audioSource.clip.name)
                        {
                            audioSource.Stop();
                            audioSource.clip = normalGroundWalkingSounds[Random.Range(0, normalGroundWalkingSounds.Length)];
                            audioSource.Play();
                        }
                    }

                }
            }
        }
    }


}
