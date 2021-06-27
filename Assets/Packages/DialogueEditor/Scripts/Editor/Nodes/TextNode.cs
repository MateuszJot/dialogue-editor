using System;
using UnityEngine;
using UnityEditor;

public class TextNode : Node {

    public GUIStyle style;

    public TextNode(Vector2 position, float width, float height) {
        this.rect = new Rect(position.x, position.y, width, height);
        style = DefaultStyle;
        this.header = "Text";
    }

    public override void Draw() {
        GUI.Box(rect, header, style);
    }
}