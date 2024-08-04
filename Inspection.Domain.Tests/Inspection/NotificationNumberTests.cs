using FluentAssertions;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;

namespace Inspection.Domain.Tests.Inspection
{
    public class NotificationNumberTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]

        public void Constructor_should_ThrowArgumentException_WhenValueIsInvalid(string? value)
        {
            NotificationNumber Action() => new(value);

            //Assert
            FluentActions.Invoking(Action).Should().Throw<ArgumentException>()
                .Which.ParamName.Should().Be("value");
        }
    }
}