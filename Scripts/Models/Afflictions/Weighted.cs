using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

[RegisterAffliction]
public class Weighted : ModAfflictionTemplate
{
    public override bool HasExtraCardText => true;
    
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
		if (cardPlay.Card != base.Card)
		{
			return;
		}
        await PlayerCmd.LoseEnergy(1, base.Card.Owner);
    }
}