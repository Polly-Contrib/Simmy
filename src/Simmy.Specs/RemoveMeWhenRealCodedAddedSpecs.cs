using FluentAssertions;
using Xunit;

namespace Simmy.Specs
{
    public class RemoveMeWhenRealCodedAddedSpecs
    {
        [Fact]
        public void RemoveMeTest()
        {
            var instance = new RemoveMeWhenRealCodedAdded();

            instance.Should().NotBeNull();
        }
    }
}
