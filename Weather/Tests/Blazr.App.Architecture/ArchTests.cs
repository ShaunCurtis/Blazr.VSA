using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using ArchUnitNET.Fluent;

using Xunit;

//add a using directive to ArchUnitNET.Fluent.ArchRuleDefinition to easily define ArchRules
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace App.Weather.Tests.Arch
{
    public class ArchTests
    {
        private static readonly Architecture Architecture = new ArchLoader().LoadAssemblies(
            System.Reflection.Assembly.Load("Blazr.App"),
            //System.Reflection.Assembly.Load("Blazr.App.Weather.API"),
            System.Reflection.Assembly.Load("Blazr.App.EntityFramework")
            ).Build();

        private readonly IObjectProvider<IType> ExampleLayer =
            Types().That().ResideInAssembly("ExampleAssembly").As("Example Layer");

        public ArchTests()
        {

        }

        [Fact]
        public void Test1()
        {

        }
    }
}
