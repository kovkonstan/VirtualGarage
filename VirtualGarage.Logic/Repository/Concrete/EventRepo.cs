using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InostudioSolutions.Data;
using VirtualGarage.Logic.DataModel;

namespace VirtualGarage.Logic.Repository
{
    class EventRepo : RepositoryEF<Event>, IEventRepo
    {
        public EventRepo(IUnitOfWorkEF unitOfWork)
            : base(unitOfWork)
        {
        }

    }
}
