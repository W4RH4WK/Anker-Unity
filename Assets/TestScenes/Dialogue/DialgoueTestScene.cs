using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class DialgoueTestScene : MonoBehaviour
{
    [SerializeField]
    DialogueCharacter Steve;

    [SerializeField]
    DialogueCharacter Tim;

    [SerializeField]
    Document TestDoc;

    IEnumerator Start()
    {
        var filament = FindObjectOfType<Filament>();
        var dialogue = filament.DialogueSystem;
        var document = filament.DocumentSystem;

        Assert.IsNotNull(Steve);
        Assert.IsNotNull(Tim);
        // Tim.SetAlias("Tom");

        Assert.IsNotNull(TestDoc);

        yield return new WaitForSeconds(1.0f);

        yield return dialogue.Tell("Our fearless protagonist approaches his fellow mate.");

        yield return dialogue.Say(Tim, $"Ahoi {Steve}!");
        yield return dialogue.Say("'ow you doin', chum?");

        yield return dialogue.SayRight(Steve, $"Oi {Tim}! Me is fine. How 'bout you?");

        yield return dialogue.Say(Tim, "O' good, o' good.");
        yield return dialogue.Say("Take a look at this!");

        yield return document.Show(TestDoc);

        yield return dialogue.SayRight(Steve, "Oright, see ya.");

        yield return dialogue.Tell($"{Steve} carries on along without {Tim}.");
        yield return dialogue.HidePortraitLeft();

        yield return dialogue.Tell("— Fin.");
        yield return dialogue.Hide();

        yield return new WaitForSeconds(1.0f);
        Application.Quit();
    }
}
