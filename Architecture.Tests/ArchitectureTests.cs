using NetArchTest.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Inspection.Architecture.Tests
{
    public class ArchitectureTests
    {
        private const string DomainNamespace= "Domain";
        private const string ApplicationNamespace = "Application";
        private const string InfrastructureNamespace = "Infrastructure";
        private const string PresentationNamespace = "Presentation";
        private const string ApiNamespace = "Api";

        [Fact]
        public void Domain_Should_Not_HaveDependencyOnOtherProjects() {
            //Arrange

            var assembly = typeof(Domain.Model.Attachment).Assembly;
            var otherProjects = new[] {
                InfrastructureNamespace,
                ApiNamespace
            };

            //Act
            var testResult = Types
                .InAssembly(assembly)
                .ShouldNot()
                .HaveDependencyOnAll(otherProjects)
                .GetResult();

            //Assert
            testResult.IsSuccessful.CompareTo(true);
        }
    }
}
