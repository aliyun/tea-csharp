using System;
using System.Collections.Generic;
using Darabonba;
using Xunit;

namespace DaraUnitTests
{
    public class ValidatorTest
    {
        [Fact]
        public void TestDaraValidator()
        {
            Validator validator = new Validator(null, "propertyName");
            validator.ValidateRequired("test");
            validator.ValidateRegex("test");
            Assert.NotNull(validator);

            ValidationAttribute attribute = new ValidationAttribute();
            attribute.Required = false;
            validator.Attribute = attribute;
            validator.ValidateRequired("test");
            Assert.NotNull(validator);

            attribute.Pattern = "";
            validator.ValidateRegex("test");
            Assert.NotNull(validator);

            attribute.Pattern = "pattern";
            validator.ValidateRegex(null);
            Assert.NotNull(validator);

            validator.ValidateRegex("patternTest");
            Assert.NotNull(validator);

            Assert.Equal("propertyName is not match pattern",
                Assert.Throws<ArgumentException>(() => { validator.ValidateRegex("test"); }).Message
            );

            attribute.Required = true;
            Assert.Equal("propertyName is required.",
                Assert.Throws<ArgumentException>(() => { validator.ValidateRequired(null); }).Message
            );

            attribute.MaxLength = 3;
            validator.ValidateMaxLength("阿里");
            Assert.Equal("propertyName is exceed max-length: 3",
                Assert.Throws<ArgumentException>(() => { validator.ValidateMaxLength("阿里test"); }).Message
            );

            List<string> list = new List<string>{ "1", "2","3","4" };
            validator.ValidateMaxLength("阿里");
            Assert.Equal("propertyName is exceed max-length: 3",
                Assert.Throws<ArgumentException>(() => { validator.ValidateMaxLength(list); }).Message
            );

            attribute.MinLength = 2;
            validator.ValidateMinLength("阿里");
            Assert.Equal("propertyName is less than min-length: 2",
                Assert.Throws<ArgumentException>(() => { validator.ValidateMinLength("阿"); }).Message
            );

            attribute.Maximun = 1.5;
            validator.ValidateMaximum("1");
            Assert.Equal("propertyName is exceed maximum: 1.5",
                Assert.Throws<ArgumentException>(() => { validator.ValidateMaximum(2); }).Message
            );

            attribute.Minimum = 1;
            validator.ValidateMinimum(1.5);
            Assert.Equal("propertyName is less than Minimum: 1",
                Assert.Throws<ArgumentException>(() => { validator.ValidateMinimum(-2); }).Message
            );
        }
    }
}
