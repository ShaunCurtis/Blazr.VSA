/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Cadmium.Core;
using Blazr.Diode;

namespace Blazr.Cadmium.Extensions;

public static class IRecordMutorExtensions
{
    extension<TRecord>(IRecordMutor<TRecord> @this)
        where TRecord : class
    {
        public StateRecord<TRecord> ToStateRecord()
        {
            var state = @this.IsNew
                ? EditState.New
                : @this.IsDirty
                    ? EditState.Dirty
                    : EditState.Clean;

            return StateRecord<TRecord>.Create(@this.Mutate(), state);
        }

        public Bool<TRecord> ToBoolT()
            => BoolT.Read(@this.Mutate());
    }
}


