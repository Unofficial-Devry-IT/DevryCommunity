using System;
using System.Collections.Generic;
using System.Text;

namespace DevryService.Core
{
    public class EntityWithTypedId<TId> : IEntityWithTypedId<TId>
    {
        public TId Id { get; set; }
    }
}
