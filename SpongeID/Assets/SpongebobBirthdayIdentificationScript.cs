using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;

public class SpongebobBirthdayIdentificationScript : MonoBehaviour
{

    public KMBombInfo Bomb;
    public KMAudio Audio;
    public KMBombModule Module;
    public KMSelectable[] Keyboard;
    public TextMesh inputBox;
    public MeshRenderer display;
    public List<MeshRenderer> leds;
    public Texture[] allImages;
    public Texture cri, dollar;
    public Material litColor;
    public MeshRenderer bg;
    public GameObject ledParent;

    private static readonly string[] brokenKeys = new string[] { "Ctrl", "Menu", "Alt", "Tab", "Win" };
    private KeyCode[] TypableKeys =
    {
        KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.T, KeyCode.Y, KeyCode.U, KeyCode.I, KeyCode.O, KeyCode.P, KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.Z, KeyCode.X, KeyCode.C, KeyCode.V, KeyCode.B, KeyCode.N, KeyCode.M,
        KeyCode.Quote, KeyCode.LeftBracket, KeyCode.RightBracket, KeyCode.Backslash, KeyCode.Semicolon, KeyCode.BackQuote, KeyCode.Comma, KeyCode.Period, KeyCode.Slash, KeyCode.Tab, KeyCode.CapsLock, KeyCode.Return,
        KeyCode.LeftShift, KeyCode.RightShift, KeyCode.LeftControl, KeyCode.RightControl, KeyCode.LeftWindows, KeyCode.RightWindows, KeyCode.LeftAlt, KeyCode.RightAlt, KeyCode.Menu, KeyCode.Minus, KeyCode.Equals,
        KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0, KeyCode.Backspace, KeyCode.Space
    };
    private bool TwitchPlaysActive;


    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;
    private bool capsLock;
    private bool shift;
    private bool displaying;
    private string answer;
    private int stage;
    private static Vector2 bgMove;
    private float hue;
    private bool focused;
    private bool uncapped;
    private bool uncapping;
    private int stageCount = 3;

    void Awake()
    {
        moduleId = moduleIdCounter++;
        bgMove = new Vector2(UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1, UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1);
        foreach (KMSelectable button in Keyboard) 
            button.OnInteract += delegate () { KeyPress(button); return false; };
        GetComponent<KMSelectable>().OnFocus += delegate () { focused = true; };
        GetComponent<KMSelectable>().OnDefocus += delegate () { focused = false; };
        if (Application.isEditor)
            focused = true;
    }

    void KeyPress(KMSelectable button)
    {
        button.AddInteractionPunch(0.1f);
        Audio.PlaySoundAtTransform("boom", button.transform);
        string buttonLabel = button.GetComponentInChildren<TextMesh>().text;
        if (buttonLabel == "Shift")
        {
            shift = !shift;
            DoShift();
        }
        else if (buttonLabel == "Caps Lock")
        {
            capsLock = !capsLock;
            DoShift();
        }
        else if (!brokenKeys.Contains(buttonLabel))
        {
            if (buttonLabel == "Backspace")
            {
                if (inputBox.text.Length != 0)
                    inputBox.text = inputBox.text.Substring(0, inputBox.text.Length - 1);
            }
            else if (inputBox.text.Length >= 30)
                return;
            else if (buttonLabel == "Space")
                inputBox.text += ' ';
            else if (buttonLabel == "Entree")
                Submit();
            else inputBox.text += button.GetComponentInChildren<TextMesh>().text;
            shift = false;
            DoShift();
        }
       
    }

    void Submit()
    {
        if (moduleSolved)
            return;
        Debug.Log(moduleSolved);
        if (inputBox.text.Equals("uncap", StringComparison.InvariantCultureIgnoreCase) && !uncapped && !TwitchPlaysActive)
        {
            uncapped = true;
            if (Bomb.GetModuleNames().Count > 3)
                stageCount = Bomb.GetModuleNames().Count;
            inputBox.text = string.Empty;
            Audio.PlaySoundAtTransform("arc", transform);
            return;
        }
        if (!displaying)
        {
            display.material.mainTexture = allImages.PickRandom();
            answer = display.material.mainTexture.name;
            if (answer == "Jon JonJon Eric Cabebe Jr.")
                answer = "Jon \"JonJon\" Eric Cabebe Jr.";
            Debug.LogFormat("[Spongebob Birthday Identification #{0}] AYOOO IS THAT {1}??????? LMAOOOOOOOOOOOOO!!", moduleId, answer);
            displaying = true;
        }
        else
        {
            if (inputBox.text == answer)
            {
                Debug.LogFormat("[Spongebob Birthday Identification #{0}] YOU SBMITUINTTED {1} THATS FUNNY BRO.", moduleId, inputBox.text);
                if (stage < 3)
                    leds[stage].material = litColor;
                else
                {
                    leds.Add(Instantiate(leds[1], ledParent.transform));
                    leds[leds.Count - 1].transform.localPosition = new Vector3(UnityEngine.Random.Range(-0.066f, 0.087f), 0.0078f, UnityEngine.Random.Range(0.085f, -0.07f));
                }
                stage++;
                displaying = false;
                display.material.mainTexture = cri;
                if (stage == stageCount)
                    StartCoroutine(Solve());
                else Audio.PlaySoundAtTransform("stage pass", transform);
            }
            else
            {
                Debug.LogFormat("[Spongebob Birthday Identification #{0}] AW HELL NAW THAT AINT {1} SPUNCH BOB SAD NOW :(((", moduleId, inputBox.text);
                Module.HandleStrike();
                Audio.PlaySoundAtTransform("anoming splornge", transform);
                displaying = false;
                display.material.mainTexture = cri;
            }
            inputBox.text = string.Empty;
        }

    }

