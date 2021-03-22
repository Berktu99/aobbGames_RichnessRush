using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThirdPersonCharController : MonoBehaviour
{
    //public int coinAmount ;

    //public Text coinAmountText;

    public Joystick joystick;

    public float currentSpeed;

    private bool playerIsStunted;

    [HideInInspector]
    public bool isWalking;
    [HideInInspector]
    public bool isWalkingOnWater;

    #region TPP Move variables
    public float waterSpeed = 2.5f;
    public float speed = 5f;
    public float jumpHeight = 2f;
    public float waterJumpHeight = 1f;
    public float Gravity = -9.81f;
    public float GroundDistance = 0.2f;
    public float DashDistance = 5f;
    public LayerMask Ground;
    public Vector3 Drag;
    private bool jumpedFromWater, jumpedFromNormalGround;
    private CharacterController characterController;
    private Vector3 velocity;
    private bool isGrounded = true;
    private Transform _groundChecker;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVel;
    [SerializeField] private float groundCheckRayRange = 300f;
    private bool isAboveWater, isAboveNormalGround;
    #endregion

    public Animator animator;

    private void Awake()
    { 
        playerIsStunted = false;

        jumpedFromNormalGround = true;
        jumpedFromWater = false;
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        _groundChecker = transform.GetChild(0);

        move = Vector3.zero;
    }

    public Vector3 move;
    public bool isBot;

    private void LateUpdate()
    {
        if (isBot)
        {
            //manageSounds();

            manageAnimation();
            //Debug.Log(coinAmount);

            Vector3 DirectionRay = transform.TransformDirection(Vector3.down);
            Debug.DrawRay(transform.position, DirectionRay * groundCheckRayRange, Color.blue);
            RaycastHit Hit;
            if (Physics.Raycast(transform.position, DirectionRay, out Hit, groundCheckRayRange))
            {
                if (Hit.collider.CompareTag("WaterGround"))
                {
                    isAboveWater = true;
                    isAboveNormalGround = false;

                    //Debug.Log("Raycast hits water");
                }
                else if (Hit.collider.CompareTag("NormalGround"))
                {
                    isAboveWater = false;
                    isAboveNormalGround = true;

                    //Debug.Log("Raycast hits normal ground");
                }
            }


            isGrounded = Physics.CheckSphere(_groundChecker.position, GroundDistance, Ground);
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = 0f;
            }

            //new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;


            if (Input.GetButtonDown("Jump") && isGrounded && !playerIsStunted)
            {
                if (isAboveNormalGround)
                {
                    velocity.y += Mathf.Sqrt(jumpHeight * -2f * Gravity);
                    jumpedFromWater = false;
                    jumpedFromNormalGround = true;
                }
                if (isAboveWater)
                {
                    velocity.y += Mathf.Sqrt(waterJumpHeight * -2f * Gravity);
                    jumpedFromWater = true;
                    jumpedFromNormalGround = false;
                }
            }

            if (move.magnitude >= 0.1f && !playerIsStunted)
            {
                //Debug.Log("HEY");

                float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;

                if (isGrounded)
                {
                    if (isAboveNormalGround)
                    {
                        isWalking = true;
                        isWalkingOnWater = false;
                        //Debug.Log("Move normal ground");

                        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVel, turnSmoothTime);
                        transform.rotation = Quaternion.Euler(0f, angle, 0f);

                        Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                        characterController.Move(moveDir * speed * Time.deltaTime);

                        animator.SetFloat("MoveSpeed", move.magnitude);
                        //animator.SetFloat("MoveSpeed", 1f);
                    }
                    else if (isAboveWater)
                    {
                        isWalking = true;
                        isWalkingOnWater = true;
                        //Debug.Log("Move water");

                        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVel, turnSmoothTime);
                        transform.rotation = Quaternion.Euler(0f, angle, 0f);

                        Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                        characterController.Move(moveDir * waterSpeed * Time.deltaTime);

                        animator.SetFloat("MoveSpeed", move.magnitude);
                        //animator.SetFloat("MoveSpeed", 0.4f);
                    }
                }
                else
                {
                    isWalking = false;
                    isWalkingOnWater = false;

                    if (jumpedFromNormalGround)
                    {
                        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVel, turnSmoothTime);
                        transform.rotation = Quaternion.Euler(0f, angle, 0f);

                        Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                        characterController.Move(moveDir * speed * Time.deltaTime);


                    }
                    else if (jumpedFromWater)
                    {
                        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVel, turnSmoothTime);
                        transform.rotation = Quaternion.Euler(0f, angle, 0f);

                        Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                        characterController.Move(moveDir * waterSpeed * Time.deltaTime);
                    }
                }
            }
            else if (move.magnitude <= 0.1f)
            {
                isWalking = false;
                animator.SetFloat("MoveSpeed", 0f);
            }


            if (playerIsStunted)
            {
                animator.SetFloat("MoveSpeed", 0f);
            }


            // Gravity 
            velocity.y += Gravity * Time.deltaTime;

            velocity.x /= 1 + Drag.x * Time.deltaTime;
            velocity.y /= 1 + Drag.y * Time.deltaTime;
            velocity.z /= 1 + Drag.z * Time.deltaTime;

            characterController.Move(velocity * Time.deltaTime);
        }
    }

    void Update()
    {
        if (!isBot)
        {
            //manageSounds();

            manageAnimation();
            //Debug.Log(coinAmount);

            Vector3 DirectionRay = transform.TransformDirection(Vector3.down);
            Debug.DrawRay(transform.position, DirectionRay * groundCheckRayRange, Color.blue);
            RaycastHit Hit;
            if (Physics.Raycast(transform.position, DirectionRay, out Hit, groundCheckRayRange))
            {
                if (Hit.collider.CompareTag("WaterGround"))
                {
                    isAboveWater = true;
                    isAboveNormalGround = false;

                    //Debug.Log("Raycast hits water");
                }
                else if (Hit.collider.CompareTag("NormalGround"))
                {
                    isAboveWater = false;
                    isAboveNormalGround = true;

                    //Debug.Log("Raycast hits normal ground");
                }
            }


            isGrounded = Physics.CheckSphere(_groundChecker.position, GroundDistance, Ground);
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = 0f;
            }

            move = new Vector3(joystick.Horizontal, 0f, joystick.Vertical); //new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;


            if (Input.GetButtonDown("Jump") && isGrounded && !playerIsStunted)
            {
                if (isAboveNormalGround)
                {
                    velocity.y += Mathf.Sqrt(jumpHeight * -2f * Gravity);
                    jumpedFromWater = false;
                    jumpedFromNormalGround = true;
                }
                if (isAboveWater)
                {
                    velocity.y += Mathf.Sqrt(waterJumpHeight * -2f * Gravity);
                    jumpedFromWater = true;
                    jumpedFromNormalGround = false;
                }
            }

            if (move.magnitude >= 0.1f && !playerIsStunted)
            {
                //Debug.Log("HEY");

                float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;

                if (isGrounded)
                {
                    if (isAboveNormalGround)
                    {
                        isWalking = true;
                        isWalkingOnWater = false;
                        //Debug.Log("Move normal ground");

                        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVel, turnSmoothTime);
                        transform.rotation = Quaternion.Euler(0f, angle, 0f);

                        Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                        characterController.Move(moveDir * speed * Time.deltaTime);

                        animator.SetFloat("MoveSpeed", move.magnitude);
                        //animator.SetFloat("MoveSpeed", 1f);
                    }
                    else if (isAboveWater)
                    {
                        isWalking = true;
                        isWalkingOnWater = true;
                        //Debug.Log("Move water");

                        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVel, turnSmoothTime);
                        transform.rotation = Quaternion.Euler(0f, angle, 0f);

                        Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                        characterController.Move(moveDir * waterSpeed * Time.deltaTime);

                        animator.SetFloat("MoveSpeed", move.magnitude);
                        //animator.SetFloat("MoveSpeed", 0.4f);
                    }
                }
                else
                {
                    isWalking = false;
                    isWalkingOnWater = false;

                    if (jumpedFromNormalGround)
                    {
                        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVel, turnSmoothTime);
                        transform.rotation = Quaternion.Euler(0f, angle, 0f);

                        Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                        characterController.Move(moveDir * speed * Time.deltaTime);


                    }
                    else if (jumpedFromWater)
                    {
                        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVel, turnSmoothTime);
                        transform.rotation = Quaternion.Euler(0f, angle, 0f);

                        Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                        characterController.Move(moveDir * waterSpeed * Time.deltaTime);
                    }
                }
            }
            else if (move.magnitude <= 0.1f)
            {
                isWalking = false;
                animator.SetFloat("MoveSpeed", 0f);
            }


            if (playerIsStunted)
            {
                animator.SetFloat("MoveSpeed", 0f);
            }


            // Gravity 
            velocity.y += Gravity * Time.deltaTime;

            velocity.x /= 1 + Drag.x * Time.deltaTime;
            velocity.y /= 1 + Drag.y * Time.deltaTime;
            velocity.z /= 1 + Drag.z * Time.deltaTime;

            characterController.Move(velocity * Time.deltaTime);
        }        
    }

    public void stunPlayer()
    {
        playerIsStunted = true;
    }

    public void unstunPlayer()
    {
        playerIsStunted = false;
    }

    public void jumpPlayer()
    {
        if (isGrounded && !playerIsStunted)
        {
            if (isAboveNormalGround)
            {
                velocity.y += Mathf.Sqrt(jumpHeight * -2f * Gravity);
                jumpedFromWater = false;
                jumpedFromNormalGround = true;
            }
            if (isAboveWater)
            {
                velocity.y += Mathf.Sqrt(waterJumpHeight * -2f * Gravity);
                jumpedFromWater = true;
                jumpedFromNormalGround = false;
            }
        }
    }

    private void manageAnimation()
    {
        if (isGrounded)
        {
            animator.SetBool("Grounded", true);
        }
        else
        {
            animator.SetBool("Grounded", false);
        }
    }

    public void dashPlayer()
    {
        //Debug.Log("Dash");

        characterController.Move(transform.forward * Time.deltaTime * DashDistance);

        //velocity += Vector3.Scale(transform.forward, DashDistance * Vector3.one); //new Vector3((Mathf.Log(1f / (Time.deltaTime * Drag.x + 1)) / -Time.deltaTime), 0, (Mathf.Log(1f / (Time.deltaTime * Drag.z + 1)) / -Time.deltaTime)));
    }

    public void becomeImmune(float time)
    {
        StartCoroutine(immunity(time));
    }

    private IEnumerator immunity(float time)
    {
        yield return null;

        float _waterSpeed = waterSpeed;
        float _waterJumpHeight = waterJumpHeight;

        while (time >= 0)
        {
            waterSpeed = speed;
            waterJumpHeight = jumpHeight;

            time -= Time.deltaTime;

            yield return new WaitForSeconds(Time.deltaTime);
        }

        waterSpeed = _waterSpeed;
        waterJumpHeight = _waterJumpHeight;
    }


    //private void manageSounds()
    //{
    //    if (isWalking)
    //    {
    //        if (isWalkingOnWater)
    //        {
    //            if (!audioSource.isPlaying)
    //            {
    //                audioSource.clip = waterWalking[Random.Range(0, waterWalking.Length)];
    //                audioSource.Play();
    //            }
    //            else
    //            {
    //                for (int i=0; i<normalGroundWalking.Length; i++)
    //                {
    //                    if (normalGroundWalking[i].name == audioSource.clip.name)
    //                    {
    //                        audioSource.Stop();
    //                        audioSource.clip = waterWalking[Random.Range(0, waterWalking.Length)];
    //                        audioSource.Play();
    //                    }
    //                }
                    
    //            }
    //        }
    //        else
    //        {
    //            if (!audioSource.isPlaying)
    //            {
    //                audioSource.clip = normalGroundWalking[Random.Range(0, normalGroundWalking.Length)];
    //                audioSource.Play();
    //            }
    //            else
    //            {
    //                for (int i = 0; i < waterWalking.Length; i++)
    //                {
    //                    if (waterWalking[i].name == audioSource.clip.name)
    //                    {
    //                        audioSource.Stop();
    //                        audioSource.clip = normalGroundWalking[Random.Range(0, normalGroundWalking.Length)];
    //                        audioSource.Play();
    //                    }
    //                }

    //            }
    //        }
    //    }        
    //}



    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Coin"))
    //    {
    //        //Debug.log("Collect coin");
    //        coinAmount += 100;

    //        coinAmountText.text = coinAmount.ToString();

    //        Destroy(other.gameObject);
    //    }
    //    else if (other.CompareTag("Teleporter"))
    //    {
    //        if (coinAmount >= 100)
    //        {
    //            //Debug.Log("Teleport");

    //            characterController.enabled = false;
    //            transform.position = other.transform.GetChild(1).position;
    //            characterController.enabled = true;
    //        }
    //    }
    //    //else if (other.CompareTag("BombPowerUp"))
    //    //{
    //    //    if (!playerPossesPowerUp)
    //    //    {
    //    //        playerPossesPowerUp = true;
    //    //        Destroy(other.gameObject);
    //    //        currentPowerUp = PowerUps.BombPowerUp;

    //    //        powerUpButtonImg.sprite = bombPowerUpButtonImg;
    //    //    }
    //    //}
    //    //else if (other.CompareTag("ProjectilePowerUp"))
    //    //{
    //    //    if (!playerPossesPowerUp)
    //    //    {
    //    //        playerPossesPowerUp = true;
    //    //        Destroy(other.gameObject);
    //    //        currentPowerUp = PowerUps.ProjectilePowerUp;

    //    //        powerUpButtonImg.sprite = projectilePowerUpButtonImg;
    //    //    }
    //    //}
    //}

    //public void usePowerUp()
    //{
    //    if (playerPossesPowerUp)
    //    {
    //        if (currentPowerUp == PowerUps.BombPowerUp)
    //        {
    //            useBombPowerUp();
    //        }
    //        if (currentPowerUp == PowerUps.ProjectilePowerUp)
    //        {
    //            useProjectilePowerUp();
    //        }
    //    }     
    //}

    //private void useBombPowerUp()
    //{
    //    // Instantiate and send bomb opposite to player direction

    //    currentPowerUp = PowerUps.None;
    //    GameObject holder = Instantiate(bombPowerUpPrefab, transform.position, Quaternion.identity);

    //    Vector3 force = new Vector3(0f, 1f, -transform.position.z);

    //    holder.GetComponent<Rigidbody>().AddForce(force * holder.GetComponent<TrapBomb>().force, ForceMode.Impulse);

    //    powerUpButtonImg.sprite = emptyPowerUpSprite;
    //}

    //public void caughtInBombExplosion()
    //{
    //    Debug.Log("Caught in explosion");
    //}

    //public void isInBlastRadius()
    //{
    //    Debug.Log("Is in blast radius");
    //}

    

    //private void useProjectilePowerUp()
    //{
    //    currentPowerUp = PowerUps.None;
    //    GameObject holder = Instantiate(projectilePowerUpPrefab, transform.position, Quaternion.identity);

    //    Vector3 velocity = new Vector3(0f, 0f, transform.position.z);

    //    holder.GetComponent<Rigidbody>().velocity = velocity;

    //    powerUpButtonImg.sprite = emptyPowerUpSprite;
    //}

}
