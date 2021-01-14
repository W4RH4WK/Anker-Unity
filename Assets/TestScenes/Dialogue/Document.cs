using UnityEngine;

[CreateAssetMenu(menuName = "Anker/Dialogue/Document")]
public class Document : ScriptableObject
{
    public Document(string title, string body)
    {
        _Title = title;
        _Body = body;
    }

    public string Title => _Title;

    [SerializeField]
    string _Title;

    public string Body => _Body;

    [SerializeField]
    [TextArea]
    string _Body;
}
