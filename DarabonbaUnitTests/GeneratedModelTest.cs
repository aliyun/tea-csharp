using System;
using System.Collections;
using System.Collections.Generic;
using Darabonba;
using Darabonba.Models;
using Xunit;

namespace DaraUnitTests
{
    public class GeneratedModelTest
    {
        private class Generated : Model
        {
            public string ReflectionOnly { get { throw new Exception("Mapping used reflection"); } }
            public override Dictionary<string, object> ToMapCore(bool noStream = false)
            {
                return new Dictionary<string, object> { { "value", "generated" } };
            }
        }

        private class Legacy : Model
        {
            [NameInMap("value")]
            public string Value { get; set; }
        }

        [Fact]
        public void DispatchesGeneratedModelsAndPreservesNullExtensionCalls()
        {
            Model model = new Generated();
            Assert.Equal("generated", model.ToMap()["value"]);
            model = null;
            Assert.Null(model.ToMap());
            Assert.Equal("legacy", new Legacy { Value = "legacy" }.ToMap()["value"]);
        }

        [Fact]
        public void BuiltinModelsConvertNestedMapsAndBoxedNumbers()
        {
            var runtime = RuntimeOptions.FromMap((IDictionary)new Dictionary<string, object>
            {
                { "retryOptions", new Dictionary<string, object> { { "retryable", true } } },
                { "maxAttempts", 3L },
                { "extendsParameters", new Dictionary<string, object>
                    { { "headers", new Dictionary<string, object> { { "name", "value" }, { "null", null } } },
                      { "queries", new Dictionary<string, object>() } } }
            });
            Assert.Equal(3, runtime.MaxAttempts);
            Assert.True(runtime.RetryOptions.Retryable);
            Assert.Equal("value", runtime.ExtendsParameters.Headers["name"]);
            Assert.Null(runtime.ExtendsParameters.Headers["null"]);
            Assert.Empty(runtime.ExtendsParameters.Queries);
            Assert.Equal("value", RuntimeOptions.FromMap(runtime.ToMap()).ExtendsParameters.Headers["name"]);
            var evt = SSEEvent.FromMap((IDictionary)new Dictionary<string, object> { { "retry", 5L }, { "data", "hello" } });
            Assert.Equal(5, evt.Retry);
            Model baseEvent = evt;
            Assert.Equal("hello", baseEvent.ToMap()["data"]);
        }
    }
}