    void DoShift()
    {
        for (int i = 0; i < 26; i++)
        {
            Keyboard[i].GetComponentInChildren<TextMesh>().text = (shift || capsLock) ?
                Keyboard[i].GetComponentInChildren<TextMesh>().text.ToUpper() : Keyboard[i].GetComponentInChildren<TextMesh>().text.ToLower();
        }
        Keyboard[26].GetComponentInChildren<TextMesh>().text = (shift || capsLock) ? "\"" : "'";
    }
    IEnumerator Solve()
    {
        moduleSolved = true;
        display.material.mainTexture = dollar;
        Audio.PlaySoundAtTransform("solvenuts", transform);
        yield return null; //For the tp handler; makes sure that points are awarded for uncapped on solve.  
        yield return null;
        Module.HandlePass();
    }
    void Update()
    {
        bg.material.mainTextureOffset += 0.02f * (stage + 1) * bgMove;
        bg.material.color = Color.HSVToRGB(hue, 0.5f, 1);
        hue += 0.075f * (stage + 1) * Time.deltaTime;
        hue %= 1;
        if (focused)
        {
            for (int i = 0; i < TypableKeys.Count(); i++)
            {
                if (Input.GetKeyDown(TypableKeys[i]))
                {
                    if (Input.GetKeyDown(TypableKeys[i]))
                        Keyboard[i].OnInteract();
                }
            }
        }

    }

#pragma warning disable 414
    private readonly string TwitchHelpMessage = "Use !{0} submit Jon \"JonJon\" Cabebe Jr. to submit Jon \"JonJon\" Cabebe Jr. into the module. Use !{0} start to just press enter. Use !{0} uncap to uncap the module.";
#pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string command)
    {
        command = command.Trim();
        List<string> parameters = command.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        char[] keyLetters = Keyboard.Select(x => x.GetComponentInChildren<TextMesh>().text.ToUpper()[0]).Concat(" \"").ToArray();
        if (command.Equals("uncap", StringComparison.InvariantCultureIgnoreCase) && !uncapped)
        {
            if (!uncapping)
            {
                uncapping = true;
                yield return "sendtochat Are you sure you want to uncap? Doing so will increase the required number of stages to the number of modules on the bomb. Send any other command to cancel this.";
                yield break;
            }
            uncapped = true;
            if (Bomb.GetModuleNames().Count > 3)
                stageCount = Bomb.GetModuleNames().Count;
            Audio.PlaySoundAtTransform("arc", transform);
            yield return "sendtochat IT HAS BEEN UNHINGED WOAAHAWHWHOAWHOAWOWAHOWHAWOHWAOHA"; ;
            yield break;
        }
        uncapping = false;
        if (command.Equals("start", StringComparison.InvariantCultureIgnoreCase))
        {
            yield return null;
            Keyboard[37].OnInteract();
        }
        else if (parameters.First().Equals("submit", StringComparison.InvariantCultureIgnoreCase) && parameters.Skip(1).Join("").All(x => keyLetters.Contains(char.ToUpper(x))))
        {
            yield return null;
            while (inputBox.text.Length != 0)
            {
                Keyboard[59].OnInteract();
                yield return new WaitForSeconds(0.1f);
            }
            if (capsLock)
                Keyboard[36].OnInteract();
            foreach (char letter in parameters.Skip(1).Join())
            {
                if ("ABCDEFGHIJKLMNOPQRSTUVWXYZ\"".Contains(letter) ^ shift)
                {
                    Keyboard[38].OnInteract();
                    yield return new WaitForSeconds(0.1f);
                }
                if (letter == ' ')
                    Keyboard[60].OnInteract();
                else if (letter == '"')
                    Keyboard[26].OnInteract();
                else Keyboard[Array.IndexOf(keyLetters, char.ToUpperInvariant(letter))].OnInteract();
                yield return new WaitForSeconds(0.1f);
            }
            Keyboard[37].OnInteract();
            if (moduleSolved && stageCount > 3)
                yield return "awardpointsonsolve " + (stageCount - 3);
        }
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        char[] keyLetters = Keyboard.Select(x => x.GetComponentInChildren<TextMesh>().text.ToUpper()[0]).Concat(" ").ToArray();
        float speed = uncapped ? 0.025f : 0.1f;
        while (!moduleSolved)
        {
            if (!displaying)
            {
                Keyboard[37].OnInteract();
                yield return new WaitForSeconds(speed);
            }
            while (inputBox.text.Length != 0)
            {
                Keyboard[59].OnInteract();
                yield return new WaitForSeconds(speed);
            }
            if (capsLock)
                Keyboard[36].OnInteract();
            foreach (char letter in answer)
            {
                if ("ABCDEFGHIJKLMNOPQRSTUVWXYZ\"".Contains(letter) ^ shift)
                {
                    Keyboard[38].OnInteract();
                    yield return new WaitForSeconds(speed);
                }
                if (letter == ' ')
                    Keyboard[60].OnInteract();
                else if (letter == '"')
                    Keyboard[26].OnInteract();
                else Keyboard[Array.IndexOf(keyLetters, char.ToUpperInvariant(letter))].OnInteract();
                yield return new WaitForSeconds(speed);
            }
            Keyboard[37].OnInteract();
        }
    }
}