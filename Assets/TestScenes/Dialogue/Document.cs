using UnityEngine;

[CreateAssetMenu(menuName = "Anker/Dialogue/Document")]
public class Document : ScriptableObject
{
    public string Title => _Title;

    [SerializeField]
    string _Title;

    public string Text => _Text;

    [SerializeField]
    [TextArea]
    string _Text;
}
