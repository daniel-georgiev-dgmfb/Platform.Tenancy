using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Platform.Tenant.Management.Tests.L0
{
	[TestFixture]
    [Category("ConnectionStringParser")]
    internal class ConnectionStringParserTests
    {
        [Test]
        public void Instantiate_with_serialiser_null()
        {
            //ARRANGE
            //ACT
            //ASSERT
            Assert.Throws<ArgumentNullException>(() => new TenantAttributesParser(null));
        }

        [Test]
        public void When_call_parse_with_source_null_throws_an_exception()
        {
            //ARRANGE
            var serialiser = new Mock<IJsonSerialiser>();
            var parser = new TenantAttributesParser(serialiser.Object);
            //ACT

            //ASSERT
            Assert.ThrowsAsync<ArgumentNullException>(() => parser.Parse(null, false));
        }

        [Test]
        public void When_call_parse_with_source_empty_string_throws_an_exception()
        {
            //ARRANGE
            var serialiser = new Mock<IJsonSerialiser>();
            var parser = new TenantAttributesParser(serialiser.Object);
            //ACT

            //ASSERT
            Assert.ThrowsAsync<ArgumentNullException>(() => parser.Parse(String.Empty, false));
        }

        [Test]
        public void When_call_parse_with_throw_on_failure_and_model_null_throws_invalid_operation_exception()
        {
            //ARRANGE
            var source = "String to deserialise";
            var serialiser = new Mock<IJsonSerialiser>();
            serialiser.Setup(x => x.DeserialiseFromJson<TenantAttributesParser.ConnectionStringModel>(It.IsAny<string>()))
                .ReturnsAsync((TenantAttributesParser.ConnectionStringModel)null);
           
            var parser = new TenantAttributesParser(serialiser.Object);
            //ACT

            //ASSERT
            Assert.ThrowsAsync<InvalidOperationException>(() => parser.Parse(source, true));
        }

        [Test]
        public async Task When_call_parse_with_throw_on_failure_false_and_model_null_return_null()
        {
            //ARRANGE
            var source = "String to deserialise";
            var serialiser = new Mock<IJsonSerialiser>();
            serialiser.Setup(x => x.DeserialiseFromJson<TenantAttributesParser.ConnectionStringModel>(It.IsAny<string>()))
                .ReturnsAsync((TenantAttributesParser.ConnectionStringModel)null);

            var parser = new TenantAttributesParser(serialiser.Object);
            //ACT
            var result = await parser.Parse(source, false);
            //ASSERT
            Assert.IsNull(result);
        }

        [Test]
        public async Task When_call_parse_with_throw_on_failure_false_and_model_not_null_return_not_null()
        {
            //ARRANGE
            var source = "String to deserialise";
            var serialiser = new Mock<IJsonSerialiser>();
            serialiser.Setup(x => x.DeserialiseFromJson<TenantAttributesParser.ConnectionStringModel>(It.IsAny<string>()))
                .ReturnsAsync(new TenantAttributesParser.ConnectionStringModel { shard = "server", database = "db", signInUserId = "user", signInSecret = "secret"});

            var parser = new TenantAttributesParser(serialiser.Object);
            //ACT
            var result = await parser.Parse(source, false);
            //ASSERT
            Assert.IsNotNull(result);
            Assert.AreEqual("server", result.ConnectionStringAttributes.Server);
            Assert.AreEqual("db", result.ConnectionStringAttributes.Database);
            Assert.AreEqual("user", result.ConnectionStringAttributes.User);
            Assert.AreEqual("secret", result.ConnectionStringAttributes.SecretName);
        }

        [Test]
        public async Task When_call_parse_with_throw_on_failure_true_and_model_not_null_return_not_null()
        {
            //ARRANGE
            var source = "String to deserialise";
            var serialiser = new Mock<IJsonSerialiser>();
            serialiser.Setup(x => x.DeserialiseFromJson<TenantAttributesParser.ConnectionStringModel>(It.IsAny<string>()))
                .ReturnsAsync(new TenantAttributesParser.ConnectionStringModel { shard = "server", database = "db", signInUserId = "user", signInSecret = "secret" });

            var parser = new TenantAttributesParser(serialiser.Object);
            //ACT
            var result = await parser.Parse(source, true);
            //ASSERT
            Assert.IsNotNull(result);
            Assert.AreEqual("server", result.ConnectionStringAttributes.Server);
            Assert.AreEqual("db", result.ConnectionStringAttributes.Database);
            Assert.AreEqual("user", result.ConnectionStringAttributes.User);
            Assert.AreEqual("secret", result.ConnectionStringAttributes.SecretName);
        }
    }
}