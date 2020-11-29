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

    [SerializeField]
    Document TestDoc;

    IEnumerator Start()
    {
        Dialogue = FindObjectOfType<DialogueSystem>();
        Assert.IsNotNull(Dialogue);

        Assert.IsNotNull(Steve);
        Assert.IsNotNull(Tim);

        Tim.SetAlias("Tom");

        yield return new WaitForSeconds(1.0f);

        yield return Dialogue.Tell("Our fearless protagonist approaches his fellow mate.");

        yield return Dialogue.Say(Tim, $"Ahoi {Steve}!");
        yield return Dialogue.Say("'ow you doin', chum?");

        yield return Dialogue.SayRight(Steve, $"Oi {Tim}! Me is fine. How 'bout you?");

        yield return Dialogue.Say(Tim, "O' good, o' good.");
        yield return Dialogue.Say("Take a look at this!");

        yield return Dialogue.ShowDocument(TestDoc);

        yield return Dialogue.SayRight(Steve, "Oright, see ya.");

        yield return Dialogue.Tell($"{Steve} carries on along without {Tim}.");
        yield return Dialogue.HidePortraitLeft();

        yield return Dialogue.Tell("Fin.");
        yield return Dialogue.Hide();

        yield return new WaitForSeconds(1.0f);
        Application.Quit();
    }
}
