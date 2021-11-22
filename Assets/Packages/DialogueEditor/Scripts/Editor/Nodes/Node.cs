using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class Node {

    public Rect rect;
    public string header;
    public static GUIStyle defaultNodeStyle;
    public static GUIStyle selectedNodeStyle;
    public static GUIStyle connectionPointStyle;
    public static GUIStyle defaultConnectionPointStyle;
    public static GUIStyle headerStyle;

    private bool isBeingDragged;
    private bool isSelected;

    public ConnectionPoint inPoint;
    public ConnectionPoint outPoint;

    public Action<Node> OnRemoveNode;
    public List<Connection> myConnections = new List<Connection>();
    
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
                defaultNodeStyle.normal.background = (Texture2D) Resources.Load("Default Skin/node_background");
                defaultNodeStyle.border = new RectOffset(10, 10, 10, 10);
            }

            return defaultNodeStyle;
        }
    }

    protected GUIStyle SelectedNodeStyle {
        get {
            if(selectedNodeStyle == null) {
                selectedNodeStyle = new GUIStyle(defaultNodeStyle);
                selectedNodeStyle.normal.background = (Texture2D) Resources.Load("Default Skin/node_background_selected");
            }

            return selectedNodeStyle;
        }
    }

    protected GUIStyle DefaultConnectionPointStyle {
        get {
            if(connectionPointStyle == null) {
                connectionPointStyle = new GUIStyle();
                connectionPointStyle.normal.background = (Texture2D) Resources.Load("Default Skin/node_background");
                connectionPointStyle.active.background = (Texture2D) Resources.Load("Default Skin/node_background");
                connectionPointStyle.border = new RectOffset(4, 4, 12, 12);
            }

            return connectionPointStyle;
        }
    }

    protected GUIStyle HeaderStyle {
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

    public bool ManageEvents(Event e, DialogueEditorWindow editorWindow) {

        switch(e.type) {
            case EventType.MouseDown:
                if(e.button == 0) {
                    if(rect.Contains(e.mousePosition)) {
                        isBeingDragged = true;
                        isSelected = true;
                        editorWindow.selectedNode = this;
                        GUI.FocusControl(null);
                        GUI.changed = true;
                    } else {
                        isSelected = false;
                        GUI.changed = true;
                        if(editorWindow.selectedNode == this 
                        && !editorWindow.nodeInspector.Contains(e.mousePosition))
                                 editorWindow.selectedNode = null;
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
                    Move(e);
                    e.Use();
                    return true;
                }
            break;
        }

        return false;
    }

    public abstract void Draw();
    public abstract void DrawInspectorContent();

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

    public void Move(Event e) {

        ///TODO: Dedcide when round to grid!!!
        /// Custom input system in dialogue editor
        bool roundToGrid = false;

        if(!roundToGrid) {
            rect.position += e.delta;
        } else {
            rect.position = new Vector2(SnapValue(e.mousePosition.x), SnapValue(e.mousePosition.y));
        }
    }

    private float SnapValue(float val, float snapSize = 20f) {
       return snapSize * Mathf.Round((val / snapSize));
    }

    #region Universal GUI Drawers
    protected void DrawHeader() {
        GUILayout.BeginVertical();
            GUILayout.Label("<b><size=15>" + header + "</size></b>", HeaderStyle);
        GUILayout.EndVertical();
    }
    #endregion
}