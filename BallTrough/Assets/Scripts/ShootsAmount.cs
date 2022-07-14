using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShootsAmount : MonoBehaviour
{
    //En este script controlo el texto de los disparos restantes, el fondo semi transparente de la pantalla de victoria y el texto de "you win"
   
    public int shootsLeftAmountValue;
    public GameObject youLoseGameBG;
    public GameObject youWinGameBG;
    public GameObject youLoseText;
    public GameObject youWinText;
    public Text flippedTargetsText;
    public Text amountOfTargetsText;
    public Text flippedCansText;
    public Text amountOfCansText;
    public Text shootsLeft;
    public GameObject ballHud;
    public GameObject targetHud;
    public GameObject cansHud;
    
    public int amountOfTargets;
    private bool won = false;
    private int amountOfCans = 6;
    private Scene currentScene;
    private string currentSceneName;
    private static int cansScoreCounter;
    private static int targetsCounter;
    public static int CansScoreCounter { get => cansScoreCounter; set => cansScoreCounter = value; }
    public static int TargetsCounter { get => targetsCounter; set => targetsCounter = value; }

    void Start()
    {
        youLoseGameBG.SetActive(false);
        youLoseText.SetActive(false);
        youWinText.SetActive(false);
    }

    void Update()
    {
        currentScene = SceneManager.GetActiveScene();
        currentSceneName = currentScene.name;

        shootsLeft.text = shootsLeftAmountValue.ToString();
        amountOfTargetsText.text = amountOfTargets.ToString();
        flippedTargetsText.text = targetsCounter.ToString();
        flippedCansText.text = cansScoreCounter.ToString();

        if (shootsLeftAmountValue > 0 && cansScoreCounter < amountOfCans)
        {
            shootsLeft.text = shootsLeftAmountValue.ToString();
            flippedCansText.text = cansScoreCounter.ToString();
        }
        else if (cansScoreCounter == amountOfCans && targetsCounter == amountOfTargets)
        {
            flippedCansText.text = amountOfCans.ToString();

            if (currentSceneName.Equals("Lvl1"))
            {
                cansScoreCounter = 0;
                targetsCounter = 0;
                SceneManager.LoadScene("Lvl2");
            }
            if (currentSceneName.Equals("Lvl2"))
            {
                won = true;
                
                cansScoreCounter = 0;
                targetsCounter = 0;
                StartCoroutine(ChangeScene());
                youWinGameBG.SetActive(true);
                youWinText.SetActive(true);
            }
            if (currentSceneName.Equals("Menu"))
            {
                ballHud.SetActive(true);
                cansScoreCounter = 0;
                targetsCounter = 0;
            }
        }
        else if (shootsLeftAmountValue == 0 && won == false)   //&& cansScoreCounter < 6 && targetsCounter < 2)
        {
            targetHud.SetActive(false);
            cansHud.SetActive(false);
            ballHud.SetActive(false);
            cansScoreCounter = 0;
            targetsCounter = 0;
            StartCoroutine(ChangeScene());
            shootsLeft.text = 0.ToString();
            youLoseGameBG.SetActive(true);
            youLoseText.SetActive(true);
        }
        
    }

    IEnumerator ChangeScene()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(0);
    }
}