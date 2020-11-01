using UnityEngine;

[CreateAssetMenu(menuName = "Anker/Dialogue/Character")]
public class DialogueCharacter : ScriptableObject
{
    public string Name => Alias.Length > 0? Alias : _Name;

    [SerializeField]
    string _Name;

    public void SetAlias(string alias) => Alias = alias;
    public void ClearAlias() => Alias = "";
    string Alias;

    public Sprite Image => _Image;

    [SerializeField]
    Sprite _Image;

    public override string ToString() => Name;

    public DialogueCharacter Clone() => MemberwiseClone() as DialogueCharacter;
}
