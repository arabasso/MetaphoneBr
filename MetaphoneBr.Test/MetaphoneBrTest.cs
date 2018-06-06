using System;
using Shouldly;
using Xunit;

namespace MetaphoneBr.Test {
    public class MetaphoneBrTest {
        [Theory]
        [InlineData ("Raphael", "2FL")]
        [InlineData ("Karla", "KRL")]
        [InlineData ("Carla", "KRL")]
        public void Translate_a (
            string input,
            string expected) {
            input.ToPhonetic (new MetaphoneBr ()).ShouldBe (expected);
        }
    }
}