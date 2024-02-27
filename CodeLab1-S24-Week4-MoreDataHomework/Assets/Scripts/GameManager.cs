using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // MATT I'M SORRY ABOUT THE OBSCENE AMOUNT OF COMMENTS IN THIS DOCUMENT
    // IT'S EXCESSIVE I KNOW, I WAS TRYING TO FIGURE STUFF OUT
    public static GameManager instance;
    [SerializeField] bool isInGame = true; 
    
    private float timer = 0;
    public int maxTime = 10;
    public int levelTime = 2; // couldn't get this to divide by scene count  
    public TextMeshProUGUI display;

    public int score;
    // int highScores = 0;
    const string FILE_DIR = "/DATA/";
    const string DATA_FILE = "highScores.txt";
    string FILE_FULL_PATH;
    
    public int Score                // Score property, used in WhackADot script 
    {                               // instead of the variable directly
        get
        {
            return score;          
        }

        set
        {
            score = value;
        }
    }

    string highScoresString = "";                                       // declaring a string to hold the scoreBoardText (used above)
    List<int> highScores;                                               // a LIST of high scores, holds several pieces of data

    public List<int> HighScores                                         // a property to wrap around HighScore
    {
        get
        {
            if (highScores == null)                                     // if nothing has been put into the list yet (return empty list to highScores)
            {
                Debug.Log("got from file");
                highScores = new List<int>();                           // 
                highScoresString = File.ReadAllText(FILE_FULL_PATH);    // get the contents of the data file (a string)

                highScoresString = highScoresString.Trim();             // to prevent an error, this "trim" function cuts off
                                                                        // empty space at the beginning or end of the string
                
                string[] highScoreArray = highScoresString.Split("\n"); // highScoreArray is a temporary variable
                                                                        // it's breaking up the contents of that file into an array
                                                                        // the split tells the array where to start a new component

                for (int i = 0; i < highScoreArray.Length; i++)         // go through the array and take each value out
                {
                    int currentScore = Int32.Parse(highScoreArray[i]);  // Int32 used to convert the strings to ints
                    highScores.Add(currentScore);                       // insert into our high score list
                }
            }  else if(highScores == null)
            {
                Debug.Log("NOPE");
                highScores = new List<int>();
                highScores.Add(3);
                highScores.Add(2);
                highScores.Add(1);
                highScores.Add(0);
            }

            return highScores;
        }
    }      
    
    void Awake()                        // happens before the scene runs (before start)
    {                                   // makes game manager into a singleton across scenes
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else                            // if there is already an instance of game manager 
        {                               // this one will be destroyed
            Destroy(gameObject);        // https://www.youtube.com/watch?v=uNZU9JjSR3A
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        FILE_FULL_PATH = Application.dataPath + FILE_DIR + DATA_FILE;
    }

    
    // Update is called once per frame
    void Update()
    {
        if (isInGame)                           // while game is running, set display to show score and time
        {
            //Debug.Log("Is in game bool: TRUE");
            display.text = "Score: " + score + "\nTime: " + (maxTime - (int)timer);
        }
        else                                    // change display text for end scene to show score and GAME OVER
        {
            //Debug.Log("Is in game bool: FALSE");
            display.text = "GAME OVER\nFINAL SCORE: " + score + "\nHigh Scores:\n" + highScoresString;   // list of high scores
        }
        
        if (timer >= levelTime && isInGame)     // when timer reaches levelTime seconds, go to next scene
        {                                       // and increase levelTime for following level
            SceneManager.LoadScene(             // i probably should have built two timers instead of sharing one...
                SceneManager.GetActiveScene().buildIndex + 1);
            levelTime = Mathf.RoundToInt(levelTime + levelTime);
        }

        if (timer >= maxTime && isInGame)       // when timer reaches maxTime seconds, load end scene
        {                                       // set isInGame bool to false which stops the maxTime and levelTime timers
            isInGame = false;                   // destroy any prizes with tag "prize"
            SceneManager.LoadScene("EndScene");
            Destroy(GameObject.FindWithTag("prize"));
            SetHighScore();
        }

        timer += Time.deltaTime;
    }
    
    bool isHighScore(int score)                     // TODO: how do i make this only run at the end?
    {
        for (int i = 0; i < HighScores.Count; i++)  // go through the high scores list to see if the score
        {                                           // is higher, the Count is how many slots are in the list
            if (highScores[i] < score)              // if the highscore is less than the current score
            {                                       // return true, ie this score is a high score
                return true;
            }
        }

        return false;                               // if not, return false, ie the score is not a high score
    }

    void SetHighScore()
    {
        if (isHighScore(score))
        {
            Debug.Log("isHighScore check was true");
            int highScoreSlot = -1;                     // default to -1 because ????

            for (int i = 0; i < HighScores.Count; i++)  // forloop to go through the HighScores List
            {
                if (score > highScores[i])              // if it finds a score that is higher than a highscore
                {                                       // in a certain slot
                    highScoreSlot = i;                  // this tells you where you want to insert the new value
                    break;                              // and breaks (ends) the forloop
                }
            }

            highScores.Insert(highScoreSlot, score);    // inserts the score into the highScoreSlot 
                                                                 // the highScoreSlot was found in the foreloop above
            highScores = highScores.GetRange(0, 5);    // this limits the size of the highScoresList to 5 slots

            string scoreBoardText = "";                          // turns the ints into a string

            foreach (var highScore in highScores)             // foreach reads the whole highScores list in order
            {
                scoreBoardText += highScore + "\n";              // \n puts each score on it's own line
            }

            highScoresString = scoreBoardText;                   // string to hold the scoreBoardText

            File.WriteAllText(FILE_FULL_PATH, highScoresString);
        }
    }
}
