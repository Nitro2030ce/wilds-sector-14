using Content.Shared.Speech.EntitySystems;
using Robust.Shared.GameStates;

namespace Content.Shared.Speech.Components;

/// <summary>
/// wawa?
/// </summary>
[RegisterComponent, NetworkedComponent]
[Access(typeof(ScurretAccentSystem))]
public sealed partial class ScurretAccentComponent : BaseAccentComponent;
