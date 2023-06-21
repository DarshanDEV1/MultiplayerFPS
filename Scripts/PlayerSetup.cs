using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;
using Photon.Realtime;
using TMPro;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public GameObject[] FPS_Hands;
    public GameObject[] FPS_Body;
    public GameObject playerUIPrefab;
    public Camera fpsCamera;
    private Animator animator;
    private Shooting shooter;
    private PlayerMovementController playerMovementController;
    [SerializeField] private GameObject _fireParticleSystem;
    //[SerializeField] TMP_Text scoreText;

    // Start is called before the first frame update
    void Start()
    {
        //scoreText = playerUIPrefab.transform.Find("ScoreText").GetComponent<TMP_Text>();

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
            playerUiGameobject.transform.Find("FireButton").GetComponent<Button>().onClick.AddListener(()=>
            {
                shooter.Fire();
                _fireParticleSystem.GetComponent<ParticleSystem>().Play();
                _fireParticleSystem.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
                _fireParticleSystem.transform.GetChild(1).GetComponent<ParticleSystem>().Play();
                _fireParticleSystem.transform.GetChild(2).GetComponent<ParticleSystem>().Play();
                _fireParticleSystem.transform.GetChild(3).GetComponent<ParticleSystem>().Play();
            });

            if(!playerMovementController.isSprinting)
            {
                playerUiGameobject.transform.Find("SprintButton").GetComponent<Button>().onClick.AddListener(() =>
                    {
                        playerMovementController.Sprint();
                        if (playerUiGameobject.transform.Find("SprintButton").GetComponent<Button>().GetComponent<Graphic>().color == Color.white)
                            playerUiGameobject.transform.Find("SprintButton").GetComponent<Button>().GetComponent<Graphic>().color = Color.yellow;
                        else
                            playerUiGameobject.transform.Find("SprintButton").GetComponent<Button>().GetComponent<Graphic>().color = Color.white;
                    });
            }

            playerUiGameobject.transform.Find("JumpButton").GetComponent<Button>().onClick.AddListener(() =>
                {
                    playerMovementController.Jump();
                });

            playerUiGameobject.transform.Find("LandButton").GetComponent<Button>().onClick.AddListener(() =>
                {
                    playerMovementController.Land();
                });
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

    public void ScoreTextUpdate(int score)
    {
        //scoreText.text = "Kills : " + score.ToString();
    }
}
