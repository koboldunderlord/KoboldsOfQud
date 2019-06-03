using System;
using XRL.Messages;

namespace XRL.World.Parts.Mutation
{
  [Serializable]
  public class KoboldElectricResist : BaseMutation
  {

    public KoboldElectricResist()
    {
      this.DisplayName = "Fire Resistance";
    }

    public override bool CanLevel()
    {
      return false;
    }

    public override void Register(GameObject Object)
    {
      Object.RegisterPartEvent((IPart) this, "BeforeApplyDamage");
      Object.RegisterPartEvent((IPart) this, "BeforeTemperatureChange");
      base.Register(Object);
    }

    public override bool FireEvent(Event E)
    {
      if (E.ID == "BeforeApplyDamage")
      {
        Damage parameter = E.GetParameter("Damage") as Damage;
        if (!parameter.HasAttribute("Fire") && !parameter.HasAttribute("Heat"))
          return true;

        int Amount = (int) Math.Ceiling((double) parameter.Amount * (0.25 + 0.025 * (double) this.Level));
        if (this.ParentObject.IsPlayer())
          MessageQueue.AddPlayerMessage("Your scales absorb " + Amount.ToString() + " damage from the heat.");
        parameter.Amount = Math.Max(0, parameter.Amount - Amount);
        return false;
      }
      if (E.ID == "BeforeTemperatureChange")
      {
        Physics part = this.ParentObject.GetPart("Physics") as Physics;
        Event parameter = E.GetParameter("Event") as Event;
        if (parameter.HasParameter("Amount") && (int) parameter.GetParameter("Amount") > 0)
        {
          int Amount = (int) parameter.GetParameter("Amount");
          int AmountReduction = (int) Math.Ceiling((double) (Amount - part.Temperature) * (0.25 + 0.025 * (double) this.Level));
          parameter.AddParameter("Amount", Math.Max(0, Amount - AmountReduction));
          return true;
        }
        if (parameter.HasParameter("Radiant") && (int) parameter.GetParameter("Radiant") > 0)
        {
          int Radiant = (int) parameter.GetParameter("Radiant");
          int RadiantReduction = (int) Math.Ceiling((double) (Radiant - part.Temperature) * (0.25 + 0.025 * (double) this.Level));
          parameter.AddParameter("Radiant", Math.Max(0, (int) ((double) (Radiant - RadiantReduction))));
          return true;
        }
      }
      return base.FireEvent(E);
    }

    public override string GetDescription()
    {
      return "Your scales grant you with mild resistance to heat.";
    }

     public override string GetLevelText(int Level)
    {
      return "Fire Resist: +" + (object) Level;
    }
  }
}
