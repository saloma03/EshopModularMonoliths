using MediatR;
using Microsoft.AspNetCore.Components.Forms;

namespace Shared.DDD
{
    public interface IDomainEvent: INotification
    {
        Guid EventId => Guid.NewGuid();

        public DateTime OccuredOn => DateTime.Now;

        public string EventType => GetType().AssemblyQualifiedName!;
    }
}
