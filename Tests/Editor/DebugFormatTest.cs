using System;
using System.Collections;
using System.Globalization;
using NUnit.Framework;
using SWeetJelly.Debug.App;
using SWeetJelly.Debug.Contract;

namespace SWeetJelly.Debug.Tests.EditMode
{
    public sealed class DebugFormatTest
    {
        [TestCase(null, null, "<null>")]
        [TestCase("coins", null, "[coins]")]
        [TestCase(null, "value", "value")]
        [TestCase("coins", "42", "[coins] : 42")]
        public void GroupLabelsAndNulls_KeepTheirSupportedText(string label, string value, string expected)
        {
            Assert.That(new DebugFormat().Format(new DebugEntry(DebugLevel.Info, DebugLabelType.Grouping, label, value, null)), Is.EqualTo(expected));
        }
        [Test]
        public void SequenceAndDictionaryPreviews_AreBounded()
        {
            var format = new DebugFormat(2);
            Assert.That(format.Format(new DebugEntry(DebugLevel.Info, DebugLabelType.Grouping, null, new[] { 1, 2, 3 }, null)), Is.EqualTo("[1, 2, ...]"));
            Assert.That(new DebugFormat(0).Format(new DebugEntry(DebugLevel.Info, DebugLabelType.Grouping, null, new[] { 1 }, null)), Is.EqualTo("[...]"));
            Assert.That(format.Format(new DebugEntry(DebugLevel.Info, DebugLabelType.Grouping, null, new Hashtable { { "coins", 42 } }, null)), Is.EqualTo("{coins:42}"));
        }
        [Test]
        public void FormattableUsesInvariantCulture_AndConversionFailurePropagates()
        {
            var previous = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("fr-FR");
                Assert.That(new DebugFormat().Format(new DebugEntry(DebugLevel.Info, DebugLabelType.Grouping, null, 1.25m, null)), Is.EqualTo("1.25"));
                Assert.Throws<InvalidOperationException>(() => new DebugFormat().Format(new DebugEntry(DebugLevel.Info, DebugLabelType.Grouping, null, new BadValue(), null)));
            }
            finally { CultureInfo.CurrentCulture = previous; }
        }
        [Test]
        public void ExceptionLabelAndMissingException_KeepTheirText()
        {
            var format = new DebugFormat();
            Assert.That(format.Format(new DebugEntry(DebugLevel.Exception, DebugLabelType.Grouping, "load", null, new Exception("failed"))), Is.EqualTo("[load] : failed"));
            Assert.That(format.Format(new DebugEntry(DebugLevel.Exception, DebugLabelType.Grouping, null, null, null)), Is.EqualTo("[Exception] : <null> (no exception provided)"));
        }
        private sealed class BadValue
        {
            public override string ToString() => throw new InvalidOperationException("conversion failed");
        }
    }
}
