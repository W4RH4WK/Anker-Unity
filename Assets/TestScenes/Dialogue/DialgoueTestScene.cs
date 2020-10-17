using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class DialgoueTestScene : MonoBehaviour
{
    Dialogue Dialogue;

    IEnumerator Start()
    {
        Dialogue = FindObjectOfType<Dialogue>();
        Assert.IsNotNull(Dialogue);

        yield return Dialogue.Show();
        yield return Dialogue.Say("Hello World");
        yield return Dialogue.Say("How are you?");
        yield return Dialogue.Say("Fin.");
        yield return Dialogue.Hide();
        yield return new WaitForSeconds(1.0f);
        Application.Quit();
    }
}
