namespace SnakeInSpireExtend.Scripts.Extension;

/// <summary>
/// Serializable DTO that stores SneakyPhantom carry-over data for a single player.
/// Persisted via RunSavedDataStore so it survives combat transitions and save/load.
/// </summary>
public sealed class PhantomCarryOverState
{
    /// <summary>
    /// Serialized copies of the cards that were in hand when SneakyPhantom was played,
    /// along with extra per-card metadata (ReplayCount, Haste, Hysteresis) that
    /// <see cref="SerializableCard"/> does not capture.
    /// </summary>
    public List<PhantomCardEntry> Cards { get; set; } = [];

    /// <summary>
    /// The block amount the player had when SneakyPhantom was played.
    /// </summary>
    public int Block { get; set; }

    /// <summary>
    /// The energy the player had when SneakyPhantom was played.
    /// </summary>
    public int Energy { get; set; }

    /// <summary>
    /// Whether this state contains no carry-over data.
    /// </summary>
    public bool IsEmpty => Cards.Count == 0 && Block == 0 && Energy == 0;
}
