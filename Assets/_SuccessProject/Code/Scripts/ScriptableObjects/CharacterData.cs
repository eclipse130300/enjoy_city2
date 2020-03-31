using UnityEngine;

[CreateAssetMenu(fileName = "Create/New Character Data", menuName = "New Character")]
public class CharacterData : ScriptableObject {

    public string Name;
    public string FullName;
    public string Age;
    public string Hobby;

    public Sprite Avatar;
    public Color NameColor;
}
