using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;

public class EmotiguyIdentification : MonoBehaviour {

    public KMBombInfo Bomb;
    public KMAudio Audio;
    public KMSelectable[] Keyboard;
    public Material[] Emotiguy;
    public Material Gay;
    public TextMesh EmotiguyInsertion;
    public TextMesh[] LettersButShift;
    public Material[] Lights;
    public GameObject[] LEDs;
    public GameObject Display;
    string KeyboardButShift = "~!@#$%^&*()_+QWERTYUIOP{}|ASDFGHJKL:\"ZXCVBNM<>?";
    string KeyboardButNotShift = "`1234567890-=qwertyuiop[]\\asdfghjkl;\'zxcvbnm,./";
    string Input = "";
    string TopRow = "`1234567890-=";
    string SecondTopRow = "qwertyuiop[]\\";
    string HomeRow = "asdfghjkl;\'";
    string BottomRow = "zxcvbnm,./";
    string TopRowButShift = "~!@#$%^&*()_+";
    string SecondTopRowButShift = "QWERTYUIOP{}|";
    string HomeRowButShift = "ASDFGHJKL:\"";
    string BottomRowButShift = "ZXCVBNM<>?";
    string[] Names = {"Anticipation","Anxiety","blahhhhhhh","confusiob","Death","Despair","Disgust","Empty","End","Excitement","Fear","gary","Grief","Honor","hoohfhhudf","Imploration","Innocence","Insanity","Intellect","Joy","Lust","Misery","Mystique","Pleasure","Rage","Reflection","Shock","Sorrow","Temptation","the pain","gluttony","greed","Fury","ecstacy","Trapped",""};
    bool shift = false;
    bool started = false;
    int EmotiguySelector = 0;
    int Stage = 0;

    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake () {
        moduleId = moduleIdCounter++;

        foreach (KMSelectable Key in Keyboard) {
            Key.OnInteract += delegate () { KeyPress(Key); return false; };
        }
        GetComponent<KMBombModule>().OnActivate += delegate () {Aids();};
    }
    void Aids(){
      Audio.PlaySoundAtTransform("God", transform);
    }
    void KeyPress(KMSelectable Key) {
      if (moduleSolved == true) {
        return;
      }
      for (int i = 0; i < Keyboard.Count(); i++) {
        if (Key == Keyboard[i]) {
          if (i <= 12 && shift == false) {
            Input += TopRow[i];
            EmotiguyInsertion.text = Input;
          }
          else if (i == 13 && Input.Length == 0) {
            return;
          }
          else if (i == 13) {
            Input = Input.Substring(0, Input.Length - 1);
            EmotiguyInsertion.text = Input;
          }
          else if (i == 14 || i == 53 || i == 54 || i == 55 || i == 57 || i == 58 || i == 59 || i == 60) {
            return;
          }
          else if (i >= 15 && i <= 27 && shift == false) {
            Input += SecondTopRow[i - 15];
            EmotiguyInsertion.text = Input;
          }
          else if (i == 28 || i == 41 || i == 52) {
            shift = !shift;
            if (shift == true) {
              for (int j = 0; j < 47; j++) {
                LettersButShift[j].text = KeyboardButShift[j].ToString();
              }
            }
            else {
              for (int j = 0; j < 47; j++) {
                LettersButShift[j].text = KeyboardButNotShift[j].ToString();
              }
            }
          }
          else if (i >= 29 && i <= 39 && shift == false) {
            Input += HomeRow[i - 29];
            EmotiguyInsertion.text = Input;
          }
          else if (i >= 42 && i <= 51 && shift == false) {
            Input += BottomRow[i - 42];
            EmotiguyInsertion.text = Input;
          }
          else if (i <= 12) {
            Input += TopRowButShift[i];
            EmotiguyInsertion.text = Input;
          }
          else if (i >= 15 && i <= 27) {
            Input += SecondTopRowButShift[i - 15];
            EmotiguyInsertion.text = Input;
          }
          else if (i >= 29 && i <= 39) {
            Input += HomeRowButShift[i - 29];
            EmotiguyInsertion.text = Input;
          }
          else if (i >= 42 && i <= 51) {
            Input += BottomRowButShift[i - 42];
            EmotiguyInsertion.text = Input;
          }
          else if (i == 56) {
            Input += " ";
            EmotiguyInsertion.text = Input;
          }
          else if (i == 40 && started == false) {
            EmotiguySelector = UnityEngine.Random.Range(0,Emotiguy.Count());
            Display.GetComponent<MeshRenderer>().material = Emotiguy[EmotiguySelector];
            Debug.LogFormat("[Emotiguy Identification #{0}] The shown Emotiguy is {1}.", moduleId, Names[EmotiguySelector]);
            started = true;
          }
          else if (i == 40 && started == true && Input == Names[EmotiguySelector]) {
            started = false;
            Input = "";
            EmotiguyInsertion.text = Input;
            Display.GetComponent<MeshRenderer>().material = Gay;
            Stage += 1;
            switch (Stage) {
              case 1:
              LEDs[0].GetComponent<MeshRenderer>().material = Lights[1];
              Debug.LogFormat("[Emotiguy Identification #{0}] You submitted {1}. That is correct.", moduleId, Names[EmotiguySelector]);
              break;
              case 2:
              LEDs[1].GetComponent<MeshRenderer>().material = Lights[1];
              Debug.LogFormat("[Emotiguy Identification #{0}] You submitted {1}. That is correct.", moduleId, Names[EmotiguySelector]);
              break;
              case 3:
              LEDs[2].GetComponent<MeshRenderer>().material = Lights[1];
              Debug.LogFormat("[Emotiguy Identification #{0}] You submitted {1}. That is correct. Module disarmed.", moduleId, Names[EmotiguySelector]);
              moduleSolved = true;
              Display.GetComponent<MeshRenderer>().material = Emotiguy[4];
              GetComponent<KMBombModule>().HandlePass();
              Audio.PlaySoundAtTransform("Aids", transform);
              break;
            }
          }
          else {
            GetComponent<KMBombModule>().HandleStrike();
            Debug.LogFormat("[Emotiguy Identification #{0}] You submitted {1}. That is incorrect, uuwaaaaaaaaaaaaaaaaaah.", moduleId, Input);
            Input = "";
            EmotiguyInsertion.text = Input;
            Audio.PlaySoundAtTransform("Wahhh", transform);
            started = false;
            Display.GetComponent<MeshRenderer>().material = Gay;
          }
        }
      }
    }
}
