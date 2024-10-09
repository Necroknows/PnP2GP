using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;

public class playerController : MonoBehaviour, IDamage
{
    
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreMask;
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject shootRot;
    [SerializeField] GameObject bullet;
    //[SerializeField] LineRenderer lr;

    [SerializeField] int speed;
    [SerializeField] int sprintMod;
    [SerializeField] int jumpMax;
    [SerializeField] int jumpSpeed;
    [SerializeField] int gravity;
    [SerializeField] int HP;
    [SerializeField] int Ammo;


    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;

    Vector3 move;
    Vector3 playerVel;

    int HPOrig;
    int jumpCount;
    bool isSprinting;
    bool isShooting;

    // Start is called before the first frame update
    void Start()
    {

        HPOrig = HP;
        updatePlayerUI();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);

        if (!GameManager.instance.isPaused)
        {
            movement();
        }
        sprint();
    }

    void movement()
    {
        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVel = Vector3.zero;
        }


        //Input.GetAxis is a built in unity control that is case sensitive
        //move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        //using time.deltaTime does not make movement as frames but true time 
        // transform.position += move * speed * Time.deltaTime;
        move = Input.GetAxis("Vertical") * transform.forward +
             Input.GetAxis("Horizontal") * transform.right;

        controller.Move(move * speed * Time.deltaTime);

        //handling the jump
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVel.y = jumpSpeed;
        }
        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;

        //calling shoot
        if(Input.GetButton("Shoot") && !isShooting)
        StartCoroutine(shoot());
    }

    void sprint()
    {
        if(Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
            isSprinting = true;

        }
        else if(Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
            isSprinting = false;
        }

    }

    IEnumerator shoot()
    {
        isShooting = true;
        Instantiate(bullet, shootPos.position, shootRot.transform.rotation);
        ////lr.useWorldSpace = true;
        ////lr.SetPosition(0, Camera.main.transform.position);

        //// RaycastHit hit;
        //// the main camera is where we shoot from, going straigh forward,if it hit,
        //// how long it is drawn, and to ignore the player
        //if(Physics.Raycast(Camera.main.transform.position, 
        //    Camera.main.transform.forward, out hit, shootDist, ~ignoreMask))
        //{

        //    //lr.SetPosition(1, hit.point);

        //    //returns the name of what we hit
        //   // Debug.Log(hit.collider.name);
        //    //check if obj derives from iDamage
        //    IDamage dmg = hit.collider.GetComponent<IDamage>();

        //    if(dmg != null )
        //    {
        //        dmg.takeDamage(shootDamage);
        //    }
        //}
        yield return new WaitForSeconds(shootRate);
        ////lr.useWorldSpace = false;
        isShooting = false;
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        updatePlayerUI();
        StartCoroutine(flashDamage());
        //I'm dead :c
        if(HP <= 0)
        {
            //GameManager.instance.youLose();
            UIManager.Instance.ShowLoseScreen();
            StopAllCoroutines();
        }
    }

    IEnumerator flashDamage()
    {
        GameManager.instance.flashDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        GameManager.instance.flashDamageScreen.SetActive(false);
    }

    public void updatePlayerUI()
    {
        GameManager.instance.playerHpBar.fillAmount = (float)HP / HPOrig;
    }

    //getters/setters

    public int getHP()
    {
        return HP;
    }

    public int getAmmo()
    {
        return Ammo;
    }

    public void setHP(int amount)
    {
        HP = amount;
    }

    public void setAmmo(int amount)
    {
        Ammo += amount;
    }
}
