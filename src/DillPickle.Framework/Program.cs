﻿using System;
using DillPickle.Framework.Infrastructure;
using DillPickle.Framework.Io;
using DillPickle.Framework.Io.Api;
using DillPickle.Framework.Parser;
using DillPickle.Framework.Parser.Api;
using DillPickle.Framework.Runner;
using DillPickle.Framework.Runner.Api;
using GoCommando;
using GoCommando.Api;
using GoCommando.Attributes;

namespace DillPickle.Framework
{
    [Banner(@"DillPickle

(c) 2010 Mogens Heller Grabe
mookid8000@gmail.com
http://mookid.dk/oncode

Dill-flavored Gherkin-goodness for your BDD needs

Check out http://mookid.dk/oncode/dillpickle for more information.
")]
    class Program : ICommando
    {
        [PositionalArgument]
        [Description("Path to the assembly containing classes with [ActionSteps] and [TypeConverter]s")]
        [Example(@"..\src\SomeProject.Specs\bin\Debug\SomeProject.Specs.dll")]
        public string AssemblyPath { get; set; }

        [PositionalArgument]
        [Description("File pattern of feature files to load")]
        [Example(@"..\src\SomeProject.Specs\Features\*.feature")]
        public string FeaturePattern { get; set; }

        [NamedArgument("dryrun", "d")]
        public bool DryRun { get; set; }

        [NamedArgument("include", "i")]
        [Description("Specifies which tags to include")]
        public string Include { get; set; }

        [NamedArgument("exclude", "e")]
        [Description("Specifies which tags to exclude")]
        public string Exclude { get; set; }

        static int Main(string[] args)
        {
            return Go.Run<Program>(args);
        }

        public void Run()
        {
            var container = new MiniContainer();

            container
                .MapType<IActionStepsFinder, ActionStepsFinder>()
                .MapType<IAssemblyLoader, AssemblyLoader>()
                .MapType<IFeatureRunner, FeatureRunner>()
                .MapType<IGherkinParser, StupidGherkinParser>()
                .MapType<IObjectActivator, TrivialObjectActivator>()
                .MapType<IPropertySetter, IntelligentPropertySetter>()
                .MapType<IPropertySetter, TrivialPropertySetter>()
                .MapType<IFeatureFileFinder, FeatureFileFinder>()
                .MapType<IFileReader, FileReader>();

            var runner = container.Resolve<DefaultCommandLineRunner>();

            runner.Execute(new CommandLineArguments
                               {
                                   AssemblyPath = AssemblyPath,
                                   FeaturePattern = FeaturePattern,
                                   TagsToInclude = Split(Include),
                                   TagsToExclude = Split(Exclude),
                                   DruRun = DryRun,
                               });
        }

        string[] Split(string text)
        {
            return (text ?? "").Split(",;".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        }
    }
}