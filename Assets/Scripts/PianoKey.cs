using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class PianoKey : MonoBehaviour
{
    public int id;
    public AudioClip sound;
    public Piano piano;

    public AudioSource audio_src;

    public KeyCode keyMapping;

    Image img;
    Button btn;

    void Start()
    {
        audio_src = GetComponent<AudioSource>();
        audio_src.clip = sound;

        btn = GetComponent<Button>();
        img = GetComponentInChildren<Image>();
    }

    void Update() {
        if (Input.GetKeyDown(keyMapping)) {
            Debug.Log("KeyDown: " + keyMapping);
            playNote();
            img.color = btn.colors.pressedColor;
        }

        if (Input.GetKeyUp(keyMapping)) {
            Debug.Log("KeyUp: " + keyMapping);
            img.color = Color.white;
        }
    }


    public void playNote() {
        // 1. play a sound?
        // 2. tell 'piano' the note was played
        audio_src.Play();
#if !UNITY_EDITOR
        piano.onKeyPlayed(id);
#endif
    }

#if UNITY_EDITOR
    public void Initialize() {
        audio_src = GetComponent<AudioSource>();
        audio_src.clip = sound;
    }
#endif
}

#if UNITY_EDITOR
// Editor
[CustomEditor(typeof(PianoKey))]
public class PianoKeyEditor : UnityEditor.Editor {
    public override void OnInspectorGUI() {
        if (GUILayout.Button("Play Note", EditorStyles.miniButton)) {
            ((PianoKey)this.target).Initialize();
            ((PianoKey)this.target).playNote();
        }
        DrawDefaultInspector();
    }
}
#endif