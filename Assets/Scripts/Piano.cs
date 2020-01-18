using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class Piano : MonoBehaviour
{
    public PianoKey[] keys;
    public PianoKey[] currentKeyOrder;

    static KeyCode[] keyMappingLookup = new KeyCode[]{
        KeyCode.A,
        KeyCode.S,
        KeyCode.D,
        KeyCode.F,
        KeyCode.G,
        KeyCode.H,
        KeyCode.J,
        KeyCode.K,
        KeyCode.W,
        KeyCode.E,
        KeyCode.T,
        KeyCode.Y,
        KeyCode.U,
        KeyCode.O
    };


    void Start() {
        shuffleKeyPositions();
    }

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
        List<(PianoKey, float)> whiteKeys = new List<(PianoKey,float)>();
        List<(PianoKey, float)> blackKeys = new List<(PianoKey,float)>();

        for (int i = 0; i < keys.Length; i++) {
            (PianoKey, float) pair;
            pair.Item1 = keys[i];
            pair.Item2 = Random.Range(0f,1f);
            if (i < 8){
                whiteKeys.Add(pair);
            } else {
                blackKeys.Add(pair);
            }
            
        }

        whiteKeys.Sort(((PianoKey, float) first,(PianoKey, float) second) => {
            return first.Item2 > second.Item2 ? 1 : -1;
        });

        blackKeys.Sort(((PianoKey, float) first,(PianoKey, float) second) => {
            return first.Item2 > second.Item2 ? 1 : -1;
        });

        whiteKeys.AddRange(blackKeys);
        setKeyPositions(whiteKeys.ToArray());
        setKeySizes();
        setKeyMappings();
    }

    // Sets position of keys based on the list order
    void setKeyPositions(PianoKey[] keyArray) {
        for (int i = 0; i < keyArray.Length; i++) {
            if (i < 8) {
                setPosition(keyArray[i], i, true);
            } else {
                setPosition(keyArray[i], i-8, false);
            } 
        }
        currentKeyOrder = keyArray;
    }

    void setKeyPositions((PianoKey, float)[] keyArray) {
        PianoKey[] keyArr = new PianoKey[keyArray.Length];
        for (int i = 0; i < keyArray.Length; i++) {
            keyArr[i] = keyArray[i].Item1;
        }
        setKeyPositions(keyArr);
    }

    void setPosition(PianoKey k, int index, bool isWhite) {
        Vector3 p = k.transform.localPosition;
        if (isWhite) {
            p.x = 4*index - 16;
            p.y = 8;
        } else {
            p.y = 8;
            if (index < 2) {
                p.x = 4*index - 14;
            } else if (index < 5) {
                p.x = 4*index - 10;
            } else {
                p.x = 4*index - 6;
            }
        } 
        p.z = 0;
        k.transform.localPosition = p;
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
        setKeyPositions(keys);
        currentKeyOrder = keys;
        setKeySizes();
        setKeyMappings();
    }

    public void setKeySizes(){
        for (int i = 0; i < currentKeyOrder.Length; i++) {
            if (i == currentKeyOrder.Length - 1) {
                ((RectTransform)currentKeyOrder[i].transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 2);
                ((RectTransform)currentKeyOrder[i].transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 8);
            } else if (i < 8) {
                ((RectTransform)currentKeyOrder[i].transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 3);
                ((RectTransform)currentKeyOrder[i].transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 15);
            } 
            else {
                ((RectTransform)currentKeyOrder[i].transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 3);
                ((RectTransform)currentKeyOrder[i].transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 8);
            }
        }
    }

    public void setKeyMappings() {
        for (int i = 0; i < currentKeyOrder.Length; i++) {
            currentKeyOrder[i].keyMapping = keyMappingLookup[i];
        }
    }


#if UNITY_EDITOR
    public void setKeyIds() {
        for (int i = 0; i < keys.Length; i++) {
            keys[i].id = i;
            keys[i].piano = this;
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

    public void LoadAudioClips() {
        for (int i = 0; i < keys.Length; i++) {
            string fileName = keys[i].gameObject.name.Substring(9);
            keys[i].baileySound = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/bailey/" + fileName + ".mp3");
            keys[i].famiSound = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/famitracker/" + fileName + ".wav");
        }
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
        if (GUILayout.Button("Load Audio", EditorStyles.miniButton)) {
            ((Piano)this.target).LoadAudioClips();
        }
        DrawDefaultInspector();
    }
}
#endif
