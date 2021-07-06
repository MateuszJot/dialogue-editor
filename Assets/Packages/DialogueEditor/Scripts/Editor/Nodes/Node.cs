using System;
using UnityEngine;
using UnityEditor;

public abstract class Node {

    public Rect rect;
    public string header;
    public static GUIStyle defaultNodeStyle;
    public static GUIStyle selectedNodeStyle;
    public static GUIStyle connectionPointStyle;

    private bool isBeingDragged;
    private bool isSelected;

    public ConnectionPoint inPoint;
    public ConnectionPoint outPoint;

    public Action<Node> OnRemoveNode;
    
    protected GUIStyle CurrentNodeStyle {
        get {
            if(isSelected)
                return SelectedNodeStyle;
            else
                return DefaultNodeStyle;
        }
    }

    protected GUIStyle DefaultNodeStyle {
        get {
            if(defaultNodeStyle == null) {
                defaultNodeStyle = new GUIStyle();
                defaultNodeStyle.normal.background = (Texture2D) EditorGUIUtility.Load("builtin skins/darkskin/images/node2.png");
                defaultNodeStyle.border = new RectOffset(10, 10, 10, 10);
            }

            return defaultNodeStyle;
        }
    }

    protected GUIStyle SelectedNodeStyle {
        get {
            if(selectedNodeStyle == null) {
                selectedNodeStyle = new GUIStyle(defaultNodeStyle);
                selectedNodeStyle.normal.background = (Texture2D) EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png");
            }

            return selectedNodeStyle;
        }
    }

    private GUIStyle defaultConnectionPointStyle = null;
    protected GUIStyle DefaultConnectionPointStyle {
        get {
            if(connectionPointStyle == null) {
                connectionPointStyle = new GUIStyle();
                connectionPointStyle.normal.background = 
                    EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
                connectionPointStyle.active.background = 
                    EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
                connectionPointStyle.border = new RectOffset(4, 4, 12, 12);
            }

            return connectionPointStyle;
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
                        isSelected = true;
                        GUI.changed = true;
                    } else {
                        isSelected = false;
                        GUI.changed = true;
                    }
                } else if (e.button == 1 && rect.Contains(e.mousePosition)) {
                    ProcessContextMenu();
                    e.Use();
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

    protected virtual void ProcessContextMenu() {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Delete Node"), false, OnClickRemoveNode);
        genericMenu.ShowAsContext();
    }
 
    private void OnClickRemoveNode() {
        if (OnRemoveNode != null)
            OnRemoveNode(this);
    }

    public void Drag(Vector2 delta) {
        rect.position += delta;
    }
}