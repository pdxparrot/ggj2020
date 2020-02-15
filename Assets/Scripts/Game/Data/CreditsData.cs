using System;
using System.Collections.Generic;
using System.Text;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Collections;
using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Game.Data
{
    [CreateAssetMenu(fileName="CreditsData", menuName="pdxpartyparrot/Game/Data/Credits Data")]
    [Serializable]
    public sealed class CreditsData : ScriptableObject
    {
        [Serializable]
        public class Credits
        {
            [SerializeField]
            private string _title;

            public string Title => _title;

            [SerializeField]
            private bool _allowInContributorString = true;

            public bool AllowInContributorString => _allowInContributorString;

            // TODO: this can't be a ReorderableListString for some reason :(
            // the UI treats all of the sub-lists as the same list, not sure why
            [SerializeField]
            private string[] _contributors;

            public IReadOnlyCollection<string> Contributors => _contributors;
        }

        [Serializable]
        public class ReorderableListCredits : ReorderableList<Credits>
        {
        }

        [SerializeField]
        [TextArea]
        private string _preAmble;

        [SerializeField]
        [ReorderableList]
        private ReorderableListCredits _credits = new ReorderableListCredits();

        [SerializeField]
        [TextArea]
        private string _postAmble;

        [Space(10)]

        [SerializeField]
        private string _contributorString = "A {credit} Game";

        public string GetContributorString()
        {
            // TODO: move this allocation out of here
            HashSet<string> contributors = new HashSet<string>();
            foreach(Credits credits in _credits.Items) {
                if(!credits.AllowInContributorString) {
                    continue;
                }

                foreach(string contributor in credits.Contributors) {
                    contributors.Add(contributor);
                }
            }

            // TODO: handle A vs An if we can

            string credit = contributors.GetRandomEntry();
            return null == credit ? _contributorString : _contributorString.Replace("{credit}", credit);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            if(!string.IsNullOrWhiteSpace(_preAmble)) {
                builder.AppendLine(_preAmble);
                builder.AppendLine();
            }

            builder.AppendLine($"<size=36><align=\"center\">{Application.productName}</align></size>");
            builder.AppendLine(); builder.AppendLine();

            foreach(Credits credits in _credits.Items) {
                builder.AppendLine($"<size=24><align=\"center\">{credits.Title}</align></size>");
                foreach(string contributor in credits.Contributors) {
                    builder.AppendLine($"<size=18><align=\"center\"><pos=5>{contributor}</pos></align></size>");
                }
                builder.AppendLine();
            }

            if(!string.IsNullOrWhiteSpace(_postAmble)) {
                builder.AppendLine(_postAmble);
                builder.AppendLine();
            }
            return builder.ToString();
        }
    }
}
