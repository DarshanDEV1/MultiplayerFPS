using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;
using TMPro;

public class Shooting : MonoBehaviourPunCallbacks
{
    public Camera FPS_Camera;
    public GameObject hitEffectPrefab;
    private MobileFpsGameManager gameManager;

    [Header("Scoring System Variables")]
    public int myScore = 0;
    public TMP_Text scoreText; // Text to display the player's score

    [Header("Health Related Stuff")]
    public float startHealth = 100;
    private float health;
    public Image healthBar;

    private Animator animator;

    [Header("DeathWinPanels")]
    [SerializeField] GameObject deathPanel;
    [SerializeField] GameObject winPanel;

    [SerializeField] bool isDead;
    [SerializeField] AudioSource gunShotSound;
    [SerializeField] AudioClip gunShot;

    // Start is called before the first frame update
    void Start()
    {
        scoreText = GetComponent<PlayerSetup>().playerUIPrefab.GetComponent<Transform>().Find("ScoreText").GetComponent<TMP_Text>();
        gameManager = FindObjectOfType<MobileFpsGameManager>();
        health = startHealth;
        healthBar.fillAmount = health / startHealth;
        deathPanel.SetActive(false);
        winPanel.SetActive(false);
        animator = GetComponent<Animator>();
        //scoreText.text = "Score: " + myScore; // Initialize the score text
        gunShotSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Fire()
    {
        RaycastHit _hit;
        Ray ray = FPS_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        if (Physics.Raycast(ray, out _hit, 100))
        {
            Debug.Log(_hit.collider.gameObject.name);

            photonView.RPC("CreateHitEffect", RpcTarget.All, _hit.point);
            gunShotSound.PlayOneShot(gunShot);

            if (_hit.collider.gameObject.CompareTag("Player") && !_hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
            {
                PhotonView targetPhotonView = _hit.collider.gameObject.GetComponent<PhotonView>();
                string killerName = photonView.Owner.NickName;
                string victimName = targetPhotonView.Owner.NickName;
                _hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 10f, killerName, victimName);
            }
        }
    }

    [PunRPC]
    [Obsolete]
    public void TakeDamage(float _damage, string killerName, string victimName, PhotonMessageInfo info)
    {
        Photon.Realtime.Player killer = info.Sender;

        if (health > 0)
        {
            health -= _damage;
        }

        if (info.photonView.IsMine)
        {
            GameObject.Find("MyCanvas").transform.GetChild(0).GetChild(0).GetComponent<Image>().fillAmount = health / startHealth;
        }

        healthBar.fillAmount = health / startHealth;

        if (health <= 0f && !isDead)
        {
            Die(killerName, victimName);
            Debug.Log(killerName + " has killed " + victimName);
            Debug.Log(info.Sender.NickName + " killed " + info.photonView.Owner.NickName);
        }
    }

    [PunRPC]
    public void CreateHitEffect(Vector3 position)
    {
        GameObject hitEffectGameobject = Instantiate(hitEffectPrefab, position, Quaternion.identity);
        Destroy(hitEffectGameobject, 0.5f);
    }

    [Obsolete]
    void Die(string killerName, string victimName)
    {
        if (photonView.IsMine)
        {
            animator.SetBool("IsDead", true);
            deathPanel.SetActive(true);
            if (!gameManager.gameOver)
                StartCoroutine(Respawn());
            else return;

            isDead = true;
            gameManager.PlayerKilled(killerName, victimName); // Pass the killer's name and victim's name to the game manager
        }
    }

    IEnumerator Respawn()
    {
        TMP_Text reSpawnText = deathPanel.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>();

        float respawnTime = 8.0f;

        while (respawnTime > 0.0f)
        {
            yield return new WaitForSeconds(1.0f);
            respawnTime -= 1.0f;
            reSpawnText.text = "Time Left: " + respawnTime.ToString();
            transform.GetComponent<PlayerMovementController>().enabled = false;
        }

        animator.SetBool("IsDead", false);
        isDead = false;
        deathPanel.SetActive(false);
        GameObject.Find("MyCanvas").transform.GetChild(0).GetChild(0).GetComponent<Image>().fillAmount = startHealth;

        int randomPoint = UnityEngine.Random.Range(-20, 20);
        transform.position = new Vector3(randomPoint, 0, randomPoint);
        transform.GetComponent<PlayerMovementController>().enabled = true;

        photonView.RPC("RegainHealth", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void RegainHealth()
    {
        health = startHealth;
        healthBar.fillAmount = health / startHealth;
    }

    // Increase the player's score and update the score text
    public void IncreaseScore(int amount)
    {
        myScore += amount;
        scoreText.text = "Score: " + myScore;
    }
}
