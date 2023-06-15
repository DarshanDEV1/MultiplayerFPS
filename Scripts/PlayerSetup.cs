using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public GameObject[] FPS_Hands;
    public GameObject[] FPS_Body;
    public GameObject playerUIPrefab;
    public Camera fpsCamera;
    private Animator animator;
    private Shooting shooter;
    private PlayerMovementController playerMovementController;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        shooter = GetComponent<Shooting>();
        playerMovementController = GetComponent<PlayerMovementController>();
        if(photonView.IsMine)
        {
            foreach (GameObject gameObject in FPS_Hands)
            {
                gameObject.SetActive(true);
                gameObject.transform.SetParent(fpsCamera.transform);
            }

            foreach (GameObject gameObject in FPS_Body)
            {
                gameObject.SetActive(false);
                gameObject.transform.SetParent(fpsCamera.transform.parent);
            }
            GameObject playerUiGameobject = Instantiate(playerUIPrefab);
            playerMovementController.joystick = playerUiGameobject.transform.Find("Fixed Joystick").GetComponent<FixedJoystick>();
            playerMovementController.fixedTouchField = playerUiGameobject.transform.Find("Rotation Touch field").GetComponent<FixedTouchField>();
            playerUiGameobject.transform.Find("FireButton").GetComponent<Button>().onClick.AddListener(()=>shooter.Fire());
            fpsCamera.enabled = true;

            animator.SetBool("IsSoldier", false);

        }
        else
        {
            foreach (GameObject gameObject in FPS_Hands)
            {
                gameObject.SetActive(false);
                gameObject.transform.SetParent(fpsCamera.transform);
            }

            foreach (GameObject gameObject in FPS_Body)
            {
                gameObject.SetActive(true);
                gameObject.transform.SetParent(fpsCamera.transform.parent);
            }

            playerMovementController.enabled = false;

            GetComponent<RigidbodyFirstPersonController>().enabled = false;
            fpsCamera.enabled = false;
            animator.SetBool("IsSoldier", true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
