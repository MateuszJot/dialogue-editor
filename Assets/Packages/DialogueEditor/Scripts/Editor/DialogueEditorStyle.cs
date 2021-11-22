using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DialogueEditorStyle {
    private static GUIStyle defaultWindowStyle;
    public static GUIStyle DefaultWindowStyle {
        get {
            if(defaultWindowStyle == null) {
                defaultWindowStyle = new GUIStyle();
                defaultWindowStyle.normal.background = (Texture2D) Resources.Load("Default Skin/node_background");
                defaultWindowStyle.border = new RectOffset(10, 10, 10, 10);
            }

            return defaultWindowStyle;
        }
    }

    private static GUIStyle headerStyle;
    public static GUIStyle HeaderStyle {
        get {
            if(headerStyle == null) {
                headerStyle = new GUIStyle();
                headerStyle.normal.background = (Texture2D) Resources.Load("Default Skin/node_background_dark");
                headerStyle.normal.textColor = Color.white;
                headerStyle.richText = true;
                headerStyle.alignment = TextAnchor.MiddleCenter;
                headerStyle.border = new RectOffset(4, 4, 12, 12);
            }

            return headerStyle;
        }
    }
}