using Inspection.Domain.Entities.InspectionNotification;
using MediatR;
using System.Threading;
using System.Threading.Tasks;


namespace Inspection.Application.Events
{
    public class SendEmailOnInspectionNotificationCreated : INotificationHandler<InspectionNotificationCreatedEvent>
    {

        public SendEmailOnInspectionNotificationCreated()
        {
            
        }
        public Task Handle(InspectionNotificationCreatedEvent notification, CancellationToken cancellationToken)
        {
           
            return Task.CompletedTask;
        }
    }
}
