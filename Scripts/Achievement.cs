[System.Serializable]
public class Achievement
{
    public string id;
    public string description;
    public bool isUnlocked;

    public Achievement(string id, string description)
    {
        this.id = id;
        this.description = description;
        this.isUnlocked = false;
    }
}
