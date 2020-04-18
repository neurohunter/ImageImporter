using NUnit.Framework;
using System.Collections.Generic;

namespace ImageImporter.Cli.Core.Tests
{
    public class ArgumentsTest
    {
        private string DefaultExtensions;
        private string DefaultValue;

        [SetUp]
        public void Setup()
        {
            DefaultExtensions = ".ext1 .ext2";
            DefaultValue = "test";
        }

        private string[] CreateArguments(
            bool longNotation,
            bool inputDirectoryPresent,
            bool outputDirectoryPresent,
            bool configurationFilePresent,
            bool rawFilesPresent,
            bool nonRawFilesPresent,
            bool videoFilesPresent
            )
        {
            var inputDirectoryArguments = new List<string> { longNotation ? "--input-directory" : "-i", DefaultValue };
            var outputDirectoryArguments = new List<string> { longNotation ? "--output-directory" : "-o", DefaultValue };
            var configurationPathArguments = new List<string> { longNotation ? "--configuration-path" : "-c", DefaultValue };
            var rawTypesArguments = new List<string> { longNotation ? "--raw-types" : "-r", DefaultExtensions};
            var nonRawTypesArguments = new List<string> { longNotation ? "--non-raw-types" : "-n", DefaultExtensions};
            var videoTypesArguments = new List<string> { longNotation ? "--video-types" : "-v", DefaultExtensions};
            var arguments = new List<string>();
            arguments.AddRange(inputDirectoryPresent ? inputDirectoryArguments : new List<string>());
            arguments.AddRange(outputDirectoryPresent ? outputDirectoryArguments : new List<string>());
            arguments.AddRange(configurationFilePresent ? configurationPathArguments : new List<string>());
            arguments.AddRange(rawFilesPresent ? rawTypesArguments : new List<string>());
            arguments.AddRange(nonRawFilesPresent ? nonRawTypesArguments : new List<string>());
            arguments.AddRange(videoFilesPresent ? videoTypesArguments : new List<string>());
            return arguments.ToArray();
        }


        [Test]
        public void EmptyArgumentsTest()
        {
            var args = new string[0];
            var result = Arguments.Parse(args, out var arguments);
            Assert.That(result, Is.False);
        }

        [Test]
        public void ConflictingArgumentsTest([Values]bool useLongNotation)
        {
            var args = CreateArguments(useLongNotation, true, true, true, true, true, true);
            var result = Arguments.Parse(args, out var arguments);
            Assert.That(result, Is.False);
        }

        [Test]
        public void ParseForConfigurationFileTest([Values]bool useLongNotation)
        {
            var args = CreateArguments(useLongNotation, true, false, true, false, false, false);
            var result = Arguments.Parse(args, out var arguments);
            Assert.That(result, Is.True);
            Assert.That(arguments.InputDirectory, Is.EqualTo(DefaultValue));
            Assert.That(arguments.OutputDirectory, Is.EqualTo(default(string)));
            Assert.That(arguments.ConfigurationPath, Is.EqualTo(DefaultValue));
        }

        [Test]
        public void ParseForArgumentsTest([Values]bool useLongNotation, [Values]bool useRaw, [Values]bool useNonRaw, [Values]bool useVideo)
        {
            var args = CreateArguments(useLongNotation, true, true, false, useRaw, useNonRaw, useVideo);
            var result = Arguments.Parse(args, out var arguments);
            Assert.That(result, Is.True);
            Assert.That(arguments.InputDirectory, Is.EqualTo(DefaultValue));
            Assert.That(arguments.OutputDirectory, Is.EqualTo(DefaultValue));
            Assert.That(arguments.ConfigurationPath, Is.EqualTo(default(string)));
            if (useRaw)
            {
                Assert.That(string.Join(' ', arguments.RawFileExtensions), Is.EqualTo(DefaultExtensions));
            }
            else
            {
                CollectionAssert.IsEmpty(arguments.RawFileExtensions);
            }
            if (useNonRaw)
            {
                Assert.That(string.Join(' ', arguments.NonRawFileExtensions), Is.EqualTo(DefaultExtensions));
            }
            else
            {
                CollectionAssert.IsEmpty(arguments.NonRawFileExtensions);
            }
            if (useVideo)
            {
                Assert.That(string.Join(' ', arguments.VideoFileExtensions), Is.EqualTo(DefaultExtensions));
            }
            else
            {
                CollectionAssert.IsEmpty(arguments.VideoFileExtensions);
            }
        }
    }
}