namespace TowerfallAi.Api {
  public class StateArrow : StateEntity {
    public string arrowType;
    public float timeLeft;

    // The player that shot it, -1 when it was not shot by a player.
    public int playerIndex;

    // True while it kills what it hits.
    public bool dangerous;
  }
}
