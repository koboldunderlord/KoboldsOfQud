using System;
using System.Collections.Generic;

namespace XRL.World.Parts
{
  [Serializable]
  public class KoboldTinkerclawHack : CyberneticsTerminal2
  {
    public bool interfacing = false;

    public override bool HandleEvent(InventoryActionEvent E)
    {
      XRL.Messages.MessageQueue.AddPlayerMessage("LETS A GO");
      if (E.Command == "InterfaceWithBecomingNook") {
        XRL.Messages.MessageQueue.AddPlayerMessage("nook!");
        ParentObject.genotypeEntry.IsTrueKin = true;
        interfacing = true;
      } else if (interfacing && E.InterfaceExitRequested()) {
        XRL.Messages.MessageQueue.AddPlayerMessage("left!");
        ParentObject.genotypeEntry.IsTrueKin = false;
        interfacing = false;
      }
      return base.HandleEvent(E);
    }
  }
}
