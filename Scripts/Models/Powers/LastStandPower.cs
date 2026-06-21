using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using SnakeInSpireExtend.Scripts.Extension;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace SnakeInSpireExtend.Scripts.Powers;

[RegisterPower]
public class LastStandPower : ModPowerTemplate
{
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"res://SnakeInSpireExtend/images/powers/{GetType().Name}.png",
        BigIconPath: $"res://SnakeInSpireExtend/images/powers/{GetType().Name}.png"
    );
    
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterAutoPostPlayPhaseEntered(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != base.Owner.Player)
        {
            return;
        }
        List<CardModel> items = PileType.Hand.GetPile(base.Owner.Player).Cards.Where((CardModel c) => Helper.HasCustomDynamic(c, "Hysteresis")).ToList();
        if (!items.Any())
        {
            return;
        }
        decimal maxValue = items.Max(c => c.DynamicVars["Hysteresis"].BaseValue);
        items = items.Where(c => c.DynamicVars["Hysteresis"].BaseValue == maxValue).ToList();
        CardModel cardModel = base.Owner.Player.RunState.Rng.Shuffle.NextItem(items);
        if (cardModel != null)
        {
            Flash();
            while(Helper.HasCustomDynamic(cardModel, "Hysteresis"))
            {
                await CardCmd.AutoPlay(choiceContext, cardModel, null);
            }
            await CardCmd.Exhaust(choiceContext, cardModel);
        }
    }
}
