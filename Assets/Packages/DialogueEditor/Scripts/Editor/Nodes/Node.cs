using System;
using UnityEngine;
using UnityEditor;

public abstract class Node {

    public Rect rect;
    public string header;
    public GUIStyle style;

    private bool isBeingDragged;

    private GUIStyle defaultStyle = null;
    protected GUIStyle DefaultStyle {
        get {
            if(style == null) {
                style = new GUIStyle();
                style.normal.background = (Texture2D) EditorGUIUtility.Load("builtin skins/darkskin/images/node2.png");
                style.border = new RectOffset(10, 10, 10, 10);
            }

            return style;
        }
    }

    public void Move(Vector2 delta) {
        rect.position += delta;
    }

    public bool ManageEvents(Event e) {
        switch(e.type) {
            case EventType.MouseDown:
                if(e.button == 0) {
                    if(rect.Contains(e.mousePosition)) {
                        isBeingDragged = true;
                        GUI.changed = true;
                    } else {
                        GUI.changed = true;
                    }
                }
            break;
 
            case EventType.MouseUp:
                isBeingDragged = false;
            break;
 
            case EventType.MouseDrag:
                if(e.button == 0 && isBeingDragged) {
                    Move(e.delta);
                    e.Use();
                    return true;
                }
            break;
        }

        return false;
    }

    public abstract void Draw();
}