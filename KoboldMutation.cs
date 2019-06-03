using System;

namespace XRL.World.Parts.Mutation
{
  [Serializable]
  public class KoboldMutation : BaseMutation
  {

    public KoboldMutation()
    {
      this.DisplayName = "Kobold";
    }

    public override bool CanLevel()
    {
      return false;
    }

    public override void Register(GameObject Object)
    {
      base.Register(Object);
      // XRLCore.Core.Game.SetBooleanGameState("IsKobold", true);
    }

    public override string GetDescription()
    {
      return "You are a kobold.  Hooray!";
    }

    public override string GetLevelText(int Level)
    {
      return string.Empty;
    }
  }
}
