using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class DialgoueTestScene : MonoBehaviour
{
    DialogueSystem Dialogue;

    IEnumerator Start()
    {
        Dialogue = FindObjectOfType<DialogueSystem>();
        Assert.IsNotNull(Dialogue);

        yield return Dialogue.Say("Steve", "Hello World");
        yield return Dialogue.Say("How are you?");
        yield return Dialogue.Say("Fin.");
        yield return Dialogue.Hide();
        yield return new WaitForSeconds(1.0f);
        Application.Quit();
    }
}
