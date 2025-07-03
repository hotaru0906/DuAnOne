using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    private Queue<string> sentences;
    public GameObject dialogueBox; // Kéo Dialogue Box vào đây trong Inspector
    public Text dialogueText;
    public float typeSpeed = 0.03f;
    private bool isTyping = false;
    
    // Reference to current NPC in dialogue
    private MonoBehaviour currentNPC;

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

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;
    }

    public void EndDialogue()
    {
        if (dialogueBox != null)
            dialogueBox.SetActive(false);
            
        // Notify NPC that dialogue has ended
        if (currentNPC != null)
        {
            currentNPC.SendMessage("OnDialogueEnded", SendMessageOptions.DontRequireReceiver);
            currentNPC = null;
        }
    }
}
