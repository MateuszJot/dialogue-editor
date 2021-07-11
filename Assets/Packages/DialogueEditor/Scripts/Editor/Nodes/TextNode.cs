using System;
using UnityEngine;
using UnityEditor;

public class TextNode : Node {

    public TextNode(Vector2 position, float width, float height, Action<ConnectionPoint> OnClickInPoint, 
                                Action <ConnectionPoint> OnClickOutPoint, Action<Node> OnClickRemoveNode) {
        this.rect = new Rect(position.x, position.y, width, height);
        this.header = "Text";
        this.inPoint = new ConnectionPoint(this, ConnectionPointType.In, DefaultConnectionPointStyle, OnClickInPoint);
        this.outPoint = new ConnectionPoint(this, ConnectionPointType.Out, DefaultConnectionPointStyle, OnClickOutPoint);
        this.OnRemoveNode = OnClickRemoveNode;
    }

    public override void Draw() {
        GUI.Box(rect, string.Empty, CurrentNodeStyle);
        GUILayout.BeginArea(rect);
            DrawHeader();
            DrawContent();
        GUILayout.EndArea();

        inPoint.Draw();
        outPoint.Draw();
    }

    public override void DrawInspectorContent() {
        GUILayout.Label("Lorem ipsum");
    }

    private void DrawContent() {
        GUILayout.Label("Lorem ipsum");
    } 
}