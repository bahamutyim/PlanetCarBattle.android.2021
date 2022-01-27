using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace com.ahyim.planet
{
    public class ScoreListMessage
    {
        public ScoreName[] scoreNameArray;


    }

    public class ScoreName : IComparable<ScoreName>
    {
        public string playerName;
        public int score;

        public ScoreName()
        {
        }

        public ScoreName(string playerName, int score)
        {
            this.playerName = playerName;
            this.score = score;
        }

        public int CompareTo(ScoreName other)
        {
            return other.score.CompareTo(this.score);
        }

        public override string ToString()
        {
            return string.Format("[playerName:{0}, score:{1}]", playerName, score);
        }
    }
}


