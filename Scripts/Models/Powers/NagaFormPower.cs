using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace SnakeInSpireExtend.Scripts.Powers;

[RegisterPower]
public class NagaFormPower : ModPowerTemplate
{
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"res://SnakeInSpireExtend/images/powers/{GetType().Name}.png",
        BigIconPath: $"res://SnakeInSpireExtend/images/powers/{GetType().Name}.png"
    );

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    private class Data
    {
        public IEnumerable<CardModel> cards = Array.Empty<CardModel>();
    }

    protected override object InitInternalData()
    {
        return new Data();
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new StringVar("Cards")];

    public void SetCards(IEnumerable<CardModel> cards)
    {
        GetInternalData<Data>().cards = cards;
        ((StringVar)DynamicVars["Cards"]).StringValue = string.Join(", ", cards.Select(c => c.Title));
    }

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (player == Owner.Player)
        {
            foreach (CardModel card in GetInternalData<Data>().cards)
            {
                CardModel card2 = card.CreateClone();
                card2.EnergyCost.SetThisCombat(0);
                await CardCmd.AutoPlay(choiceContext, card2, null);
            }
        }
    }
}
