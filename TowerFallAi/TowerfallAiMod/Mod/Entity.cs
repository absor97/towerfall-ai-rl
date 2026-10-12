using System;
using System.Collections.Generic;
using Monocle;
using Patcher;
using TowerFall;
using TowerfallAi.Api;
using TowerfallAi.Common;
using System.Reflection;
using Microsoft.Xna.Framework;

namespace TowerfallAi.Mod {
  [Patch]
  public class ModEntity : Entity {
    public override void Update() {
      try {
        base.Update();
      } catch (Exception ex) {
        throw new Exception(GetType().ToString(), ex);
      }
    }
  }

  public static class ExtEntity {
    static Dictionary<Entity, int> ids = new Dictionary<Entity, int>();

    static Dictionary<Entity, Vec2> prevPositions = new Dictionary<Entity, Vec2>();

    // Frames between the previous update and this one.
    public static int FramesPerUpdate = 1;

    public static void Reset() {
      ids.Clear();
      prevPositions.Clear();
      FramesPerUpdate = 1;
    }

    /// <summary>
    /// Reports the speed the game moves the entity with, instead of the change in position.
    /// </summary>
    public static void SetVelocity(StateEntity state, Vector2 speed) {
      state.vel = new Vec2 {
        x = speed.X,
        y = -speed.Y
      };
    }

    static float GetDecimal(float x) {
      return x - (float)(Math.Round(x));
    }

    // The playfield wraps around, so an entity that crosses an edge moves by almost a whole screen.
    static float WrapDelta(float delta, float size) {
      if (delta > size / 2) return delta - size;
      if (delta < -size / 2) return delta + size;
      return delta;
    }

    public static void SetAiState(Entity ent, StateEntity state) {
      if (ids.ContainsKey(ent)) {
        state.id = ids[ent];
      } else {
        state.id = ids.Count;
        ids.Add(ent, state.id);
      }

      float x;
      float y;
       
      if (ent.Collider == null) {
        x = ent.Position.X;
        y = 240 - ent.Position.Y;
      } else {
        x = ent.CenterX;
        y = 240 - ent.CenterY;
      }

      if (ent is Actor) {
        Actor actor = (Actor)ent;
        x += GetDecimal(actor.ActualPosition.X);
        y += GetDecimal(240 - actor.ActualPosition.Y);
      }

      state.pos = new Vec2 { x = x, y = y };

      Vec2 prevPos;
      if (prevPositions.TryGetValue(ent, out prevPos)) {
        state.vel = new Vec2 {
          x = WrapDelta(state.pos.x - prevPos.x, 320) / FramesPerUpdate,
          y = WrapDelta(state.pos.y - prevPos.y, 240) / FramesPerUpdate,
        };
      } else {
        state.vel = new Vec2 {
          x = 0,
          y = 0,
        };
      }
      prevPositions[ent] = state.pos;

      if (ent.Collider == null) {
        state.size = new Vec2();
      } else {
        state.size = new Vec2 { x = ent.Width, y = ent.Height };
      }

      if (ent is Enemy) {
        Enemy enemy = (Enemy)ent;
        
        state.isEnemy = true;
        state.canHurt = enemy.CanHurt;
        state.canBounceOn = enemy.CanBounceOn;
        state.isDead = enemy.IsDead();
        state.facing = (int)enemy.Facing;
        state.state = ((string[])Util.GetFieldValue("names", typeof(Enemy), enemy, BindingFlags.NonPublic | BindingFlags.Instance))[enemy.State].FirstLower();
      }
    }

    public static StateEntity GetState(Entity e) {
      return GetState(e, null);
    }

    public static StateEntity GetState(Entity ent, string type) {
      var state = new StateEntity {
        type = (type == null ? ent.GetType().Name : type).FirstLower()
      };
      SetAiState(ent, state);
      return state;
    }

    public static StateItem GetStateItem(Entity ent, string itemType) {
      var state = new StateItem {
        type = Types.Item,
        itemType = itemType
      };
      SetAiState(ent, state);
      return state;
    }

    public static StateEntity GetStateArrow(Entity ent) {
      Arrow arrow = ent as Arrow;
      var state = new StateArrow {
        type = Types.Arrow,
        arrowType = arrow.ArrowType.ToString().FirstLower(),
      };
      SetAiState(arrow, state);
      SetVelocity(state, arrow.Speed);
      state.state = arrow.State.ToString().FirstLower();
      state.playerIndex = arrow.PlayerIndex;
      state.dangerous = arrow.Dangerous;
      return state;
    }

    public static StateItem GetStateBombPickup(BombPickup ent) {
      var state = new StateItem {
        type = Types.Item,
        itemType = TypesItems.Bomb
      };
      SetAiState(ent, state);
      return state;
    }
  }
}
