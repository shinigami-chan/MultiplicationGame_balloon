using System.Collections;
using System;
using UnityEngine;

public class ReactionData{

    //format: dd-MM-yyyy hh:mm:ss.fff

    public ArrayList reactionData;

    public ReactionData()
    {
        reactionData = new ArrayList();
    }

    private void addTimeStamp(TimeStamp timeStamp)
    {
        reactionData.Add(timeStamp);
    }

    public void addTimeStampClickedRight(DateTime time)
    {
        addTimeStamp(new TimeStamp(time, "clicked_right"));
    }

    public void addTimeStampClickedWrong(DateTime time)
    {
        addTimeStamp(new TimeStamp(time, "clicked_wrong"));
    }

    public void addTimeStampLoadedRound(DateTime time)
    {
        addTimeStamp(new TimeStamp(time, "loaded_round"));
    }

    public void printAllData()
    {
        foreach (TimeStamp timestamp in reactionData)
        {
            Debug.Log(timestamp.getActionTime().ToString("dd-MM-yyyy hh:mm:ss.fff") + " " + timestamp.getActionMode());
        }
    }
}
