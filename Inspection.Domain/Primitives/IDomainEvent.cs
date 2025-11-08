using MediatR;


namespace Inspection.Domain.Primitives
{

    public interface IDomainEvent : INotification
    {
    }

    //NewInspectionNotificationIsCreated
    //InspectionNotificationIsAcceptedByPidecContacts
    //InspectionNotificationDeadlineIsmet
    //LoginUserIsAuthenticated then they should be notified about deadlines
}
