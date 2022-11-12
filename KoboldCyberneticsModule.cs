// Decompiled with JetBrains decompiler
// Type: XRL.CharacterBuilds.Qud.QudCyberneticsModule
// Assembly: Assembly-CSharp, Version=2.0.203.56, Culture=neutral, PublicKeyToken=null
// MVID: 0DF3EACD-032F-4144-A4AC-41408FC20722
// Assembly location: D:\SteamLibrary\steamapps\common\Caves of Qud\CoQ_Data\Managed\Assembly-CSharp.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Caves of Qud\CoQ_Data\Managed\Assembly-CSharp.xml

using ConsoleLib.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRL.UI.Framework;
using XRL.World;
using XRL.World.Parts;

namespace XRL.CharacterBuilds.Qud
{
  public class KoboldCyberneticsModule : QudCyberneticsModule
  {
    public override bool shouldBeEnabled()
    {
      string subtype = this.builder?.GetModule<QudSubtypeModule>()?.data?.Subtype;
      return subtype != null && subtype.StartsWith("Tinkerclaw");
    }
  }
}
