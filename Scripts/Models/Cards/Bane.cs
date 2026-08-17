using System.Reflection;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace SnakeInSpireExtend.Scripts.Cards;

public class Bane() : SnakeCardTemplate(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Retain
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(9m, ValueProp.Move)
    ];

    private static readonly FieldInfo? LocalModifiersField = typeof(CardEnergyCost).GetField("_localModifiers", BindingFlags.NonPublic | BindingFlags.Instance);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        CardModel? card = (await CardSelectCmd.FromHand(choiceContext, Owner, new CardSelectorPrefs(SelectionScreenPrompt, 1),
        c => !(c.EnergyCost.CostsX || c.Keywords.Contains(CardKeyword.Unplayable)), this)).FirstOrDefault();
        if(card != null && LocalModifiersField != null)
        {
            int thisEnergyCost = EnergyCost.GetWithModifiers(CostModifiers.None);
            int cardEnergyCost = card.EnergyCost.GetWithModifiers(CostModifiers.None);
            List<LocalCostModifier>? listA = LocalModifiersField.GetValue(EnergyCost) as List<LocalCostModifier>;
            List<LocalCostModifier>? listB = LocalModifiersField.GetValue(card.EnergyCost) as List<LocalCostModifier>;
            EnergyCost.SetCustomBaseCost(cardEnergyCost);
            card.EnergyCost.SetCustomBaseCost(thisEnergyCost);
            LocalModifiersField.SetValue(EnergyCost, listB);
            LocalModifiersField.SetValue(card.EnergyCost, listA);
        }
    }

    protected override void OnUpgrade(){
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}