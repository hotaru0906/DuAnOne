using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    private Queue<string> sentences;
    public GameObject dialogueBox; // Kéo Dialogue Box vào đây trong Inspector
    public TextMeshProUGUI dialogueText;
    public float typeSpeed = 0.03f;
    private bool isTyping = false;

    // Reference to current NPC in dialogue
    private MonoBehaviour currentNPC;

    public AudioSource audioSource;
    public AudioClip typingSound;

    void Awake()
    {
        Instance = this;
        sentences = new Queue<string>();
    }

    public void StartDialogue(List<string> lines)
    {
        dialogueBox.SetActive(true);

        sentences.Clear();

        foreach (string line in lines)
            sentences.Enqueue(line);

        DisplayNextSentence();
    }

    // Overload method to accept NPC reference
    public void StartDialogue(List<string> lines, MonoBehaviour npc)
    {
        currentNPC = npc;
        StartDialogue(lines);
    }
    void Update()
    {
        if (dialogueBox.activeInHierarchy)
        {
            if (Input.anyKeyDown || Input.GetMouseButtonDown(0))
            {
                DisplayNextSentence();
            }
        }
    }


    public void DisplayNextSentence()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.text = sentences.Peek();
            isTyping = false;
            return;
        }

        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = "";

        // Play typing sound
        if (audioSource != null && typingSound != null)
        {
            audioSource.clip = typingSound;
            audioSource.loop = true;
            audioSource.Play();
        }

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }

        // Stop typing sound
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        isTyping = false;
    }

    public void EndDialogue()
    {
        dialogueBox.SetActive(false);

        CutsceneManager cutscene = FindObjectOfType<CutsceneManager>();
        if (cutscene != null)
        {
            cutscene.OnDialogueEnded();
        }

        if (currentNPC != null)
        {
            currentNPC.SendMessage("OnDialogueEnded", SendMessageOptions.DontRequireReceiver);
            currentNPC = null;
        }
    }
}
