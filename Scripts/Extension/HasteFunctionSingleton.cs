using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SnakeInSpireExtend.Scripts.Models;
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
        if (Helper.HasCustomDynamic(card, "Hysteresis"))
        {
            card.DynamicVars["Hysteresis"].UpgradeValueBy(-1m);
        }
    }

    private static async Task HasteFunction(CardModel card, PlayerChoiceContext choiceContext)
    {
        if (Helper.HasCustomDynamic(card, "Haste"))
        {
            IEnumerable<CardModel> cards = await CardPileCmd.Draw(choiceContext, await Helper.ApplyHasteDrawingAmount(card), card.Owner);
            decimal hastePassed = Helper.HastePassingAmount(card);
            card.DynamicVars["Haste"].BaseValue = 0m;
            foreach (CardModel item in cards){
                Helper.Haste(item, hastePassed);
            }
        }
    }

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (!fromHandDraw && card.Keywords.Contains(SnakeInSpireExtendCardKeywords.Keen))
        {
            await CardCmd.AutoPlay(choiceContext, card, null);
        }
    }
}