using System;
using System.Collections.Generic;
using System.Text;

namespace DevryService.Core
{
    public interface IEntityWithTypedId<TId>
    {
        TId Id { get; }
    }
}
