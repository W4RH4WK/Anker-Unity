using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class DialgoueTestScene : MonoBehaviour
{
    DialogueSystem Dialogue;

    [SerializeField]
    DialogueCharacter Steve;

    [SerializeField]
    DialogueCharacter Tim;

    IEnumerator Start()
    {
        Dialogue = FindObjectOfType<DialogueSystem>();
        Assert.IsNotNull(Dialogue);

        Assert.IsNotNull(Steve);
        Assert.IsNotNull(Tim);

        yield return new WaitForSeconds(1.0f);

        yield return Dialogue.Tell("Our fearless protagonist approaches his fellow mate.");

        yield return Dialogue.Say(Tim, $"Ahoi {Steve.Name}!");
        yield return Dialogue.Say("'ow you doin', chum?");

        yield return Dialogue.SayRight(Steve, $"Oi {Tim.Name}! Me is fine. How 'bout you?");

        yield return Dialogue.Say(Tim, "O' good, o' good.");

        yield return Dialogue.Tell("Fin.");
        yield return Dialogue.Hide();

        yield return new WaitForSeconds(1.0f);
        Application.Quit();
    }
}
