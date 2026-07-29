using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SnakeInSpireExtend.Scripts.Extension;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace SnakeInSpireExtend.Scripts.Powers;

[RegisterPower]
public class PsychokinesisPower : ModPowerTemplate
{    
    private class Data
    {
        public Type? cardType;
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new StringVar("Card")];

    protected override object InitInternalData()
    {
        return new Data();
    }

    public void SetSelectedCard(CardModel card)
    {
        GetInternalData<Data>().cardType = card.GetType();
        ((StringVar)DynamicVars["Card"]).StringValue = card.Title;
    }

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public override Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw){
        if (card.GetType() == GetInternalData<Data>().cardType)
        {
            Flash();
            Helper.Haste(card, Amount);
        }
        return Task.CompletedTask;
    }
}
