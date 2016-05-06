using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Random = System.Random;
using System;
using UnityEngine.SceneManagement;

public class MultiplicationController : MonoBehaviour
{
    Random rnd = new Random(Guid.NewGuid().GetHashCode());

    private View view;
    private Game game;
    readonly float ResetTime = 2.5f;

    // Use this for initialization
    void Awake()
    {
        //initialize game (+ npc) and view (need to be added to an object -> using camera since it exists through the whole game)
        game = GameObject.Find("Main Camera").AddComponent<Game>();
        view = GameObject.Find("Main Camera").AddComponent<View>();
        game.setNpc( GameObject.Find("Main Camera").AddComponent<Npc>());

        game.getPlayer().setPlayerName(PlayerPrefs.GetString("PlayerName"));
        view.setPlayerName(game.getPlayer().getPlayerName());
        game.getNpc().setMeanResponseTime(PlayerPrefs.GetInt("SpeedMode"));
        //load first round
        loadNewRound();
    }

    public void loadNewRound()
    {
        view.resetAllButtons();
        view.resetAllBalloons();
        //all rounds have been played -> switch to the end screen memorizing player score
        if (game.loadCurrentQuest() == null)
        {
            game.reactionData.creatingCsvFile("test_data.csv");
            Debug.Log("Alle Runden wurden gespielt.");
            PlayerPrefs.SetInt("PlayerPoints", game.getPlayer().getPoints()); //necessary for transfering this to the next scene
            SceneManager.LoadScene("end_screen");
        }
        //new quest has been loaded -> set the task to its panel and the options to the buttons, start npc behaviour
        else
        {
            Debug.Log("neue Quest wurde geladen.");
            view.setTaskText(game.currentQuest.getProblem().stringProblemTask());
            view.setButtonTexts(game.currentQuest.getOptions());
            StartCoroutine(startNpcBehaviour());
            //set time stamp
            game.reactionData.addTimeStampLoadedRound(DateTime.Now);
        }
    }


    //update points in class Player or Npc and in view
    public void updatePoints(int recievedPoints, bool player)
    {
        if (player)
        {
            game.getPlayer().setPoints(recievedPoints);
            view.refreshPoints(game.getPlayer().getPoints());
        }
        else
        {
            game.getNpc().setPoints(recievedPoints);
            view.refreshNpcPoints(game.getNpc().getPoints());
        }
    }


    public IEnumerator startNpcBehaviour()
    {
        //normal distributed response time
        yield return new WaitForSeconds(game.getNpc().responseTime()/1000);

        //100% correct answers in this version
        //bool npcAnswersCorrect = game.getNpc().isNpcAnswerCorrect();

        int i = rnd.Next(0, 8);
        while (view.buttonList[i].interactable == false ||
            !((MathOption)game.currentQuest.getOptions()[i]).getIsCorrect())
        {
            i = rnd.Next(0, 8);
        }
        
        view.buttonList[i].interactable = false;

        if (((MathOption)game.currentQuest.getOptions()[i]).getIsCorrect())
            {
            view.letBallonsFlyAwayExceptIndex(i);
                view.setDisabledButtonColor(view.buttonList[i], view.cRight);
                updatePoints(10,false);
                view.disableAllButtons();
                Invoke("loadNewRound", ResetTime);
            }
        //100% correct answers in this version
            //else
            //{
            //    view.popBalloon(i, new Color(230, 0, 255, 188));
            //    view.setDisabledButtonColor(view.buttonList[i], view.cWrong);
            //    StartCoroutine(startNpcBehaviour());
            //}
    }


    //triggers events following a clicked button (points, new task etc.)
    public void evaluateAnswer(Button button)
    {
        Debug.Log("begin evaluation: " + DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss.fff"));
        button.interactable = false;
        int buttonIndex = view.buttonList.IndexOf(button);
        MathOption clickedOption = (MathOption)game.currentQuest.getOptions()[buttonIndex];
        clickedOption.setIsClicked();

        //Debug.Log("end evaluation: " + DateTime.Now.ToString("dd:MM:yyyy:hh:mm:ss:fff"));

        if (clickedOption.getIsCorrect())
        {
            StopAllCoroutines(); //prevents any interaction with the npc

            game.reactionData.addTimeStampClickedRight(DateTime.Now);

            Debug.Log(game.currentQuest.getProblem().stringProblemTask() + " = " + game.currentQuest.getProblem().getSolution() + " correct answer given");
            view.setDisabledButtonColor(button, view.cRight);
            view.popBalloonExceptIndex(buttonIndex, Color.white);

            updatePoints(10,true);
            view.disableAllButtons();

            //Invoke: adds delay time for the methods
            Invoke("loadNewRound", ResetTime);
        }
        else
        {
            game.reactionData.addTimeStampClickedWrong(DateTime.Now);
            Debug.Log(game.currentQuest.getProblem().stringProblemTask() + " = " + game.currentQuest.getProblem().getSolution() + " wrong answer given");

            view.popBalloon(buttonIndex, Color.white);
            view.setDisabledButtonColor(button, view.cWrong);
        }
    }

    //public void goToMainMenu()
    //{
    //    SceneManager.LoadScene("main_menu");
    //}

    //public void quitGame()
    //{
    //    Application.Quit();
    //}

    //public void muteSound()
    //{
    //    view.music.mute = true;
    //    view.balloonPoppingSound.mute = true;
    //    view.muteButton.interactable = false;
    //    view.muteButton.gameObject.SetActive(false);
    //    view.unmuteButton.interactable = true;
    //    view.unmuteButton.gameObject.SetActive(true);
    //}

    //public void unMuteSound()
    //{
    //    view.music.mute = false;
    //    view.balloonPoppingSound.mute = false;
    //    view.muteButton.interactable = true;
    //    view.muteButton.gameObject.SetActive(true);
    //    view.unmuteButton.interactable = false;
    //    view.unmuteButton.gameObject.SetActive(false);
    //}
}
