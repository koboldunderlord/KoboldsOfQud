// Decompiled with JetBrains decompiler
// Type: XRL.World.Parts.Mutation.DarkVision
// Assembly: Assembly-CSharp, Version=2.0.178.0, Culture=neutral, PublicKeyToken=null
// MVID: 0CE63DEF-6C20-4B71-9249-58379842EF98
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Caves of Qud\CoQ_Data\Managed\Assembly-CSharp.dll

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
