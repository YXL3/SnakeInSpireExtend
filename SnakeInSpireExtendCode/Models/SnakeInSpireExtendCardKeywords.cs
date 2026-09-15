using MegaCrit.Sts2.Core.Entities.Cards;
using STS2RitsuLib.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;

namespace SnakeInSpireExtend.Scripts.Models;

[RegisterOwnedCardKeyword(nameof(Keen), CardDescriptionPlacement = ModKeywordCardDescriptionPlacement.BeforeCardDescription)]
public class SnakeInSpireExtendCardKeywords
{
    public static readonly CardKeyword Keen = ModContentRegistry.GetQualifiedKeywordId(Entry.ModId, nameof(Keen)).GetModCardKeyword();
}