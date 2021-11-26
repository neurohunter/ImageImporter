using ImageImporter.PathBuilder;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace ImageImporter.Test.PathBuilder
{
    [TestFixture]
    public class DestinationPathBuilderTest
    {
        private IDestinationPathBuilder _builder;

        [TestCaseSource(nameof(WhenPathComponentsAreProvided_PathShouldBeCreatedTestCases))]
        public void WhenPathComponentsAreProvided_PathShouldBeCreated(FileKind fileKind, string expectedFileKindDirectory)
        {
            _builder = new DestinationPathBuilder();
            var dateTime = DateTime.Now;
            var rootDirectory = Path.GetTempPath();
            var path = _builder.BuildDestinationDirectoryPath(rootDirectory, dateTime, fileKind);
            var directoryhandle = new DirectoryInfo(path);
            
            Assert.That(directoryhandle.Name, Is.EqualTo(expectedFileKindDirectory));
            Assert.That(directoryhandle.Parent.Name, Is.EqualTo(dateTime.ToString("yyyy_MM_dd")));
            Assert.That(directoryhandle.Parent.Parent.FullName, Is.EqualTo(rootDirectory));
        }

        private IEnumerable<TestCaseData> WhenPathComponentsAreProvided_PathShouldBeCreatedTestCases()
        {
            foreach(var fileKind in Enum.GetValues<FileKind>())
            {
                yield return new TestCaseData(fileKind, fileKind.GetAttributeOfType<DescriptionAttribute>().ToString());
            }
        }
    }
}
