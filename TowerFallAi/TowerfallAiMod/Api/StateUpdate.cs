using System.Collections.Generic;

namespace TowerfallAi.Api {
  public class StateUpdate : State {
    public StateUpdate() {
      type = "update";
    }
    
    public List<StateEntity> entities = new List<StateEntity>();

    // What happened since the previous update.
    public List<StateEvent> events = new List<StateEvent>();

    public float dt;
    public int id;
  }
}
