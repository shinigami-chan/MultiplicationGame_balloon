using UnityEngine;
using Random = System.Random;
using System.Collections;
using System;

public class Npc : MonoBehaviour
{
    readonly string npcName = "Gegner";
    //private bool hasClicked = false;
    private int playerID;
    private int points;
    private int meanResponseTime;

    Random rnd = new Random(Guid.NewGuid().GetHashCode());

    //returns whether the npc is going to answer correct or not; correctness is normal distributed
    public bool isNpcAnswerCorrect()
    {
        //float errorProbability = RandomFromDistribution.RandomNormalDistribution(0.1f,0.05f);
        float errorProbability = RandomFromDistribution.RandomNormalDistribution(0.2f, 0.05f);
        //Debug.Log(errorProbability);


        //if negative errorProbability -> no error, positive is always > negative
        return rnd.Next(0,100)>errorProbability*100;
    }

    //returns a normal distributed response time
    public float responseTime()
    {
        return RandomFromDistribution.RandomNormalDistribution(meanResponseTime,500);
    }

    public int getPoints()
    {
        return points;
    }

    public void setPoints(int receivedPoints)
    {
        points += receivedPoints;
    }

    public void setMeanResponseTime(int speedMode)
    {
        switch (speedMode)
        {
            case 0: meanResponseTime = 1500; break;
            case 1: meanResponseTime = 3000; break;
            case 2: meanResponseTime = 4000; break;
        }
    }

}
