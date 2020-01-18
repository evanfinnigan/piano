using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class Piano : MonoBehaviour
{
    public PianoKey[] keys;

    // Map
    /*
    0   c
    1   D
    2   E
    3   F
    4   G
    5   A
    6   B
    7   C
    8   c#
    9   D#
    10  F#
    11  Ab
    12  Bb
    13  C#
    */
    public void onKeyPlayed(int key) {

    }


    // Shuffle:
    /*
        1. Generate a random number for each key
        2. Sort keys based on these random numbers
        3. Position keys based on sorted order
    */
    public void shuffleKeyPositions() {

    }




    /* 
        White Keys: 0 - 7
        Black Keys: 8 - 14
        Correct positions for white keys:
            index * 4
        Correct positions for black keys:
        4*(index - 8) + 2
            - or -     
          4*index - 30
    */
    public void resetKeyPositions() {
        Vector3 p;
        for (int i = 0; i < keys.Length; i++) {
            p = keys[i].transform.localPosition;
            if (i < 8) {
                p.x = 4*i - 16;
                p.y = 8;
            } else {
                p.y = 8;
                if (i < 10) {
                    p.x = 4*(i - 8) - 14;
                } else if (i < 13) {
                    p.x = 4*(i - 8) - 10;
                } else {
                    p.x = 4*(i - 8) - 6;
                }
            } 
            p.z = 0;
            keys[i].transform.localPosition = p;
        }
    }

#if UNITY_EDITOR
    public void setKeyIds() {
        for (int i = 0; i < keys.Length; i++) {
            keys[i].id = i;
        }
    }

    public void setKeySizes(){
        for (int i = 0; i < keys.Length; i++) {
            if (i < 8) {
                ((RectTransform)keys[i].transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 3);
                ((RectTransform)keys[i].transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 15);
            } else {
                ((RectTransform)keys[i].transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 3);
                ((RectTransform)keys[i].transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 8);
            }
        }
    }

    public void setKeyColors() {
        for (int i = 0; i < keys.Length; i++) {
            Button b = keys[i].GetComponent<Button>();
            if (b != null) {
                string keyColor;
                string pressedColor;
                if (i < 8) {
                    keyColor = "#FFFFFF";
                    pressedColor = "#D1D1D1";
                } else {
                    keyColor = "#222034";
                    pressedColor = "#13121D";
                }
                b.colors = createColorBlock(keyColor, keyColor, pressedColor, keyColor, keyColor);
            }
        }
    }

    ColorBlock createColorBlock(string normalHex, string highlightedHex, string pressedHex, string selectedHex, string disabledHex) {
        ColorBlock cBlock = ColorBlock.defaultColorBlock;
        cBlock.fadeDuration = 0;

        cBlock.normalColor = createColor(normalHex);
        cBlock.highlightedColor = createColor(highlightedHex);
        cBlock.pressedColor = createColor(pressedHex);
        cBlock.selectedColor = createColor(selectedHex);
        cBlock.disabledColor = createColor(disabledHex);

        return cBlock;
    }

    Color createColor(string hex) {
        Color c = new Color();
        ColorUtility.TryParseHtmlString(hex, out c) ;
        return c;
    }

#endif

}

#if UNITY_EDITOR
// Editor
[CustomEditor(typeof(Piano))]
public class PianoEditor : UnityEditor.Editor {
    public override void OnInspectorGUI() {
        if (GUILayout.Button("Set Key IDs", EditorStyles.miniButton)) {
            ((Piano)this.target).setKeyIds();
        }
        if (GUILayout.Button("Set Key Sizes", EditorStyles.miniButton)) {
            ((Piano)this.target).setKeySizes();
        }
        if (GUILayout.Button("Set Key Colors", EditorStyles.miniButton)) {
            ((Piano)this.target).setKeyColors();
        }
        if (GUILayout.Button("Shuffle Key Positions", EditorStyles.miniButton)) {
            ((Piano)this.target).shuffleKeyPositions();
        }
        if (GUILayout.Button("Reset Key Positions", EditorStyles.miniButton)) {
            ((Piano)this.target).resetKeyPositions();
        }
        DrawDefaultInspector();
    }
}
#endif
