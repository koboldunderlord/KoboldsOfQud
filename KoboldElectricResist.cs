using System;
using XRL.Messages;
using XRL.World.Parts;
using XRL.Rules;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class KoboldElectricResist : BaseMutation
    {
        public Guid ChargeCellsActivatedAbilityID = Guid.Empty;
        public ActivatedAbilityEntry ChargeCellsActivatedAbility;

        public static int[] Charge = BuildChargeArray();
        // 1000, 2500, 4750, 7750, 11500, 16000, 21250, 27250, 34000, 41500
        protected static int[] BuildChargeArray()
        {
            int[] Arr = new int[10];
            Arr[0] = 1000;
            for (int i = 1; i < Arr.Length; i++)
            {
                Arr[i] = Arr[i - 1] + 750 * i;
            }
            return Arr;
        }

        public KoboldElectricResist()
        {
            this.DisplayName = "Electric Channeling";
        }

        public override bool CanLevel()
        {
            return true;
        }

        public override void Register(GameObject Object)
        {
            Object.RegisterPartEvent((IPart)this, "BeforeApplyDamage");
            Object.RegisterPartEvent((IPart)this, "EndTurn");
            ActivatedAbilities part = Object.GetPart("ActivatedAbilities") as ActivatedAbilities;
            this.ChargeCellsActivatedAbilityID = part.AddAbility("Charge Cells", "CommandKoboldChargeCells", "Kobold Power", 0, true, false, "Toggle to consume hunger and charge equipped cells each turn", "~", false, false);
            this.ChargeCellsActivatedAbility = part.AbilityByGuid[this.ChargeCellsActivatedAbilityID];
            base.Register(Object);
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "BeforeApplyDamage")
            {
                Damage parameter = E.GetParameter("Damage") as Damage;
                if (!(parameter.HasAttribute("Electric") || parameter.HasAttribute("Shock") || parameter.HasAttribute("Lightning") || parameter.HasAttribute("Electricity")))
                    return true;

                double ReductionDouble = (double)parameter.Amount * GetPercentageResist(this.Level);
                int Reduction = (int)Math.Floor(ReductionDouble);
                if (ReductionDouble - (double)Reduction >= Stat.Rnd.NextDouble())
                {
                    Reduction += 1;
                }
                if (Reduction > 0 && this.ParentObject.IsPlayer())
                    MessageQueue.AddPlayerMessage("Your scales absorb " + Reduction.ToString() + " damage from the electricity.");
                parameter.Amount = Math.Max(0, parameter.Amount - Reduction);
                return false;
            }
            if (E.ID == "EndTurn" && this.ChargeCellsActivatedAbility != null && this.ChargeCellsActivatedAbility.ToggleState)
            {
                foreach (GameObject EquippedObject in this.ParentObject.GetPart<Body>().GetEquippedObjects())
                {
                    if (EquippedObject == null)
                    {
                        continue;
                    }

                    EnergyCell Cell = null;
                    if (EquippedObject != null && EquippedObject.HasPart("EnergyCell"))
                    {
                        Cell = EquippedObject.GetPart<EnergyCell>();
                    }
                    else if (EquippedObject.HasPart("EnergyCellSocket"))
                    {
                        EnergyCellSocket Socket = this.ParentObject.GetPart<EnergyCellSocket>();
                        if (Socket.Cell != null && Socket.Cell.HasPart("EnergyCell"))
                        {
                            Cell = Socket.Cell.GetPart<EnergyCell>();
                        }
                    }

                    if (Cell != null && Cell.CanBeRecharged() && Cell.GetRechargeAmount() > 0)
                    {
                        // Cells have varying degrees of charge rates so we need to 
                        int RechargedAmount = ConsumeCharge(Math.Min(Cell.GetRechargeAmount(), Cell.ChargeRate));
                        if (RechargedAmount == 0) // hungry....
                        {
                            if (this.ParentObject.IsPlayer())
                                MessageQueue.AddPlayerMessage("You're too hungry to muster any more energy...");
                            this.ChargeCellsActivatedAbility.ToggleState = false;
                            break;
                        }
                        else
                        {
                            Cell.AddCharge(RechargedAmount);
                        }
                    }
                }

            }
            return base.FireEvent(E);
        }

        public override string GetDescription()
        {
            return "Your scales channel electricity and store it into your body as if it were food.";
        }

        public override string GetLevelText(int Level)
        {
            return "Electricity Damage reduced: " + (object)GetPercentageResist(Level) + "%" +
              "\nHunger treated as a battery of capacity " + (object)GetCapacity(Level) +
              "\nRecharge rate per turn shared between all equipped items: " + (object)GetRechargeRate(Level);
        }

        public double GetPercentageResist(int Level)
        {
            return (20 + (double)Level * 5);
        }

        public int GetCapacity(int Level)
        {
            if (Level == 0)
            {
                return 0;
            }
            else if (Level <= 10)
            {
                return Charge[Level - 1];
            }
            else
            {
                return Charge[9];
            }
        }

        public int GetRechargeRate(int Level)
        {
            return Level * 100;
        }

        public int TranslateFromHunger()
        {
            Stomach Stomach = this.ParentObject.GetPart("Stomach") as Stomach;
            int StomachCapacity = Stomach.CalculateCookingIncrement() * 2; // accounts for Fasting Way and Mind Over Body -- * 2 gets you total hunger until famished
            double PercentageFull = (double)(StomachCapacity - Stomach.CookingCounter) / (double)StomachCapacity;
            return Math.Max(0, (int)(PercentageFull * GetCapacity(this.Level)));
        }

        // return actual charge used
        public int ConsumeCharge(int Charge)
        {
            int ChargeAvailable = TranslateFromHunger();
            if (Charge > ChargeAvailable)
                return 0;

            Stomach Stomach = this.ParentObject.GetPart("Stomach") as Stomach;
            int StomachCapacity = Stomach.CalculateCookingIncrement() * 2; // accounts for Fasting Way and Mind Over Body -- * 2 gets you total hunger until famished
            Stomach.CookingCounter = (int)(((ChargeAvailable - Charge) / (double)GetCapacity(this.Level)) * StomachCapacity);

            return Charge;
        }

    }
}
