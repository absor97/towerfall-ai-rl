using System;
using Patcher;
using TowerFall;
using TowerfallAi.Api;
using TowerfallAi.Common;

namespace TowerfallAi.Mod {
  [Patch("TowerFall.SuperBombArrow")]
  public static class ModSuperBombArrow {
    public static StateEntity GetState(this SuperBombArrow ent) {
      var aiState = (StateArrow)ExtEntity.GetStateArrow(ent);
      aiState.timeLeft = (int)Math.Ceiling(ent.explodeAlarm.FramesLeft);

      return aiState;
    }
  }
}
