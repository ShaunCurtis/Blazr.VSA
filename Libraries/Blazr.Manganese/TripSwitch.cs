/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public class TripSwitch
{
    private bool _tripped;

    private TripSwitch() { }

    public void Trip()
        => _tripped = true;

    public void Reset()
        => _tripped = false;

    public TripSwitch Map(Func<bool> TripTest)
    {
        if (TripTest())
            Trip();
        return this;
    }

    public TripSwitch Output(Action? Tripped = null, Action? NotTripped = null)
    {
        if (_tripped && Tripped != null)
            Tripped();

        if (!_tripped && NotTripped != null)
            NotTripped();

        return this;
    }
}
