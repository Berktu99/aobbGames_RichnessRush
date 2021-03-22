using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPowersManager : MonoBehaviour
{
    public int playerSerialNumber;

    public int coinAmount;

    public Text coinAmountText;

    private CharacterController characterController;

    public GameObject stateIndicator;
    public Sprite dangerIndicator, stuntedIndicator;

    public GameObject coins;
    public int coinWorth = 100;

    private enum PowerUps
    {
        None,
        BombPowerUp,
        ImmunityPowerUp,
        DashPowerUp,
    }

    private PowerUps currentPowerUp;

    private bool playerPossesPowerUp;

    private bool isNotStunted;

    // Empty
    public Image powerUpButtonImg;
    public Sprite emptyPowerUpSprite;
    public Text powerUpCostText;

    // Teleport
    public int teleportCost;

    // Bomb
    public GameObject bombPowerUpPrefab;
    public Sprite bombPowerUpButtonImg;
    public int bombPowerUpCost;
    public int coinLossAmountBomb;
    public float bombStunTime;
    //private bool caughtInBlast;

    // Projectile
    public GameObject projectilePrefab;
    public Text projectileCostText;
    public int projectileCost;
    public Image projectileButtonImg;
    public Transform projectileSpawnPoint;
    public int coinLossAmountProjectile;
    public float projectileHitStunTime;

    // Immnuity
    public Sprite immunityPowerButtonImg;
    public int immunityPowerUpCost;
    public float immunityTime;

    // Dash
    public Sprite dashPowerButtonImg;
    public int dashPowerCost;
    public int numberOfDashUses;
    private int _numberOfDashUses;

    AudioManager audioManager;
    

    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();

        isNotStunted = true;

        //caughtInBlast = false;

        projectileCostText.text = projectileCost.ToString();

        characterController = GetComponent<CharacterController>();

        coinAmount = 0;

        coinAmountText.text = coinAmount.ToString();

        playerPossesPowerUp = false;

        currentPowerUp = PowerUps.None;

    }

    private void Start()
    {
        _numberOfDashUses = numberOfDashUses;

        PlayerPowersManager[] holder = FindObjectsOfType<PlayerPowersManager>();

        int i = 0;
        foreach (PlayerPowersManager t in holder)
        {
            t.playerSerialNumber = i;
            i++;
        }
    }

    

    private void Update()
    {
        indicatorManager();
        manageButtons();
        coinAmountText.text = coinAmount.ToString();

        if (Input.GetKeyDown("c"))
        {
            dropCoins(coinLossAmountBomb);
        }
    }

    private void indicatorManager()
    {
        if (isNotStunted)
        {
            stateIndicator.GetComponent<SpriteRenderer>().sprite = null;
        }
        else
        {
            stateIndicator.GetComponent<SpriteRenderer>().sprite = stuntedIndicator;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            audioManager.play("CoinPickUp");

            //Debug.log("Collect coin");
            coinAmount += coinWorth;

            coinAmountText.text = coinAmount.ToString();

            Destroy(other.gameObject);

            
        }
        else if (other.CompareTag("Teleporter"))
        {
            if (coinAmount >= teleportCost)
            {
                //Debug.Log("Teleport");

                audioManager.play("Teleportation");

                coinAmount -= teleportCost;

                characterController.enabled = false;
                transform.position = other.transform.GetChild(1).position;
                characterController.enabled = true;
            }
        }
        else if (other.CompareTag("BombPowerUp"))
        {
            if (!playerPossesPowerUp)
            {
                audioManager.play("PickUp");

                playerPossesPowerUp = true;
                Destroy(other.gameObject);
                currentPowerUp = PowerUps.BombPowerUp;

                powerUpButtonImg.sprite = bombPowerUpButtonImg;
            }
        }
        else if (other.CompareTag("ImmunityPowerUp"))
        {
            if (!playerPossesPowerUp)
            {
                audioManager.play("PickUp");

                playerPossesPowerUp = true;
                Destroy(other.gameObject);
                currentPowerUp = PowerUps.ImmunityPowerUp;

                powerUpButtonImg.sprite = immunityPowerButtonImg;
            }
        }
        else if (other.CompareTag("DashPowerUp"))
        {
            if (!playerPossesPowerUp)
            {
                audioManager.play("PickUp");

                playerPossesPowerUp = true;
                Destroy(other.gameObject);
                currentPowerUp = PowerUps.DashPowerUp;

                powerUpButtonImg.sprite = dashPowerButtonImg;
            }
        }
        else if (other.CompareTag("Projectile"))
        {
            if (playerSerialNumber != other.GetComponent<Projectile>().serialNumber)
            {
                Destroy(other.gameObject);
                audioManager.play("PlayerHitByProjectile");

                dropCoins(coinLossAmountProjectile);

                StartCoroutine(stunPlayerCouroutine(projectileHitStunTime));
            }
        }
    }

    private void manageButtons()
    {
        manageProjectileButton();
        managePowerUpButton();
    }

    private void managePowerUpButton()
    {
        if (currentPowerUp == PowerUps.BombPowerUp)
        {
            powerUpCostText.text = bombPowerUpCost.ToString();

            if (coinAmount >= projectileCost)
            {
                powerUpButtonImg.color = new Color(powerUpButtonImg.color.r, powerUpButtonImg.color.g, powerUpButtonImg.color.b, 1f);
            }
            else
            {
                powerUpButtonImg.color = new Color(powerUpButtonImg.color.r, powerUpButtonImg.color.g, powerUpButtonImg.color.b, 40 / 255f);
            }
        }
        else if (currentPowerUp == PowerUps.ImmunityPowerUp)
        {
            powerUpCostText.text = immunityPowerUpCost.ToString();

            if (coinAmount >= projectileCost)
            {
                powerUpButtonImg.color = new Color(powerUpButtonImg.color.r, powerUpButtonImg.color.g, powerUpButtonImg.color.b, 1f);
            }
            else
            {
                powerUpButtonImg.color = new Color(powerUpButtonImg.color.r, powerUpButtonImg.color.g, powerUpButtonImg.color.b, 40 / 255f);
            }
        }
        else if (currentPowerUp == PowerUps.DashPowerUp)
        {
            powerUpCostText.text = dashPowerCost.ToString();

            if (coinAmount >= projectileCost)
            {
                powerUpButtonImg.color = new Color(powerUpButtonImg.color.r, powerUpButtonImg.color.g, powerUpButtonImg.color.b, 1f);
            }
            else
            {
                powerUpButtonImg.color = new Color(powerUpButtonImg.color.r, powerUpButtonImg.color.g, powerUpButtonImg.color.b, 40 / 255f);
            }
        }
        else if (currentPowerUp == PowerUps.None)
        {
            powerUpCostText.text = "";

            powerUpButtonImg.sprite = emptyPowerUpSprite;
        }
    }



    private void stunPlayer()
    {
        isNotStunted = false;

        GetComponent<ThirdPersonCharController>().stunPlayer();

        stateIndicator.GetComponent<SpriteRenderer>().sprite = stuntedIndicator;
    }

    private void unstunPlayer()
    {
        isNotStunted = true;

        GetComponent<ThirdPersonCharController>().unstunPlayer();

        stateIndicator.GetComponent<SpriteRenderer>().sprite = null;
    }

    public void usePowerUp()
    {
        if (playerPossesPowerUp && isNotStunted)
        {
            if (currentPowerUp == PowerUps.BombPowerUp)
            {
                if (coinAmount >= bombPowerUpCost)
                {
                    useBombPowerUp();
                }                
            }
            else if (currentPowerUp == PowerUps.ImmunityPowerUp)
            {
                if (coinAmount >= immunityPowerUpCost)
                {
                    becomeImmune();
                }                
            }
            else if (currentPowerUp == PowerUps.DashPowerUp)
            {
                if (coinAmount >= dashPowerCost)
                {
                    playerDash();
                }                
            }
        }
    }

    private void playerDash()
    {
        //Debug.Log("Dash");

        if (numberOfDashUses > 0)
        {
            coinAmount -= dashPowerCost;

            audioManager.play("DashPowerUp");

            GetComponent<ThirdPersonCharController>().dashPlayer();
            numberOfDashUses--;

            if (numberOfDashUses <= 0)
            {
                numberOfDashUses = _numberOfDashUses;

                currentPowerUp = PowerUps.None;

                playerPossesPowerUp = false;
            }
        }
        
    }


    private void useBombPowerUp()
    {
        // Instantiate and send bomb opposite to player direction

        audioManager.play("BombPowerUp");

        coinAmount -= bombPowerUpCost;

        playerPossesPowerUp = false;

        currentPowerUp = PowerUps.None;
        GameObject holder = Instantiate(bombPowerUpPrefab, transform.position, Quaternion.identity);

        Vector3 force = -transform.forward;

        holder.GetComponent<Rigidbody>().AddForce(force * holder.GetComponent<TrapBomb>().force, ForceMode.Impulse);

        powerUpButtonImg.sprite = emptyPowerUpSprite;
    }

    public void caughtInBombExplosion()
    {
        if (currentPowerUp == PowerUps.ImmunityPowerUp)
        {
            Debug.Log("Immnuity protects player from explosion");
        }
        else
        {
            StartCoroutine(stunPlayerCouroutine(bombStunTime));
            dropCoins(bombPowerUpCost);
        }
        
    }

    public void isInBlastRadius()
    {
        //Debug.Log("Is in blast radius");

        stateIndicator.GetComponent<SpriteRenderer>().sprite = dangerIndicator;
        stateIndicator.GetComponent<SpriteRenderer>().color = new Color(255f,0f,0f);
    }


    private void dropCoins(int amount)
    {
        int dropCoinAmount = amount;

        if (dropCoinAmount > coinAmount)
        {
            dropCoinAmount = coinAmount;
        }

        coinAmount -= dropCoinAmount;

        for (int i = amount; i > 0; i -= coinWorth)
        {
            GameObject holder = Instantiate(coins, transform.position, Quaternion.identity);
            holder.AddComponent<Rigidbody>();
            holder.AddComponent<CoinManager>();

            holder.GetComponent<BoxCollider>().isTrigger = false;
            holder.tag = "Coin2";
            holder.layer = 18;

            holder.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-2.5f, 2.5f), 3f, Random.Range(-2.5f, 2.5f)), ForceMode.Impulse);
        }

        audioManager.play("CoinDrop");
    }

    private void becomeImmune()
    {
        //Debug.Log("Immunity");
        coinAmount -= immunityPowerUpCost;

        audioManager.play("ImmunityPowerUp");

        currentPowerUp = PowerUps.None;

        playerPossesPowerUp = false;

        GetComponent<ThirdPersonCharController>().becomeImmune(immunityTime);        
    }




    private void manageProjectileButton()
    {
        if (coinAmount >= projectileCost)
        {
            projectileButtonImg.color = new Color(projectileButtonImg.color.r, projectileButtonImg.color.g, projectileButtonImg.color.b, 1f);
        }
        else
        {
            projectileButtonImg.color = new Color(projectileButtonImg.color.r, projectileButtonImg.color.g, projectileButtonImg.color.b, 40 / 255f);
        }
    }
    public void fireProjectile()
    {
        if (coinAmount >= projectileCost && isNotStunted)
        {
            coinAmount -= projectileCost;

            audioManager.play("ProjectilePowerUp");

            GameObject holder = Instantiate(projectilePrefab, transform);

            holder.GetComponent<Projectile>().serialNumber = playerSerialNumber;
        }        
    }

    private IEnumerator stunPlayerCouroutine(float timer)
    {
        stunPlayer();

        while (timer >= 0f)
        {
            timer -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        unstunPlayer();
    } 
}
