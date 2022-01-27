using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace com.ahyim.planet
{
    public class ScoreListManager
    {
        private static ScoreListMessage scoreListMessage = new ScoreListMessage();
        private static List<ScoreName> scoreNameList = new List<ScoreName>();
        private static Dictionary<int, ScoreName> connectionIdScoreNameDict = new Dictionary<int, ScoreName>();


        public static ScoreListMessage  ScoreListMessage{
            get {
                scoreListMessage.scoreNameArray = scoreNameList.ToArray();
                Debug.LogFormat("ScoreListMessage scoreNameList:{0}", scoreNameList);
                Debug.LogFormat("ScoreListMessage scoreListMessage.scoreNameArray(Length{0}):{1}", scoreListMessage.scoreNameArray.Length, scoreListMessage.scoreNameArray);
                return scoreListMessage;
                
            }
        }

        public static void AddPlayer(int connectionId, string playerName)
        {
            Debug.LogFormat("AddPlayer playerUid:{0}, playerName:{1}", connectionId, playerName);
            ScoreName playerScoreName = new ScoreName(playerName, 0);
            connectionIdScoreNameDict.Add(connectionId, playerScoreName);
            scoreNameList.Add(playerScoreName);
        }
        public static void RemovePlayer(int connectionId)
        {
            Debug.LogFormat("RemovePlayer connection:{0}", connectionId);
            if (connectionIdScoreNameDict.ContainsKey(connectionId))
            {
                scoreNameList.Remove(connectionIdScoreNameDict[connectionId]);
                connectionIdScoreNameDict.Remove(connectionId);
            }
        }
        public static void UpdatePlayerScore(int connectionId, int score)
        {
            Debug.LogFormat("UpdatePlayerScore playerUid{0}, score:{1}", connectionId, score);
            if (connectionIdScoreNameDict.ContainsKey(connectionId))
            {
                connectionIdScoreNameDict[connectionId].score = score;
            }
        }
        public static void AddPlayerScore(int connectionId, int score)
        {
            Debug.LogFormat("AddPlayerScore playerUid:{0}, score:{1}", connectionId, score);
            if (connectionIdScoreNameDict.ContainsKey(connectionId))
            {
                connectionIdScoreNameDict[connectionId].score = connectionIdScoreNameDict[connectionId].score + score;
            }
        }
        public static void ScoreListToWriter(FastBufferWriter writer )
        {
            foreach( ScoreName sn in scoreNameList)
            {
                writer.WriteValue<int>(in sn.score);
                writer.WriteValue(sn.playerName);
            }
        }
        
    } 
}
