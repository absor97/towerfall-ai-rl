namespace TowerfallAi.Api {
  public class StateEvent {
    public string type;
  }

  public class StateEventDeath : StateEvent {
    public StateEventDeath() {
      type = "death";
    }

    public int playerIndex;

    // The player that caused it, -1 when it was no player. Equal to playerIndex for a suicide.
    public int killerIndex;

    public string cause;
  }

  public class StateEventRoundEnd : StateEvent {
    public StateEventRoundEnd() {
      type = "roundEnd";
    }

    // Indexed by team in versus: 0 is blue, 1 is red.
    public int[] scores;

    // The index in scores that won the match, -1 while the match goes on.
    public int winner;
  }
}
