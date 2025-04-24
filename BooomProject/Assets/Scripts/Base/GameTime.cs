[System.Serializable]
public class GameTime {
    public int day = 1;
    public int hour = 6;
    public int minute = 0;

    public override string ToString() {
        return $"Day {day} - {hour:00}:{minute:00}";
    }
}