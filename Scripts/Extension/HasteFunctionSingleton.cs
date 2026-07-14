using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SnakeInSpireExtend.Scripts.Models;
using SnakeInSpireExtend.Scripts.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Models;

namespace SnakeInSpireExtend.Scripts.Extension;

[RegisterSingleton]

public class HasteFunctionSingleton : HookedSingletonModel
{
    public HasteFunctionSingleton() : base(HookType.Combat){}
    
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        CardModel card = cardPlay.Card;
        await HasteFunction(card, choiceContext);
        if (card is DualEffectCardTemplate)
        {
            await HasteFunction(card, choiceContext, "Hysteresis");
        }
        if (Helper.HasCustomDynamic(card, "Hysteresis"))
        {
            card.DynamicVars["Hysteresis"].UpgradeValueBy(-1m);
        }
    }

    private static async Task HasteFunction(CardModel card, PlayerChoiceContext choiceContext, string functionalHaste = "Haste")
    {
        if (!Helper.HasCustomDynamic(card, functionalHaste))
        {
            return;
        }
        IEnumerable<CardModel> cards = await CardPileCmd.Draw(choiceContext, await Helper.ApplyHasteDrawingAmount(card), card.Owner);
        if (functionalHaste == "Haste")
        {
            decimal hastePassed = Helper.HastePassingAmount(card);
            card.DynamicVars[functionalHaste].BaseValue = 0m;
            foreach (CardModel item in cards){
                Helper.Haste(item, hastePassed);
                if (item.Keywords.Contains(SnakeInSpireExtendCardKeywords.Keen) && item != card)
                {
                    await KeenAction(item, choiceContext);
                }
            }
        }
    }

    private static async Task KeenAction(CardModel card, PlayerChoiceContext choiceContext)
    {
        decimal snakeFury = Helper.GetOwnerPowerAmount<SnakeFuryPower>(card);
        if(snakeFury != 0)
        {
            Helper.Haste(card, snakeFury);
            card.GiveSingleTurnRetain();
        }
        else
        {
            await CardCmd.AutoPlay(choiceContext, card, null);
        }
    }
}