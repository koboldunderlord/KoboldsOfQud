using System;
using System.Collections.Generic;

namespace XRL.World.Parts
{
  [Serializable]
  public class WatertailWeightBonus : IPart
  {
    public override void Register(GameObject Object)
    {
      Object.RegisterPartEvent(this, "GetMaxWeight");
      base.Register(Object);
    }

    public override bool FireEvent(Event E)
    {
      XRL.Messages.MessageQueue.AddPlayerMessage("LETS A GO");
      if (E.ID == "GetMaxWeight") {
        XRL.Messages.MessageQueue.AddPlayerMessage("LETS A GO 2");
        E.SetParameter("Weight", E.GetIntParameter("Weight")+60);
      }
      return base.FireEvent(E);
    }
  }
}
