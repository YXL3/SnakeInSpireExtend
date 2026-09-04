using MegaCrit.Sts2.Core.Saves.Runs;

namespace SnakeInSpireExtend.Scripts.Extension;

/// <summary>
/// Wraps a <see cref="SerializableCard"/> with additional per-card metadata that
/// <see cref="SerializableCard"/> itself does not capture — namely BaseReplayCount
/// and custom DynamicVars (Haste, Hysteresis). These are restored onto the
/// newly created card in <see cref="SneakyPhantomSingleton.AfterPlayerTurnStart"/>.
/// </summary>
public sealed class PhantomCardEntry
{
    /// <summary>
    /// The serialized card data (ModelId, upgrade level, enchantment, etc.).
    /// </summary>
    public SerializableCard Card { get; set; } = null!;

    /// <summary>
    /// <see cref="MegaCrit.Sts2.Core.Models.CardModel.BaseReplayCount"/> at the time
    /// SneakyPhantom was played. Default 0 means no extra replays.
    /// </summary>
    public int ReplayCount { get; set; }

    /// <summary>
    /// Haste DynamicVar base value at the time SneakyPhantom was played.
    /// 0 means the card did not have Haste.
    /// </summary>
    public decimal Haste { get; set; }

    /// <summary>
    /// Hysteresis DynamicVar base value at the time SneakyPhantom was played.
    /// 0 means the card did not have Hysteresis.
    /// </summary>
    public decimal Hysteresis { get; set; }

    /// <summary>
    /// The card's energy cost (base cost, local modifiers, and X-value for X-cost cards)
    /// at the time SneakyPhantom was played. Null means no energy-cost data was captured
    /// (e.g. a save created before this field existed).
    /// </summary>
    public PhantomEnergyCost? EnergyCost { get; set; }
}