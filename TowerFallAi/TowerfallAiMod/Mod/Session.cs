using System;
using Microsoft.Xna.Framework;
using Patcher;
using TowerFall;
using TowerfallAi.Api;
using TowerfallAi.Common;
using TowerfallAi.Core;

namespace TowerfallAi.Mod {
  [Patch]
  public class ModSession : Session {
    Action originalOnLevelLoadFinish;

    public ModSession(MatchSettings matchSettings) : base(matchSettings) { 
      originalOnLevelLoadFinish = Util.GetAction("$original_OnLevelLoadFinish", typeof(Session), this);
    }

    public override void OnLevelLoadFinish() {
      originalOnLevelLoadFinish();

      if (AiMod.Enabled) {
        Agents.NotifyLevelLoad(this.CurrentLevel);
      }
    }

    public override void OnPlayerDeath(Player player, PlayerCorpse corpse, int playerIndex, DeathCause deathType, Vector2 position, int killerIndex) {
      base.OnPlayerDeath(player, corpse, playerIndex, deathType, position, killerIndex);

      if (AiMod.Enabled) {
        Agents.AddEvent(new StateEventDeath {
          playerIndex = playerIndex,
          killerIndex = killerIndex,
          cause = deathType.ToString().FirstLower(),
        });
      }
    }

    public override void EndRound() {
      base.EndRound();

      if (AiMod.Enabled) {
        Agents.AddEvent(new StateEventRoundEnd {
          scores = (int[])this.Scores.Clone(),
          winner = this.GetWinner(),
        });
      }
    }
  }
}
