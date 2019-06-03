using System;
using XRL.Messages;
using XRL.Rules;

namespace XRL.World.Parts.Mutation
{
  [Serializable]
  public class KoboldFireResist : BaseMutation
  {

    public KoboldFireResist()
    {
      this.DisplayName = "Fire Resistance";
    }

    public override bool CanLevel()
    {
      return true;
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

        double ReductionDouble = (double) parameter.Amount * (0.25 + 0.025 * (double) this.Level);
        int Reduction = (int) Math.Floor(ReductionDouble);
        if (ReductionDouble - (double) Reduction >= Stat.Rnd.NextDouble()) {
          Reduction += 1;
        }
        if (Reduction > 0 && this.ParentObject.IsPlayer())
          MessageQueue.AddPlayerMessage("Your scales absorb " + Reduction.ToString() + " damage from the heat.");
        parameter.Amount = Math.Max(0, parameter.Amount - Reduction);
        return false;
      }
      if (E.ID == "BeforeTemperatureChange")
      {
        Physics part = this.ParentObject.GetPart("Physics") as Physics;
        Event parameter = E.GetParameter("Event") as Event;
        if (parameter.HasParameter("Amount") && (int) parameter.GetParameter("Amount") > 0 && part.Temperature > 0)
        {
          int Amount = (int) parameter.GetParameter("Amount");
          int AmountReduction = (int) Math.Floor((double) Amount * (0.25 + 0.025 * (double) this.Level));
          parameter.AddParameter("Amount", Math.Max(0, Amount - AmountReduction));
          return true;
        }
        if (parameter.HasParameter("Radiant") && (int) parameter.GetParameter("Radiant") > 0 && part.Temperature > 0)
        {
          int Radiant = (int) parameter.GetParameter("Radiant");
          int RadiantReduction = (int) Math.Ceiling((double) Radiant * (0.25 + 0.025 * (double) this.Level));
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
      return "Fire Resist: " + (object) (25.0 + (double) Level * 2.5) + "% Reduction";
    }
  }
}
