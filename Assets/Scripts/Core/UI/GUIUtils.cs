﻿using System.Text.RegularExpressions;

using UnityEngine;

namespace pdxpartyparrot.Core.UI
{
    public static class GUIUtils
    {
        public const int MinimumButtonWidth = 100;
        public const int MinimumButtonHeight = 25;

        public static GUILayoutOption[] GetLayoutButtonSize(string text)
        {
            return GetLayoutButtonSize(text, GUI.skin.button);
        }

        public static GUILayoutOption[] GetLayoutButtonSize(string text, GUIStyle style)
        {
            GUIContent titleContent = new GUIContent(text);

            Vector2 size = style.CalcSize(titleContent);

            // enforce some decent minimums
            if(size.x < MinimumButtonWidth) {
                size.x = MinimumButtonWidth;
            }
            if(size.y < MinimumButtonHeight) {
                size.y = MinimumButtonHeight;
            }

            return new[] { GUILayout.Width(size.x), GUILayout.Height(size.y) };
        }

        public static bool LayoutButton(string text)
        {
            return GUILayout.Button(text, GetLayoutButtonSize(text));
        }

        public static bool LayoutButton(string text, GUIStyle style)
        {
            return GUILayout.Button(text, style, GetLayoutButtonSize(text, style));
        }

// TODO: these don't really work, we need to be able to hold the string that is the text
// but also the float that comes out of it so that typing out 1.0 or -1 or whatever doesn't fuck up
/*
        public static int IntField(int currentValue)
        {
            string text = GUILayout.TextField($"{currentValue}");
            text = Regex.Replace(text, @"[^0-9]" ,"");

            int value;
            return int.TryParse(text, out value) ? value : currentValue;
        }

        public static float FloatField(float currentValue)
        {
            string text = GUILayout.TextField($"{currentValue}");
            text = Regex.Replace(text, @"[^0-9\.]" ,"");

            if(text[text.Length - 1] == '.') {
                text += '0';
            }

            float value;
            return float.TryParse(text, out value) ? value : currentValue;
        }
*/

        public static bool Foldout(bool foldout, string content)
        {
            content = $"{(foldout ? 'v' : '>')} {content}";
            if(GUILayout.Button(content)) {
                return !foldout;
            }
            return foldout;
        }
    }
}
