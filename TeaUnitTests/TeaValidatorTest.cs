using System;
using System.Collections.Generic;
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

            attribute.MaxLength = 3;
            teaValidator.ValidateMaxLength("阿里");
            Assert.Equal("propertyName is exceed max-length: 3",
                Assert.Throws<ArgumentException>(() => { teaValidator.ValidateMaxLength("阿里test"); }).Message
            );

            List<string> list = new List<string>{ "1", "2","3","4" };
            teaValidator.ValidateMaxLength("阿里");
            Assert.Equal("propertyName is exceed max-length: 3",
                Assert.Throws<ArgumentException>(() => { teaValidator.ValidateMaxLength(list); }).Message
            );

            attribute.MinLength = 2;
            teaValidator.ValidateMinLength("阿里");
            Assert.Equal("propertyName is less than min-length: 2",
                Assert.Throws<ArgumentException>(() => { teaValidator.ValidateMinLength("阿"); }).Message
            );

            attribute.Maximun = 1.5;
            teaValidator.ValidateMaximum("1");
            Assert.Equal("propertyName is exceed maximum: 1.5",
                Assert.Throws<ArgumentException>(() => { teaValidator.ValidateMaximum(2); }).Message
            );

            attribute.Minimum = 1;
            teaValidator.ValidateMinimum(1.5);
            Assert.Equal("propertyName is less than Minimum: 1",
                Assert.Throws<ArgumentException>(() => { teaValidator.ValidateMinimum(-2); }).Message
            );
        }
    }
}
