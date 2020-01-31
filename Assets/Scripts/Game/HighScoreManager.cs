using System;
using System.Collections.Generic;
using System.Text;

using pdxpartyparrot.Core.Collections;
using pdxpartyparrot.Core.DebugMenu;
using pdxpartyparrot.Core.Time;
using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Game
{
    public sealed class HighScoreManager : SingletonBehavior<HighScoreManager>
    {
        public enum HighScoreSortOrder
        {
            Ascending,
            Descending
        }

        public class HighScoreEntry : IComparable<HighScoreEntry>
        {
            public long timestamp;

            public string playerName = "default";

            public int playerCount = 1;

            public int score;

            public readonly Dictionary<string, object> extra = new Dictionary<string, object>();

            public HighScoreEntry()
            {
                timestamp = TimeManager.Instance.CurrentUnixMs;
            }

            public int CompareTo(HighScoreEntry other)
            {
                if(other.score == score) {
                    // sort equal scores ascending by default
                    return timestamp.CompareTo(other.timestamp);
                }

                // sort descending by default
                return other.score.CompareTo(score);
            }
        }

        public HighScoreSortOrder SortOrder { get; set; } = HighScoreSortOrder.Descending;

        private readonly SortedSet<HighScoreEntry> _highScores = new SortedSet<HighScoreEntry>();

        public IEnumerable<HighScoreEntry> HighScores
        {
            get
            {
                switch(SortOrder)
                {
                case HighScoreSortOrder.Ascending:
                    return _highScores.Reverse();
                case HighScoreSortOrder.Descending:
                default:
                    return _highScores;
                }
            }
        }

        private readonly HashSet<string> _extras = new HashSet<string>();

#region Unity Lifecycle
        private void Awake()
        {
            InitDebugMenu();
        }
#endregion

        public void AddHighScore(string playerName, int score)
        {
            AddHighScore(new HighScoreEntry
            {
                playerName = playerName,
                playerCount = 1,
                score = score
            });
        }

        public void AddHighScore(string playerName, int score, int playerCount)
        {
            AddHighScore(new HighScoreEntry
            {
                playerName = playerName,
                playerCount = playerCount,
                score = score
            });
        }

        public void AddHighScore(string playerName, int score, Dictionary<string, object> extra)
        {
            HighScoreEntry entry = new HighScoreEntry
            {
                playerName = playerName,
                playerCount = 1,
                score = score,
            };

            foreach(var kvp in extra) {
                entry.extra[kvp.Key] = kvp.Value;
                _extras.Add(kvp.Key);
            }

            AddHighScore(entry);
        }

        public void AddHighScore(string playerName, int score, int playerCount, Dictionary<string, object> extra)
        {
            HighScoreEntry entry = new HighScoreEntry
            {
                playerName = playerName,
                playerCount = playerCount,
                score = score,
            };

            foreach(var kvp in extra) {
                entry.extra[kvp.Key] = kvp.Value;
                _extras.Add(kvp.Key);
            }

            AddHighScore(entry);
        }

        public void AddHighScore(int playerCount, int score)
        {
            AddHighScore(new HighScoreEntry
            {
                playerName = string.Empty,
                playerCount = playerCount,
                score = score,
            });
        }

        public void AddHighScore(int playerCount, int score, Dictionary<string, object> extra)
        {
            HighScoreEntry entry = new HighScoreEntry
            {
                playerName = string.Empty,
                playerCount = playerCount,
                score = score,
            };

            foreach(var kvp in extra) {
                entry.extra[kvp.Key] = kvp.Value;
                _extras.Add(kvp.Key);
            }

            AddHighScore(entry);
        }

        private void AddHighScore(HighScoreEntry entry)
        {
            if(!_highScores.Add(entry)) {
                Debug.LogWarning("Unable to add high score entry!");
            }
        }

        public void HighScoresText(Dictionary<string, StringBuilder> columns)
        {
            int i=1;
            foreach(HighScoreEntry highScore in _highScores) {
                StringBuilder rank = columns.GetOrAdd("rank");
                rank.AppendLine($"{i}");

                StringBuilder playerName = columns.GetOrAdd("name");
                playerName.AppendLine(highScore.playerName);

                StringBuilder score = columns.GetOrAdd("score");
                score.AppendLine($"{highScore.score}");

                StringBuilder playerCount = columns.GetOrAdd("playerCount");
                playerCount.AppendLine($"{highScore.playerCount}");

                foreach(string extra in _extras) {
                    StringBuilder extraVal = columns.GetOrAdd(extra);
                    extraVal.AppendLine($"{highScore.extra.GetOrDefault(extra)}\t");
                }

                i++;
            }
        }

        private void InitDebugMenu()
        {
            DebugMenuNode debugMenuNode = DebugMenuManager.Instance.AddNode(() => "Game.HighScoreManager");
            debugMenuNode.RenderContentsAction = () => {
                GUILayout.BeginVertical("High Scores", GUI.skin.box);
                    StringBuilder builder = new StringBuilder("Rank\tName\tScore\tPlayers");
                    foreach(string extra in _extras) {
                        builder.Append($"\t{extra}");
                    }
                    GUILayout.Label(builder.ToString());

                    int i=1;
                    foreach(HighScoreEntry highScore in HighScores) {
                        builder.Clear();

                        builder.Append($"{i}\t");
                        builder.Append($"{highScore.playerName}\t");
                        builder.Append($"{highScore.score}\t");
                        builder.Append($"{highScore.playerCount}");

                        foreach(string extra in _extras) {
                            builder.Append($"\t{highScore.extra.GetOrDefault(extra)}");
                        }

                        GUILayout.Label(builder.ToString());

                        i++;
                    }
                GUILayout.EndVertical();
            };
        }
    }
}
