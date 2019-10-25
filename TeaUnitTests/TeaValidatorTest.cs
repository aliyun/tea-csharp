using System;

using Tea;

using Xunit;

namespace TeaUnitTests
{
    public class TeaValidatorTest
    {
        [Fact]
        public void TestTeaValidator()
        {
            TeaValidator teaValidator = new TeaValidator(null, "propertyName");
            teaValidator.ValidateRequired("test");
            teaValidator.ValidateRegex("test");
            Assert.NotNull(teaValidator);

            ValidationAttribute attribute = new ValidationAttribute();
            attribute.Required = false;
            teaValidator.Attribute = attribute;
            teaValidator.ValidateRequired("test");
            Assert.NotNull(teaValidator);

            attribute.Pattern = "";
            teaValidator.ValidateRegex("test");
            Assert.NotNull(teaValidator);

            attribute.Pattern = "pattern";
            teaValidator.ValidateRegex(null);
            Assert.NotNull(teaValidator);

            teaValidator.ValidateRegex("patternTest");
            Assert.NotNull(teaValidator);

            Assert.Equal("propertyName is not match pattern",
                Assert.Throws<ArgumentException>(() => { teaValidator.ValidateRegex("test"); }).Message
            );

            attribute.Required = true;
            Assert.Equal("propertyName is required.",
                Assert.Throws<ArgumentException>(() => { teaValidator.ValidateRequired(null); }).Message
            );
            Assert.Equal("propertyName is required.",
                Assert.Throws<ArgumentException>(() => { teaValidator.ValidateRequired(""); }).Message
            );
        }
    }
}
